namespace Ntk.Chrome.Models;

public class AppSettings
{
    public string ChromeDriverPath { get; set; } = string.Empty;
    public string ProgramPath { get; set; } = string.Empty;
    public string LogPath { get; set; } = "%TEMP%\\NtkChromeDriver\\logs"; // مسیر پیش‌فرض برای ذخیره لاگ‌ها
    public bool EnableProgramStatus { get; set; }
    public string ChromiumVersion { get; set; } = "120.0.6099.109"; // شماره build نسخه 120.0.6099.109 Chromium
    public string ChromeDriverVersion { get; set; } = "120.0.6099.109"; // نسخه ChromeDriver سازگار با Chromium 120
}

public class WebsiteSettings
{
    public string WebsiteUrl { get; set; } = string.Empty;
    public List<WebsiteField> Fields { get; set; } = new();
}

public class WebsiteField
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class RequestInfo
{
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string RequestHeaders { get; set; } = string.Empty;
    public string RequestBody { get; set; } = string.Empty;
    public string ResponseHeaders { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
