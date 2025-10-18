using Ntk.Chrome.Models;
using Newtonsoft.Json;

namespace Ntk.Chrome.Services;

public class VersionManager
{
    private readonly string _driverPath;
    private readonly ILogger _logger;
    private readonly string _chromiumVersionPath;
    private readonly string _chromeDriverVersionPath;

    public VersionManager(string driverPath, ILogger logger)
    {
        _driverPath = driverPath;
        _logger = logger;
        _chromiumVersionPath = Path.Combine(_driverPath, "chromium", "version.txt");
        _chromeDriverVersionPath = Path.Combine(_driverPath, "driver", "version.txt");
    }

    /// <summary>
    /// Reads the current version from version files
    /// </summary>
    public async Task<VersionInfo> ReadCurrentVersionsAsync()
    {
        try
        {
            var chromiumVersion = await ReadVersionFromFileAsync(_chromiumVersionPath);
            var chromeDriverVersion = await ReadVersionFromFileAsync(_chromeDriverVersionPath);

            _logger.Information($"نسخه‌های فعلی - Chromium: {chromiumVersion ?? "نامشخص"}, ChromeDriver: {chromeDriverVersion ?? "نامشخص"}");

            return new VersionInfo
            {
                ChromiumVersion = chromiumVersion,
                ChromeDriverVersion = chromeDriverVersion
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در خواندن نسخه‌ها از فایل‌های ورژن");
            return new VersionInfo();
        }
    }

    /// <summary>
    /// Automatically detects and writes version information to files
    /// </summary>
    public async Task WriteVersionsAsync(string chromiumVersion, string chromeDriverVersion)
    {
        try
        {
            await WriteVersionToFileAsync(_chromiumVersionPath, chromiumVersion);
            await WriteVersionToFileAsync(_chromeDriverVersionPath, chromeDriverVersion);

            _logger.Information($"نسخه‌ها با موفقیت ذخیره شدند - Chromium: {chromiumVersion}, ChromeDriver: {chromeDriverVersion}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در نوشتن نسخه‌ها به فایل‌های ورژن");
            throw;
        }
    }

    /// <summary>
    /// Automatically detects version from executable files and updates version files
    /// </summary>
    public async Task AutoDetectAndWriteVersionsAsync()
    {
        try
        {
            // Detect Chromium version
            var chromiumVersion = await DetectChromiumVersionAsync();
            
            // Detect ChromeDriver version
            var chromeDriverVersion = await DetectChromeDriverVersionAsync();

            if (!string.IsNullOrEmpty(chromiumVersion) && !string.IsNullOrEmpty(chromeDriverVersion))
            {
                await WriteVersionsAsync(chromiumVersion, chromeDriverVersion);
                _logger.Information("نسخه‌ها به صورت خودکار تشخیص و ذخیره شدند");
            }
            else
            {
                _logger.Warning("نمی‌توان نسخه‌ها را به صورت خودکار تشخیص داد");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در تشخیص خودکار نسخه‌ها");
            throw;
        }
    }

    /// <summary>
    /// Checks if versions match the expected versions from settings
    /// </summary>
    public async Task<bool> AreVersionsUpToDateAsync(AppSettings settings)
    {
        try
        {
            var currentVersions = await ReadCurrentVersionsAsync();
            
            var chromiumMatch = string.IsNullOrEmpty(currentVersions.ChromiumVersion) || 
                               currentVersions.ChromiumVersion == settings.ChromiumVersion;
            
            var driverMatch = string.IsNullOrEmpty(currentVersions.ChromeDriverVersion) || 
                             currentVersions.ChromeDriverVersion == settings.ChromeDriverVersion;

            var isUpToDate = chromiumMatch && driverMatch;
            
            if (!isUpToDate)
            {
                _logger.Information($"نسخه‌ها نیاز به به‌روزرسانی دارند - Chromium: {currentVersions.ChromiumVersion} -> {settings.ChromiumVersion}, ChromeDriver: {currentVersions.ChromeDriverVersion} -> {settings.ChromeDriverVersion}");
            }
            else
            {
                _logger.Information("نسخه‌ها به‌روز هستند");
            }

            return isUpToDate;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در بررسی وضعیت نسخه‌ها");
            return false;
        }
    }

    private async Task<string> ReadVersionFromFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            var version = await File.ReadAllTextAsync(filePath);
            return version.Trim();
        }
        catch (Exception ex)
        {
            _logger.Warning($"خطا در خواندن فایل ورژن {filePath}: {ex.Message}");
            return string.Empty;
        }
    }

    private async Task WriteVersionToFileAsync(string filePath, string version)
    {
        try
        {
            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, version);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"خطا در نوشتن فایل ورژن {filePath}");
            throw;
        }
    }

    private Task<string> DetectChromiumVersionAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                var chromiumExePath = Path.Combine(_driverPath, "chromium", "chrome-win64", "chrome.exe");
                if (!File.Exists(chromiumExePath))
                {
                    chromiumExePath = Path.Combine(_driverPath, "chromium", "chrome-win", "chrome.exe");
                }

                if (File.Exists(chromiumExePath))
                {
                    var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromiumExePath);
                    var version = versionInfo.FileVersion;
                    _logger.Information($"نسخه Chromium تشخیص داده شد: {version}");
                    return version ?? string.Empty;
                }

                _logger.Warning("فایل اجرایی Chromium یافت نشد");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Warning($"خطا در تشخیص نسخه Chromium: {ex.Message}");
                return string.Empty;
            }
        });
    }

    private Task<string> DetectChromeDriverVersionAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                var chromeDriverExePath = Path.Combine(_driverPath, "driver", "chromedriver.exe");

                if (File.Exists(chromeDriverExePath))
                {
                    var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(chromeDriverExePath);
                    var version = versionInfo.FileVersion;
                    _logger.Information($"نسخه ChromeDriver تشخیص داده شد: {version}");
                    return version ?? string.Empty;
                }

                _logger.Warning("فایل اجرایی ChromeDriver یافت نشد");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Warning($"خطا در تشخیص نسخه ChromeDriver: {ex.Message}");
                return string.Empty;
            }
        });
    }

    /// <summary>
    /// Updates version files after successful download/extraction
    /// </summary>
    public async Task UpdateVersionsAfterDownloadAsync(string chromiumVersion, string chromeDriverVersion)
    {
        try
        {
            // Wait a bit to ensure files are fully extracted
            await Task.Delay(2000);

            // Re-detect versions from extracted files
            await AutoDetectAndWriteVersionsAsync();
            
            _logger.Information($"نسخه‌ها پس از دانلود به‌روزرسانی شدند - Chromium: {chromiumVersion}, ChromeDriver: {chromeDriverVersion}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "خطا در به‌روزرسانی نسخه‌ها پس از دانلود");
        }
    }
}

public class VersionInfo
{
    public string ChromiumVersion { get; set; } = string.Empty;
    public string ChromeDriverVersion { get; set; } = string.Empty;
}
