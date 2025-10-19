using System;
using System.Threading.Tasks;
using Serilog;
using Ntk.Chrome.Services;

namespace Ntk.Chrome.Services
{
    /// <summary>
    /// نمونه استفاده از کلاس ApiTestLogin
    /// </summary>
    public class ApiTestLoginExample
    {
        private readonly ApiTestLogin _apiTestLogin;
        private readonly Serilog.ILogger _logger;

        public ApiTestLoginExample()
        {
            _logger = Serilog.Log.Logger;
            _apiTestLogin = new ApiTestLogin("https://b2b.isaco.ir/api", _logger);
        }

        /// <summary>
        /// نمونه ورود کاربر
        /// </summary>
        public async Task TestLoginAsync()
        {
            try
            {
                _logger.Information("شروع تست ورود کاربر...");

                // اطلاعات ورود
                var userName = "testuser";
                var password = "testpassword";
                var scus = "1234567"; // کد مشتری (اختیاری)
                var clientVersion = "1.0.0";

                // ورود کاربر
                var loginResponse = await _apiTestLogin.LoginAsync(userName, password, scus, clientVersion);

                if (loginResponse.Success)
                {
                    _logger.Information($"ورود موفقیت‌آمیز بود. وضعیت: {loginResponse.StatusCode}");
                    
                    if (loginResponse.Data != null)
                    {
                        _logger.Information($"نام کاربری: {loginResponse.Data.UserName}");
                        _logger.Information($"توکن: {loginResponse.Data.Token}");
                        _logger.Information($"وضعیت: {loginResponse.Data.Status}");
                        
                        // بررسی وضعیت ورود
                        switch (loginResponse.Data.Status)
                        {
                            case 1:
                                _logger.Information("کاربر باید رمز عبور را تغییر دهد");
                                break;
                            case 2:
                                _logger.Information("ورود موفقیت‌آمیز");
                                break;
                            default:
                                _logger.Warning($"وضعیت نامشخص: {loginResponse.Data.Status}");
                                break;
                        }
                    }
                }
                else
                {
                    _logger.Error($"خطا در ورود. وضعیت: {loginResponse.StatusCode}");
                    _logger.Error($"کد خطا: {loginResponse.MessageCode}");
                    _logger.Error($"پیام خطا: {loginResponse.MessageDes}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست ورود کاربر");
            }
        }

        /// <summary>
        /// نمونه دریافت Captcha
        /// </summary>
        public async Task TestGetCaptchaAsync()
        {
            try
            {
                _logger.Information("شروع تست دریافت Captcha...");

                // دریافت Captcha
                var captchaResponse = await _apiTestLogin.GetCaptchaAsync("fa-IR");

                if (captchaResponse.Success)
                {
                    _logger.Information($"دریافت Captcha موفقیت‌آمیز بود. وضعیت: {captchaResponse.StatusCode}");
                    
                    if (captchaResponse.Data != null)
                    {
                        _logger.Information($"تصویر Captcha: {captchaResponse.Data.ImgCapcha}");
                        _logger.Information($"زمان سرور: {captchaResponse.Data.ServerTime}");
                        
                        // در اینجا می‌توانید تصویر Captcha را نمایش دهید
                        // یا آن را در فایل ذخیره کنید
                    }
                }
                else
                {
                    _logger.Error($"خطا در دریافت Captcha. وضعیت: {captchaResponse.StatusCode}");
                    _logger.Error($"کد خطا: {captchaResponse.MessageCode}");
                    _logger.Error($"پیام خطا: {captchaResponse.MessageDes}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست دریافت Captcha");
            }
        }

        /// <summary>
        /// نمونه ارسال پیامک
        /// </summary>
        public async Task TestSendSmsAsync()
        {
            try
            {
                _logger.Information("شروع تست ارسال پیامک...");

                // شماره موبایل
                var mobileNumber = "09123456789";

                // ارسال پیامک
                var smsResponse = await _apiTestLogin.SendSmsAsync(mobileNumber);

                if (smsResponse.Success)
                {
                    _logger.Information($"ارسال پیامک موفقیت‌آمیز بود. وضعیت: {smsResponse.StatusCode}");
                    _logger.Information($"پیام: {smsResponse.MessageDes}");
                    
                    if (smsResponse.Data != null)
                    {
                        _logger.Information($"وضعیت ارسال: {smsResponse.Data.Sent}");
                        _logger.Information($"پیام: {smsResponse.Data.Message}");
                    }
                }
                else
                {
                    _logger.Error($"خطا در ارسال پیامک. وضعیت: {smsResponse.StatusCode}");
                    _logger.Error($"کد خطا: {smsResponse.MessageCode}");
                    _logger.Error($"پیام خطا: {smsResponse.MessageDes}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست ارسال پیامک");
            }
        }

        /// <summary>
        /// نمونه ورود با Captcha
        /// </summary>
        public async Task TestLoginWithCaptchaAsync()
        {
            try
            {
                _logger.Information("شروع تست ورود با Captcha...");

                // ابتدا Captcha دریافت کنید
                var captchaResponse = await _apiTestLogin.GetCaptchaAsync("fa-IR");
                
                if (!captchaResponse.Success)
                {
                    _logger.Error("خطا در دریافت Captcha");
                    return;
                }

                // اطلاعات ورود با Captcha
                var userName = "testuser";
                var password = "testpassword";
                var captchaCode = "1234"; // کد Captcha که کاربر وارد کرده
                var scus = "1234567";

                // ورود کاربر با Captcha
                var loginResponse = await _apiTestLogin.LoginAsync(userName, password, scus);

                if (loginResponse.Success)
                {
                    _logger.Information($"ورود موفقیت‌آمیز بود. وضعیت: {loginResponse.StatusCode}");
                    
                    if (loginResponse.Data != null)
                    {
                        _logger.Information($"نام کاربری: {loginResponse.Data.UserName}");
                        _logger.Information($"توکن: {loginResponse.Data.Token}");
                        _logger.Information($"وضعیت: {loginResponse.Data.Status}");
                    }
                }
                else
                {
                    _logger.Error($"خطا در ورود. وضعیت: {loginResponse.StatusCode}");
                    _logger.Error($"کد خطا: {loginResponse.MessageCode}");
                    _logger.Error($"پیام خطا: {loginResponse.MessageDes}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست ورود با Captcha");
            }
        }

        /// <summary>
        /// نمونه ورود با مدیریت خطا
        /// </summary>
        public async Task TestLoginWithErrorHandlingAsync()
        {
            try
            {
                _logger.Information("شروع تست ورود با مدیریت خطا...");

                // اطلاعات ورود
                var userName = "invaliduser";
                var password = "wrongpassword";

                // ورود کاربر
                var loginResponse = await _apiTestLogin.LoginAsync(userName, password);

                if (loginResponse.Success)
                {
                    _logger.Information($"ورود موفقیت‌آمیز بود. وضعیت: {loginResponse.StatusCode}");
                }
                else
                {
                    _logger.Error($"خطا در ورود. وضعیت: {loginResponse.StatusCode}");
                    _logger.Error($"کد خطا: {loginResponse.MessageCode}");
                    _logger.Error($"پیام خطا: {loginResponse.MessageDes}");
                    
                    // مدیریت خطاهای مختلف
                    switch (loginResponse.MessageCode)
                    {
                        case "INVALID_CREDENTIALS":
                            _logger.Information("نام کاربری یا رمز عبور اشتباه است");
                            break;
                        case "ACCOUNT_LOCKED":
                            _logger.Information("حساب کاربری قفل شده است");
                            break;
                        case "CAPTCHA_REQUIRED":
                            _logger.Information("نیاز به وارد کردن Captcha است");
                            break;
                        default:
                            _logger.Information($"خطای نامشخص: {loginResponse.MessageCode}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست ورود با مدیریت خطا");
            }
        }

        /// <summary>
        /// اجرای تمام تست‌ها
        /// </summary>
        public async Task RunAllTestsAsync()
        {
            try
            {
                _logger.Information("شروع اجرای تمام تست‌های ورود...");

                await TestGetCaptchaAsync();
                await Task.Delay(1000); // تاخیر بین تست‌ها

                await TestSendSmsAsync();
                await Task.Delay(1000);

                await TestLoginAsync();
                await Task.Delay(1000);

                await TestLoginWithCaptchaAsync();
                await Task.Delay(1000);

                await TestLoginWithErrorHandlingAsync();

                _logger.Information("تمام تست‌های ورود با موفقیت اجرا شدند.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در اجرای تست‌های ورود");
            }
            finally
            {
                // آزاد کردن منابع
                _apiTestLogin?.Dispose();
            }
        }
    }
}
