using OpenQA.Selenium.Chrome;

namespace Ntk.Chrome.Services;

public interface IChromeDriverService
{
    Task InitializeAsync();
    Task<ChromeDriver> CreateDriverAsync();
    Task<bool> FillFormAsync();
    void StopDriver();
}
