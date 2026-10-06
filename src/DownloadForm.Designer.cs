namespace TeleVisionGuide
{
    partial class DownloadForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _cts?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            labelStatus = new Label();
            progressBarFile = new ProgressBar();
            labelOverall = new Label();
            progressBarOverall = new ProgressBar();
            buttonAbort = new Button();
            SuspendLayout();
            // 
            // labelStatus
            // 
            labelStatus.AutoEllipsis = true;
            labelStatus.Location = new Point(12, 9);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(460, 20);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "Preparing download...";
            // 
            // progressBarFile
            // 
            progressBarFile.Location = new Point(12, 32);
            progressBarFile.Name = "progressBarFile";
            progressBarFile.Size = new Size(460, 23);
            progressBarFile.TabIndex = 1;
            // 
            // labelOverall
            // 
            labelOverall.Location = new Point(12, 64);
            labelOverall.Name = "labelOverall";
            labelOverall.Size = new Size(460, 20);
            labelOverall.TabIndex = 2;
            labelOverall.Text = "Overall progress";
            // 
            // progressBarOverall
            // 
            progressBarOverall.Location = new Point(12, 87);
            progressBarOverall.Name = "progressBarOverall";
            progressBarOverall.Size = new Size(460, 23);
            progressBarOverall.TabIndex = 3;
            // 
            // buttonAbort
            // 
            buttonAbort.Location = new Point(372, 122);
            buttonAbort.Name = "buttonAbort";
            buttonAbort.Size = new Size(100, 28);
            buttonAbort.TabIndex = 4;
            buttonAbort.Text = "Abort";
            buttonAbort.UseVisualStyleBackColor = true;
            buttonAbort.Click += buttonAbort_Click;
            // 
            // DownloadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 162);
            Controls.Add(buttonAbort);
            Controls.Add(progressBarOverall);
            Controls.Add(labelOverall);
            Controls.Add(progressBarFile);
            Controls.Add(labelStatus);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DownloadForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Download";
            ResumeLayout(false);
        }

        #endregion

        private Label labelStatus;
        private ProgressBar progressBarFile;
        private Label labelOverall;
        private ProgressBar progressBarOverall;
        private Button buttonAbort;
    }
}
