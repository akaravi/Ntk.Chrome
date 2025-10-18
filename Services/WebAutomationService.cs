using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Ntk.Chrome.Models;

namespace Ntk.Chrome.Services;

public class WebAutomationService
{
    private readonly ChromeDriver _driver;
    private readonly ILogger _logger;
    private readonly WebsiteSettings _settings;
    private readonly WebDriverWait _wait;

    public WebAutomationService(ChromeDriver driver, ILogger logger, WebsiteSettings settings)
    {
        _driver = driver;
        _logger = logger;
        _settings = settings;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
    }

    public async Task NavigateToWebsiteAsync()
    {
        try
        {
            _logger.Information($"Navigating to {_settings.WebsiteUrl}");
            await Task.Run(() => _driver.Navigate().GoToUrl(_settings.WebsiteUrl));
            _logger.Information("Navigation completed");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error navigating to website");
            throw;
        }
    }

    public async Task FillFormAsync()
    {
        try
        {
            _logger.Information("Starting form fill process");
            
            foreach (var field in _settings.Fields)
            {
                await FillFieldAsync(field);
            }

            _logger.Information("Form fill completed");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error filling form");
            throw;
        }
    }

    private async Task FillFieldAsync(WebsiteField field)
    {
        try
        {
            _logger.Information($"Filling field: {field.Name}");

            // Try different strategies to find the field
            var element = await FindElementAsync(field.Name);
            
            if (element != null)
            {
                await Task.Run(() =>
                {
                    element.Clear();
                    element.SendKeys(field.Value);
                });
                _logger.Information($"Field {field.Name} filled successfully");
            }
            else
            {
                _logger.Warning($"Field {field.Name} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error filling field {field.Name}");
            throw;
        }
    }

    private async Task<IWebElement?> FindElementAsync(string fieldName)
    {
        return await Task.Run(() =>
        {
            try
            {
                // Try by ID
                if (_driver.FindElements(By.Id(fieldName)).Count > 0)
                    return _driver.FindElement(By.Id(fieldName));

                // Try by Name
                if (_driver.FindElements(By.Name(fieldName)).Count > 0)
                    return _driver.FindElement(By.Name(fieldName));

                // Try by CSS Selector
                if (_driver.FindElements(By.CssSelector(fieldName)).Count > 0)
                    return _driver.FindElement(By.CssSelector(fieldName));

                // Try by XPath
                if (_driver.FindElements(By.XPath($"//*[@id='{fieldName}' or @name='{fieldName}' or contains(@class, '{fieldName}')]")).Count > 0)
                    return _driver.FindElement(By.XPath($"//*[@id='{fieldName}' or @name='{fieldName}' or contains(@class, '{fieldName}')]"));

                return null;
            }
            catch
            {
                return null;
            }
        });
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
