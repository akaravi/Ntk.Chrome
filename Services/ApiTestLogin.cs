using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using static Ntk.Chrome.Forms.TestForm;

namespace Ntk.Chrome.Services
{
    public class ApiTestLogin
    {
        private readonly HttpClient _httpClient;
        private readonly Serilog.ILogger _logger;
        private readonly string _baseUrl;
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly CleanDataTransmissionManager _dataManager;
        private readonly CleanEncryptionManager _encryptionManager;

        // کلیدهای RSA برای رمزنگاری و رمزگشایی
        private const string REACT_APP_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
        
        private const string REACT_APP_PRIVATE_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";

        public ApiTestLogin(string baseUrl = "https://b2b.isaco.ir/api", Serilog.ILogger logger = null)
        {
            _baseUrl = baseUrl;
            _logger = logger ?? Serilog.Log.Logger;
            _httpClient = new HttpClient();
            _publicKey = REACT_APP_PUBLIC_KEY;
            _privateKey = REACT_APP_PRIVATE_KEY;
            _dataManager = new CleanDataTransmissionManager(_logger);
            _encryptionManager = new CleanEncryptionManager(_logger);
            
            // تنظیم headers پیش‌فرض
            SetupDefaultHeaders();
        }

        /// <summary>
        /// تنظیم headers پیش‌فرض برای HttpClient
        /// </summary>
        private void SetupDefaultHeaders()
        {
            try
            {
                _logger.Information("تنظیم headers پیش‌فرض...");
                _dataManager.SetupRequestHeaders(_httpClient);
                _logger.Information("Headers پیش‌فرض با موفقیت تنظیم شدند");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تنظیم headers پیش‌فرض");
            }
        }

        /// <summary>
        /// ورود کاربر با نام کاربری و رمز عبور
        /// </summary>
        /// <param name="userName">نام کاربری</param>
        /// <param name="password">رمز عبور</param>
        /// <param name="scus">کد مشتری (اختیاری)</param>
        /// <param name="clientVersion">نسخه کلاینت</param>
        /// <param name="language">زبان</param>
        /// <returns>نتیجه ورود</returns>
        public async Task<LoginResponse> LoginAsync(string userName, string password, string scus = "", string clientVersion = "1.0.0", string language = "fa-IR")
        {
            try
            {
                _logger.Information($"شروع فرآیند ورود برای کاربر: {userName}");

                // ایجاد داده‌های ورود
                var loginData = new LoginRequest
                {
                    Scus = scus,
                    UserName = userName.ToLower(),
                    Password = password,
                    ClientVersion = clientVersion,
                    Lng = language
                };

                // آماده‌سازی داده‌ها با استفاده از CleanDataTransmissionManager
                var jsonData = _dataManager.PrepareDataForTransmission(loginData);
                _logger.Information($"داده‌های ورود آماده شده: {jsonData.Length} کاراکتر");

                // رمزنگاری داده‌ها با استفاده از CleanEncryptionManager
                var encryptedData = _encryptionManager.EncryptData(jsonData);
                _logger.Information($"داده‌های رمزنگاری شده: {encryptedData.Length} کاراکتر");

                // ایجاد محتوای فرم با استفاده از CleanDataTransmissionManager
                var formContent = _dataManager.CreateFormContent(encryptedData, "Sec", "1");


                //var originalHeaders = "sec-ch-ua: \"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\"\nsec-ch-ua-mobile: ?0\nAuthorization: eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6IjEyODc2Nzc4NDMtNDAwMjAxNiIsInR5cGUiOjIsImlhdCI6MTc2MDg0ODQ0NSwiZXhwIjoxNzYwODkwNDQ1fQ.FOLd_4ihks0LKncucglGkI0kR_6kBZRz3ogR1w_twxo\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\nContent-Type: application/x-www-form-urlencoded\nAccept: application/json, text/plain, */*\nReferer: https://b2b.isaco.ir/home\nsec: site\nsec-ch-ua-platform: \"Windows\"";
                var originalHeaders = "sec-ch-ua: \"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\"\nsec-ch-ua-mobile: ?0\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\nContent-Type: application/x-www-form-urlencoded\nAccept: application/json, text/plain, */*\nReferer: https://b2b.isaco.ir/home\nsec: site\nsec-ch-ua-platform: \"Windows\"";


                // استخراج و تنظیم تمام headers از درخواست اصلی
                if (!string.IsNullOrEmpty(originalHeaders))
                {
                    //txtResult.Text += "📋 استفاده از تمام Headers اصلی:\n";
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
                                    if (_httpClient.DefaultRequestHeaders.Contains(headerName))
                                    {
                                        _httpClient.DefaultRequestHeaders.Remove(headerName);
                                    }

                                    // اضافه کردن header جدید
                                    _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
                                    //txtResult.Text += $"  ✓ {headerName}: {headerValue}\n";
                                }
                                catch (Exception ex)
                                {
                                    //txtResult.Text += $"  ❌ خطا در اضافه کردن {headerName}: {ex.Message}\n";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // اگر headers اصلی موجود نیست، از headers پیش‌فرض استفاده کن
                    //_httpClient.DefaultRequestHeaders.Add("Authorization", authorizationValue);
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    //txtResult.Text += "⚠️ Headers اصلی موجود نیست، از Headers پیش‌فرض استفاده می‌شود\n";
                }
                // ارسال درخواست POST
                var response = await _httpClient.PostAsync($"{_baseUrl}/Sec/Login/v2", formContent);
                
                _logger.Information($"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}");

                // دریافت و پردازش محتوای پاسخ
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information($"محتوای پاسخ خام: {responseContent.Length} کاراکتر");
                
                // پردازش داده‌های دریافتی با استفاده از CleanDataTransmissionManager
                var processedResponse = await _dataManager.ProcessReceivedDataAsync(responseContent);
                
                // رمزگشایی پاسخ با استفاده از CleanEncryptionManager
                var decryptedResponse = await DecryptResponseAsync(processedResponse);
                
                // تبدیل پاسخ به مدل
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(decryptedResponse);
                
                return new LoginResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = loginResponse?.Data,
                    MessageCode = loginResponse?.MessageCode,
                    MessageDes = loginResponse?.MessageDes,
                    RawContent = responseContent,
                    DecryptedContent = decryptedResponse
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در فرآیند ورود");
                return new LoginResponse
                {
                    Success = false,
                    StatusCode = 0,
                    Data = null,
                    MessageCode = "LOGIN_ERROR",
                    MessageDes = ex.Message,
                    RawContent = ex.Message,
                    DecryptedContent = ex.Message
                };
            }
        }

