namespace TeleVisionGuide
{
    public record DownloadItem(Uri Source, string TargetPath);

    public partial class DownloadForm : Form
    {
        private static readonly HttpClient _httpClient = InitializeHttpClient();

        private readonly IReadOnlyList<DownloadItem> _items;
        private CancellationTokenSource? _cts;
        private const int MaxParallelDownloads = 10;
        private int _completedDownloads = 0;
        private int _failedDownloads = 0;
        private readonly object _progressLock = new();

        public Exception? Error { get; private set; }

        private static HttpClient InitializeHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "TeleVisionGuide/1.0 (Windows NT; .NET 10)");
            return client;
        }

        public DownloadForm(IEnumerable<DownloadItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            _items = items.ToList();
            InitializeComponent();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await RunDownloadsAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_cts is not null && !_cts.IsCancellationRequested)
            {
                // Request abort; the form closes once the download loop has finished.
                e.Cancel = true;
                Abort();
            }
            base.OnFormClosing(e);
        }

        private async Task RunDownloadsAsync()
        {
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            progressBarOverall.Maximum = Math.Max(1, _items.Count);
            progressBarOverall.Value = 0;
            _completedDownloads = 0;
            _failedDownloads = 0;

            try
            {
                // Normalize all URIs first
                List<DownloadItem> normalizedItems = _items.Select(NormalizeDownloadItem).ToList();

                // Download in batches of up to MaxParallelDownloads
                for (int batchStart = 0; batchStart < normalizedItems.Count; batchStart += MaxParallelDownloads)
                {
                    int batchEnd = Math.Min(batchStart + MaxParallelDownloads, normalizedItems.Count);
                    List<DownloadItem> batchItems = normalizedItems.Skip(batchStart).Take(batchEnd - batchStart).ToList();

                    labelOverall.Text = $"Files {batchStart + 1}-{batchEnd} of {normalizedItems.Count}";

                    // Create download tasks for the current batch
                    List<Task> downloadTasks = batchItems.Select((item, index) =>
                        DownloadFileWithTrackingAsync(item, batchStart + index, normalizedItems.Count, token)
                    ).ToList();

                    // Wait for all downloads in this batch to complete
                    try
                    {
                        await Task.WhenAll(downloadTasks);
                    }
                    catch (Exception ex) when (!(ex is OperationCanceledException))
                    {
                        // Continue with remaining batches even if some downloads fail
                        Error = ex;
                    }
                }

                if (_failedDownloads > 0)
                {
                    labelStatus.Text = $"Download completed with {_failedDownloads} error(s).";
                    DialogResult = _completedDownloads > 0 ? DialogResult.OK : DialogResult.Abort;
                }
                else
                {
                    labelStatus.Text = "Download completed.";
                    DialogResult = DialogResult.OK;
                }
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "Download aborted.";
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                Error = ex;
                MessageBox.Show(this, ex.Message, "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
            }
            finally
            {
                CancellationTokenSource cts = _cts;
                _cts = null;
                cts?.Dispose();
            }
        }

        private DownloadItem NormalizeDownloadItem(DownloadItem item)
        {
            // Normalize addresses: if the provided URI is not absolute (no scheme),
            // assume HTTPS so users can provide addresses like "example.com/path".
            if (!item.Source.IsAbsoluteUri)
            {
                string original = item.Source.OriginalString;
                // Trim any leading slashes to avoid malformed URIs like https:///...
                string candidate = original.TrimStart('/');
                if (Uri.TryCreate("https://" + candidate, UriKind.Absolute, out Uri? httpsUri))
                {
                    item = item with { Source = httpsUri };
                }
            }
            return item;
        }

        private async Task DownloadFileWithTrackingAsync(DownloadItem item, int itemIndex, int totalItems, CancellationToken token)
        {
            try
            {
                UpdateUIForDownload(itemIndex, item, totalItems);
                await DownloadFileAsync(item, token);

                lock (_progressLock)
                {
                    _completedDownloads++;
                    progressBarOverall.Value = _completedDownloads + _failedDownloads;
                }
            }
            catch (HttpRequestException http)
            {
                lock (_progressLock)
                {
                    _failedDownloads++;
                    progressBarOverall.Value = _completedDownloads + _failedDownloads;
                }
                Console.WriteLine($"Failed to download {item.Source}: {http.Message}");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                lock (_progressLock)
                {
                    _failedDownloads++;
                    progressBarOverall.Value = _completedDownloads + _failedDownloads;
                }
                Console.WriteLine($"Error downloading {item.Source}: {ex.Message}");
            }
        }

        private void UpdateUIForDownload(int itemIndex, DownloadItem item, int totalItems)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateUIForDownload(itemIndex, item, totalItems));
                return;
            }

            labelStatus.Text = $"Downloading {item.Source}";
        }

        private async Task DownloadFileAsync(DownloadItem item, CancellationToken token)
        {
            progressBarFile.Style = ProgressBarStyle.Continuous;
            progressBarFile.Value = 0;

            using HttpResponseMessage response = await _httpClient.GetAsync(item.Source, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;
            if (totalBytes is null or <= 0)
            {
                progressBarFile.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                progressBarFile.Maximum = 100;
            }

            string? directory = Path.GetDirectoryName(Path.GetFullPath(item.TargetPath));
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool completed = false;
            try
            {
                await using (Stream source = await response.Content.ReadAsStreamAsync(token))
                await using (FileStream target = new FileStream(item.TargetPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    byte[] buffer = new byte[81920];
                    long received = 0;
                    int read;
                    while ((read = await source.ReadAsync(buffer, token)) > 0)
                    {
                        await target.WriteAsync(buffer.AsMemory(0, read), token);
                        received += read;
                        if (totalBytes > 0)
                        {
                            if (InvokeRequired)
                            {
                                int progress = (int)Math.Min(100, received * 100 / totalBytes.Value);
                                Invoke(() => progressBarFile.Value = progress);
                            }
                            else
                            {
                                progressBarFile.Value = (int)Math.Min(100, received * 100 / totalBytes.Value);
                            }
                        }
                    }
                }
                completed = true;
                progressBarFile.Style = ProgressBarStyle.Continuous;
                progressBarFile.Maximum = 100;
                progressBarFile.Value = 100;
            }
            finally
            {
                if (!completed && File.Exists(item.TargetPath))
                {
                    try { File.Delete(item.TargetPath); } catch (IOException) { }
                }
            }
        }

        private void Abort()
        {
            if (_cts is null || _cts.IsCancellationRequested)
            {
                return;
            }
            buttonAbort.Enabled = false;
            labelStatus.Text = "Aborting...";
            _cts.Cancel();
        }

        private void buttonAbort_Click(object? sender, EventArgs e)
        {
            Abort();
        }
    }
}
