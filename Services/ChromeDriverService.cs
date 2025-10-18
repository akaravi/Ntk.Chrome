using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V120.Network;
using System.Net.Http;
using System.IO.Compression;
using Ntk.Chrome.Models;
using Newtonsoft.Json;

namespace Ntk.Chrome.Services;

public class CustomChromeDriverService : IChromeDriverService
{
    private readonly string _driverPath;
    private readonly ILogger interydata;
    private const string CHROME_DRIVER_URL = "https://storage.googleapis.com/chrome-for-testing-public/";
    private const string CHROMIUM_URL = "https://storage.googleapis.com/chrome-for-testing-public/{0}/win64/chrome-win64.zip";
    private ChromeDriver? _driver;
        private readonly string _chromiumVersion;
        private readonly string _chromeDriverVersion;

    public CustomChromeDriverService(string driverPath, ILogger interydata)
    {
        var settings = File.Exists("settings.json") 
            ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("settings.json")) 
            ?? new AppSettings()
            : new AppSettings();

        // جایگزینی متغیرهای محیطی در مسیر و تبدیل به مسیر مطلق
        var expandedPath = Environment.ExpandEnvironmentVariables(driverPath);
        _driverPath = Path.IsPathRooted(expandedPath) 
            ? expandedPath 
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, expandedPath);
        this.interydata = interydata;
        _chromiumVersion = settings.ChromiumVersion;
        _chromeDriverVersion = settings.ChromeDriverVersion;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // بررسی وجود Chromium
            var chromiumPath = Path.Combine(_driverPath, "chromium", "chrome-win", "chrome.exe");
            var driverPath = Path.Combine(_driverPath, "driver", "chromedriver.exe");
            var shouldDownload = false;

            if (!File.Exists(chromiumPath))
            {
                interydata.Information($"Chromium در مسیر {chromiumPath} یافت نشد");
                shouldDownload = true;
            }
            else
            {
                interydata.Information($"Chromium در مسیر {chromiumPath} یافت شد");
                var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromiumPath).FileVersion;
                if (fileVersion != _chromiumVersion)
                {
                    interydata.Information($"نسخه Chromium ({fileVersion}) با نسخه مورد نظر ({_chromiumVersion}) متفاوت است");
                    shouldDownload = true;
                }
            }

            if (!File.Exists(driverPath))
            {
                interydata.Information($"ChromeDriver در مسیر {driverPath} یافت نشد");
                shouldDownload = true;
            }
            else
            {
                interydata.Information($"ChromeDriver در مسیر {driverPath} یافت شد");
            }

            if (shouldDownload)
            {
                interydata.Information("شروع دانلود و نصب Chromium و ChromeDriver...");
                await DownloadChromiumAsync();
                await DownloadChromeDriverAsync();
                interydata.Information("دانلود و نصب با موفقیت انجام شد");
            }
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در بررسی یا دانلود Chromium و ChromeDriver");
            throw;
        }
    }

    private async Task DownloadChromiumAsync()
    {
        try
        {
            interydata.Information($"شروع دانلود Chromium نسخه {_chromiumVersion}...");
            
            var chromiumPath = Path.Combine(_driverPath, "chromium");
            var chromiumExePath = Path.Combine(chromiumPath, "chrome-win", "chrome.exe");
            var shouldDownload = false;

            // بررسی وجود نسخه در آرشیو
            var archivePath = Path.Combine(_driverPath, "chromium", "archive");
            Directory.CreateDirectory(archivePath);
            var archiveFile = Path.Combine(archivePath, $"chromium-{_chromiumVersion}.zip");

            // اگر فایل اجرایی وجود دارد و نسخه آن درست است، نیازی به کاری نیست
            if (File.Exists(chromiumExePath))
            {
                var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromiumExePath).FileVersion;
                if (string.IsNullOrEmpty(fileVersion)) return;
                var currentMajorVersion = int.Parse(fileVersion.Split('.')[0]);
                if (currentMajorVersion == 120)
                {
                    interydata.Information($"نسخه فعلی Chromium ({fileVersion}) صحیح است");
                    return;
                }
            }

            // اگر فایل در آرشیو وجود دارد، از آن استفاده می‌کنیم
            if (File.Exists(archiveFile))
            {
                interydata.Information($"نسخه {_chromiumVersion} Chromium در آرشیو یافت شد");
                interydata.Information("استخراج Chromium از آرشیو...");
                
                try
                {
                    if (Directory.Exists(chromiumPath))
                    {
                        var result = MessageBox.Show(
                            $"آیا مایل به حذف فایل‌های قدیمی Chromium در مسیر {chromiumPath} هستید؟\n" +
                            "(فایل‌های موجود در پوشه archive حذف نخواهند شد)",
                            "حذف فایل‌های قدیمی",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            interydata.Information("عملیات حذف فایل‌های قدیمی توسط کاربر لغو شد");
                            return;
                        }
                    }
                    Directory.CreateDirectory(chromiumPath);
                    ZipFile.ExtractToDirectory(archiveFile, chromiumPath, true);
                    interydata.Information("Chromium با موفقیت از آرشیو استخراج شد");
                    return;
                }
                catch (Exception ex)
                {
                    interydata.Error(ex, "خطا در استخراج Chromium از آرشیو");
                    // حتی در صورت خطا در استخراج، فایل آرشیو را حفظ می‌کنیم
                }
            }

            // اگر در آرشیو نبود، نیاز به دانلود داریم
            interydata.Information($"نسخه {_chromiumVersion} Chromium در آرشیو یافت نشد");
            shouldDownload = true;

            // اگر نسخه قبلی نصب است، از کاربر تأیید می‌گیریم
            if (File.Exists(chromiumExePath))
            {
                var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromiumExePath).FileVersion;
                interydata.Information($"نسخه فعلی Chromium: {fileVersion}");
                {
                    interydata.Information($"نسخه Chromium ({fileVersion}) با نسخه مورد نظر ({_chromiumVersion}) متفاوت است");
                    shouldDownload = true;

                    var result = MessageBox.Show(
                        $"نسخه Chromium موجود ({fileVersion}) با نسخه مورد نظر (120.0.6099.109) سازگار نیست.\n" +
                        "برای عملکرد صحیح برنامه، نیاز به دانلود نسخه سازگار است.\n" +
                        "آیا می‌خواهید نسخه جدید دانلود شود؟",
                        "به‌روزرسانی Chromium",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            var deleteResult = MessageBox.Show(
                                $"آیا مایل به حذف فایل‌های قدیمی Chromium در مسیر {chromiumPath} هستید؟\n" +
                                "(فایل‌های موجود در پوشه archive حذف نخواهند شد)",
                                "حذف فایل‌های قدیمی",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (deleteResult == DialogResult.No)
                            {
                                interydata.Information("عملیات حذف فایل‌های قدیمی توسط کاربر لغو شد");
                                return;
                            }

                            // متوقف کردن تمام پروسه‌های Chrome
                            foreach (var process in System.Diagnostics.Process.GetProcessesByName("chrome"))
                            {
                                try 
                                { 
                                    process.Kill();
                                    process.WaitForExit(5000); // صبر می‌کنیم تا پروسه کاملاً بسته شود
                                } 
                                catch (Exception ex) 
                                {
                                    interydata.Warning($"خطا در بستن پروسه Chrome: {ex.Message}");
                                }
                            }
                            
                            // کمی صبر می‌کنیم تا مطمئن شویم همه پروسه‌ها بسته شده‌اند
                            System.Threading.Thread.Sleep(1000);
                            
                            // حذف فایل‌های Chromium به جز پوشه archive و محتویات آن
                            var files = Directory.GetFiles(chromiumPath, "*.*", SearchOption.TopDirectoryOnly);
                            foreach (var file in files)
                            {
                                try 
                                { 
                                    File.Delete(file);
                                    interydata.Information($"فایل {Path.GetFileName(file)} حذف شد");
                                } 
                                catch (Exception ex) 
                                {
                                    interydata.Warning($"خطا در حذف فایل {Path.GetFileName(file)}: {ex.Message}");
                                }
                            }
                            var dirs = Directory.GetDirectories(chromiumPath);
                            var archiveDir = Path.Combine(chromiumPath, "archive");
                            foreach (var dir in dirs)
                            {
                                if (!dir.Equals(archiveDir, StringComparison.OrdinalIgnoreCase))
                                {
                                    try 
                                    { 
                                        Directory.Delete(dir, true);
                                        interydata.Information($"پوشه {Path.GetFileName(dir)} حذف شد");
                                    } 
                                    catch (Exception ex) 
                                    {
                                        interydata.Warning($"خطا در حذف پوشه {Path.GetFileName(dir)}: {ex.Message}");
                                    }
                                }
                            }
                            interydata.Information("فایل‌های قدیمی Chromium حذف شدند");
                        }
                        catch (Exception ex)
                        {
                            interydata.Warning($"خطا در حذف پوشه قدیمی: {ex.Message}");
                        }
                    }
                    else
                    {
                        interydata.Information("به‌روزرسانی Chromium توسط کاربر لغو شد");
                        return;
                    }
                }
            }
            
            if (shouldDownload)
            {
                Directory.CreateDirectory(chromiumPath);
                
                using var client = new HttpClient();
                var downloadUrl = string.Format(CHROMIUM_URL, _chromiumVersion);
                Directory.CreateDirectory(archivePath);
                var zipPath = Path.Combine(archivePath, $"chromium-{_chromiumVersion}.zip");

                // بررسی وجود نسخه قبلی در آرشیو
                if (!File.Exists(zipPath))
                {
                    interydata.Information($"شروع دانلود Chromium از آدرس: {downloadUrl}");
                    await client.DownloadFileAsync(downloadUrl, zipPath, interydata);
                    interydata.Information("Chromium با موفقیت دانلود شد");
                }
                else
                {
                    interydata.Information($"استفاده از نسخه موجود Chromium از آرشیو: {_chromiumVersion}");
                }

                ZipFile.ExtractToDirectory(zipPath, chromiumPath, true);
                interydata.Information("Chromium با موفقیت استخراج شد");
            }
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در دانلود Chromium");
            MessageBox.Show($"خطا در دانلود Chromium:\n{ex.Message}\n\nلطفاً لاگ‌ها را بررسی کنید.", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    private async Task DownloadChromeDriverAsync()
    {
        try
        {
            interydata.Information("شروع دانلود ChromeDriver...");
            
            var driverPath = Path.Combine(_driverPath, "driver");
            var driverExePath = Path.Combine(driverPath, "chromedriver.exe");
            var shouldDownload = false;

            // بررسی وجود نسخه در آرشیو
            var archivePath = Path.Combine(_driverPath, "driver", "archive");
            Directory.CreateDirectory(archivePath);
            var archiveFile = Path.Combine(archivePath, $"chromedriver-{_chromeDriverVersion}.zip");
            
            using var client = new HttpClient();

            // اگر فایل اجرایی وجود دارد، نسخه آن را بررسی می‌کنیم
            if (File.Exists(driverExePath))
            {
                interydata.Information($"ChromeDriver در مسیر {driverExePath} یافت شد");
                
                // بررسی نسخه ChromeDriver
                var result = await client.GetAsync($"{CHROME_DRIVER_URL}{_chromeDriverVersion}/win64/chromedriver-win64.zip");
                if (!result.IsSuccessStatusCode)
                {
                    interydata.Information($"نسخه ChromeDriver ({_chromeDriverVersion}) نیاز به به‌روزرسانی دارد");
                    shouldDownload = true;
                }
                else
                {
                    // بررسی نسخه فایل اجرایی
                    var fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(driverExePath).FileVersion;
                    if (fileVersion != _chromeDriverVersion)
                    {
                        interydata.Information($"نسخه ChromeDriver موجود ({fileVersion}) با نسخه مورد نظر ({_chromeDriverVersion}) متفاوت است");
                        shouldDownload = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // اگر فایل در آرشیو وجود دارد، از آن استفاده می‌کنیم
            if (File.Exists(archiveFile))
            {
                interydata.Information($"نسخه {_chromeDriverVersion} ChromeDriver در آرشیو یافت شد");
                interydata.Information("استخراج ChromeDriver از آرشیو...");
                
                try
                {
                    if (Directory.Exists(driverPath))
                    {
                        var result = MessageBox.Show(
                            $"آیا مایل به حذف فایل‌های قدیمی ChromeDriver در مسیر {driverPath} هستید؟\n" +
                            "(فایل‌های موجود در پوشه archive حذف نخواهند شد)",
                            "حذف فایل‌های قدیمی",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            interydata.Information("عملیات حذف فایل‌های قدیمی توسط کاربر لغو شد");
                            return;
                        }

                        // متوقف کردن تمام پروسه‌های ChromeDriver
                        foreach (var process in System.Diagnostics.Process.GetProcessesByName("chromedriver"))
                        {
                            try 
                            { 
                                process.Kill();
                                process.WaitForExit(5000); // صبر می‌کنیم تا پروسه کاملاً بسته شود
                            } 
                            catch (Exception ex) 
                            {
                                interydata.Warning($"خطا در بستن پروسه ChromeDriver: {ex.Message}");
                            }
                        }
                        
                        // کمی صبر می‌کنیم تا مطمئن شویم همه پروسه‌ها بسته شده‌اند
                        System.Threading.Thread.Sleep(1000);

                        // حذف فایل‌های ChromeDriver به جز پوشه archive و محتویات آن
                        var files = Directory.GetFiles(driverPath, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            try 
                            { 
                                File.Delete(file);
                                interydata.Information($"فایل {Path.GetFileName(file)} حذف شد");
                            } 
                            catch (Exception ex) 
                            {
                                interydata.Warning($"خطا در حذف فایل {Path.GetFileName(file)}: {ex.Message}");
                            }
                        }

                        var dirs = Directory.GetDirectories(driverPath);
                        var archiveDir = Path.Combine(driverPath, "archive");
                        foreach (var dir in dirs)
                        {
                            if (!dir.Equals(archiveDir, StringComparison.OrdinalIgnoreCase))
                            {
                                try 
                                { 
                                    Directory.Delete(dir, true);
                                    interydata.Information($"پوشه {Path.GetFileName(dir)} حذف شد");
                                } 
                                catch (Exception ex) 
                                {
                                    interydata.Warning($"خطا در حذف پوشه {Path.GetFileName(dir)}: {ex.Message}");
                                }
                            }
                        }
                    }
                    Directory.CreateDirectory(driverPath);
                    
                    // استخراج به یک پوشه موقت
                    var extractPath = Path.Combine(driverPath, "temp");
                    Directory.CreateDirectory(extractPath);
                    ZipFile.ExtractToDirectory(archiveFile, extractPath, true);

                    // انتقال فایل‌ها به پوشه اصلی
                    var extractedFiles = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in extractedFiles)
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(driverPath, fileName);
                        if (File.Exists(destFile))
                        {
                            File.Delete(destFile);
                        }
                        File.Move(file, destFile);
                    }

                    // حذف پوشه موقت
                    Directory.Delete(extractPath, true);
                    interydata.Information("ChromeDriver با موفقیت از آرشیو استخراج شد");
                    return;
                }
                catch (Exception ex)
                {
                    interydata.Error(ex, "خطا در استخراج ChromeDriver از آرشیو");
                    // حتی در صورت خطا در استخراج، فایل آرشیو را حفظ می‌کنیم
                }
            }

            // اگر در آرشیو نبود، نیاز به دانلود داریم
            interydata.Information($"نسخه {_chromeDriverVersion} ChromeDriver در آرشیو یافت نشد");
            shouldDownload = true;

            if (shouldDownload)
            {
                var result = MessageBox.Show(
                    "ChromeDriver موجود نیاز به به‌روزرسانی دارد.\n" +
                    "آیا می‌خواهید نسخه جدید دانلود شود؟",
                    "به‌روزرسانی ChromeDriver",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                           // متوقف کردن تمام پروسه‌های ChromeDriver
                           foreach (var process in System.Diagnostics.Process.GetProcessesByName("chromedriver"))
                           {
                               try 
                               { 
                                   process.Kill();
                                   process.WaitForExit(5000); // صبر می‌کنیم تا پروسه کاملاً بسته شود
                               } 
                               catch (Exception ex) 
                               {
                                   interydata.Warning($"خطا در بستن پروسه ChromeDriver: {ex.Message}");
                               }
                           }
                           
                           // کمی صبر می‌کنیم تا مطمئن شویم همه پروسه‌ها بسته شده‌اند
                           System.Threading.Thread.Sleep(1000);
                        
                        // حذف فایل‌های ChromeDriver به جز پوشه archive و محتویات آن
                        var files = Directory.GetFiles(driverPath, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            try { File.Delete(file); } catch { }
                        }
                        var dirs = Directory.GetDirectories(driverPath);
                        var archiveDir = Path.Combine(driverPath, "archive");
                        foreach (var dir in dirs)
                        {
                            if (!dir.Equals(archiveDir, StringComparison.OrdinalIgnoreCase))
                            {
                                try { Directory.Delete(dir, true); } catch { }
                            }
                        }
                        interydata.Information("فایل‌های قدیمی ChromeDriver حذف شدند");
                    }
                    catch (Exception ex)
                    {
                        interydata.Warning($"خطا در حذف پوشه قدیمی: {ex.Message}");
                    }
                }
                else
                {
                    interydata.Information("به‌روزرسانی ChromeDriver توسط کاربر لغو شد");
                    return;
                }
                
                Directory.CreateDirectory(driverPath);
            }
            else
            {
                return;
            }
            
            var version = GetLatestChromeDriverVersion();
            var downloadUrl = $"{CHROME_DRIVER_URL}{version}/win64/chromedriver-win64.zip";

            Directory.CreateDirectory(archivePath);
            var zipPath = Path.Combine(archivePath, $"chromedriver-{_chromeDriverVersion}.zip");

            // بررسی وجود نسخه قبلی در آرشیو
            if (!File.Exists(zipPath))
            {
                interydata.Information($"شروع دانلود ChromeDriver از آدرس: {downloadUrl}");
                await client.DownloadFileAsync(downloadUrl, zipPath, interydata);
                interydata.Information("ChromeDriver با موفقیت دانلود شد");
            }
            else
            {
                interydata.Information($"استفاده از نسخه موجود ChromeDriver از آرشیو: {_chromeDriverVersion}");
            }

            // استخراج به یک پوشه موقت
            var tempPath = Path.Combine(driverPath, "temp");
            Directory.CreateDirectory(tempPath);
            ZipFile.ExtractToDirectory(zipPath, tempPath, true);

            // انتقال فایل‌های chromedriver به پوشه اصلی
            var chromedriverFiles = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in chromedriverFiles)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(driverPath, fileName);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }
                File.Move(file, destFile);
            }

            // حذف پوشه موقت
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch
            {
                // نادیده گرفتن خطای حذف پوشه موقت
            }
            interydata.Information("ChromeDriver با موفقیت استخراج شد");

            // فایل زیپ را در آرشیو نگه می‌داریم برای استفاده‌های بعدی
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در دانلود ChromeDriver");
            MessageBox.Show($"خطا در دانلود ChromeDriver:\n{ex.Message}\n\nلطفاً لاگ‌ها را بررسی کنید.", "خطا",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

    private string GetLatestChromeDriverVersion()
    {
        try
        {
            // استفاده از نسخه 120 که با Chromium ما سازگار است
            interydata.Information("در حال دانلود ChromeDriver نسخه 120...");
            return "120.0.6099.109";
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در دریافت نسخه ChromeDriver");
            throw;
        }
    }

    private string GetChromeVersion()
    {
        try
        {
            var chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (File.Exists(chromePath))
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromePath);
                return versionInfo.FileVersion ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در خواندن نسخه Chrome");
        }
        return string.Empty;
    }

    public async Task<ChromeDriver> CreateDriverAsync()
    {
        try
        {
            interydata.Information("تنظیم ChromeDriver...");

            var chromiumPath = Path.Combine(_driverPath, "chromium", "chrome-win", "chrome.exe");
            var driverPath = Path.Combine(_driverPath, "driver");
            if (!File.Exists(chromiumPath))
                chromiumPath = Path.Combine(_driverPath, "chromium", "chrome-win64", "chrome.exe");

            if (!File.Exists(chromiumPath))
            {
                throw new InvalidOperationException($"Chromium در مسیر {chromiumPath} یافت نشد. لطفاً برنامه را مجدداً راه‌اندازی کنید.");
            }

            var options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            options.SetLoggingPreference(LogType.Performance, LogLevel.All);
            // تنظیم نسخه DevTools به 120
            options.AddAdditionalOption("devtools.version", 120);
            options.AddArgument("--enable-logging");
            options.AddArgument("--v=1");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.BinaryLocation = chromiumPath;
            // تنظیم آرگومان‌های Chrome برای سازگاری با نسخه 112
            options.AddArgument("--remote-debugging-port=0");
            options.AddArgument("--remote-allow-origins=*");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-first-run");
            options.AddArgument("--no-default-browser-check");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-prompt-on-repost");
            options.AddArgument("--disable-hang-monitor");
            options.AddArgument("--disable-client-side-phishing-detection");
            options.AddArgument("--disable-sync");
            options.AddArgument("--disable-default-apps");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-save-password-bubble");
            options.AddArgument("--disable-single-click-autofill");
            options.AddArgument("--disable-autofill-keyboard-accessory-view");
            options.AddArgument("--disable-component-extensions-with-background-pages");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-background-networking");
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-renderer-backgrounding");
            options.AddArgument("--disable-features=TranslateUI,BlinkGenPropertyTrees");
            options.AddArgument("--metrics-recording-only");
            options.AddArgument("--mute-audio");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-site-isolation-trials");
            options.AddArgument("--force-color-profile=srgb");
            options.AddArgument("--force-device-scale-factor=1");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--ignore-certificate-errors-spki-list");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=IsolateOrigins,site-per-process");
            options.AddArgument("--disable-setuid-sandbox");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--disable-webgl");
            options.AddArgument("--disable-threaded-animation");
            options.AddArgument("--disable-threaded-scrolling");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-xss-auditor");
            options.AddArgument("--no-experiments");
            options.AddArgument("--no-pings");
            options.AddArgument("--no-proxy-server");
            options.AddArgument("--no-service-autorun");
            options.AddArgument("--password-store=basic");
            options.AddArgument("--use-mock-keychain");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            // options.AddArgument("--headless=new"); // فعلاً غیرفعال می‌کنیم تا مشکل DevTools حل شود
            options.AddArgument("--force-device-scale-factor=1");
            options.AddArgument("--high-dpi-support=1");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-setuid-sandbox");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-features=IsolateOrigins,site-per-process,TranslateUI");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-blink-features");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-component-extensions-with-background-pages");
            options.AddArgument("--disable-default-apps");
            options.AddArgument("--disable-sync");
            options.AddArgument("--disable-background-networking");
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-renderer-backgrounding");
            options.AddArgument("--disable-metrics");
            options.AddArgument("--disable-metrics-reporting");
            options.AddArgument("--disable-client-side-phishing-detection");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-prompt-on-repost");
            options.AddArgument("--disable-hang-monitor");
            options.AddArgument("--disable-domain-reliability");
            options.AddArgument("--disable-download-protection");
            options.AddArgument("--disable-client-side-phishing-detection");
            options.AddArgument("--disable-component-update");
            options.AddArgument("--disable-field-trial-config");
            options.AddArgument("--disable-background-downloads");
            options.AddArgument("--disable-background-mode");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-component-update");
            options.AddArgument("--disable-default-apps");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-domain-reliability");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-features=TranslateUI");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-print-preview");
            options.AddArgument("--disable-prompt-on-repost");
            options.AddArgument("--disable-sync");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-webgl");
            options.AddArgument("--enable-automation");
            options.AddArgument("--enable-logging");
            options.AddArgument("--force-color-profile=srgb");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--log-level=0");
            options.AddArgument("--metrics-recording-only");
            options.AddArgument("--mute-audio");
            options.AddArgument("--no-default-browser-check");
            options.AddArgument("--no-experiments");
            options.AddArgument("--no-first-run");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--no-service-autorun");
            options.AddArgument("--password-store=basic");
            options.AddArgument("--test-type");
            options.AddArgument("--use-mock-keychain");
            options.AddArgument("--window-size=1920,1080");
            // تنظیم پورت DevTools به صورت خودکار
            options.AddArgument("--remote-debugging-port=0");
            options.AddArgument("--remote-allow-origins=*");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 2);
            options.AddUserProfilePreference("profile.managed_default_content_settings.images", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.javascript", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 1);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.cookie_controls_mode", 0);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("safebrowsing.enabled", true);
            options.AddUserProfilePreference("safebrowsing.disable_download_protection", true);
            options.AddUserProfilePreference("default_content_settings.popups", 0);
            options.AddUserProfilePreference("managed_default_content_settings.images", 1);
            options.AddUserProfilePreference("translate.enabled", false);

            var service = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService(driverPath);
            service.HideCommandPromptWindow = true;
            service.EnableVerboseLogging = true;
            service.EnableAppendLog = true;
            service.LogPath = Path.Combine(_driverPath, "chromedriver.log");
            service.PortServerAddress = "127.0.0.1";
            service.WhitelistedIPAddresses = "127.0.0.1";

            interydata.Information($"راه‌اندازی ChromeDriver با Chromium نسخه {_chromiumVersion}...");
            
            // تنظیم زمان انتظار برای راه‌اندازی
            var timeout = TimeSpan.FromMinutes(3);
            _driver = new ChromeDriver(service, options, timeout);

            interydata.Information("تنظیم DevTools...");
            await Task.Delay(2000); // کمی صبر می‌کنیم تا مرورگر کاملاً آماده شود
            
            try
            {
                var devTools = _driver.GetDevToolsSession();
                var version = devTools.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V120.DevToolsSessionDomains>();
                interydata.Information($"نسخه DevTools: {version}");

                var network = version.Network;
                await network.Enable(new OpenQA.Selenium.DevTools.V120.Network.EnableCommandSettings());

                interydata.Information("DevTools با موفقیت تنظیم شد");
            }
            catch (Exception ex)
            {
                interydata.Warning($"خطا در تنظیم DevTools: {ex.Message}");
                // ادامه می‌دهیم حتی اگر DevTools تنظیم نشد
            }

            return _driver;
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در راه‌اندازی ChromeDriver");
            throw;
        }
    }

    public async Task<bool> FillFormAsync()
    {
        try
        {
            if (_driver == null)
            {
                interydata.Error(new InvalidOperationException("ChromeDriver راه‌اندازی نشده است"), "ChromeDriver راه‌اندازی نشده است");
                return false;
            }

            interydata.Information("شروع فرآیند پر کردن فرم...");

            // خواندن تنظیمات وب‌سایت
            var websiteSettings = File.Exists("website_settings.json") 
                ? JsonConvert.DeserializeObject<WebsiteSettings>(File.ReadAllText("website_settings.json")) 
                : null;

            if (websiteSettings == null)
            {
                interydata.Error(new FileNotFoundException("تنظیمات وب‌سایت یافت نشد"), "تنظیمات وب‌سایت یافت نشد");
                return false;
            }

            interydata.Information($"رفتن به آدرس: {websiteSettings.WebsiteUrl}");
            _driver.Navigate().GoToUrl(websiteSettings.WebsiteUrl);

            // صبر برای بارگذاری صفحه
            await Task.Delay(3000);

            // پر کردن فیلدها به ترتیب
            interydata.Information("پر کردن فیلدهای ورودی به ترتیب...");
            
            foreach (var field in websiteSettings.Fields)
            {
                if (string.IsNullOrEmpty(field.Name) || string.IsNullOrEmpty(field.Value))
                    continue;

                try
                {
                    interydata.Information($"پر کردن فیلد: {field.Name}");
                    
                    // تلاش برای پیدا کردن فیلد با روش‌های مختلف
                    IWebElement fieldElement = null;
                    
                    try
                    {
                        fieldElement = _driver.FindElement(By.Name(field.Name));
                    }
                    catch
                    {
                        try
                        {
                            fieldElement = _driver.FindElement(By.Id(field.Name));
                        }
                        catch
                        {
                            try
                            {
                                fieldElement = _driver.FindElement(By.XPath($"//input[@placeholder='{field.Name}'] | //input[@name='{field.Name}'] | //input[@id='{field.Name}']"));
                            }
                            catch
                            {
                                interydata.Warning($"فیلد {field.Name} یافت نشد");
                                continue;
                            }
                        }
                    }

                    if (fieldElement != null)
                    {
                        fieldElement.Clear();
                        await Task.Delay(500); // کمی صبر بین ورودی‌ها
                        fieldElement.SendKeys(field.Value);
                        interydata.Information($"فیلد {field.Name} با موفقیت پر شد");
                    }
                }
                catch (Exception ex)
                {
                    interydata.Warning($"خطا در پر کردن فیلد {field.Name}: {ex.Message}");
                }
            }

            // پیدا کردن دکمه ارسال فرم و کلیک روی آن
            try
            {
                var submitButton = _driver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit'] | //button[contains(text(), 'ارسال')] | //button[contains(text(), 'Submit')] | //button[contains(text(), 'ورود')] | //button[contains(text(), 'Login')]"));
                interydata.Information("کلیک روی دکمه ارسال فرم...");
                submitButton.Click();

                // صبر برای پردازش فرم
                await Task.Delay(3000);

                interydata.Information("فرم با موفقیت پر و ارسال شد");
                return true;
            }
            catch (Exception ex)
            {
                interydata.Warning($"دکمه ارسال فرم یافت نشد یا خطا در کلیک: {ex.Message}");
                interydata.Information("فرم پر شد اما دکمه ارسال یافت نشد");
                return true; // فرم پر شده است، حتی اگر دکمه ارسال یافت نشود
            }
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در فرآیند لاگین");
            return false;
        }
    }

    public void StopDriver()
    {
        try
        {
            _driver?.Quit();
            _driver?.Dispose();
            _driver = null;
            interydata.Information("ChromeDriver متوقف شد");
        }
        catch (Exception ex)
        {
            interydata.Error(ex, "خطا در توقف ChromeDriver");
            throw;
        }
    }
}

public static class HttpClientExtensions
{
    public static async Task DownloadFileAsync(this HttpClient client, string url, string filename, ILogger logger)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = File.Create(filename);

        var buffer = new byte[8192];
        var totalBytesRead = 0L;
        var lastProgressReport = 0;

        while (true)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead == 0)
                break;

            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalBytesRead += bytesRead;

            if (totalBytes != -1)
            {
                var progressPercentage = (int)((totalBytesRead * 100) / totalBytes);
                if (progressPercentage > lastProgressReport)
                {
                    var downloadedMB = totalBytesRead / 1024.0 / 1024.0;
                    var totalMB = totalBytes / 1024.0 / 1024.0;
                    logger.Information($"دانلود شده: {downloadedMB:F1} MB از {totalMB:F1} MB ({progressPercentage}%)");
                    lastProgressReport = progressPercentage;
                }
            }
        }
    }
}
