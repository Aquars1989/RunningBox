namespace RunningBox
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.runningBox1 = new RunningBox.SceneStand();
            this.SuspendLayout();
            // 
            // runningBox1
            // 
            this.runningBox1.BackColor = System.Drawing.Color.White;
            this.runningBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runningBox1.EndDelayTicks = 0;
            this.runningBox1.EndDelayLimit = 30;
            this.runningBox1.MainRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.runningBox1.IsEnding = false;
            this.runningBox1.IsStart = false;
            this.runningBox1.Level = 0;
            this.runningBox1.Location = new System.Drawing.Point(0, 0);
            this.runningBox1.Name = "runningBox1";
            this.runningBox1.PlayerObject = null;
            this.runningBox1.RectOfEngery = new System.Drawing.Rectangle(80, 30, 100, 10);
            this.runningBox1.Score = 0;
            this.runningBox1.Size = new System.Drawing.Size(584, 562);
            this.runningBox1.TabIndex = 0;
            this.runningBox1.TrackPoint = new System.Drawing.Point(0, 0);
            this.runningBox1.SceneSlow = 1F;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.runningBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion

        private SceneStand runningBox1;
    }
}

