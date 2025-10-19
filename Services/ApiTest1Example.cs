using System;
using System.Threading.Tasks;
using Serilog;
using Ntk.Chrome.Services;

namespace Ntk.Chrome.Services
{
    /// <summary>
    /// نمونه استفاده از کلاس ApiTest1
    /// </summary>
    public class ApiTest1Example
    {
        private readonly ApiTest1 _apiTest;
        private readonly Serilog.ILogger _logger;

        public ApiTest1Example()
        {
            _logger = Serilog.Log.Logger;
            _apiTest = new ApiTest1("https://your-api-base-url.com", _logger);
        }

        /// <summary>
        /// نمونه استفاده از API RepairCard
        /// </summary>
        public async Task TestRepairCardAPI()
        {
            try
            {
                _logger.Information("شروع تست API RepairCard...");

                // نمونه داده‌های کارت تعمیر
                var repairCardData = new
                {
                    CardNumber = "1234567890",
                    CustomerName = "احمد محمدی",
                    PhoneNumber = "09123456789",
                    IssueDescription = "مشکل در عملکرد دستگاه",
                    RequestDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Priority = "High",
                    Location = "تهران"
                };

                // ارسال درخواست به API
                var response = await _apiTest.SendRepairCardRequestAsync(repairCardData);

                if (response.Success)
                {
                    _logger.Information($"درخواست با موفقیت ارسال شد. وضعیت: {response.StatusCode}");
                    _logger.Information($"پاسخ دریافتی: {response.Data}");
                }
                else
                {
                    _logger.Error($"خطا در ارسال درخواست. وضعیت: {response.StatusCode}");
                    _logger.Error($"پیام خطا: {response.RawContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست API RepairCard");
            }
        }

        /// <summary>
        /// نمونه استفاده از API عمومی
        /// </summary>
        public async Task TestGeneralAPI()
        {
            try
            {
                _logger.Information("شروع تست API عمومی...");

                // نمونه داده‌های عمومی
                var generalData = new
                {
                    UserId = "12345",
                    Action = "GetUserInfo",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Language = "fa-IR"
                };

                // ارسال درخواست به API
                var response = await _apiTest.SendEncryptedRequestAsync(
                    "Srv/User/GetInfo", 
                    generalData, 
                    "Srv/User", 
                    1
                );

                if (response.Success)
                {
                    _logger.Information($"درخواست با موفقیت ارسال شد. وضعیت: {response.StatusCode}");
                    _logger.Information($"پاسخ دریافتی: {response.Data}");
                }
                else
                {
                    _logger.Error($"خطا در ارسال درخواست. وضعیت: {response.StatusCode}");
                    _logger.Error($"پیام خطا: {response.RawContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست API عمومی");
            }
        }

        /// <summary>
        /// نمونه استفاده از API با داده‌های پیچیده
        /// </summary>
        public async Task TestComplexDataAPI()
        {
            try
            {
                _logger.Information("شروع تست API با داده‌های پیچیده...");

                // نمونه داده‌های پیچیده
                var complexData = new
                {
                    RequestId = Guid.NewGuid().ToString(),
                    User = new
                    {
                        Id = 12345,
                        Name = "احمد محمدی",
                        Email = "ahmad@example.com",
                        Phone = "09123456789"
                    },
                    Items = new[]
                    {
                        new { Id = 1, Name = "آیتم 1", Quantity = 5 },
                        new { Id = 2, Name = "آیتم 2", Quantity = 3 }
                    },
                    Metadata = new
                    {
                        CreatedAt = DateTime.Now,
                        Version = "1.0",
                        Source = "MobileApp"
                    }
                };

                // ارسال درخواست به API
                var response = await _apiTest.SendEncryptedRequestAsync(
                    "Srv/Order/Create", 
                    complexData, 
                    "Srv/Order", 
                    2
                );

                if (response.Success)
                {
                    _logger.Information($"درخواست با موفقیت ارسال شد. وضعیت: {response.StatusCode}");
                    _logger.Information($"پاسخ دریافتی: {response.Data}");
                }
                else
                {
                    _logger.Error($"خطا در ارسال درخواست. وضعیت: {response.StatusCode}");
                    _logger.Error($"پیام خطا: {response.RawContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست API با داده‌های پیچیده");
            }
        }

        /// <summary>
        /// اجرای تمام تست‌ها
        /// </summary>
        public async Task RunAllTests()
        {
            try
            {
                _logger.Information("شروع اجرای تمام تست‌ها...");

                await TestRepairCardAPI();
                await Task.Delay(1000); // تاخیر بین تست‌ها

                await TestGeneralAPI();
                await Task.Delay(1000);

                await TestComplexDataAPI();

                _logger.Information("تمام تست‌ها با موفقیت اجرا شدند.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در اجرای تست‌ها");
            }
            finally
            {
                // آزاد کردن منابع
                _apiTest?.Dispose();
            }
        }
    }
}
