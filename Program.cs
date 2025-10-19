namespace Ntk.Chrome;

static class Program
{
    [STAThread]
    static async Task Main()
    {
        //ApplicationConfiguration.Initialize();
        //Application.Run(new Forms.MainForm());
        // Run TestDecrypt instead of the main form
        TestDecrypt.RunSimpleTest();

        // Keep console open to see results
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
