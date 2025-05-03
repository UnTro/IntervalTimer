using System;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace IntervalTimer
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.NumericUpDown minutesNumeric;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button testSoundButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label minutesLabel;

        private System.Windows.Forms.Timer uiUpdateTimer;
        private Thread timerThread;
        private bool isRunning = false;
        private DateTime nextBeepTime;

        public Form1()
        {
            FormInitializeComponent();
        }

        private void FormInitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.minutesLabel = new System.Windows.Forms.Label();
            this.minutesNumeric = new System.Windows.Forms.NumericUpDown();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.testSoundButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.uiUpdateTimer = new System.Windows.Forms.Timer();

            ((System.ComponentModel.ISupportInitialize)(this.minutesNumeric)).BeginInit();
            this.SuspendLayout();

            // titleLabel
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.Location = new System.Drawing.Point(85, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(130, 25);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Interval Timer";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // minutesLabel
            this.minutesLabel.AutoSize = true;
            this.minutesLabel.Location = new System.Drawing.Point(50, 70);
            this.minutesLabel.Name = "minutesLabel";
            this.minutesLabel.Size = new System.Drawing.Size(90, 15);
            this.minutesLabel.TabIndex = 1;
            this.minutesLabel.Text = "Minutes:";

            // minutesNumeric
            this.minutesNumeric.Location = new System.Drawing.Point(150, 68);
            this.minutesNumeric.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.minutesNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.minutesNumeric.Name = "minutesNumeric";
            this.minutesNumeric.Size = new System.Drawing.Size(80, 23);
            this.minutesNumeric.TabIndex = 2;
            this.minutesNumeric.Value = new decimal(new int[] { 3, 0, 0, 0 });

            // startButton
            this.startButton.Location = new System.Drawing.Point(50, 110);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(80, 30);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start Timer";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);

            // stopButton
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(150, 110);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 30);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop Timer";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);

            // testSoundButton
            this.testSoundButton.Location = new System.Drawing.Point(100, 150);
            this.testSoundButton.Name = "testSoundButton";
            this.testSoundButton.Size = new System.Drawing.Size(80, 30);
            this.testSoundButton.TabIndex = 5;
            this.testSoundButton.Text = "Test Sound";
            this.testSoundButton.UseVisualStyleBackColor = true;
            this.testSoundButton.Click += new System.EventHandler(this.TestSoundButton_Click);

            // statusLabel
            this.statusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.Location = new System.Drawing.Point(0, 195);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(284, 23);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // uiUpdateTimer
            this.uiUpdateTimer.Interval = 500;
            this.uiUpdateTimer.Tick += new System.EventHandler(this.UiUpdateTimer_Tick);

            // Form1
            this.ClientSize = new System.Drawing.Size(284, 218);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.testSoundButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.minutesNumeric);
            this.Controls.Add(this.minutesLabel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interval Timer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

            ((System.ComponentModel.ISupportInitialize)(this.minutesNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            int intervalMinutes = (int)minutesNumeric.Value;
            if (intervalMinutes <= 0)
            {
                MessageBox.Show("Please enter a positive number of minutes.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isRunning = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            nextBeepTime = DateTime.Now.AddMinutes(intervalMinutes);
            statusLabel.Text = "Timer running...";

            // Start the timer thread
            timerThread = new Thread(TimerLoop);
            timerThread.IsBackground = true;
            timerThread.Start(intervalMinutes);

            // Start UI update timer
            uiUpdateTimer.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopTimer();
        }

        private void StopTimer()
        {
            isRunning = false;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            statusLabel.Text = "Timer stopped";
            uiUpdateTimer.Stop();
        }

        private void TestSoundButton_Click(object sender, EventArgs e)
        {
            PlaySound();
        }

        private void TimerLoop(object state)
        {
            int intervalMinutes = (int)state;
            int intervalMs = intervalMinutes * 60 * 1000;

            while (isRunning)
            {
                Thread.Sleep(intervalMs);

                if (isRunning)
                {
                    PlaySound();

                    // Update for next interval
                    nextBeepTime = DateTime.Now.AddMinutes(intervalMinutes);
                }
            }
        }

        private void PlaySound()
        {
            try
            {
                // Play system sound
                SystemSounds.Asterisk.Play();

                // Update status from the UI thread
                this.Invoke(new Action(() => {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");
                    statusLabel.Text = $"Sound played at {currentTime}";
                }));
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                this.Invoke(new Action(() => {
                    statusLabel.Text = $"Error playing sound: {ex.Message}";
                }));
            }
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                TimeSpan remaining = nextBeepTime - DateTime.Now;
                if (remaining.TotalSeconds > 0)
                {
                    string remainingText = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                    statusLabel.Text = $"Next beep in: {remainingText}";
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Make sure to stop the timer thread when closing
            isRunning = false;
            if (timerThread != null && timerThread.IsAlive)
            {
                timerThread.Join(100);
            }
        }
    }
}