namespace TeleVisionGuide
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            headerPanel = new TableLayoutPanel();
            logoLabel = new Label();
            appNameLabel = new Label();
            updateButton = new Button();
            settingsButton = new Button();
            infoButton = new Button();
            daySelector = new DaySelectorControl();
            toolTip = new ToolTip(components);
            timeTable = new TimeTableControl();
            headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.ColumnCount = 6;
            headerPanel.ColumnStyles.Add(new ColumnStyle());
            headerPanel.ColumnStyles.Add(new ColumnStyle());
            headerPanel.ColumnStyles.Add(new ColumnStyle());
            headerPanel.ColumnStyles.Add(new ColumnStyle());
            headerPanel.ColumnStyles.Add(new ColumnStyle());
            headerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerPanel.Controls.Add(logoLabel, 0, 0);
            headerPanel.Controls.Add(appNameLabel, 1, 0);
            headerPanel.Controls.Add(updateButton, 2, 0);
            headerPanel.Controls.Add(settingsButton, 3, 0);
            headerPanel.Controls.Add(infoButton, 4, 0);
            headerPanel.Controls.Add(daySelector, 5, 0);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(8);
            headerPanel.RowCount = 1;
            headerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            headerPanel.Size = new Size(1100, 68);
            headerPanel.TabIndex = 0;
            // 
            // logoLabel
            // 
            logoLabel.Anchor = AnchorStyles.Left;
            logoLabel.Location = new Point(11, 10);
            logoLabel.Name = "logoLabel";
            logoLabel.Size = new Size(48, 48);
            logoLabel.TabIndex = 0;
            logoLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // appNameLabel
            // 
            appNameLabel.Anchor = AnchorStyles.Left;
            appNameLabel.AutoSize = true;
            appNameLabel.Location = new Point(65, 26);
            appNameLabel.Margin = new Padding(3, 0, 24, 0);
            appNameLabel.Name = "appNameLabel";
            appNameLabel.Size = new Size(94, 15);
            appNameLabel.TabIndex = 1;
            appNameLabel.Text = "TeleVision Guide";
            // 
            // updateButton
            // 
            updateButton.AccessibleName = "Update";
            updateButton.Anchor = AnchorStyles.Left;
            updateButton.Location = new Point(186, 11);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(46, 46);
            updateButton.TabIndex = 2;
            updateButton.Click += updateButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.AccessibleName = "Settings";
            settingsButton.Anchor = AnchorStyles.Left;
            settingsButton.Location = new Point(238, 11);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(46, 46);
            settingsButton.TabIndex = 3;
            settingsButton.Click += settingsButton_Click;
            // 
            // infoButton
            // 
            infoButton.AccessibleName = "Info";
            infoButton.Anchor = AnchorStyles.Left;
            infoButton.Location = new Point(290, 11);
            infoButton.Margin = new Padding(3, 3, 24, 3);
            infoButton.Name = "infoButton";
            infoButton.Size = new Size(46, 46);
            infoButton.TabIndex = 4;
            infoButton.Click += infoButton_Click;
            // 
            // daySelector
            // 
            daySelector.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            daySelector.BorderColor = Color.LightGray;
            daySelector.DayBackColor = Color.WhiteSmoke;
            daySelector.DayCount = 12;
            daySelector.HoverBackColor = Color.LightSteelBlue;
            daySelector.Location = new Point(369, 11);
            daySelector.Name = "daySelector";
            daySelector.SelectedBackColor = Color.SteelBlue;
            daySelector.SelectedDate = new DateTime(2026, 10, 11, 0, 0, 0, 0);
            daySelector.SelectedForeColor = Color.White;
            daySelector.Size = new Size(720, 46);
            daySelector.StartDate = new DateTime(2026, 10, 9, 0, 0, 0, 0);
            daySelector.TabIndex = 5;
            // 
            // timeTable
            // 
            timeTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            timeTable.AutoScroll = true;
            timeTable.AutoScrollMinSize = new Size(61, 2191);
            timeTable.BackColor = Color.White;
            timeTable.ColumnWidth = 250;
            timeTable.EndTime = TimeSpan.Parse("1.00:00:00");
            timeTable.GridLineColor = Color.LightGray;
            timeTable.HeaderBackColor = Color.SteelBlue;
            timeTable.HeaderFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            timeTable.HeaderForeColor = Color.White;
            timeTable.HeaderHeight = 30;
            timeTable.HeaderImageSize = new Size(24, 24);
            timeTable.Location = new Point(12, 74);
            timeTable.Name = "timeTable";
            timeTable.RowHeight = 60;
            timeTable.RowInterval = TimeSpan.Parse("00:30:00");
            timeTable.Size = new Size(1076, 614);
            timeTable.StartTime = TimeSpan.Parse("06:00:00");
            timeTable.TabIndex = 1;
            timeTable.TimeColumnWidth = 60;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 700);
            Controls.Add(timeTable);
            Controls.Add(headerPanel);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TeleVision Guide";
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel headerPanel;
        private Label logoLabel;
        private Label appNameLabel;
        private Button updateButton;
        private Button settingsButton;
        private Button infoButton;
        private DaySelectorControl daySelector;
        private ToolTip toolTip;
        private TimeTableControl timeTable;
    }
}
