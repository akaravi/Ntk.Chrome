using OpenQA.Selenium.Chrome;
using Ntk.Chrome.Models;
using Ntk.Chrome.Services;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Text;

namespace Ntk.Chrome.Forms;

/// <summary>
/// فرم اصلی برنامه که فقط مسئول نمایش و تعامل با کاربر است
/// تمام منطق‌های غیر نمایشی به سرویس‌ها منتقل شده‌اند
/// </summary>
public partial class MainForm : Form
{
    #region Fields and Properties
    
    /// <summary>
    /// سرویس لاگ‌گیری برای ثبت رویدادها و خطاها
    /// </summary>
    private readonly ILogger _logger;
    
    /// <summary>
    /// سرویس مدیریت ChromeDriver برای کنترل مرورگر
    /// </summary>
    private readonly IChromeDriverService _chromeDriverService;
    
    /// <summary>
    /// سرویس مدیریت شبکه و درخواست‌ها
    /// </summary>
    private readonly INetworkService _networkService;
    
    /// <summary>
    /// نمونه ChromeDriver برای کنترل مرورگر
    /// </summary>
    private ChromeDriver? _driver;
    
    /// <summary>
    /// لیست درخواست‌های شبکه که در جدول نمایش داده می‌شوند
    /// </summary>
    private readonly BindingList<RequestInfo> _requests;
    
    /// <summary>
    /// دکمه شروع برنامه
    /// </summary>
    private Button btnStart = null!;
    
    /// <summary>
    /// دکمه توقف برنامه
    /// </summary>
    private Button btnStop = null!;
    
    /// <summary>
    /// نشانگر وضعیت راه‌اندازی اولیه برنامه
    /// </summary>
    private bool _isInitialized;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// سازنده فرم اصلی
    /// تنظیمات اولیه، بارگذاری تنظیمات و راه‌اندازی سرویس‌ها
    /// </summary>
    public MainForm()
    {
        // راه‌اندازی کامپوننت‌های فرم
        InitializeComponent();
        
        // ایجاد لیست درخواست‌ها برای نمایش در جدول
        _requests = new BindingList<RequestInfo>();
        
        // راه‌اندازی سرویس لاگ‌گیری
        _logger = new LoggingService(logTextBox);
        
        // راه‌اندازی سرویس شبکه
        _networkService = new NetworkService(_logger);
        
        // بارگذاری تنظیمات از فایل JSON یا ایجاد تنظیمات پیش‌فرض
        var settings = File.Exists("settings.json") 
            ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json")) 
            ?? new AppSettings()
            : new AppSettings();

        // راه‌اندازی سرویس ChromeDriver
        _chromeDriverService = new CustomChromeDriverService(
            settings.ChromeDriverPath ?? "ChromeDriver",
            _logger
        );

        // تنظیم رویدادهای سرویس شبکه
        SetupNetworkServiceEvents();

        // شروع فرآیند راه‌اندازی اولیه
        InitializeAsync();
    }
    
    #endregion

    #region Initialization Methods
    
