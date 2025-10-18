using Serilog;
using System.Drawing;
using Newtonsoft.Json;
using Ntk.Chrome.Models;

namespace Ntk.Chrome.Services;

public interface ILogger
{
    void Information(string message);
    void Warning(string message);
    void Error(Exception ex, string message);
    void Debug(string message);
}

public class LoggingService : ILogger
{
    private readonly RichTextBox _logTextBox;
    private readonly ILogger _fileLogger;

    public LoggingService(RichTextBox logTextBox)
    {
        try
        {
            _logTextBox = logTextBox;

            // خواندن تنظیمات
            var settings = File.Exists("settings.json") 
                ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json")) 
                ?? new AppSettings()
                : new AppSettings();

            // گسترش متغیرهای محیطی در مسیر لاگ
            var logPath = Environment.ExpandEnvironmentVariables(settings.LogPath);
            
            // اطمینان از وجود مسیر لاگ
            Directory.CreateDirectory(logPath);

            var fileLogger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(logPath, "app.log"), 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
                
            _fileLogger = new FileLogger(fileLogger);

            Information("سیستم لاگینگ راه‌اندازی شد");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطا در راه‌اندازی سیستم لاگینگ: {ex.Message}", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    public void Information(string message)
    {
        LogWithColor(message, Color.Black, "INFO");
        _fileLogger.Information(message);
    }

    public void Warning(string message)
    {
        LogWithColor(message, Color.Orange, "WARN");
        _fileLogger.Warning(message);
    }

    public void Error(Exception ex, string message)
    {
        LogWithColor($"{message}\n{ex}", Color.Red, "ERROR");
        _fileLogger.Error(ex, message);
    }

    public void Debug(string message)
    {
        LogWithColor(message, Color.Gray, "DEBUG");
        _fileLogger.Debug(message);
    }

    private void LogWithColor(string message, Color color, string level)
    {
        if (_logTextBox.InvokeRequired)
        {
            _logTextBox.Invoke(new Action(() => AddColoredText(message, color, level)));
        }
        else
        {
            AddColoredText(message, color, level);
        }
    }

    private void AddColoredText(string message, Color color, string level)
    {
        _logTextBox.SelectionStart = _logTextBox.TextLength;
        _logTextBox.SelectionLength = 0;

        // Add timestamp
        _logTextBox.SelectionColor = Color.Gray;
        _logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] ");

        // Add level with background color
        _logTextBox.SelectionColor = Color.White;
        _logTextBox.SelectionBackColor = color;
        _logTextBox.AppendText($" {level} ");
        _logTextBox.SelectionBackColor = _logTextBox.BackColor;

        // Add message
        _logTextBox.SelectionColor = color;
        _logTextBox.AppendText($"{message}\n");

        _logTextBox.ScrollToCaret();
    }
}
