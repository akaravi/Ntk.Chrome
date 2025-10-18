using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V120.Network;
using Ntk.Chrome.Models;
using System.ComponentModel;

namespace Ntk.Chrome.Services;

/// <summary>
/// سرویس مدیریت شبکه و درخواست‌های HTTP
/// این سرویس مسئول لاگ‌گیری، ذخیره و مدیریت درخواست‌های شبکه است
/// </summary>
public class NetworkService : INetworkService
{
    #region Fields and Properties
    
    /// <summary>
    /// سرویس لاگ‌گیری برای ثبت رویدادها
    /// </summary>
    private readonly ILogger _logger;
    
    /// <summary>
    /// نمونه ChromeDriver برای دسترسی به DevTools
    /// </summary>
    private ChromeDriver? _driver;
    
    /// <summary>
    /// لیست درخواست‌های شبکه
    /// </summary>
    private readonly BindingList<RequestInfo> _requests;
    
    /// <summary>
    /// رویداد تغییر درخواست‌ها برای اطلاع‌رسانی به UI
    /// </summary>
    public event EventHandler<RequestInfo>? RequestAdded;
    
    /// <summary>
    /// رویداد به‌روزرسانی درخواست برای اطلاع‌رسانی به UI
    /// </summary>
    public event EventHandler<RequestInfo>? RequestUpdated;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// سازنده سرویس شبکه
    /// </summary>
    /// <param name="logger">سرویس لاگ‌گیری</param>
    public NetworkService(ILogger logger)
    {
        _logger = logger;
        _requests = new BindingList<RequestInfo>();
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// راه‌اندازی سرویس شبکه با ChromeDriver
    /// </summary>
    /// <param name="driver">نمونه ChromeDriver</param>
    public async Task InitializeAsync(ChromeDriver driver)
    {
        try
        {
            _driver = driver;
            
            // دریافت DevTools session
            var devTools = _driver.GetDevToolsSession();
            var domains = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V120.DevToolsSessionDomains>();
            
            // فعال کردن Network domain
            await domains.Network.Enable(new OpenQA.Selenium.DevTools.V120.Network.EnableCommandSettings());
            
            // تنظیم رویداد دریافت پاسخ
            domains.Network.ResponseReceived += async (sender, e) => 
            {
                await HandleResponseReceived(e);
            };

            // تنظیم رویداد ارسال درخواست
            domains.Network.RequestWillBeSent += (sender, e) =>
            {
                HandleRequestWillBeSent(e);
            };

            _logger.Information("سرویس شبکه با موفقیت راه‌اندازی شد");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در راه‌اندازی سرویس شبکه");
            throw;
        }
    }

    /// <summary>
    /// دریافت لیست درخواست‌ها
    /// </summary>
    /// <returns>لیست درخواست‌های شبکه</returns>
    public BindingList<RequestInfo> GetRequests()
    {
        return _requests;
    }

    /// <summary>
    /// پاک کردن تمام درخواست‌ها
    /// </summary>
    public void ClearRequests()
    {
        _requests.Clear();
        _logger.Information("لیست درخواست‌ها پاک شد");
    }

    /// <summary>
    /// دریافت درخواست بر اساس URL
    /// </summary>
    /// <param name="url">آدرس URL</param>
    /// <returns>درخواست مربوطه یا null</returns>
    public RequestInfo? GetRequestByUrl(string url)
    {
        return _requests.FirstOrDefault(r => r.Url == url);
    }

    /// <summary>
    /// دریافت درخواست‌های فیلتر شده بر اساس وضعیت
    /// </summary>
    /// <param name="statusCode">کد وضعیت HTTP</param>
    /// <returns>لیست درخواست‌های فیلتر شده</returns>
    public List<RequestInfo> GetRequestsByStatus(int statusCode)
    {
        return _requests.Where(r => r.StatusCode == statusCode).ToList();
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// مدیریت رویداد ارسال درخواست
    /// </summary>
    /// <param name="e">اطلاعات رویداد درخواست</param>
    private void HandleRequestWillBeSent(RequestWillBeSentEventArgs e)
    {
        try
        {
            // ایجاد رکورد درخواست جدید
            var request = new RequestInfo
            {
                Url = e.Request.Url,
                Method = e.Request.Method,
                RequestHeaders = FormatHeaders(e.Request.Headers),
                RequestBody = e.Request.PostData ?? string.Empty,
                Timestamp = DateTime.Now
            };

            // اضافه کردن به لیست
            _requests.Add(request);
            
            // اطلاع‌رسانی به UI
            RequestAdded?.Invoke(this, request);
            
            // فقط آدرس صفحات اصلی را در لاگ نمایش دهید
            if (IsMainPageRequest(e.Request.Url))
            {
                _logger.Information($"صفحه بارگذاری شد: {e.Request.Url}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در پردازش درخواست");
        }
    }

    /// <summary>
    /// مدیریت رویداد دریافت پاسخ
    /// </summary>
    /// <param name="e">اطلاعات رویداد پاسخ</param>
    private async Task HandleResponseReceived(ResponseReceivedEventArgs e)
    {
        try
        {
            // پیدا کردن درخواست مربوطه
            var request = _requests.FirstOrDefault(r => r.Url == e.Response.Url);
            if (request != null)
            {
                // به‌روزرسانی اطلاعات پاسخ
                request.ResponseHeaders = FormatResponseHeaders(e.Response.Headers);
                request.StatusCode = (int)e.Response.Status;
                request.ContentType = e.Response.MimeType;
                
                // اطلاع‌رسانی اولیه به UI
                RequestUpdated?.Invoke(this, request);
                
                // دریافت Response Body با تاخیر
                _ = Task.Run(async () =>
                {
                    await Task.Delay(100); // تاخیر کوتاه برای اطمینان از تکمیل پاسخ
                    await GetResponseBodyAsync(request, e.RequestId);
                    
                    // اطلاع‌رسانی مجدد به UI پس از دریافت Response Body
                    RequestUpdated?.Invoke(this, request);
                });
                
                // فقط پاسخ صفحات اصلی را در لاگ نمایش دهید
                if (IsMainPageRequest(e.Response.Url))
                {
                    _logger.Information($"پاسخ دریافت شد: {request.StatusCode} {request.Url}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در پردازش پاسخ");
        }
    }

    /// <summary>
    /// دریافت محتوای پاسخ
    /// </summary>
    /// <param name="request">درخواست برای به‌روزرسانی</param>
    /// <param name="requestId">شناسه درخواست</param>
    private async Task GetResponseBodyAsync(RequestInfo request, string requestId)
    {
        try
        {
            if (_driver != null)
            {
                var devTools = _driver.GetDevToolsSession();
                if (devTools != null)
                {
                    var domains = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V120.DevToolsSessionDomains>();
                    var network = domains.Network;
                    
                    if (network != null)
                    {
                        // فعال کردن Network domain اگر فعال نباشد
                        await network.Enable(new OpenQA.Selenium.DevTools.V120.Network.EnableCommandSettings());
                        
                        // دریافت Response Body
                        var responseBody = await network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = requestId });
                        if (responseBody != null && !string.IsNullOrEmpty(responseBody.Body))
                        {
                            request.ResponseBody = responseBody.Body;
                            _logger.Debug($"Response Body دریافت شد برای: {request.Url}");
                        }
                        else
                        {
                            _logger.Debug($"Response Body خالی است برای: {request.Url}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning($"خطا در دریافت Response Body برای {request.Url}: {ex.Message}");
        }
    }

    /// <summary>
    /// بررسی اینکه آیا درخواست مربوط به صفحه اصلی است یا خیر
    /// </summary>
    /// <param name="url">آدرس URL</param>
    /// <returns>true اگر درخواست مربوط به صفحه اصلی باشد</returns>
    private bool IsMainPageRequest(string url)
    {
        // فیلتر کردن درخواست‌های مربوط به منابع (CSS, JS, Images, etc.)
        var resourceExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf" };
        var resourcePaths = new[] { "/api/", "/assets/", "/static/", "/css/", "/js/", "/images/", "/fonts/" };
        
        // بررسی پسوند فایل
        if (resourceExtensions.Any(ext => url.ToLower().Contains(ext)))
            return false;
            
        // بررسی مسیرهای منابع
        if (resourcePaths.Any(path => url.ToLower().Contains(path)))
            return false;
            
        // بررسی درخواست‌های XHR/Fetch
        if (url.Contains("/api/") || url.Contains("/ajax/") || url.Contains("/fetch/"))
            return false;
            
        return true;
    }

    /// <summary>
    /// فرمت کردن headers به صورت قابل خواندن
    /// </summary>
    /// <param name="headers">headers خام</param>
    /// <returns>headers فرمت شده</returns>
    private string FormatHeaders(OpenQA.Selenium.DevTools.V120.Network.Headers headers)
    {
        if (headers == null)
            return string.Empty;
            
        var headerList = new List<string>();
        
        // استفاده از reflection برای دریافت تمام properties
        var properties = headers.GetType().GetProperties();
        foreach (var property in properties)
        {
            try
            {
                var value = property.GetValue(headers);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    headerList.Add($"{property.Name}: {value}");
                }
            }
            catch
            {
                // نادیده گرفتن خطاهای reflection
            }
        }
        
        return headerList.Count > 0 ? string.Join("\n", headerList) : "No headers available";
    }

    /// <summary>
    /// فرمت کردن response headers به صورت قابل خواندن
    /// </summary>
    /// <param name="headers">response headers خام</param>
    /// <returns>response headers فرمت شده</returns>
    private string FormatResponseHeaders(OpenQA.Selenium.DevTools.V120.Network.Headers headers)
    {
        if (headers == null)
            return string.Empty;
            
        var headerList = new List<string>();
        
        // استفاده از reflection برای دریافت تمام properties
        var properties = headers.GetType().GetProperties();
        foreach (var property in properties)
        {
            try
            {
                var value = property.GetValue(headers);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    headerList.Add($"{property.Name}: {value}");
                }
            }
            catch
            {
                // نادیده گرفتن خطاهای reflection
            }
        }
        
        return headerList.Count > 0 ? string.Join("\n", headerList) : "No headers available";
    }
    
    #endregion
}

/// <summary>
/// رابط سرویس شبکه
/// </summary>
public interface INetworkService
{
    /// <summary>
    /// رویداد اضافه شدن درخواست جدید
    /// </summary>
    event EventHandler<RequestInfo>? RequestAdded;
    
    /// <summary>
    /// رویداد به‌روزرسانی درخواست
    /// </summary>
    event EventHandler<RequestInfo>? RequestUpdated;
    
    /// <summary>
    /// راه‌اندازی سرویس شبکه
    /// </summary>
    /// <param name="driver">نمونه ChromeDriver</param>
    Task InitializeAsync(ChromeDriver driver);
    
    /// <summary>
    /// دریافت لیست درخواست‌ها
    /// </summary>
    /// <returns>لیست درخواست‌های شبکه</returns>
    BindingList<RequestInfo> GetRequests();
    
    /// <summary>
    /// پاک کردن تمام درخواست‌ها
    /// </summary>
    void ClearRequests();
    
    /// <summary>
    /// دریافت درخواست بر اساس URL
    /// </summary>
    /// <param name="url">آدرس URL</param>
    /// <returns>درخواست مربوطه یا null</returns>
    RequestInfo? GetRequestByUrl(string url);
    
    /// <summary>
    /// دریافت درخواست‌های فیلتر شده بر اساس وضعیت
    /// </summary>
    /// <param name="statusCode">کد وضعیت HTTP</param>
    /// <returns>لیست درخواست‌های فیلتر شده</returns>
    List<RequestInfo> GetRequestsByStatus(int statusCode);
}