        /// <summary>
        /// دریافت کد Captcha
        /// </summary>
        /// <param name="language">زبان</param>
        /// <returns>نتیجه دریافت Captcha</returns>
        public async Task<CaptchaResponse> GetCaptchaAsync(string language = "fa-IR")
        {
            try
            {
                _logger.Information("دریافت کد Captcha...");

                // ایجاد داده‌های درخواست
                var captchaData = new CaptchaRequest
                {
                    Lng = language
                };

                // آماده‌سازی داده‌ها با استفاده از CleanDataTransmissionManager
                var jsonData = _dataManager.PrepareDataForTransmission(captchaData);
                _logger.Information($"داده‌های Captcha آماده شده: {jsonData.Length} کاراکتر");

                // رمزنگاری داده‌ها با استفاده از CleanEncryptionManager
                var encryptedData = _encryptionManager.EncryptData(jsonData);
                _logger.Information($"داده‌های رمزنگاری شده: {encryptedData.Length} کاراکتر");

                // ایجاد محتوای فرم با استفاده از CleanDataTransmissionManager
                var formContent = _dataManager.CreateFormContent(encryptedData, "Sec", "0");

                // ارسال درخواست POST
                var response = await _httpClient.PostAsync($"{_baseUrl}/Sec/Login/v2", formContent);
                
                _logger.Information($"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}");

                // دریافت محتوای پاسخ
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information($"محتوای پاسخ خام: {responseContent}");

                // رمزگشایی پاسخ
                var decryptedResponse = await DecryptResponseAsync(responseContent);
                
                // تبدیل پاسخ به مدل
                var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(decryptedResponse);
                
                return new CaptchaResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = captchaResponse?.Data,
                    MessageCode = captchaResponse?.MessageCode,
                    MessageDes = captchaResponse?.MessageDes,
                    RawContent = responseContent,
                    DecryptedContent = decryptedResponse
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در دریافت Captcha");
                return new CaptchaResponse
                {
                    Success = false,
                    StatusCode = 0,
                    Data = null,
                    MessageCode = "CAPTCHA_ERROR",
                    MessageDes = ex.Message,
                    RawContent = ex.Message,
                    DecryptedContent = ex.Message
                };
            }
        }

