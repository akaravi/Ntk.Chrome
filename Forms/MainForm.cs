using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V120.Network;
using Ntk.Chrome.Models;
using Ntk.Chrome.Services;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Ntk.Chrome.Forms;

public partial class MainForm : Form
{
    private readonly ILogger _logger;
    private readonly Services.IChromeDriverService _chromeDriverService;
    private ChromeDriver? _driver;
    private readonly BindingList<RequestInfo> _requests;
    private Button btnStart = null!;
    private Button btnStop = null!;
    private bool _isInitialized;

    public MainForm()
    {
        InitializeComponent();
        _requests = new BindingList<RequestInfo>();
        _logger = new LoggingService(logTextBox);
        var settings = File.Exists("settings.json") 
            ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json")) 
            ?? new AppSettings()
            : new AppSettings();

        _chromeDriverService = new CustomChromeDriverService(
            settings.ChromeDriverPath ?? "ChromeDriver",
            _logger
        );

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            // غیرفعال کردن دکمه‌ها در زمان راه‌اندازی
            if (btnStart != null) btnStart.Enabled = false;
            if (btnStop != null) btnStop.Enabled = false;

            _logger.Information("شروع راه‌اندازی برنامه...");

            // Create necessary directories
            Directory.CreateDirectory("logs");
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "ChromeDriver"));

            await _chromeDriverService.InitializeAsync();
            
            if (!File.Exists("settings.json"))
            {
                _logger.Information("نمایش فرم تنظیمات...");
                using var settingsForm = new SettingsForm();
                if (settingsForm.ShowDialog() != DialogResult.OK)
                {
                    _logger.Warning("تنظیمات لغو شد");
                    Application.Exit();
                    return;
                }
                _logger.Information("تنظیمات با موفقیت ذخیره شد");
            }

            if (!File.Exists("website_settings.json"))
            {
                _logger.Information("نمایش فرم تنظیمات سایت...");
                using var websiteSettingsForm = new WebsiteSettingsForm();
                if (websiteSettingsForm.ShowDialog() != DialogResult.OK)
                {
                    _logger.Warning("تنظیمات سایت لغو شد");
                    Application.Exit();
                    return;
                }
                _logger.Information("تنظیمات سایت با موفقیت ذخیره شد");
            }

            // فعال کردن دکمه شروع پس از راه‌اندازی موفق
            if (btnStart != null) btnStart.Enabled = true;
            _isInitialized = true;
            _logger.Information("راه‌اندازی اولیه با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در راه‌اندازی برنامه");
            MessageBox.Show("خطا در راه‌اندازی برنامه. لطفاً لاگ‌ها را بررسی کنید.", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        if (!_isInitialized)
        {
            MessageBox.Show("لطفاً صبر کنید تا راه‌اندازی اولیه تکمیل شود.", "هشدار",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            // غیرفعال کردن همه دکمه‌ها
            if (btnStart != null) btnStart.Enabled = false;
            if (btnStop != null) btnStop.Enabled = false;

            await InitializeChromeDriverAsync();
            
            // فعال کردن دکمه توقف پس از راه‌اندازی موفق
            if (btnStop != null) btnStop.Enabled = true;
            _logger.Information("برنامه با موفقیت شروع به کار کرد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در شروع برنامه");
            
            // در صورت خطا، فقط دکمه شروع فعال می‌شود
            if (btnStart != null) btnStart.Enabled = true;
            if (btnStop != null) btnStop.Enabled = false;
            
            MessageBox.Show("خطا در راه‌اندازی برنامه. لطفاً لاگ‌ها را بررسی کنید.", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        try
        {
            _chromeDriverService.StopDriver();
            _driver = null;
            _requests.Clear();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            _logger.Information("برنامه متوقف شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در توقف برنامه");
        }
    }

    private async Task InitializeChromeDriverAsync()
    {
        try
        {
            _driver = await _chromeDriverService.CreateDriverAsync();

            var devTools = _driver.GetDevToolsSession();
            var domains = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V120.DevToolsSessionDomains>();
            
            domains.Network.ResponseReceived += (sender, e) => 
            {
                try
                {
                    var request = _requests.FirstOrDefault(r => r.Url == e.Response.Url);
                    if (request != null)
                    {
                        request.ResponseHeaders = e.Response.Headers.ToString();
                        request.StatusCode = (int)e.Response.Status;
                        request.ContentType = e.Response.MimeType;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "خطا در پردازش پاسخ درخواست");
                }
            };

            domains.Network.RequestWillBeSent += (sender, e) =>
            {
                try
                {
                    var request = new RequestInfo
                    {
                        Url = e.Request.Url,
                        Method = e.Request.Method,
                        RequestHeaders = e.Request.Headers.ToString(),
                        Timestamp = DateTime.Now
                    };

                    this.Invoke(() =>
                    {
                        _requests.Add(request);
                        requestsDataGridView.DataSource = _requests;
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "خطا در پردازش درخواست");
                }
            };

            _logger.Information("ChromeDriver با موفقیت راه‌اندازی شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در راه‌اندازی ChromeDriver");
            throw;
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    private RequestInfo? GetSelectedRequest()
    {
        if (requestsDataGridView.SelectedRows.Count > 0)
        {
            return (RequestInfo)requestsDataGridView.SelectedRows[0].DataBoundItem;
        }
        return null;
    }

    private void requestsDataGridView_SelectionChanged(object sender, EventArgs e)
    {
        if (requestsDataGridView.SelectedRows.Count > 0)
        {
            var request = (RequestInfo)requestsDataGridView.SelectedRows[0].DataBoundItem;
            UpdateRequestDetails(request);
        }
    }

    private void UpdateRequestDetails(RequestInfo? request)
    {
        if (request == null)
        {
            requestDetailsTextBox.Text = string.Empty;
            return;
        }

        if (radioButtonText.Checked)
        {
            requestDetailsTextBox.Text = $"URL: {request.Url}\n" +
                                       $"Method: {request.Method}\n" +
                                       $"Status: {request.StatusCode}\n" +
                                       $"Content-Type: {request.ContentType}\n\n" +
                                       $"Request Headers:\n{request.RequestHeaders ?? "-"}\n\n" +
                                       $"Response Headers:\n{request.ResponseHeaders ?? "-"}\n\n" +
                                       $"Response Body:\n{request.ResponseBody ?? "-"}";
        }
        else if (radioButtonJson.Checked)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    request.Url,
                    request.Method,
                    request.StatusCode,
                    request.ContentType,
                    request.RequestHeaders,
                    request.ResponseHeaders,
                    request.ResponseBody
                }, Formatting.Indented);
                requestDetailsTextBox.Text = json;
            }
            catch
            {
                requestDetailsTextBox.Text = "Error formatting as JSON";
            }
        }
        else // HTML
        {
            requestDetailsTextBox.Text = request.ResponseBody ?? string.Empty;
        }
    }
}
