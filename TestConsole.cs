using System;

namespace Ntk.Chrome
{
    public class TestConsole
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== شروع تست رمزگشایی ===");
            
            try
            {
                string result = ""; TestDecrypt_AES.RunTest();
                Console.WriteLine($"نتیجه نهایی: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