        /// <summary>
        /// ارسال پیامک برای بازیابی رمز عبور
        /// </summary>
        /// <param name="mobileNumber">شماره موبایل</param>
        /// <returns>نتیجه ارسال پیامک</returns>
        public async Task<SmsResponse> SendSmsAsync(string mobileNumber)
        {
            try
            {
                _logger.Information($"ارسال پیامک به شماره: {mobileNumber}");

                // ایجاد داده‌های درخواست
                var smsData = new SmsRequest
                {
                    MobileNumber = mobileNumber
                };

                // آماده‌سازی داده‌ها با استفاده از CleanDataTransmissionManager
                var jsonData = _dataManager.PrepareDataForTransmission(smsData);
                _logger.Information($"داده‌های پیامک آماده شده: {jsonData.Length} کاراکتر");

                // رمزنگاری داده‌ها با استفاده از CleanEncryptionManager
                var encryptedData = _encryptionManager.EncryptData(jsonData);
                _logger.Information($"داده‌های رمزنگاری شده: {encryptedData.Length} کاراکتر");

                // ایجاد محتوای فرم با استفاده از CleanDataTransmissionManager
                var formContent = _dataManager.CreateFormContent(encryptedData, "Sec", "11");

                // ارسال درخواست POST
                var response = await _httpClient.PostAsync($"{_baseUrl}/Sec/Login/v2", formContent);
                
                _logger.Information($"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}");

                // دریافت محتوای پاسخ
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information($"محتوای پاسخ خام: {responseContent}");

                // رمزگشایی پاسخ
                var decryptedResponse = await DecryptResponseAsync(responseContent);
                
                // تبدیل پاسخ به مدل
                var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(decryptedResponse);
                
                return new SmsResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = smsResponse?.Data,
                    MessageCode = smsResponse?.MessageCode,
                    MessageDes = smsResponse?.MessageDes,
                    RawContent = responseContent,
                    DecryptedContent = decryptedResponse
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در ارسال پیامک");
                return new SmsResponse
                {
                    Success = false,
                    StatusCode = 0,
                    Data = null,
                    MessageCode = "SMS_ERROR",
                    MessageDes = ex.Message,
                    RawContent = ex.Message,
                    DecryptedContent = ex.Message
                };
            }
        }

