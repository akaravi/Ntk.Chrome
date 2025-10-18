using Serilog;

namespace Ntk.Chrome.Services;

public class FileLogger : ILogger
{
    private readonly Serilog.ILogger _logger;

    public FileLogger(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public void Information(string message)
    {
        _logger.Information(message);
    }

    public void Warning(string message)
    {
        _logger.Warning(message);
    }

    public void Error(Exception ex, string message)
    {
        _logger.Error(ex, message);
    }

    public void Debug(string message)
    {
        _logger.Debug(message);
    }
}
