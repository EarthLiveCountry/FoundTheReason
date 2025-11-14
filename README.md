# FoundTheReason

**FoundTheReason** is a safe Windows diagnostic tool designed to **scan for potential causes of system crashes (BSOD)** without modifying the system or requiring admin rights. It is educational, easy to use, and fully safe for anyone to run.

---

## QUICK NOTE

FoundTheReason is officialy switching the way to install it, as part of the latest upcoming update as the **redesign**, Expected lots of changes.
## Features

- Scan user-accessible **Windows Event Logs** for error patterns.
- Detect **recently installed apps** or programs that may affect stability.
- List **startup programs** that could contribute to crashes.
- Monitor **memory and CPU usage trends** for potential issues.
- Provide a **risk score** and guidance based on findings.
- Export **JSON reports** for easy sharing or review.

---

## How to Use

1. Download the latest release from [GitHub](https://github.com/EarthLiveCountry/FoundTheReason/releases).  
2. Run `FoundTheReason.exe`. **No admin rights are required**.  
3. Click **Scan** to begin.  
4. Review the results and export the report if needed.  

> ⚠️ **Disclaimer:** This tool does not modify your system or trigger crashes. It is for **educational and diagnostic purposes only**.

---

## Risk Score

FoundTheReason calculates a score based on detected issues:

- **0–19**: Low risk — monitor normally.  
- **20–49**: Medium risk — review findings and take precautions.  
- **50–99**: High risk — backup files, review apps and memory usage.  
- **100+**: Immediate attention — seek help from a trusted adult or technician.

---

## Educational Value

This tool is ideal for:

- Learning about BSOD causes and system diagnostics.
- Practicing safe troubleshooting methods.
- Understanding how programs, startup items, and resource usage can affect Windows stability.

---

## Exported Report (JSON)

Reports include:

- Date and time of scan.
- Risk score and summary.
- List of potentially problematic apps or startup programs.
- Memory and CPU usage observations.
- Recommended next steps for safety.

---

## License

**MIT License** — see LICENSE file.  

---

## Contributing

FoundTheReason is open for contributions. If you want to help:

- Test your changes in a **sandbox or VM**.
- Ensure **no destructive actions** are performed.
- Submit pull requests or issues with detailed information.
