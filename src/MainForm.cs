using System.Threading.Channels;
using System.Xml;
using TeleVisionGuide.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace TeleVisionGuide
{
    public partial class MainForm : Form
    {
        private static readonly Color HeaderColor = Color.FromArgb(0, 84, 166);
        private static readonly Color HeaderHoverColor = Color.FromArgb(25, 110, 195);
        private static readonly Color HeaderPressedColor = Color.FromArgb(0, 60, 125);
        private static readonly Color HeaderAccentColor = Color.FromArgb(255, 196, 0);
        private static readonly Color HeaderSeparatorColor = Color.FromArgb(0, 50, 105);

        private const string TvGlyph = "\uE7F4";
        private const string UpdateGlyph = "\uE895";
        private const string SettingsGlyph = "\uE713";
        private const string InfoGlyph = "\uE946";
        // 11 days
        private const int AvailableDaysInFuture = 10;
        private const int AvailableDaysInPast = 2;
        private string _dataPath = null;
        private Dictionary<DateTime, T4aRoot> _t4aBroadCasts = new Dictionary<DateTime, T4aRoot>();
        private T4aChannels _t4AChannels = null;
        private T4aGenres _t4aGenres = null;

        public MainForm()
        {
            InitializeComponent();
            ApplyHeaderDesign();
            LoadData();
            ShowData();
        }

        private static Font CreateIconFont(float size)
        {
            foreach (string family in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
            {
                using Font font = new Font(family, size);
                if (font.Name == family)
                    return new Font(family, size);
            }
            return new Font(FontFamily.GenericSansSerif, size);
        }

        private static DirectoryInfo TryGetSolutionDirectoryInfo()
        {
            DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.slnx").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        //private static void GetHeaderColor(string channelId)
        //{
        //}
        //
        //private static void GetColor(string channelId)
        //{
        //}

        private void ApplyHeaderDesign()
        {
            headerPanel.BackColor = HeaderColor;
            headerPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(HeaderSeparatorColor, 3);
                int y = headerPanel.Height - 2;
                e.Graphics.DrawLine(pen, 0, y, headerPanel.Width, y);
            };

            logoLabel.Font = CreateIconFont(26f);
            logoLabel.Text = TvGlyph;
            logoLabel.ForeColor = HeaderAccentColor;

            appNameLabel.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            appNameLabel.ForeColor = Color.White;

            StyleHeaderButton(updateButton, UpdateGlyph, "Update");
            StyleHeaderButton(settingsButton, SettingsGlyph, "Settings");
            StyleHeaderButton(infoButton, InfoGlyph, "Info");

            daySelector.Font = new Font("Segoe UI", 9f);
            daySelector.ForeColor = Color.White;
            daySelector.DayBackColor = HeaderColor;
            daySelector.HoverBackColor = HeaderHoverColor;
            daySelector.SelectedBackColor = HeaderAccentColor;
            daySelector.SelectedForeColor = HeaderSeparatorColor;
            daySelector.BorderColor = HeaderSeparatorColor;
            daySelector.StartDate = DateTime.Today.AddDays(-AvailableDaysInPast);
            daySelector.DayCount = AvailableDaysInPast + AvailableDaysInFuture;
            daySelector.SelectedDate = DateTime.Today;
            daySelector.SelectedDateChanged += (s, e) => ShowData();

            timeTable.HeaderBackColor = HeaderColor;
            timeTable.HeaderForeColor = Color.White;
        }

        private void StyleHeaderButton(Button button, string glyph, string toolTipText)
        {
            button.Text = glyph;
            button.Font = CreateIconFont(18f);
            button.ForeColor = Color.White;
            button.BackColor = HeaderColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = HeaderHoverColor;
            button.FlatAppearance.MouseDownBackColor = HeaderPressedColor;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            toolTip.SetToolTip(button, toolTipText);
        }

        private void ShowData()
        {
            timeTable.SuspendLayout();
            timeTable.Clear();
            timeTable.HeaderHeight = 64;
            timeTable.HeaderImageSize = new Size(48, 48);
            timeTable.RowHeight = 64;

            if (_t4AChannels != null && _t4aBroadCasts.ContainsKey(daySelector.SelectedDate))
            {
                List<T4aChannel> orderedChannels = _t4AChannels.Channels.OrderBy(c => c.Order).ToList();
                foreach (T4aChannel channel in orderedChannels)
                {
                    string logoPath = Path.Combine(GetChannelLogoPath(), $"{channel.ChannelId}.png");
                    Image logoImage = File.Exists(logoPath) ? Image.FromFile(logoPath) : null;
                    timeTable.AddColumn(channel.Name, logoImage);
                }

                T4aEvent[] selectedDayEvents = _t4aBroadCasts[daySelector.SelectedDate].Events;
                timeTable.StartTime = selectedDayEvents.Min(m => m.StartTime.TimeOfDay);
                timeTable.EndTime = selectedDayEvents.Max(m => m.EndTime.TimeOfDay);

                foreach (T4aEvent t4aEvent in selectedDayEvents)
                {
                    TimeSpan startTime = t4aEvent.StartTime.TimeOfDay;
                    TimeSpan duration = t4aEvent.EndTime - t4aEvent.StartTime;
                    string channelName = orderedChannels.FirstOrDefault(c => c.Id == t4aEvent.Channel)?.Name;
                    TeleVisionEventControl cell = timeTable.AddEvent(channelName, startTime, duration);
                    cell.LeftHeadline = $"{(int)startTime.TotalHours % 24:00}:{startTime.Minutes:00}";
                    cell.RightHeadline = t4aEvent.Title;
                    if (t4aEvent.Photo?.Url != null)
                    {
                        string imagePath = Path.Combine(GetEventImagePath(), Path.GetFileName(t4aEvent.Photo.Url));
                        if (File.Exists(imagePath))
                        {
                            cell.RightImage = Image.FromFile(imagePath);
                            cell.RightImageSize = new Size(64, 64);
                        }
                    }
                    //string description = t4aEvent.Content?.Texts?.TextsClass?.Long?.Value;
                    //if(string.IsNullOrEmpty(description))
                    //{
                    //    description = t4aEvent.Content?.TitleLang;
                    //}

                    //string description = t4aEvent.Content?.Texts?.TextsClass?.Long?
                    string genere = "";// _t4aGenres.Genres.FirstOrDefault(g => g.Id == t4aEvent.Content.Genre)?.Title ?? "Unknown Genre";
                    cell.RightText = $"{duration.TotalMinutes} min, {genere}, {t4aEvent.Content.Country} {t4aEvent.Content.Year}"; ;
                }


                /*
                timeTable.StartTime = TimeSpan.FromHours(18);
                timeTable.EndTime = TimeSpan.FromHours(24);
                timeTable.RowInterval = TimeSpan.FromMinutes(30);
                timeTable.RowHeight = 60;
                timeTable.HeaderHeight = 36;
                timeTable.AddColumn("Das Erste", null, Color.RoyalBlue, Color.AliceBlue);
                timeTable.AddColumn("ZDF", null, Color.DarkOrange, Color.OldLace);
                timeTable.AddColumn("RTL", null, Color.Firebrick, Color.MistyRose);
                timeTable.AddColumn("ProSieben", null, Color.SeaGreen, Color.Honeydew);

                var rnd = new Random(daySelector.SelectedDate.DayOfYear);
                string[] titles = { "News", "Movie", "Documentary", "Talk Show", "Series", "Sports", "Quiz" };
                foreach (string channel in timeTable.Columns.ToList())
                {
                    TimeSpan time = timeTable.StartTime;
                    while (time < timeTable.EndTime)
                    {
                        TimeSpan duration = TimeSpan.FromMinutes(15 * rnd.Next(2, 9));
                        if (time + duration > timeTable.EndTime)
                            duration = timeTable.EndTime - time;

                        string title = titles[rnd.Next(titles.Length)];
                        var cell = timeTable.AddEvent(channel, time, duration);
                        cell.LeftHeadline = $"{(int)time.TotalHours % 24:00}:{time.Minutes:00}";
                        cell.RightHeadline = title;
                        cell.RightText = $"{title} on {channel}, {duration.TotalMinutes} minutes.";

                        time += duration;
                    }
                }*/
            }

            timeTable.ResumeLayout();
        }

        private static List<DownloadItem> GetChannelLogos(string logoPath, T4aChannels channels)
        {
            List<string> channelLogoFiles = channels.Channels.Select(s => Path.Combine(logoPath, $"{s.ChannelId}.png")).ToList();
            List<string> downloadLogoFiles = channelLogoFiles.Where(w => !File.Exists(w)).ToList();
            return downloadLogoFiles.Select(s => new DownloadItem(new Uri($"https://tvfueralle.de/channel-logos/{Path.GetFileName(s)}"), s)).ToList();
        }

        private static List<DownloadItem> GetEventImages(string eventImagePath, T4aRoot[] t4aBroadCasts)
        {
            List<string> eventImageFiles = new List<string>();
            foreach (T4aRoot t4aRoot in t4aBroadCasts)
            {
                eventImageFiles.AddRange(t4aRoot.Events.Where(w => w.Photo?.Url != null)
                    .Select(s => Path.Combine(eventImagePath, Path.GetFileName(s.Photo.Url))));
            }
            eventImageFiles = eventImageFiles.Distinct().ToList();
            List<string> downloadImageFiles = eventImageFiles.Where(w => !File.Exists(w)).ToList();
            if (Directory.Exists(eventImagePath))
            {
                // cleanup old event images that are no longer needed
                string[] files = Directory.GetFiles(eventImagePath);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (!eventImageFiles.Any(a => string.Compare(Path.GetFileName(a), fileName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        File.Delete(file);
                    }
                }
            }
            return downloadImageFiles.Select(s => new DownloadItem(new Uri($"https://tvfueralle.de/api/upload/{Path.GetFileName(s)}"), s)).ToList();
        }

        private string GetDataPath()
        {
            if (string.IsNullOrEmpty(_dataPath))
            {
                _dataPath = Path.Combine(TryGetSolutionDirectoryInfo().FullName, "data");
            }
            return _dataPath;
        }

        private string GetChannelLogoPath()
        {
            return Path.Combine(GetDataPath(), "logo");
        }

        private string GetEventImagePath()
        {
            return Path.Combine(GetDataPath(), "events");
        }

        private void LoadData()
        {
            try
            {
                _t4aBroadCasts.Clear();
                for (int i = -AvailableDaysInPast; i < AvailableDaysInFuture; i++)
                {
                    DateTime currentDate = DateTime.Today.AddDays(i);
                    string date = $"{currentDate.Year}-{currentDate.Month}-{currentDate.Day}";
                    string filePath = Path.Combine(GetDataPath(), $"{date}.json");
                    if(File.Exists(filePath))
                        _t4aBroadCasts.Add(currentDate, T4aRoot.FromJson(File.ReadAllText(filePath)));
                }
                _t4AChannels = T4aChannels.FromJson(File.ReadAllText(Path.Combine(GetDataPath(), "channels.json")));
                // _t4aGenres = T4aGenres.FromJson(File.ReadAllText(Path.Combine(GetDataPath(), "genres.json")));
            }
            catch
            {
                _t4AChannels = null;
                _t4aGenres = null;
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            List<DownloadItem> downloads = new List<DownloadItem>();

            string channelPath = Path.Combine(GetDataPath(), "channels.json");
            if (File.Exists(channelPath))
                downloads.Add(new DownloadItem(new Uri("https://tvfueralle.de/api/channels"), channelPath));

            string genresPath = Path.Combine(GetDataPath(), "genres.json");
            if (File.Exists(genresPath))
                downloads.Add(new DownloadItem(new Uri("https://tvfueralle.de/api/genres"), genresPath));

            for (int i = -AvailableDaysInPast; i < AvailableDaysInFuture; i++)
            {
                DateTime currentDate = DateTime.Today.AddDays(i);
                string date = $"{currentDate.Year}-{currentDate.Month}-{currentDate.Day}";
                string filePath = Path.Combine(GetDataPath(), $"{date}.json");
                if (!File.Exists(filePath))
                {
                    downloads.Add(new DownloadItem(new Uri("https://tvfueralle.de/api/broadcasts/" + date), filePath));
                }
            }
            DownloadForm downloadForm = new DownloadForm(downloads);
            downloadForm.ShowDialog();
            LoadData();
            downloads = new List<DownloadItem>();
            downloads.Clear();
            downloads.AddRange(GetChannelLogos(GetChannelLogoPath(), _t4AChannels));
            downloads.AddRange(GetEventImages(GetEventImagePath(), _t4aBroadCasts.Select(s => s.Value).ToArray()));
            downloadForm = new DownloadForm(downloads);
            downloadForm.ShowDialog();
            ShowData();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Settings are not available yet.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "TeleVision Guide", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