    /// <summary>
    /// راه‌اندازی اولیه برنامه
    /// ایجاد پوشه‌های لازم، بارگذاری تنظیمات و آماده‌سازی محیط
    /// </summary>
    private async void InitializeAsync()
    {
        // جلوگیری از راه‌اندازی مجدد
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

            // ایجاد پوشه‌های لازم
            Directory.CreateDirectory("logs");
            Directory.CreateDirectory(Path.Combine(Application.StartupPath, "ChromeDriver"));

            // راه‌اندازی سرویس ChromeDriver
            await _chromeDriverService.InitializeAsync();
            
            // بررسی وجود فایل تنظیمات و نمایش فرم تنظیمات در صورت عدم وجود
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

            // بررسی وجود فایل تنظیمات سایت و نمایش فرم تنظیمات سایت در صورت عدم وجود
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
    
    #endregion

    #region Event Handlers
    
    /// <summary>
    /// رویداد کلیک دکمه شروع
    /// راه‌اندازی ChromeDriver، شروع فرآیند پر کردن فرم و مدیریت خطاها
    /// </summary>
    /// <param name="sender">فرستنده رویداد</param>
    /// <param name="e">اطلاعات رویداد</param>
    private async void btnStart_Click(object sender, EventArgs e)
    {
        // بررسی وضعیت راه‌اندازی اولیه
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

            // راه‌اندازی ChromeDriver
            await InitializeChromeDriverAsync();
            
            // شروع فرآیند پر کردن فرم
            _logger.Information("شروع فرآیند پر کردن فرم...");
            var formSuccess = await _chromeDriverService.FillFormAsync();
            
            // بررسی نتیجه پر کردن فرم
            if (formSuccess)
            {
                _logger.Information("فرم با موفقیت پر شد");
                MessageBox.Show("فرم با موفقیت پر شد!", "موفقیت", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _logger.Warning("پر کردن فرم ناموفق");
                MessageBox.Show("پر کردن فرم ناموفق بود. لطفاً اطلاعات ورودی را بررسی کنید.", "خطا", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
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

    /// <summary>
    /// رویداد کلیک دکمه توقف
    /// متوقف کردن ChromeDriver و پاک کردن داده‌ها
    /// </summary>
    /// <param name="sender">فرستنده رویداد</param>
    /// <param name="e">اطلاعات رویداد</param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        try
        {
            // توقف ChromeDriver
            _chromeDriverService.StopDriver();
            _driver = null;
            
            // پاک کردن لیست درخواست‌ها
            _networkService.ClearRequests();
            _requests.Clear();
            
            // تنظیم وضعیت دکمه‌ها
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            
            _logger.Information("برنامه متوقف شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در توقف برنامه");
        }
    }

    /// <summary>
    /// رویداد بسته شدن فرم
    /// پاک کردن منابع ChromeDriver
    /// </summary>
    /// <param name="sender">فرستنده رویداد</param>
    /// <param name="e">اطلاعات رویداد</param>
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // بستن و آزاد کردن منابع ChromeDriver
        _driver?.Quit();
        _driver?.Dispose();
    }
    
    #endregion

    #region ChromeDriver Management
    
    /// <summary>
    /// راه‌اندازی ChromeDriver و تنظیم سرویس شبکه
    /// </summary>
    /// <returns>Task برای عملیات ناهمزمان</returns>
    private async Task InitializeChromeDriverAsync()
    {
        try
        {
            // ایجاد نمونه ChromeDriver
            _driver = await _chromeDriverService.CreateDriverAsync();

            // راه‌اندازی سرویس شبکه
            await _networkService.InitializeAsync(_driver);

            _logger.Information("ChromeDriver با موفقیت راه‌اندازی شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در راه‌اندازی ChromeDriver");
            throw;
        }
    }
    
    #endregion

    #region Network Service Events
    
    /// <summary>
    /// تنظیم رویدادهای سرویس شبکه
    /// </summary>
    private void SetupNetworkServiceEvents()
    {
        // رویداد اضافه شدن درخواست جدید
        _networkService.RequestAdded += (sender, request) =>
        {
            this.Invoke(() =>
            {
                _requests.Add(request);
                requestsDataGridView.DataSource = _requests;
            });
        };

        // رویداد به‌روزرسانی درخواست
        _networkService.RequestUpdated += (sender, request) =>
        {
            this.Invoke(() =>
            {
                // به‌روزرسانی نمایش جدول
                requestsDataGridView.Refresh();
            });
        };
    }
    
    #endregion

    #region Request Management
    
    /// <summary>
    /// دریافت درخواست انتخاب شده از جدول
    /// </summary>
    /// <returns>درخواست انتخاب شده یا null در صورت عدم انتخاب</returns>
    private RequestInfo? GetSelectedRequest()
    {
        if (requestsDataGridView.SelectedRows.Count > 0)
        {
            return (RequestInfo)requestsDataGridView.SelectedRows[0].DataBoundItem;
        }
        return null;
    }

    /// <summary>
    /// رویداد تغییر انتخاب در جدول درخواست‌ها
    /// به‌روزرسانی نمایش جزئیات درخواست انتخاب شده
    /// </summary>
    /// <param name="sender">فرستنده رویداد</param>
    /// <param name="e">اطلاعات رویداد</param>
    private void requestsDataGridView_SelectionChanged(object sender, EventArgs e)
    {
        if (requestsDataGridView.SelectedRows.Count > 0)
        {
            var request = (RequestInfo)requestsDataGridView.SelectedRows[0].DataBoundItem;
            UpdateRequestDetails(request);
        }
    }

    /// <summary>
    /// به‌روزرسانی نمایش جزئیات درخواست
    /// نمایش اطلاعات کامل درخواست و پاسخ در فرمت‌های مختلف
    /// </summary>
    /// <param name="request">درخواست برای نمایش جزئیات</param>
    private void UpdateRequestDetails(RequestInfo? request)
    {
        // بررسی وجود درخواست
        if (request == null)
        {
            requestDetailsTextBox.Text = string.Empty;
            return;
        }

        // نمایش در فرمت متنی
        if (radioButtonText.Checked)
        {
            var details = new StringBuilder();
            
            // بخش درخواست
            details.AppendLine($"=== REQUEST ===");
            details.AppendLine($"URL: {request.Url}");
            details.AppendLine($"Method: {request.Method}");
            details.AppendLine($"Timestamp: {request.Timestamp:yyyy-MM-dd HH:mm:ss}");
            details.AppendLine($"Request Headers:");
            details.AppendLine(request.RequestHeaders ?? "-");
            if (!string.IsNullOrEmpty(request.RequestBody))
            {
                details.AppendLine($"Request Body:");
                details.AppendLine(request.RequestBody);
            }
            
            // بخش پاسخ
            details.AppendLine($"\n=== RESPONSE ===");
            details.AppendLine($"Status: {request.StatusCode}");
            details.AppendLine($"Content-Type: {request.ContentType}");
            details.AppendLine($"Response Headers:");
            details.AppendLine(request.ResponseHeaders ?? "-");
            if (!string.IsNullOrEmpty(request.ResponseBody))
            {
                details.AppendLine($"Response Body:");
                details.AppendLine(request.ResponseBody);
            }
            
            requestDetailsTextBox.Text = details.ToString();
        }
        // نمایش در فرمت JSON
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
                    request.Timestamp,
                    RequestHeaders = request.RequestHeaders,
                    RequestBody = request.RequestBody,
                    ResponseHeaders = request.ResponseHeaders,
                    ResponseBody = request.ResponseBody
                }, Formatting.Indented);
                requestDetailsTextBox.Text = json;
            }
            catch
            {
                requestDetailsTextBox.Text = "Error formatting as JSON";
            }
        }
        // نمایش در فرمت HTML
        else // HTML
        {
            requestDetailsTextBox.Text = request.ResponseBody ?? string.Empty;
        }
    }
    
    #endregion
}