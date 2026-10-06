namespace TeleVisionGuide
{
    public record DownloadItem(Uri Source, string TargetPath);

    public partial class DownloadForm : Form
    {
        private static readonly HttpClient _httpClient = InitializeHttpClient();

        private readonly IReadOnlyList<DownloadItem> _items;
        private CancellationTokenSource? _cts;

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

            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    DownloadItem item = _items[i];

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

                    labelOverall.Text = $"File {i + 1} of {_items.Count}";
                    labelStatus.Text = $"Downloading {item.Source}";
                    await DownloadFileAsync(item, token);
                    progressBarOverall.Value = i + 1;
                }

                labelStatus.Text = "Download completed.";
                DialogResult = DialogResult.OK;
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
                cts.Dispose();
            }
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
                            progressBarFile.Value = (int)Math.Min(100, received * 100 / totalBytes.Value);
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
