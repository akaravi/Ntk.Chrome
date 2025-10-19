using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;

namespace Ntk.Chrome.Services
{
    public class ApiTest1
    {
        private readonly HttpClient _httpClient;
        private readonly Serilog.ILogger _logger;
        private readonly string _baseUrl;
        private readonly string _publicKey;
        private readonly string _privateKey;

        // کلیدهای RSA برای رمزنگاری و رمزگشایی
        private const string REACT_APP_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
        
        private const string REACT_APP_PRIVATE_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";

        public ApiTest1(string baseUrl = "https://your-api-base-url.com", Serilog.ILogger logger = null)
        {
            _baseUrl = baseUrl;
            _logger = logger ?? Serilog.Log.Logger;
            _httpClient = new HttpClient();
            _publicKey = REACT_APP_PUBLIC_KEY;
            _privateKey = REACT_APP_PRIVATE_KEY;
        }

        /// <summary>
        /// ارسال درخواست به API با رمزنگاری RSA
        /// </summary>
        /// <param name="endpoint">آدرس API</param>
        /// <param name="data">داده‌های ارسالی</param>
        /// <param name="root">Root parameter</param>
        /// <param name="path">Path parameter</param>
        /// <returns>پاسخ رمزگشایی شده</returns>
        public async Task<ApiResponse> SendEncryptedRequestAsync(string endpoint, object data, string root, int path)
        {
            try
            {
                _logger.Information($"ارسال درخواست رمزنگاری شده به {endpoint}");
                
                // تبدیل داده‌ها به JSON
                var jsonData = JsonConvert.SerializeObject(data);
                _logger.Information($"داده‌های JSON: {jsonData}");

                // رمزنگاری داده‌ها با RSA
                var encryptedData = EncryptData(jsonData);
                _logger.Information($"داده‌های رمزنگاری شده: {encryptedData}");

                // ایجاد محتوای فرم
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("data", encryptedData),
                    new KeyValuePair<string, string>("root", root),
                    new KeyValuePair<string, string>("path", path.ToString())
                });

                // ارسال درخواست POST
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", formContent);
                
                _logger.Information($"وضعیت پاسخ: {(int)response.StatusCode} {response.StatusCode}");

                // دریافت محتوای پاسخ
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Information($"محتوای پاسخ خام: {responseContent}");

                // رمزگشایی پاسخ
                var decryptedResponse = await DecryptResponseAsync(responseContent);
                
                return new ApiResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = decryptedResponse,
                    RawContent = responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در ارسال درخواست به API");
                return new ApiResponse
                {
                    Success = false,
                    StatusCode = 0,
                    Data = null,
                    RawContent = ex.Message
                };
            }
        }

        /// <summary>
        /// ارسال درخواست به API RepairCard
        /// </summary>
        /// <param name="repairCardData">داده‌های کارت تعمیر</param>
        /// <returns>پاسخ API</returns>
        public async Task<ApiResponse> SendRepairCardRequestAsync(object repairCardData)
        {
            return await SendEncryptedRequestAsync(
                "Srv/rcpt/RepairCard/Drcds/q", 
                repairCardData, 
                "Srv/rcpt/RepairCard", 
                1
            );
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
                using (var rsa = RSA.Create())
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
        /// رمزگشایی پاسخ دریافتی از API
        /// </summary>
        /// <param name="encryptedContent">محتوای رمزنگاری شده</param>
        /// <returns>محتوای رمزگشایی شده</returns>
        private async Task<string> DecryptResponseAsync(string encryptedContent)
        {
            try
            {
                _logger.Information("شروع رمزگشایی محتوای پاسخ...");

                // بررسی اینکه آیا محتوا JSON است یا نه
                if (IsValidJson(encryptedContent))
                {
                    var dataModel = JsonConvert.DeserializeObject<EncryptedDataModel>(encryptedContent);
                    if (dataModel != null && !string.IsNullOrEmpty(dataModel.Data) && !string.IsNullOrEmpty(dataModel.Key))
                    {
                        _logger.Information($"داده‌های JSON یافت شد - data: {dataModel.Data}, key: {dataModel.Key}");
                        
                        // رمزگشایی با کلید خصوصی
                        var decryptedData = DecryptWithPrivateKey(dataModel.Data, dataModel.Key);
                        return decryptedData;
                    }
                }

                // اگر محتوا Base64 است، آن را رمزگشایی کن
                if (IsValidBase64(encryptedContent))
                {
                    var decryptedData = DecryptWithPrivateKey(encryptedContent, _privateKey);
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
                using (var rsa = RSA.Create())
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
    /// مدل پاسخ API
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Data { get; set; }
        public string RawContent { get; set; }
    }

    /// <summary>
    /// مدل داده‌های رمزنگاری شده
    /// </summary>
    public class EncryptedDataModel
    {
        public string Data { get; set; }
        public string Key { get; set; }
    }
}
