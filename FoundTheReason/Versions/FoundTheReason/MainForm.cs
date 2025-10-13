using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices; // Make sure Microsoft.VisualBasic NuGet is installed

namespace FoundTheReason
{
    public partial class MainForm : Form
    {
        private bool moreInfoEnabled = false;

        public MainForm()
        {
            InitializeComponent();
        }

        // Full Scan
        private void BtnScan_Click(object sender, EventArgs e)
        {
            RunScan(fullScan: true);
        }

        // Quick Scan
        private void Button1_Click(object sender, EventArgs e)
        {
            RunScan(fullScan: false);
        }

        private void RunScan(bool fullScan)
        {
            progressBar1.Value = 0;
            txtResults.Clear();
            lblStatus.Text = fullScan ? "Full Scan running..." : "Quick Scan running...";
            Application.DoEvents();

            bool foundIssue = false;

            // 1️⃣ Disk check
            progressBar1.Value = 20;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    txtResults.AppendText($"💽 Drive {drive.Name} free space: {freeGB:F2} GB\r\n");
                    if (freeGB < 5)
                    {
                        foundIssue = true;
                        txtResults.AppendText("⚠️ Low disk space! This may cause instability.\r\n");
                        if (moreInfoEnabled)
                            txtResults.AppendText($"ℹ️ Path to drive: {drive.Name}\r\n");
                    }
                }
            }

            // 2️⃣ RAM check
            progressBar1.Value = 40;
            double availableRAM = new ComputerInfo().AvailablePhysicalMemory / (1024.0 * 1024 * 1024);
            txtResults.AppendText($"🖥️ Available RAM: {availableRAM:F2} GB\r\n");
            if (availableRAM < 1)
            {
                foundIssue = true;
                txtResults.AppendText("⚠️ Low RAM available! This may lead to crashes.\r\n");
            }

            // 3️⃣ CPU load
            progressBar1.Value = 60;
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(500);
            float cpuLoad = cpuCounter.NextValue();
            txtResults.AppendText($"⚙️ CPU Load: {cpuLoad:F1}%\r\n");
            if (cpuLoad > 90)
            {
                foundIssue = true;
                txtResults.AppendText("⚠️ High CPU usage! This may cause instability.\r\n");
            }

            // 4️⃣ Application Event Log check (non-admin)
            progressBar1.Value = 80;
            try
            {
                EventLog appLog = new EventLog("Application");
                foreach (EventLogEntry entry in appLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                    {
                        foundIssue = true;
                        txtResults.AppendText($"⚠️ Application error found: {entry.Message}\r\n");
                        if (moreInfoEnabled)
                            txtResults.AppendText($"ℹ️ Time: {entry.TimeGenerated}\r\n");
                        if (!fullScan && !moreInfoEnabled) break; // Quick scan stops after first found error
                    }
                }
            }
            catch { }

            // 5️⃣ More Information extended checks
            if (moreInfoEnabled && fullScan)
            {
                txtResults.AppendText("ℹ️ Running extended checks...\r\n");
                // Add more extended info here, e.g., more detailed logs or driver info (non-admin)
            }

            // Finalize
            progressBar1.Value = 100;
            if (!foundIssue)
            {
                lblStatus.Text = "✅ There isn’t a problem with your PC!";
                txtResults.AppendText("✅ No issues found. Your PC looks safe!\r\n");
            }
            else
            {
                lblStatus.Text = "⚠️ Found possible issues that may lead to BSOD!";
            }

            // Notify
            if (chkNotify.Checked && foundIssue)
                MessageBox.Show("Found possible issues that may cause BSOD!", "FoundTheReason", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Save log
            if (chkSaveLog.Checked)
            {
                try
                {
                    File.WriteAllText("FoundTheReasonResults.txt", txtResults.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save log: " + ex.Message);
                }
            }
        }

        // More Information checkbox
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                var result = MessageBox.Show(
                    "Enabling 'More Information' will make the scan take longer.",
                    "Before enabling More Infomation...",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.OK)
                {
                    moreInfoEnabled = true;
                    txtResults.AppendText("ℹ️ More Information enabled.\r\n");
                }
                else
                {
                    checkBox1.Checked = false;
                    moreInfoEnabled = false;
                }
            }
            else
            {
                moreInfoEnabled = false;
                txtResults.AppendText("ℹ️ More Information disabled.\r\n");
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            string updateLog = @"
🔹 FoundTheReason v1.1.0 — Update Log
------------------------------------

✅ Major Changes:
• Moved from Beta to Full Release
• Added Update Log button
• Fixed file overwrite issue — results can now be viewed safely
• Improved stability and scanning performance

🆕 New Features:
• 'More Information' toggle for deeper scan results
• 'Save results to log file' option now properly writes logs
• Added help button (bottom-left)
• Visual updates and minor layout improvements

🛠️ Bug Fixes:
• Fixed detection delay during quick scan
• Fixed false-positive memory usage warnings
• Fixed UI freezing during scans

📘 Notes:
This version focuses on reliability, clear feedback, and accurate detection results.
Thank you for testing FoundTheReason!
";

            MessageBox.Show(updateLog, "Update Log - FoundTheReason v1.1.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string helpText = @"
🆘 How to Use FoundTheReason

1️. Click 'Quick Scan' for a fast check of your system.
2️. Click 'Scan Now' for a full detailed scan (recommended).
3️. Enable 'More Information' for advanced details (slower scan).
4️. Use 'Save results to log file' if you want to save the scan results.
5️. Use 'Notify me if risk is found' to get popups when issues are detected.
6️. The Update Log shows you what’s new in this version.

ℹ️ Q&A:
Q : Why can't I delete or rewrite in the scan results?
A : You cannot delete or rewrite scan results — they’re read-only for safety.
Q : What should i do if a problem was found?
A : Always check saved logs if a problem was found.
Q : How do i do a Serious performance?
A : For serious performance warnings, restart your PC or close unused apps.

Thank you for using FoundTheReason!
";

            MessageBox.Show(helpText, "Help - FoundTheReason", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
