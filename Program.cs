namespace Ntk.Chrome;

static class Program
{
    [STAThread]
    static async Task Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.MainForm());

    }
}
