using System;
using System.Diagnostics;
using System.IO;
using System.Management;           // Needed for driver info
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices; // Needed for ComputerInfo


namespace FoundTheReason
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            txtResults.Clear();
            bool foundIssue = false;

            lblStatus.Text = "Scanning...";
            Application.DoEvents();

            // 1️⃣ Application Event Log (non-admin)
            progressBar1.Value = 20;
            try
            {
                EventLog appLog = new EventLog("Application");
                foreach (EventLogEntry entry in appLog.Entries)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                    {
                        foundIssue = true;
                        txtResults.AppendText($"⚠️ Application error found:\r\n{entry.Message}\r\n\r\n");
                    }
                }
            }
            catch { /* skip if unreadable */ }

            // 2️⃣ Disk check
            progressBar1.Value = 40;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    txtResults.AppendText($"💽 Drive {drive.Name} free space: {freeGB:F2} GB\r\n");
                    if (freeGB < 5) // warning threshold
                    {
                        foundIssue = true;
                        txtResults.AppendText($"⚠️ Low disk space on {drive.Name}! This may cause instability.\r\n\r\n");
                    }
                }
            }

            // 3️⃣ Memory / CPU usage
            progressBar1.Value = 60;
            var availableRAM = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory / (1024.0 * 1024 * 1024);
            txtResults.AppendText($"🖥️ Available RAM: {availableRAM:F2} GB\r\n");
            if (availableRAM < 1)
            {
                foundIssue = true;
                txtResults.AppendText("⚠️ Low RAM available! This may lead to crashes.\r\n\r\n");
            }

            // CPU load (approx)
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // first call always 0
            System.Threading.Thread.Sleep(500);
            float cpuLoad = cpuCounter.NextValue();
            txtResults.AppendText($"⚙️ CPU Load: {cpuLoad:F1}%\r\n");
            if (cpuLoad > 90)
            {
                foundIssue = true;
                txtResults.AppendText("⚠️ High CPU usage! This may cause instability.\r\n\r\n");
            }

            // 4️⃣ Basic driver info (non-admin)
            progressBar1.Value = 80;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string driverName = obj["DeviceName"]?.ToString();
                    string driverStatus = obj["Status"]?.ToString();
                    if (!string.IsNullOrEmpty(driverStatus) && driverStatus != "OK")
                    {
                        foundIssue = true;
                        txtResults.AppendText($"⚠️ Driver issue detected: {driverName} ({driverStatus})\r\n\r\n");
                    }
                }
            }
            catch { /* ignore if access denied */ }

            // Final progress
            progressBar1.Value = 100;

            // 5️⃣ Show status
            if (!foundIssue)
            {
                lblStatus.Text = "✅ There isn’t a problem with your PC!";
                txtResults.AppendText("✅ No issues found. Your PC looks safe!\r\n");
            }
            else
            {
                lblStatus.Text = "⚠️ Found possible issues that may lead to BSOD! See details above.";
            }

            // Notify
            if (chkNotify.Checked && foundIssue)
            {
                MessageBox.Show("Found possible issues that may cause BSOD!", "FoundTheReason", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
    }
}