        /// <summary>
        /// رمزنگاری داده‌ها با RSA
        /// </summary>
        /// <param name="plainText">متن اصلی</param>
        /// <returns>متن رمزنگاری شده به صورت Base64</returns>
        private string EncryptData(string plainText)
        {
            try
            {
                using (var rsa = RSA.Create(2048))
                {
                    // بارگذاری کلید عمومی
                    var publicKeyBytes = Convert.FromBase64String(_publicKey);
                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    
                    // رمزنگاری داده‌ها
                    var dataBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
                    
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزنگاری داده‌ها");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی پاسخ دریافتی از API با استفاده از CleanEncryptionManager
        /// </summary>
        /// <param name="encryptedContent">محتوای رمزنگاری شده</param>
        /// <returns>محتوای رمزگشایی شده</returns>
        private async Task<string> DecryptResponseAsync(string encryptedContent)
        {
            try
            {
                _logger.Information("شروع رمزگشایی محتوای پاسخ با CleanEncryptionManager...");

                // بررسی اینکه آیا محتوا JSON است یا نه
                if (IsValidJson(encryptedContent))
                {
                    var dataModel = JsonConvert.DeserializeObject<dataModel>(encryptedContent);
                    if (dataModel != null && !string.IsNullOrEmpty(dataModel.data) && !string.IsNullOrEmpty(dataModel.key))
                    {
                        _logger.Information($"داده‌های JSON یافت شد - data length: {dataModel.data.Length}, key length: {dataModel.key.Length}");
                        
                        // بررسی فرمت CryptoJS (U2FsdGVkX1 prefix)
                        if (dataModel.data.StartsWith("U2FsdGVkX1"))
                        {
                            _logger.Information("تشخیص فرمت CryptoJS - استفاده از رمزگشایی ترکیبی (AES + RSA)");
                            // رمزگشایی ترکیبی: AES data + RSA key
                            var decryptedData = _encryptionManager.DecryptHybridData(dataModel.data, dataModel.key);
                            return decryptedData;
                        }
                        else
                        {
                            _logger.Information("استفاده از رمزگشایی RSA مستقیم");
                            // رمزگشایی مستقیم با RSA
                            var decryptedData = _encryptionManager.DecryptWithWebCryptoParams(dataModel.data, dataModel.key);
                            return decryptedData;
                        }
                    }
                }

                // اگر محتوا Base64 است، آن را رمزگشایی کن با WebCrypto API parameters
                if (IsValidBase64(encryptedContent))
                {
                    var decryptedData = _encryptionManager.DecryptWithWebCryptoParams(encryptedContent, _encryptionManager.PrivateKey);
                    return decryptedData;
                }

                // اگر محتوا متن عادی است، آن را برگردان
                _logger.Information("محتوای دریافتی متن عادی است. رمزگشایی لازم نیست.");
                return encryptedContent;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی پاسخ");
                return encryptedContent; // در صورت خطا، محتوای اصلی را برگردان
            }
        }

        /// <summary>
        /// رمزگشایی داده‌ها با کلید خصوصی
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده</param>
        /// <param name="privateKey">کلید خصوصی</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        private string DecryptWithPrivateKey(string encryptedData, string privateKey)
        {
            try
            {
                using (var rsa = RSA.Create(2048))
                {
                    var privateKeyBytes = Convert.FromBase64String(privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedData);
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                    
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی با کلید خصوصی");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌ها با کلید دریافتی از سرور
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده</param>
        /// <param name="serverKey">کلید دریافتی از سرور</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
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
                    throw;
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
                return $"خطا در رمزگشایی با کلید سرور: {ex.Message}";
            }
        }

        /// <summary>
        /// بارگذاری کلید خصوصی
        /// </summary>
        /// <param name="privateKeyString">رشته کلید خصوصی</param>
        /// <returns>کلید RSA</returns>
        private RSA LoadPrivateKey(string privateKeyString)
        {
            try
            {
                var rsa = RSA.Create(2048);
                var privateKeyBytes = Convert.FromBase64String(privateKeyString);
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                return rsa;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در بارگذاری کلید خصوصی");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌های بزرگ
        /// </summary>
        /// <param name="encryptedBytes">داده‌های رمزنگاری شده</param>
        /// <param name="rsa">کلید RSA</param>
        /// <returns>متن رمزگشایی شده</returns>
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

        /// <summary>
        /// بررسی معتبر بودن Base64
        /// </summary>
        /// <param name="base64String">رشته Base64</param>
        /// <returns>true اگر معتبر باشد</returns>
        private bool IsValidBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// بررسی معتبر بودن JSON
        /// </summary>
        /// <param name="jsonString">رشته JSON</param>
        /// <returns>true اگر معتبر باشد</returns>
        private bool IsValidJson(string jsonString)
        {
            try
            {
                JsonConvert.DeserializeObject(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// آزاد کردن منابع
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// مدل درخواست ورود
    /// </summary>
    public class LoginRequest
    {
        public string Scus { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientVersion { get; set; }
        public string Lng { get; set; }
    }

    /// <summary>
    /// مدل پاسخ ورود
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public LoginData Data { get; set; }
        public string MessageCode { get; set; }
        public string MessageDes { get; set; }
        public string RawContent { get; set; }
        public string DecryptedContent { get; set; }
    }

    /// <summary>
    /// مدل داده‌های ورود
    /// </summary>
    public class LoginData
    {
        public int Status { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Key { get; set; }
        public int BasktCnt { get; set; }
        public string ImgCapcha { get; set; }
        public string ServerTime { get; set; }
    }

    /// <summary>
    /// مدل درخواست Captcha
    /// </summary>
    public class CaptchaRequest
    {
        public string Lng { get; set; }
    }

    /// <summary>
    /// مدل پاسخ Captcha
    /// </summary>
    public class CaptchaResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public CaptchaData Data { get; set; }
        public string MessageCode { get; set; }
        public string MessageDes { get; set; }
        public string RawContent { get; set; }
        public string DecryptedContent { get; set; }
    }

    /// <summary>
    /// مدل داده‌های Captcha
    /// </summary>
    public class CaptchaData
    {
        public string ImgCapcha { get; set; }
        public string ServerTime { get; set; }
    }

    /// <summary>
    /// مدل درخواست پیامک
    /// </summary>
    public class SmsRequest
    {
        public string MobileNumber { get; set; }
    }

    /// <summary>
    /// مدل پاسخ پیامک
    /// </summary>
    public class SmsResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public SmsData Data { get; set; }
        public string MessageCode { get; set; }
        public string MessageDes { get; set; }
        public string RawContent { get; set; }
        public string DecryptedContent { get; set; }
    }

    /// <summary>
    /// مدل داده‌های پیامک
    /// </summary>
    public class SmsData
    {
        public string Message { get; set; }
        public bool Sent { get; set; }
    }

    /// <summary>
    /// مدیریت انتقال داده‌ها با الهام از فایل‌های JavaScript تمیز
    /// </summary>
    public class CleanDataTransmissionManager
    {
        private readonly Serilog.ILogger _logger;

        public CleanDataTransmissionManager(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// آماده‌سازی داده‌ها برای ارسال با فرمت مناسب
        /// </summary>
        /// <param name="data">داده‌های اصلی</param>
        /// <returns>داده‌های آماده شده</returns>
        public string PrepareDataForTransmission(object data)
        {
            try
            {
                _logger.Information("آماده‌سازی داده‌ها برای انتقال...");
                
                // تبدیل به JSON
                var jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                _logger.Information($"داده‌های JSON آماده شده: {jsonData.Length} کاراکتر");
                return jsonData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در آماده‌سازی داده‌ها");
                throw;
            }
        }

        /// <summary>
        /// پردازش پاسخ دریافتی از سرور
        /// </summary>
        /// <param name="responseContent">محتوای پاسخ</param>
        /// <returns>داده‌های پردازش شده</returns>
        public async Task<string> ProcessReceivedDataAsync(string responseContent)
        {
            try
            {
                _logger.Information("پردازش داده‌های دریافتی...");
                
                if (string.IsNullOrEmpty(responseContent))
                {
                    _logger.Warning("محتوای پاسخ خالی است");
                    return string.Empty;
                }

                // بررسی نوع محتوا
                if (IsValidJson(responseContent))
                {
                    _logger.Information("محتوای دریافتی JSON معتبر است");
                    return responseContent;
                }

                if (IsValidBase64(responseContent))
                {
                    _logger.Information("محتوای دریافتی Base64 است");
                    return responseContent;
                }

                _logger.Information("محتوای دریافتی متن عادی است");
                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در پردازش داده‌های دریافتی");
                return responseContent;
            }
        }

        /// <summary>
        /// ایجاد محتوای فرم برای ارسال
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده</param>
        /// <param name="root">ریشه درخواست</param>
        /// <param name="path">مسیر درخواست</param>
        /// <returns>محتوای فرم</returns>
        public FormUrlEncodedContent CreateFormContent(string encryptedData, string root, string path)
        {
            try
            {
                _logger.Information($"ایجاد محتوای فرم - Root: {root}, Path: {path}");
                
                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("data", encryptedData),
                    new KeyValuePair<string, string>("root", root),
                    new KeyValuePair<string, string>("path", path)
                };

                return new FormUrlEncodedContent(formData);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در ایجاد محتوای فرم");
                throw;
            }
        }

        /// <summary>
        /// تنظیم headers برای درخواست HTTP
        /// </summary>
        /// <param name="httpClient">کلاینت HTTP</param>
        /// <param name="additionalHeaders">headers اضافی</param>
        public void SetupRequestHeaders(HttpClient httpClient, Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                _logger.Information("تنظیم headers درخواست...");

                // حذف headers قبلی
                httpClient.DefaultRequestHeaders.Clear();

                // تنظیم headers پیش‌فرض
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\"");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                httpClient.DefaultRequestHeaders.Add("sec", "site");
                httpClient.DefaultRequestHeaders.Add("Referer", "https://b2b.isaco.ir/home");

                // اضافه کردن headers اضافی
                if (additionalHeaders != null)
                {
                    foreach (var header in additionalHeaders)
                    {
                        try
                        {
                            if (httpClient.DefaultRequestHeaders.Contains(header.Key))
                            {
                                httpClient.DefaultRequestHeaders.Remove(header.Key);
                            }
                            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning($"خطا در اضافه کردن header {header.Key}: {ex.Message}");
                        }
                    }
                }

                _logger.Information("Headers با موفقیت تنظیم شدند");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تنظیم headers");
                throw;
            }
        }

        /// <summary>
        /// بررسی معتبر بودن JSON
        /// </summary>
        /// <param name="jsonString">رشته JSON</param>
        /// <returns>true اگر معتبر باشد</returns>
        private bool IsValidJson(string jsonString)
        {
            try
            {
                JsonConvert.DeserializeObject(jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// بررسی معتبر بودن Base64
        /// </summary>
        /// <param name="base64String">رشته Base64</param>
        /// <returns>true اگر معتبر باشد</returns>
        private bool IsValidBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// مدیریت رمزنگاری و رمزگشایی با الهام از فایل‌های JavaScript تمیز
    /// </summary>
    public class CleanEncryptionManager
    {
        private readonly Serilog.ILogger _logger;
        private readonly string _publicKey;
        private readonly string _privateKey;

        public string PublicKey => _publicKey;
        public string PrivateKey => _privateKey;

        public CleanEncryptionManager(Serilog.ILogger logger, string publicKey = null, string privateKey = null)
        {
            _logger = logger;
            _publicKey = publicKey ?? "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
            _privateKey = privateKey ?? "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";
        }

        /// <summary>
        /// رمزنگاری داده‌ها با RSA - مطابق با پارامترهای JavaScript WebCrypto API
        /// </summary>
        /// <param name="plainText">متن اصلی</param>
        /// <returns>متن رمزنگاری شده</returns>
        public string EncryptData(string plainText)
        {
            try
            {
                _logger.Information("شروع رمزنگاری داده‌ها با پارامترهای WebCrypto API...");
                
                if (string.IsNullOrEmpty(plainText))
                {
                    _logger.Warning("متن ورودی خالی است");
                    return string.Empty;
                }

                using (var rsa = RSA.Create(2048)) // modulusLength: 2048
                {
                    // بارگذاری کلید عمومی
                    var publicKeyBytes = Convert.FromBase64String(_publicKey);
                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    
                    // رمزنگاری داده‌ها با RSA-OAEP SHA-1 (مطابق با WebCrypto API)
                    var dataBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA1);
                    
                    var encryptedText = Convert.ToBase64String(encryptedBytes);
                    _logger.Information($"رمزنگاری با RSA-OAEP SHA-1 با موفقیت انجام شد. طول متن رمز شده: {encryptedText.Length}");
                    
                    return encryptedText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزنگاری داده‌ها با RSA-OAEP");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌ها با کلید خصوصی - مطابق با پارامترهای JavaScript WebCrypto API
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        public string DecryptData(string encryptedData)
        {
            try
            {
                _logger.Information("شروع رمزگشایی داده‌ها با پارامترهای WebCrypto API...");
                
                if (string.IsNullOrEmpty(encryptedData))
                {
                    _logger.Warning("داده‌های رمز شده خالی است");
                    return string.Empty;
                }

                using (var rsa = RSA.Create(2048)) // modulusLength: 2048
                {
                    var privateKeyBytes = Convert.FromBase64String(_privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedData);
                    
                    // استفاده از RSA-OAEP با SHA-1 (مطابق با WebCrypto API)
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
                    
                    var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    _logger.Information($"رمزگشایی با RSA-OAEP SHA-1 موفقیت انجام شد. طول متن: {decryptedText.Length}");
                    
                    return decryptedText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی داده‌ها با RSA-OAEP");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌ها با کلید دریافتی از سرور - مطابق با پارامترهای JavaScript WebCrypto API
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده</param>
        /// <param name="serverKey">کلید دریافتی از سرور</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        public string DecryptWithServerKey(string encryptedData, string serverKey)
        {
            try
            {
                _logger.Information("شروع رمزگشایی با کلید سرور - WebCrypto API parameters...");
                
                if (string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(serverKey))
                {
                    _logger.Warning("داده‌های رمز شده یا کلید سرور خالی است");
                    return encryptedData;
                }

                using (var rsa = RSA.Create(2048)) // modulusLength: 2048
                {
                    var serverKeyBytes = Convert.FromBase64String(serverKey);
                    rsa.ImportPkcs8PrivateKey(serverKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedData);
                    
                    // استفاده از RSA-OAEP با SHA-1 (مطابق با WebCrypto API)
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
                    
                    var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    _logger.Information($"رمزگشایی با کلید سرور و RSA-OAEP SHA-1 با موفقیت انجام شد");
                    
                    return decryptedText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی با کلید سرور");
                return $"خطا در رمزگشایی: {ex.Message}";
            }
        }

        /// <summary>
        /// ایجاد هش SHA256 از داده‌ها
        /// </summary>
        /// <param name="data">داده‌های ورودی</param>
        /// <returns>هش SHA256</returns>
        public string CreateHash(string data)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                    return Convert.ToBase64String(hashBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در ایجاد هش");
                throw;
            }
        }

        /// <summary>
        /// بررسی صحت هش
        /// </summary>
        /// <param name="data">داده‌های اصلی</param>
        /// <param name="hash">هش برای مقایسه</param>
        /// <returns>true اگر هش صحیح باشد</returns>
        public bool VerifyHash(string data, string hash)
        {
            try
            {
                var computedHash = CreateHash(data);
                return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در بررسی هش");
                return false;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌های ترکیبی (AES + RSA) - مطابق با خروجی سرور
        /// </summary>
        /// <param name="encryptedData">داده‌های AES رمزنگاری شده</param>
        /// <param name="encryptedKey">کلید AES رمزنگاری شده با RSA</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        public string DecryptHybridData(string encryptedData, string encryptedKey)
        {
            try
            {
                _logger.Information("شروع رمزگشایی داده‌های ترکیبی (AES + RSA)...");
                
                if (string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(encryptedKey))
                {
                    _logger.Warning("داده‌های رمز شده یا کلید رمز شده خالی است");
                    return encryptedData;
                }

                // مرحله 1: رمزگشایی کلید AES با RSA
                _logger.Information("مرحله 1: رمزگشایی کلید AES با RSA...");
                var aesKey = DecryptRSAKey(encryptedKey);
                _logger.Information($"کلید AES رمزگشایی شد. طول: {aesKey.Length} بایت");

                // مرحله 2: رمزگشایی داده‌ها با AES
                _logger.Information("مرحله 2: رمزگشایی داده‌ها با AES...");
                var decryptedData = DecryptAESData(encryptedData, aesKey);
                _logger.Information($"داده‌های AES رمزگشایی شد. طول: {decryptedData.Length} کاراکتر");

                return decryptedData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی داده‌های ترکیبی");
                return $"خطا در رمزگشایی ترکیبی: {ex.Message}";
            }
        }

        /// <summary>
        /// رمزگشایی کلید AES با RSA
        /// </summary>
        /// <param name="encryptedKey">کلید رمزنگاری شده</param>
        /// <returns>کلید AES</returns>
        private byte[] DecryptRSAKey(string encryptedKey)
        {
            try
            {
                using (var rsa = RSA.Create(2048))
                {
                    var privateKeyBytes = Convert.FromBase64String(_privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedKey);
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                    
                    _logger.Information($"کلید RSA رمزگشایی شد. طول: {decryptedBytes.Length} بایت");
                    return decryptedBytes;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی کلید RSA");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌های AES (CryptoJS format)
        /// </summary>
        /// <param name="encryptedData">داده‌های AES رمزنگاری شده</param>
        /// <param name="key">کلید AES</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        private string DecryptAESData(string encryptedData, byte[] key)
        {
            try
            {
                _logger.Information($"شروع رمزگشایی AES - طول داده: {encryptedData.Length}");
                
                // بررسی فرمت CryptoJS (U2FsdGVkX1 prefix)
                if (!encryptedData.StartsWith("U2FsdGVkX1"))
                {
                    _logger.Warning("فرمت داده‌های AES معتبر نیست (CryptoJS format expected)");
                    return encryptedData;
                }

                // حذف prefix "U2FsdGVkX1" و تبدیل به Base64
                var base64Data = encryptedData;//.Substring(10); // حذف "U2FsdGVkX1"
                var fullData = Convert.FromBase64String(base64Data);
                
                _logger.Information($"داده‌های Base64 تبدیل شد - طول: {fullData.Length} بایت");

                // بررسی حداقل طول داده‌ها (8 بایت salt + حداقل 16 بایت encrypted data)
                if (fullData.Length < 24)
                {
                    _logger.Error($"داده‌های ناکافی - طول: {fullData.Length} بایت");
                    return encryptedData;
                }

                // استخراج salt (8 بایت اول)
                var salt = new byte[8];
                Array.Copy(fullData, 0, salt, 0, 8);
                _logger.Information($"Salt استخراج شد - طول: {salt.Length} بایت");

                // استخراج encrypted data (باقی داده‌ها)
                var encrypted = new byte[fullData.Length - 8];
                Array.Copy(fullData, 8, encrypted, 0, encrypted.Length);
                _logger.Information($"داده‌های رمز شده استخراج شد - طول: {encrypted.Length} بایت");

                // تولید کلید و IV از password و salt
                var keyIv = DeriveKeyAndIV(key, salt);
                var derivedKey = keyIv.Item1;
                var iv = keyIv.Item2;
                
                _logger.Information($"کلید و IV تولید شد - کلید: {derivedKey.Length} بایت، IV: {iv.Length} بایت");

                // رمزگشایی با AES-256-CBC
                using (var aes = Aes.Create())
                {
                    aes.Key = derivedKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(encrypted))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        var decryptedText = srDecrypt.ReadToEnd();
                        _logger.Information($"داده‌های AES رمزگشایی شد. طول: {decryptedText.Length} کاراکتر");
                        return decryptedText;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی داده‌های AES");
                return $"خطا در رمزگشایی AES: {ex.Message}";
            }
        }

        /// <summary>
        /// تولید کلید و IV از password و salt (مطابق با CryptoJS)
        /// </summary>
        /// <param name="password">رمز عبور</param>
        /// <param name="salt">نمک</param>
        /// <returns>کلید و IV</returns>
        private (byte[], byte[]) DeriveKeyAndIV(byte[] password, byte[] salt)
        {
            try
            {
                _logger.Information($"شروع تولید کلید و IV - طول password: {password.Length}, طول salt: {salt.Length}");
                
                using (var md5 = MD5.Create())
                {
                    var key = new byte[32]; // 256 bits
                    var iv = new byte[16];   // 128 bits
                    
                    var derived = new List<byte>();
                    var currentHash = new byte[0];
                    
                    // CryptoJS uses multiple rounds of MD5 hashing
                    while (derived.Count < 48) // 32 bytes key + 16 bytes IV
                    {
                        var toHash = new List<byte>();
                        
                        // Add previous hash if exists
                        if (currentHash.Length > 0)
                        {
                            toHash.AddRange(currentHash);
                        }
                        
                        // Add password
                        toHash.AddRange(password);
                        
                        // Add salt
                        toHash.AddRange(salt);
                        
                        // Compute MD5 hash
                        currentHash = md5.ComputeHash(toHash.ToArray());
                        derived.AddRange(currentHash);
                        
                        _logger.Information($"MD5 round completed - derived length: {derived.Count}");
                    }
                    
                    // Extract key and IV
                    var derivedArray = derived.ToArray();
                    Array.Copy(derivedArray, 0, key, 0, 32);
                    Array.Copy(derivedArray, 32, iv, 0, 16);
                    
                    _logger.Information($"کلید و IV تولید شد - کلید: {Convert.ToBase64String(key)}, IV: {Convert.ToBase64String(iv)}");
                    
                    return (key, iv);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تولید کلید و IV");
                throw;
            }
        }

        /// <summary>
        /// رمزنگاری داده‌ها با پارامترهای دقیق WebCrypto API - مطابق با JavaScript
        /// </summary>
        /// <param name="plainText">متن اصلی</param>
        /// <param name="publicKey">کلید عمومی</param>
        /// <returns>داده‌های رمزنگاری شده (Base64)</returns>
        public string EncryptWithWebCryptoParams(string plainText, string publicKey)
        {
            try
            {
                _logger.Information("شروع رمزنگاری با پارامترهای دقیق WebCrypto API...");
                
                if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(publicKey))
                {
                    _logger.Warning("متن ورودی یا کلید عمومی خالی است");
                    return string.Empty;
                }

                using (var rsa = RSA.Create(2048)) // modulusLength: 2048
                {
                    // بارگذاری کلید عمومی
                    var publicKeyBytes = Convert.FromBase64String(publicKey);
                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    
                    // تبدیل متن به بایت
                    var dataBytes = Encoding.UTF8.GetBytes(plainText);
                    
                    // رمزنگاری با RSA-OAEP SHA-1 (مطابق با name: "RSA-OAEP")
                    var encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA1);
                    
                    // تبدیل به Base64
                    var encryptedText = Convert.ToBase64String(encryptedBytes);
                    
                    _logger.Information($"رمزنگاری با WebCrypto API parameters موفقیت انجام شد. طول متن رمز شده: {encryptedText.Length}");
                    
                    return encryptedText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزنگاری با WebCrypto API parameters");
                throw;
            }
        }

        /// <summary>
        /// پردازش ArrayBuffer و Uint8Array مطابق با JavaScript WebCrypto API
        /// </summary>
        /// <param name="arrayBuffer">داده‌های ArrayBuffer</param>
        /// <returns>داده‌های پردازش شده</returns>
        public byte[] ProcessArrayBuffer(byte[] arrayBuffer)
        {
            try
            {
                _logger.Information($"پردازش ArrayBuffer با اندازه: {arrayBuffer.Length} بایت");
                
                // شبیه‌سازی Uint8Array از JavaScript
                var uint8Array = new byte[arrayBuffer.Length];
                Array.Copy(arrayBuffer, uint8Array, arrayBuffer.Length);
                
                _logger.Information($"Uint8Array ایجاد شد با طول: {uint8Array.Length}");
                
                return uint8Array;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در پردازش ArrayBuffer");
                throw;
            }
        }

        /// <summary>
        /// رمزگشایی داده‌ها با پارامترهای دقیق WebCrypto API - مطابق با JavaScript
        /// </summary>
        /// <param name="encryptedData">داده‌های رمزنگاری شده (Base64)</param>
        /// <param name="privateKey">کلید خصوصی</param>
        /// <returns>داده‌های رمزگشایی شده</returns>
        public string DecryptWithWebCryptoParams(string encryptedData, string privateKey)
        {
            try
            {
                _logger.Information("شروع رمزگشایی با پارامترهای دقیق WebCrypto API...");
                
                if (string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(privateKey))
                {
                    _logger.Warning("داده‌های رمز شده یا کلید خصوصی خالی است");
                    return encryptedData;
                }

                using (var rsa = RSA.Create(2048)) // modulusLength: 2048
                {
                    // بارگذاری کلید خصوصی
                    var privateKeyBytes = Convert.FromBase64String(privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    // تبدیل داده‌های رمز شده از Base64 به بایت
                    var encryptedBytes = Convert.FromBase64String(encryptedData);
                    
                    // بررسی اندازه داده‌ها (مطابق با byteLength: 3)
                    _logger.Information($"اندازه داده‌های رمز شده: {encryptedBytes.Length} بایت");
                    
                    // رمزگشایی با RSA-OAEP SHA-1 (مطابق با name: "RSA-OAEP")
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
                    
                    // تبدیل به رشته UTF-8
                    var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                    
                    _logger.Information($"رمزگشایی با WebCrypto API parameters موفقیت انجام شد. طول متن: {decryptedText.Length}");
                    
                    return decryptedText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی با WebCrypto API parameters");
                return $"خطا در رمزگشایی: {ex.Message}";
            }
        }
    }

}
