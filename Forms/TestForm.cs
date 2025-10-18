using Newtonsoft.Json;
using Ntk.Chrome.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V118.Runtime;
using OpenQA.Selenium.DevTools.V120.Network;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ntk.Chrome.Forms;

public partial class TestForm : Form
{
    private readonly Services.ILogger _logger;
    private readonly Services.IChromeDriverService _chromeDriverService;
    
    // متغیرهای ذخیره اطلاعات برای تست دستی
    private string? _lastAuthorizationValue = null;
    private string? _lastCustomUrl = null;
    private string? _lastRequestHeaders = null;

    public TestForm(Services.ILogger logger, Services.IChromeDriverService chromeDriverService)
    {
        _logger = logger;
        _chromeDriverService = chromeDriverService;
        InitializeComponent();
        
        // نمایش اطلاعات درخواست
        txtUrl.Text = "https://b2b.isaco.ir/login";
        txtParams.Text = "Scus: aa\nUserName: aa\nPassword: aa";
        
        // تنظیم مقدار پیش‌فرض برای آدرس سفارشی
        txtCustomUrl.Text = "https://b2b.isaco.ir/api/Srv/Emdad/EmdadVijehSt";
    }

    private async void btnTest_Click(object sender, EventArgs e)
    {
        ChromeDriver? driver = null;
        try
        {
            _logger.Information("شروع تست ورود به سایت...");
            txtResult.Text = "در حال باز کردن صفحه لاگین...\n";
            
            // پارامترهای ثابت مطابق درخواست شما
            var loginData = new
            {
                Scus = "4002016",
                UserName = "1287677843", 
                Password = "Am7677843"
            };
            
            string loginUrl = "https://b2b.isaco.ir/login";
            
            // دریافت ChromeDriver از سرویس اصلی
            driver = await _chromeDriverService.CreateDriverAsync();
            
            // دریافت DevTools session
            var devTools = driver.GetDevToolsSession();
            var domains = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V120.DevToolsSessionDomains>();
            
            // فعال کردن Network domain
            await domains.Network.Enable(new OpenQA.Selenium.DevTools.V120.Network.EnableCommandSettings());
            
            // متغیر برای ذخیره پاسخ
            string? responseBody = null;
            int statusCode = 0;
            string? responseHeaders = null;
            
            // متغیر برای ذخیره اطلاعات آدرس سفارشی
            string? customRequestData = null;
            string? customResponseData = null;
            string? customRequestHeaders = null;
            string? customResponseHeaders = null;
            int customStatusCode = 0;
            
            // تنظیم رویداد ارسال درخواست
            domains.Network.RequestWillBeSent += (sender, e) => 
            {
                var customUrl = txtCustomUrl.Text.Trim();
                if (!string.IsNullOrEmpty(customUrl) && e.Request.Url.Contains(customUrl))
                {
                    customRequestHeaders = FormatHeaders(e.Request.Headers);
                    if (!string.IsNullOrEmpty(e.Request.PostData))
                    {
                        customRequestData = e.Request.PostData;
                    }
                    _logger.Information($"درخواست به آدرس سفارشی پیدا شد: {e.Request.Url}");
                }
            };
            
            // تنظیم رویداد دریافت پاسخ
            domains.Network.ResponseReceived += async (sender, e) => 
            {
                var customUrl = txtCustomUrl.Text.Trim();
                if (!string.IsNullOrEmpty(customUrl) && e.Response.Url.Contains(customUrl))
                {
                    customStatusCode = (int)e.Response.Status;
                    customResponseHeaders = FormatHeaders(e.Response.Headers);
                    
                    // دریافت Response Body
                    try
                    {
                        var bodyResponse = await domains.Network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = e.RequestId });
                        customResponseData = bodyResponse.Body;
                        _logger.Information($"پاسخ از آدرس سفارشی دریافت شد: {e.Response.Url}");
                    }
                    catch (Exception ex)
                    {
                        customResponseData = $"خطا در دریافت Response Body: {ex.Message}";
                    }
                }
                
                // همچنین اطلاعات کلی را ذخیره کن
                if (e.Response.Url.Contains("b2b.isaco.ir"))
                {
                    statusCode = (int)e.Response.Status;
                    responseHeaders = FormatHeaders(e.Response.Headers);
                    
                    try
                    {
                        var bodyResponse = await domains.Network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = e.RequestId });
                        responseBody = bodyResponse.Body;
                    }
                    catch (Exception ex)
                    {
                        responseBody = $"خطا در دریافت Response Body: {ex.Message}";
                    }
                }
            };

            // رفتن به صفحه لاگین
            txtResult.Text += "در حال بارگذاری صفحه...\n";
            driver.Navigate().GoToUrl(loginUrl);
            await Task.Delay(3000);
            
            txtResult.Text += "صفحه بارگذاری شد. در حال پر کردن فرم...\n";
            
            // پر کردن فرم لاگین
            try
            {
                // پیدا کردن فیلد Scus
                var scusField = driver.FindElement(By.Name("Scus"));
                scusField.Clear();
                scusField.SendKeys(loginData.Scus);
                txtResult.Text += $"فیلد Scus پر شد: {loginData.Scus}\n";
                
                // پیدا کردن فیلد UserName
                var usernameField = driver.FindElement(By.Name("UserName"));
                usernameField.Clear();
                usernameField.SendKeys(loginData.UserName);
                txtResult.Text += $"فیلد UserName پر شد: {loginData.UserName}\n";
                
                // پیدا کردن فیلد Password
                var passwordField = driver.FindElement(By.Name("Password"));
                passwordField.Clear();
                passwordField.SendKeys(loginData.Password);
                txtResult.Text += $"فیلد Password پر شد\n";
                
                await Task.Delay(1000);
                
                txtResult.Text += "فرم پر شد. در حال ارسال...\n";
                
                // کلیک روی دکمه ورود
                var loginButton = driver.FindElement(By.CssSelector("input[type='submit'], button[type='submit'], .login-button, #loginBtn"));
                loginButton.Click();
                
                txtResult.Text += "دکمه ورود کلیک شد. در انتظار پاسخ...\n";
                await Task.Delay(5000);
                
                // بررسی آدرس سفارشی در ترافیک شبکه
                var customUrl = txtCustomUrl.Text.Trim();
                var currentUrl = driver.Url;
                var pageSource = driver.PageSource;
                
                if (!string.IsNullOrEmpty(customUrl))
                {
                    txtResult.Text += $"بررسی ترافیک شبکه برای آدرس سفارشی: {customUrl}\n";
                    
                    // بررسی اینکه آیا آدرس سفارشی در ترافیک شبکه دیده شده یا نه
                    if (!string.IsNullOrEmpty(customRequestData) || !string.IsNullOrEmpty(customResponseData))
                    {
                        txtResult.Text += "✅ آدرس سفارشی در ترافیک شبکه پیدا شد!\n";
                        
                        // نمایش اطلاعات آدرس سفارشی از ترافیک شبکه
                        var customResult = new StringBuilder();
                        customResult.AppendLine($"=== اطلاعات آدرس سفارشی از ترافیک شبکه ===");
                        customResult.AppendLine($"آدرس سفارشی: {customUrl}");
                        customResult.AppendLine($"وضعیت پاسخ: {customStatusCode}");
                        customResult.AppendLine();
                        
                        customResult.AppendLine("=== داده‌های ارسالی به آدرس سفارشی ===");
                        customResult.AppendLine($"Headers درخواست:");
                        customResult.AppendLine(customRequestHeaders ?? "دریافت نشد");
                        customResult.AppendLine();
                        
                        // استخراج مقدار Authorization
                        string? authorizationValue = null;
                        if (!string.IsNullOrEmpty(customRequestHeaders))
                        {
                            var lines = customRequestHeaders.Split('\n');
                            foreach (var line in lines)
                            {
                                if (line.Trim().StartsWith("Authorization:", StringComparison.OrdinalIgnoreCase))
                                {
                                    authorizationValue = line.Substring("Authorization:".Length).Trim();
                                    break;
                                }
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(authorizationValue))
                        {
                            customResult.AppendLine("🔑 مقدار Authorization:");
                            customResult.AppendLine($"Authorization: {authorizationValue}");
                            customResult.AppendLine();
                            
                            // ذخیره مقادیر برای تست دستی
                            _lastAuthorizationValue = authorizationValue;
                            _lastCustomUrl = customUrl;
                            _lastRequestHeaders = customRequestHeaders;
                        }
                        else
                        {
                            customResult.AppendLine("❌ مقدار Authorization در header یافت نشد");
                            customResult.AppendLine();
                        }
                        
                        customResult.AppendLine($"Body درخواست:");
                        customResult.AppendLine(customRequestData ?? "داده‌ای ارسال نشد");
                        customResult.AppendLine();
                        
                        customResult.AppendLine("=== داده‌های دریافتی از آدرس سفارشی ===");
                        customResult.AppendLine($"Headers پاسخ:");
                        customResult.AppendLine(customResponseHeaders ?? "دریافت نشد");
                        customResult.AppendLine();
                        customResult.AppendLine($"Body پاسخ:");
                        customResult.AppendLine(customResponseData ?? "پاسخی دریافت نشد");
                        customResult.AppendLine();
                        
                        customResult.AppendLine("=== اطلاعات کلی صفحه ===");
                        customResult.AppendLine($"آدرس لاگین: {loginUrl}");
                        customResult.AppendLine($"آدرس فعلی: {currentUrl}");
                        customResult.AppendLine($"محتوای صفحه فعلی:");
                        customResult.AppendLine(pageSource);
                        
                        // نمایش اطلاعات آدرس سفارشی
                        txtResult.Text = customResult.ToString();
                        
                        // نمایش پاپ‌آپ برای آدرس سفارشی
                        string popupMessage = $"✅ آدرس سفارشی در ترافیک شبکه پیدا شد!\nآدرس: {customUrl}\nوضعیت: {customStatusCode}";
                        
                        if (!string.IsNullOrEmpty(authorizationValue))
                        {
                            popupMessage += $"\n🔑 Authorization: {authorizationValue}";
                        }
                        
                        popupMessage += "\nمحتوای کامل در فرم نمایش داده شده است.";
                        
                        MessageBox.Show(
                            popupMessage,
                            "نتیجه تست - آدرس سفارشی",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        txtResult.Text += $"❌ آدرس سفارشی در ترافیک شبکه پیدا نشد.\n";
                        
                        // نمایش صفحه لاگین عادی
                        var result = new StringBuilder();
                        result.AppendLine($"=== نتیجه تست ورود ===");
                        result.AppendLine($"آدرس لاگین: {loginUrl}");
                        result.AppendLine($"آدرس فعلی: {currentUrl}");
                        result.AppendLine($"آدرس سفارشی مورد نظر: {customUrl}");
                        result.AppendLine($"❌ آدرس سفارشی در ترافیک شبکه پیدا نشد");
                        result.AppendLine($"وضعیت پاسخ: {statusCode}");
                        result.AppendLine($"Headers پاسخ: {responseHeaders ?? "دریافت نشد"}");
                        result.AppendLine($"محتوای صفحه:");
                        result.AppendLine(pageSource);
                        
                        txtResult.Text = result.ToString();
                        
                        MessageBox.Show(
                            $"❌ آدرس سفارشی در ترافیک شبکه پیدا نشد!\nآدرس مورد نظر: {customUrl}\nمحتوای صفحه فعلی در فرم نمایش داده شده است.",
                            "نتیجه تست",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // اگر آدرس سفارشی وارد نشده، نمایش نتیجه عادی
                    var result = new StringBuilder();
                    result.AppendLine($"=== نتیجه تست ورود ===");
                    result.AppendLine($"آدرس لاگین: {loginUrl}");
                    result.AppendLine($"آدرس فعلی: {currentUrl}");
                    result.AppendLine($"وضعیت پاسخ: {statusCode}");
                    result.AppendLine($"Headers پاسخ: {responseHeaders ?? "دریافت نشد"}");
                    result.AppendLine($"محتوای صفحه:");
                    result.AppendLine(pageSource);
                    
                    txtResult.Text = result.ToString();
                    
                    MessageBox.Show(
                        $"تست ورود با موفقیت انجام شد!\nوضعیت: {statusCode}\nمحتوای صفحه در فرم نمایش داده شده است.",
                        "نتیجه تست",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                
                _logger.Information("تست ورود با موفقیت انجام شد");
            }
            catch (Exception formEx)
            {
                _logger.Error(formEx, "خطا در پر کردن فرم لاگین");
                txtResult.Text += $"خطا در پر کردن فرم لاگین:\n{formEx.Message}\n";
                
                // نمایش صفحه فعلی
                var pageSource = driver.PageSource;
                txtResult.Text += $"محتوای صفحه فعلی:\n{pageSource}";
                
                MessageBox.Show(
                    $"خطا در پر کردن فرم:\n{formEx.Message}\n\nصفحه فعلی در فرم نمایش داده شده است.",
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در تست ورود");
            txtResult.Text = $"خطا در تست ورود:\n{ex.Message}";
            MessageBox.Show(
                $"خطا در تست ورود:\n{ex.Message}",
                "خطا",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }


    private string FormatHeaders(IEnumerable<KeyValuePair<string, string>> headers)
    {
        var headerList = new List<string>();
        foreach (var header in headers)
        {
            headerList.Add($"{header.Key}: {header.Value}");
        }
        return string.Join("\n", headerList);
    }

    private RSA LoadPrivateKey(string privateKeyBase64)
    {
        _logger.Information($"طول کلید دریافتی: {privateKeyBase64.Length} کاراکتر");
        _logger.Information($"نمونه کلید (50 کاراکتر اول): {privateKeyBase64.Substring(0, Math.Min(50, privateKeyBase64.Length))}");
        
        // بررسی اینکه آیا کلید در فرمت PEM است
        if (privateKeyBase64.Contains("-----BEGIN") && privateKeyBase64.Contains("-----END"))
        {
            _logger.Information("کلید در فرمت PEM تشخیص داده شد");
            return LoadPemKey(privateKeyBase64);
        }
        
        // تلاش برای بارگذاری کلید در فرمت‌های مختلف
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        _logger.Information($"طول کلید پس از تبدیل Base64: {privateKeyBytes.Length} بایت");
        _logger.Information($"نمونه کلید (16 بایت اول): {Convert.ToHexString(privateKeyBytes.Take(16).ToArray())}");
        
        // بررسی فرمت کلید و تلاش برای بارگذاری
        try
        {
            // ابتدا تلاش با ImportRSAPrivateKey (فرمت PKCS#1)
            var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            _logger.Information("کلید خصوصی با فرمت PKCS#1 بارگذاری شد");
            return rsa;
        }
        catch (Exception ex1)
        {
            _logger.Warning($"خطا در بارگذاری PKCS#1: {ex1.Message}");
            
            try
            {
                // تلاش با ImportPkcs8PrivateKey (فرمت استاندارد PKCS#8)
                var rsa = RSA.Create(2048);
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                _logger.Information("کلید خصوصی با فرمت PKCS#8 بارگذاری شد");
                return rsa;
            }
            catch (Exception ex2)
            {
                _logger.Warning($"خطا در بارگذاری PKCS#8: {ex2.Message}");
                
                try
                {
                    // تلاش با ImportSubjectPublicKeyInfo (کلید عمومی)
                    var rsa = RSA.Create(2048);
                    rsa.ImportSubjectPublicKeyInfo(privateKeyBytes, out _);
                    _logger.Information("کلید عمومی با فرمت SPKI بارگذاری شد");
                    return rsa;
                }
                catch (Exception ex3)
                {
                    _logger.Warning($"خطا در بارگذاری SPKI: {ex3.Message}");
                    
                    // تلاش با فرمت‌های غیراستاندارد
                    try
                    {
                        return TryLoadCustomFormat(privateKeyBase64);
                    }
                    catch (Exception ex4)
                    {
                        _logger.Warning($"خطا در بارگذاری فرمت سفارشی: {ex4.Message}");
                        
                        // اگر همه روش‌ها شکست خورد، خطا را پرتاب کن
                        throw new InvalidOperationException($"نمی‌توان کلید خصوصی را بارگذاری کرد. فرمت کلید نامعتبر است. خطاهای: PKCS#1: {ex1.Message}, PKCS#8: {ex2.Message}, SPKI: {ex3.Message}, Custom: {ex4.Message}");
                    }
                }
            }
        }
    }

    private RSA LoadPemKey(string pemKey)
    {
        try
        {
            _logger.Information("تبدیل کلید PEM به DER...");
            var lines = pemKey.Split('\n').Where(line => !line.StartsWith("-----")).ToArray();
            var base64Content = string.Join("", lines);
            var derBytes = Convert.FromBase64String(base64Content);
            
            _logger.Information($"طول کلید DER: {derBytes.Length} بایت");
            
            var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(derBytes, out _);
            _logger.Information("کلید PEM با موفقیت بارگذاری شد");
            return rsa;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در بارگذاری کلید PEM");
            throw;
        }
    }

    private RSA TryLoadCustomFormat(string privateKeyBase64)
    {
        try
        {
            _logger.Information("تلاش برای بارگذاری کلید در فرمت سفارشی...");
            
            // تلاش برای اضافه کردن header های استاندارد
            var keyBytes = Convert.FromBase64String(privateKeyBase64);
            
            // بررسی اینکه آیا کلید نیاز به header دارد
            if (keyBytes[0] != 0x30) // SEQUENCE tag
            {
                _logger.Information("کلید نیاز به header دارد. تلاش برای اضافه کردن header...");
                
                // اضافه کردن header های PKCS#8
                var pkcs8Header = new byte[] { 0x30, 0x82 }; // SEQUENCE + length
                var lengthBytes = BitConverter.GetBytes((ushort)keyBytes.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lengthBytes);
                
                var fullKey = new byte[pkcs8Header.Length + lengthBytes.Length + keyBytes.Length];
                Array.Copy(pkcs8Header, 0, fullKey, 0, pkcs8Header.Length);
                Array.Copy(lengthBytes, 0, fullKey, pkcs8Header.Length, lengthBytes.Length);
                Array.Copy(keyBytes, 0, fullKey, pkcs8Header.Length + lengthBytes.Length, keyBytes.Length);
                
                var rsa = RSA.Create(2048);
                rsa.ImportPkcs8PrivateKey(fullKey, out _);
                _logger.Information("کلید با header سفارشی بارگذاری شد");
                return rsa;
            }
            else
            {
                // کلید ممکن است در فرمت صحیح باشد اما نیاز به پردازش داشته باشد
                _logger.Information("کلید در فرمت صحیح است اما نیاز به پردازش دارد");
                
                var rsa = RSA.Create(2048);
                rsa.ImportRSAPrivateKey(keyBytes, out _);
                _logger.Information("کلید با پردازش سفارشی بارگذاری شد");
                return rsa;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در بارگذاری فرمت سفارشی");
            throw;
        }
    }

    private byte[]? ConvertPemToDer(string pemKey)
    {
        try
        {
            // بررسی اینکه آیا کلید در فرمت PEM است
            if (pemKey.Contains("-----BEGIN") && pemKey.Contains("-----END"))
            {
                _logger.Information("کلید در فرمت PEM تشخیص داده شد");
                var lines = pemKey.Split('\n').Where(line => !line.StartsWith("-----")).ToArray();
                var base64Content = string.Join("", lines);
                return Convert.FromBase64String(base64Content);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.Warning($"خطا در تبدیل PEM: {ex.Message}");
            return null;
        }
    }

    private bool IsValidBase64(string input)
    {
        try
        {
            // بررسی اینکه آیا رشته Base64 معتبر است
            if (string.IsNullOrWhiteSpace(input))
                return false;
                
            // بررسی کاراکترهای مجاز Base64
            var base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
            foreach (char c in input)
            {
                if (!base64Chars.Contains(c))
                    return false;
            }
            
            // تلاش برای تبدیل
            Convert.FromBase64String(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidJson(string input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
                
            // بررسی اینکه آیا رشته با { یا [ شروع می‌شود (نشانه JSON)
            var trimmed = input.Trim();
            if (!trimmed.StartsWith("{") && !trimmed.StartsWith("["))
                return false;
                
            // تلاش برای پارس کردن JSON
            JsonDocument.Parse(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string DecryptLargeData(byte[] encryptedBytes, RSA rsa)
    {
        try
        {
            var keySize = rsa.KeySize;
            var blockSize = keySize / 8;
            
            _logger.Information($"رمزگشایی داده‌های بزرگ: {encryptedBytes.Length} بایت در بلوک‌های {blockSize} بایتی");
            
            var decryptedParts = new List<byte>();
            
            // تقسیم داده‌ها به بلوک‌های کوچکتر
            for (int i = 0; i < encryptedBytes.Length; i += blockSize)
            {
                var blockLength = Math.Min(blockSize, encryptedBytes.Length - i);
                var block = new byte[blockSize];
                Array.Copy(encryptedBytes, i, block, 0, blockLength);
                
                try
                {
                    var decryptedBlock = rsa.Decrypt(block, RSAEncryptionPadding.OaepSHA256);
                    decryptedParts.AddRange(decryptedBlock);
                    _logger.Information($"بلوک {i / blockSize + 1} رمزگشایی شد");
                }
                catch (Exception ex)
                {
                    _logger.Warning($"خطا در رمزگشایی بلوک {i / blockSize + 1}: {ex.Message}");
                    // اگر یک بلوک شکست خورد، ادامه دهیم
                }
            }
            
            var result = Encoding.UTF8.GetString(decryptedParts.ToArray());
            _logger.Information($"رمزگشایی داده‌های بزرگ با موفقیت انجام شد: {result.Length} کاراکتر");
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در رمزگشایی داده‌های بزرگ");
            return $"خطا در رمزگشایی داده‌های بزرگ: {ex.Message}";
        }
    }

    private string DecryptWithServerKey(string encryptedData, string serverKey)
    {
        try
        {
            _logger.Information("شروع رمزگشایی با کلید دریافتی از سرور...");
            _logger.Information($"طول داده‌های رمز شده: {encryptedData.Length} کاراکتر");
            _logger.Information($"طول کلید سرور: {serverKey.Length} کاراکتر");
            
            // بررسی اینکه آیا داده‌ها Base64 هستند
            if (!IsValidBase64(encryptedData))
            {
                _logger.Warning("داده‌های رمز شده Base64 معتبر نیست");
                return encryptedData; // اگر Base64 نیست، همان داده را برگردان
            }
            
            // تلاش برای بارگذاری کلید خصوصی از سرور
            RSA rsa;
            try
            {
                rsa = LoadPrivateKey(serverKey);
            }
            catch (Exception keyEx)
            {
                _logger.Error(keyEx, "خطا در بارگذاری کلید سرور");
                
                // اگر کلید قابل بارگذاری نیست، تلاش برای استفاده از کلیدهای ثابت
                _logger.Information("تلاش برای استفاده از کلیدهای ثابت...");
                try
                    {
                        rsa = LoadPrivateKey("MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=");
                        _logger.Information("کلید ثابت با موفقیت بارگذاری شد");
                    }
                    catch (Exception fallbackEx)
                    {
                        _logger.Error(fallbackEx, "خطا در بارگذاری کلید ثابت");
                        return $"خطا در بارگذاری کلید سرور: {keyEx.Message}\nخطا در بارگذاری کلید ثابت: {fallbackEx.Message}";
                    }
            }
            
            // تبدیل داده‌های رمز شده از Base64 به بایت
            var encryptedBytes = Convert.FromBase64String(encryptedData);
            _logger.Information($"طول داده‌های رمز شده: {encryptedBytes.Length} بایت");
            
            // بررسی اندازه کلید و داده‌ها
            var keySize = rsa.KeySize;
            _logger.Information($"اندازه کلید: {keySize} بیت");
            
            if (encryptedBytes.Length != (keySize / 8))
            {
                _logger.Warning($"طول داده‌های رمز شده ({encryptedBytes.Length} بایت) با اندازه کلید ({keySize / 8} بایت) مطابقت ندارد.");
                
                if (encryptedBytes.Length < (keySize / 8))
                {
                    _logger.Information("تلاش برای padding داده‌ها...");
                    var paddedBytes = new byte[keySize / 8];
                    Array.Copy(encryptedBytes, paddedBytes, encryptedBytes.Length);
                    encryptedBytes = paddedBytes;
                }
                else
                {
                    _logger.Warning("داده‌ها بزرگتر از اندازه کلید هستند. استفاده از متد رمزگشایی داده‌های بزرگ...");
                    return DecryptLargeData(encryptedBytes, rsa);
                }
            }
            
            // رمزگشایی با کلید خصوصی دریافتی از سرور
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            
            // تبدیل بایت‌های رمزگشایی شده به رشته
            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            
            _logger.Information($"رمزگشایی با کلید سرور با موفقیت انجام شد. طول متن: {decryptedText.Length} کاراکتر");
            return decryptedText;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در رمزگشایی با کلید سرور");
            return $"خطا در رمزگشایی با کلید سرور: {ex.Message}\nجزئیات خطا: {ex.ToString()}";
        }
    }

    private string DecryptResponseContent(string encryptedContent, string publicKey, string privateKey)
    {
        try
        {
            _logger.Information("شروع عملیات رمزگشایی محتوای پاسخ...");
            _logger.Information($"طول محتوای رمز شده: {encryptedContent.Length} کاراکتر");
            _logger.Information($"نمونه محتوای دریافتی (100 کاراکتر اول): {encryptedContent.Substring(0, Math.Min(100, encryptedContent.Length))}");
            
            // بررسی اینکه آیا محتوا Base64 است یا نه
            if (!IsValidBase64(encryptedContent))
            {
                _logger.Warning("محتوای دریافتی Base64 معتبر نیست. ممکن است محتوا از قبل رمزگشایی شده باشد یا فرمت دیگری داشته باشد.");
                
                // اگر محتوا JSON است، آن را به عنوان متن عادی برگردان
                if (IsValidJson(encryptedContent))
                {
                    _logger.Information("محتوای دریافتی JSON معتبر است. احتمالاً از قبل رمزگشایی شده است.");
                    return encryptedContent;
                }
                
                // اگر محتوا متن عادی است، آن را برگردان
                _logger.Information("محتوای دریافتی متن عادی است. رمزگشایی لازم نیست.");
                return encryptedContent;
            }
            
            // بارگذاری کلید خصوصی با متد کمکی
            var rsa = LoadPrivateKey(privateKey);
            
            // تبدیل محتوای رمز شده از Base64 به بایت
            var encryptedBytes = Convert.FromBase64String(encryptedContent);
            _logger.Information($"طول داده‌های رمز شده: {encryptedBytes.Length} بایت");
            
            // بررسی اندازه کلید و داده‌ها
            var keySize = rsa.KeySize;
            var maxDataSize = (keySize / 8) - 42; // برای OAEP padding
            _logger.Information($"اندازه کلید: {keySize} بیت، حداکثر اندازه داده: {maxDataSize} بایت");
            
            if (encryptedBytes.Length != (keySize / 8))
            {
                _logger.Warning($"طول داده‌های رمز شده ({encryptedBytes.Length} بایت) با اندازه کلید ({keySize / 8} بایت) مطابقت ندارد.");
                
                // اگر داده‌ها کوچکتر از اندازه کلید هستند، ممکن است نیاز به padding داشته باشند
                if (encryptedBytes.Length < (keySize / 8))
                {
                    _logger.Information("تلاش برای padding داده‌ها...");
                    var paddedBytes = new byte[keySize / 8];
                    Array.Copy(encryptedBytes, paddedBytes, encryptedBytes.Length);
                    encryptedBytes = paddedBytes;
                }
                else
                {
                    _logger.Warning("داده‌ها بزرگتر از اندازه کلید هستند. استفاده از متد رمزگشایی داده‌های بزرگ...");
                    return DecryptLargeData(encryptedBytes, rsa);
                }
            }
            
            // رمزگشایی با کلید خصوصی - استفاده از RSA-OAEP با SHA-256 (مطابق با کد JavaScript)
            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            
            // تبدیل بایت‌های رمزگشایی شده به رشته
            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            
            _logger.Information($"رمزگشایی با موفقیت انجام شد. طول متن رمزگشایی شده: {decryptedText.Length} کاراکتر");
            return decryptedText;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در رمزگشایی محتوای پاسخ");
            return $"خطا در رمزگشایی: {ex.Message}\nجزئیات خطا: {ex.ToString()}";
        }
    }

    private async void btnManualTest_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_lastAuthorizationValue) || string.IsNullOrEmpty(_lastCustomUrl))
            {
                MessageBox.Show(
                    "ابتدا باید تست اصلی را انجام دهید تا مقدار Authorization دریافت شود.",
                    "خطا",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _logger.Information("شروع تست دستی API...");
            txtResult.Text = "در حال انجام تست دستی API...\n";
            
            // فراخوانی فانکشن testApi
            await testApi(_lastCustomUrl, _lastAuthorizationValue, _lastRequestHeaders);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در تست دستی");
            txtResult.Text = $"خطا در تست دستی:\n{ex.Message}";
            MessageBox.Show(
                $"خطا در تست دستی:\n{ex.Message}",
                "خطا",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async Task testApi(string apiUrl, string authorizationValue, string? originalHeaders)
    {
        try
        {
            txtResult.Text += $"🔗 تست API: {apiUrl}\n";
            txtResult.Text += $"🔑 Authorization: {authorizationValue}\n";
            txtResult.Text += "در حال تنظیم تمام Headers...\n";

            // ایجاد HttpClient
            using var httpClient = new HttpClient();
            
            // استخراج و تنظیم تمام headers از درخواست اصلی
            if (!string.IsNullOrEmpty(originalHeaders))
            {
                txtResult.Text += "📋 استفاده از تمام Headers اصلی:\n";
                var lines = originalHeaders.Split('\n');
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.Contains(':'))
                    {
                        var parts = trimmedLine.Split(':', 2);
                        if (parts.Length == 2)
                        {
                            var headerName = parts[0].Trim();
                            var headerValue = parts[1].Trim();
                            
                            try
                            {
                                // حذف header قبلی اگر وجود داشته باشد
                                if (httpClient.DefaultRequestHeaders.Contains(headerName))
                                {
                                    httpClient.DefaultRequestHeaders.Remove(headerName);
                                }
                                
                                // اضافه کردن header جدید
                                httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
                                txtResult.Text += $"  ✓ {headerName}: {headerValue}\n";
                            }
                            catch (Exception ex)
                            {
                                txtResult.Text += $"  ❌ خطا در اضافه کردن {headerName}: {ex.Message}\n";
                            }
                        }
                    }
                }
            }
            else
            {
                // اگر headers اصلی موجود نیست، از headers پیش‌فرض استفاده کن
                httpClient.DefaultRequestHeaders.Add("Authorization", authorizationValue);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                txtResult.Text += "⚠️ Headers اصلی موجود نیست، از Headers پیش‌فرض استفاده می‌شود\n";
            }

            txtResult.Text += "در حال ارسال درخواست...\n";


            var formContent = new FormUrlEncodedContent(new[]
           {
                    new KeyValuePair<string, string>("VarCarSt", ""),
                    new KeyValuePair<string, string>("VarCus", ""),
                    new KeyValuePair<string, string>("VarExitDteFr", ""),
                    new KeyValuePair<string, string>("VarExitDteTo", ""),
                    new KeyValuePair<string, string>("VarYer", ""),

                });
            // ارسال درخواست GET
            var response = await httpClient.PostAsync(apiUrl, formContent);
            
            txtResult.Text += $"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}\n";
            
            // دریافت محتوای پاسخ
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // کلیدهای RSA برای رمزگشایی
            var REACT_APP_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
            var REACT_APP_PRIVATE_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";


            // فرض بر این است که اینجا کلید به صورت باینری وجود دارد.
            byte[] publicKeyBytes = Convert.FromBase64String(REACT_APP_PUBLIC_KEY); // کلید عمومی
            byte[] privateKeyBytes = Convert.FromBase64String(REACT_APP_PRIVATE_KEY); // کلید خصوصی

            // وارد کردن کلید عمومی
            RSA publicKey = ImportPublicKey(publicKeyBytes);
            Console.WriteLine("کلید عمومی وارد شد.");

            // وارد کردن کلید خصوصی
            RSA privateKey = ImportPrivateKey(privateKeyBytes);
            Console.WriteLine("کلید خصوصی وارد شد.");

            // متن ورودی برای رمزنگاری
            string plaintext = "Hello, this is a secret message!";
            Console.WriteLine($"متن ورودی: {plaintext}");

            // رمزنگاری متن
            byte[] encryptedData = Encrypt(publicKey, plaintext);
            Console.WriteLine($"متن رمزنگاری شده: {Convert.ToBase64String(encryptedData)}");

            // رمزگشایی متن
            string decryptedText = Decrypt(privateKey, encryptedData);
            Console.WriteLine($"متن رمزگشایی شده: {decryptedText}");


            // رمزگشایی محتوای پاسخ
            txtResult.Text += "🔐 در حال رمزگشایی محتوای پاسخ...\n";
            
            // ابتدا JSON را deserialize کنیم
            var dataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dataModel>(responseContent);
            if (dataModel != null && !string.IsNullOrEmpty(dataModel.data) && !string.IsNullOrEmpty(dataModel.key))
            {
                txtResult.Text += $"📋 داده‌های JSON:\n";
                txtResult.Text += $"  - data: {dataModel.data}\n";
                txtResult.Text += $"  - key: {dataModel.key}\n\n";

                try {   
 

        byte[] edData = Convert.FromBase64String(dataModel.data);
       

        // رمزگشایی متن
        string dt = Decrypt(privateKey, edData);
        Console.WriteLine($"متن رمزگشایی شده: {dt}");
            }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در testApi");
            txtResult.Text = $"خطا در تست API:\n{ex.Message}";
            //throw;
        }



                // استفاده از کلید دریافتی از سرور برای رمزگشایی
                var decryptedContent = "";// DecryptWithServerKey(dataModel.data, dataModel.key);
                txtResult.Text += $"✅ محتوای رمزگشایی شده:\n{decryptedContent}\n\n";
            }
            else
            {
                txtResult.Text += "❌ خطا در deserialize کردن JSON یا فیلدهای data/key خالی است\n";
            }
            // دریافت headers پاسخ
            var responseHeaders = new StringBuilder();
            foreach (var header in response.Headers)
            {
                responseHeaders.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            
            // نمایش نتیجه
            var result = new StringBuilder();
            result.AppendLine($"=== نتیجه تست دستی API ===");
            result.AppendLine($"آدرس API: {apiUrl}");
            result.AppendLine($"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}");
            result.AppendLine();
            
            result.AppendLine("=== Headers ارسالی ===");
            if (!string.IsNullOrEmpty(originalHeaders))
            {
                result.AppendLine(originalHeaders);
            }
            else
            {
                result.AppendLine("Headers پیش‌فرض استفاده شده");
            }
            result.AppendLine();
            
            result.AppendLine("=== Headers پاسخ ===");
            result.AppendLine(responseHeaders.ToString());
            result.AppendLine();
            
            result.AppendLine("=== محتوای پاسخ اصلی ===");
            result.AppendLine(responseContent);
            result.AppendLine();
            
            // اگر JSON deserialize شده، اطلاعات آن را نمایش ده
            var dataModelResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dataModel>(responseContent);
            if (dataModelResult != null && !string.IsNullOrEmpty(dataModelResult.data) && !string.IsNullOrEmpty(dataModelResult.key))
            {
                result.AppendLine("=== داده‌های JSON استخراج شده ===");
                result.AppendLine($"data: {dataModelResult.data}");
                result.AppendLine($"key: {dataModelResult.key}");
                result.AppendLine();
                
                result.AppendLine("=== محتوای رمزگشایی شده با کلید سرور ===");
                var decryptedContent = DecryptWithServerKey(dataModelResult.data, dataModelResult.key);
                result.AppendLine(decryptedContent);
            }
            else
            {
                result.AppendLine("=== محتوای رمزگشایی شده ===");
                result.AppendLine("خطا در deserialize کردن JSON یا فیلدهای data/key خالی است");
            }
            
            // نمایش در UI
            txtResult.Text = result.ToString();
            _logger.Information("تست دستی API با موفقیت انجام شد");
            
            // نمایش پاپ‌آپ
            MessageBox.Show(
                $"✅ تست دستی API با موفقیت انجام شد!\nوضعیت: {(int)response.StatusCode} {response.StatusCode}\nتمام Headers اصلی استفاده شده است.\nمحتوای پاسخ در فرم نمایش داده شده است.",
                "نتیجه تست دستی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در testApi");
            txtResult.Text = $"خطا در تست API:\n{ex.Message}";
            //throw;
        }
    }

    // تابع برای وارد کردن کلید عمومی
    private static RSA ImportPublicKey(byte[] publicKeyBytes)
    {
        RSA rsa = RSA.Create(2048);
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _); // وارد کردن کلید عمومی
        return rsa;
    }

    // تابع برای وارد کردن کلید خصوصی
    private static RSA ImportPrivateKey(byte[] privateKeyBytes)
    {
        RSA rsa = RSA.Create(2048);
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _); // وارد کردن کلید خصوصی
        return rsa;
    }

    // تابع برای رمزنگاری متن
    private static byte[] Encrypt(RSA publicKey, string plaintext)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        return publicKey.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256); // رمزنگاری با استفاده از پدینگ OAEP
    }

    // تابع برای رمزگشایی متن
    private static string Decrypt(RSA privateKey, byte[] encryptedData)
    {
        byte[] decryptedBytes = privateKey.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256); // رمزگشایی با استفاده از پدینگ OAEP
        return Encoding.UTF8.GetString(decryptedBytes);
    }
    public class dataModel
    {
       public string? data { get; set; }
       public string? key { get; set; }
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
