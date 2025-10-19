using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Ntk.Chrome
{
    /// <summary>
    /// Test Decryption Class
    /// Tests the decryption functionality based on JavaScript patterns
    /// </summary>
    public class TestDecrypt
    {
        private readonly Serilog.ILogger _logger;
        private readonly string _privateKey;
        private readonly string _publicKey;

        public TestDecrypt()
        {
            // Initialize logger
            _logger = new LoggerConfiguration()
                .WriteTo.File("test-decrypt.log")
                .CreateLogger();

            // Private key for RSA decryption
            _privateKey = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";
            
            // Public key for RSA encryption/verification
            _publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
        }


        /// <summary>
        /// Run the decryption test
        /// </summary>
        public async Task RunDecryptionTest()
        {
            try
            {
                _logger.Information("=== شروع تست رمزگشایی ===");
                Console.WriteLine("=== شروع تست رمزگشایی ===");
                
                // Test data from the provided JSON
                var testData = "U2FsdGVkX195i9WtFcoUJEd4Ja+SJMn+Ff9yyAsvc/6unZbpslHisJ7pT67p9eu5wwzSryy55NKCdsSXIh6jFr8PdUmjPit5OEvatXzu+O/qO6NO53jUq6tMjX4CvW0Hw+SPhLeK3qLOvKSscYH4TEuBXnwuwp3C62NGh2CAnH+fh+CkZyF8QeoiE3LqF8ultYg8VPA2jT2fFL1z4ZM8wKOEAGsAZU4UtppM7k9CZsU=";
                var testKey = "gIAC+naVMUUTrRROzqPs/gTjBEGf6A3G1QB1OJl7pNM7joq8USLj9ljTuFvM8sqDEab0CS998+yjBcKGMjxY2RM7nDoBRrLwz0FmTC/JZ4MNCzI4AdTYqyNn5Jx4DrQXiSSeVCJKzsMZzMTE/N1VIGO+xiguu8U6bhGNlzsnmGF6N1eSVtKxlRQFyz/gAxSPNRIP0NTlchxXV1CdcB4PNZ9n/R6azU0eB6p7H++ZY4JMqjZwwJFssfDbtSnwlw3QO70WlBew3TQXpDRzDM63kIn6/2prObA6V5d0kd+RbaL4hz6HhxDLstMVZTPNJUmc6mhMyzM+7fNqLoLon8yhSg==";
                _logger.Information($"داده‌های تست:");
                Console.WriteLine($"داده‌های تست:");

                // Test 1: Decrypt the RSA key first
                _logger.Information("\n=== تست 1: رمزگشایی کلید RSA ===");
                Console.WriteLine("\n=== تست 1: رمزگشایی کلید RSA ===");
                var aesKey = await DecryptRSAKey(testKey);
                _logger.Information($"کلید AES رمزگشایی شد. طول: {aesKey.Length} بایت");
                Console.WriteLine($"کلید AES رمزگشایی شد. طول: {aesKey.Length} بایت");
                _logger.Information($"کلید AES (Base64): {Convert.ToBase64String(aesKey)}");
                Console.WriteLine($"کلید AES (Base64): {Convert.ToBase64String(aesKey)}");

                // Test 2: Decrypt the AES data
                _logger.Information("\n=== تست 2: رمزگشایی داده‌های AES ===");
                Console.WriteLine("\n=== تست 2: رمزگشایی داده‌های AES ===");
                var decryptedData = await DecryptAESData(testData, aesKey);
                _logger.Information($"داده‌های رمزگشایی شده: {decryptedData}");
                Console.WriteLine($"داده‌های رمزگشایی شده: {decryptedData}");
                
                // Test 2.1: Try to decrypt the AES key itself
                _logger.Information("\n=== تست 2.1: رمزگشایی کلید AES ===");
                Console.WriteLine("\n=== تست 2.1: رمزگشایی کلید AES ===");
                try
                {
                    var aesKeyString = System.Text.Encoding.UTF8.GetString(aesKey);
                    _logger.Information($"کلید AES به صورت رشته: {aesKeyString}");
                    Console.WriteLine($"کلید AES به صورت رشته: {aesKeyString}");
                    
                    // Try to decode the AES key as Base64
                    try
                    {
                        var decodedAESKey = Convert.FromBase64String(aesKeyString);
                        _logger.Information($"کلید AES رمزگشایی شده از Base64. طول: {decodedAESKey.Length} بایت");
                        Console.WriteLine($"کلید AES رمزگشایی شده از Base64. طول: {decodedAESKey.Length} بایت");
                        
                        // Try to use this decoded key for AES decryption
                        var decryptedWithDecodedKey = await DecryptAESData(testData, decodedAESKey);
                        _logger.Information($"داده‌های رمزگشایی شده با کلید رمزگشایی شده: {decryptedWithDecodedKey}");
                        Console.WriteLine($"داده‌های رمزگشایی شده با کلید رمزگشایی شده: {decryptedWithDecodedKey}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Information($"کلید AES نمی‌تواند به عنوان Base64 رمزگشایی شود: {ex.Message}");
                        Console.WriteLine($"کلید AES نمی‌تواند به عنوان Base64 رمزگشایی شود: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "خطا در رمزگشایی کلید AES");
                    Console.WriteLine($"خطا در رمزگشایی کلید AES: {ex.Message}");
                }

                // Test 3: Try direct RSA decryption (fallback)
                _logger.Information("\n=== تست 3: رمزگشایی مستقیم RSA (fallback) ===");
                Console.WriteLine("\n=== تست 3: رمزگشایی مستقیم RSA (fallback) ===");
                var directDecrypt = await DecryptDirectRSA(testData);
                _logger.Information($"رمزگشایی مستقیم RSA: {directDecrypt}");
                Console.WriteLine($"رمزگشایی مستقیم RSA: {directDecrypt}");

                // Test 4: Try to decrypt the RSA key itself
                _logger.Information("\n=== تست 4: رمزگشایی کلید RSA ===");
                Console.WriteLine("\n=== تست 4: رمزگشایی کلید RSA ===");
                try
                {
                    // Try to decrypt the RSA key with itself
                    var rsaKeyBytes = Convert.FromBase64String(_privateKey);
                    _logger.Information($"کلید RSA خصوصی. طول: {rsaKeyBytes.Length} بایت");
                    Console.WriteLine($"کلید RSA خصوصی. طول: {rsaKeyBytes.Length} بایت");
                    
                    // Try to decrypt the RSA key with the public key
                    var publicKeyBytes = Convert.FromBase64String(_publicKey);
                    _logger.Information($"کلید RSA عمومی. طول: {publicKeyBytes.Length} بایت");
                    Console.WriteLine($"کلید RSA عمومی. طول: {publicKeyBytes.Length} بایت");
                    
                    // Try to use the RSA key to decrypt itself
                    using (var rsa = RSA.Create(2048))
                    {
                        rsa.ImportPkcs8PrivateKey(rsaKeyBytes, out _);
                        
                        // Try to decrypt the RSA key with itself
                        try
                        {
                            var decryptedRSAKey = rsa.Decrypt(rsaKeyBytes, RSAEncryptionPadding.OaepSHA256);
                            _logger.Information($"کلید RSA با خودش رمزگشایی شد. طول: {decryptedRSAKey.Length} بایت");
                            Console.WriteLine($"کلید RSA با خودش رمزگشایی شد. طول: {decryptedRSAKey.Length} بایت");
                        }
                        catch (Exception ex)
                        {
                            _logger.Information($"کلید RSA نمی‌تواند با خودش رمزگشایی شود: {ex.Message}");
                            Console.WriteLine($"کلید RSA نمی‌تواند با خودش رمزگشایی شود: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "خطا در رمزگشایی کلید RSA");
                    Console.WriteLine($"خطا در رمزگشایی کلید RSA: {ex.Message}");
                }

                // Test 5: Verify key pair compatibility
                _logger.Information("\n=== تست 5: بررسی سازگاری کلیدها ===");
                Console.WriteLine("\n=== تست 5: بررسی سازگاری کلیدها ===");
                await TestKeyPairCompatibility();

                _logger.Information("\n=== تست رمزگشایی تکمیل شد ===");
                Console.WriteLine("\n=== تست رمزگشایی تکمیل شد ===");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست رمزگشایی");
            }
        }

        /// <summary>
        /// Decrypt RSA key to get AES key
        /// </summary>
        /// <param name="encryptedKey">Encrypted RSA key</param>
        /// <returns>AES key</returns>
        private async Task<byte[]> DecryptRSAKey(string encryptedKey)
        {
            try
            {
                _logger.Information("شروع رمزگشایی کلید RSA...");
                
                using (var rsa = RSA.Create(2048))
                {
                    var privateKeyBytes = Convert.FromBase64String(_privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedKey);
                    _logger.Information($"اندازه کلید رمز شده: {encryptedBytes.Length} بایت");
                    
                    // Try different padding methods
                    byte[] decryptedBytes = null;
                    
                    try
                    {
                        // Use OAEP SHA-256 as specified in the algorithm parameters
                        decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                        _logger.Information("رمزگشایی با OAEP SHA-256 موفقیت‌آمیز بود");
                    }
                    catch (Exception ex1)
                    {
                        _logger.Warning($"OAEP SHA-256 failed: {ex1.Message}");
                        
                        try
                        {
                            // Try OAEP SHA-1 as fallback
                            decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
                            _logger.Information("رمزگشایی با OAEP SHA-1 موفقیت‌آمیز بود");
                        }
                        catch (Exception ex2)
                        {
                            _logger.Warning($"OAEP SHA-1 failed: {ex2.Message}");
                            
                            try
                            {
                                // Try PKCS1 as last resort
                                decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                                _logger.Information("رمزگشایی با PKCS1 موفقیت‌آمیز بود");
                            }
                            catch (Exception ex3)
                            {
                                _logger.Error($"All RSA decryption methods failed: {ex3.Message}");
                                throw;
                            }
                        }
                    }
                    
                    _logger.Information($"کلید AES رمزگشایی شد. طول: {decryptedBytes.Length} بایت");
                    return decryptedBytes;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی کلید RSA");
                throw;
            }
        }

        /// <summary>
        /// Decrypt AES data using CryptoJS format
        /// </summary>
        /// <param name="encryptedData">Encrypted AES data</param>
        /// <param name="aesKey">AES key</param>
        /// <returns>Decrypted data</returns>
        private async Task<string> DecryptAESData(string encryptedData, byte[] aesKey)
        {
            try
            {
                _logger.Information("شروع رمزگشایی داده‌های AES...");
                
                // Check CryptoJS format
                if (!encryptedData.StartsWith("U2FsdGVkX1"))
                {
                    _logger.Warning("فرمت داده‌های AES معتبر نیست (CryptoJS format expected)");
                    return encryptedData;
                }

                // Remove "U2FsdGVkX1" prefix and decode Base64
                var base64Data = encryptedData.Substring(10);
                _logger.Information($"Base64 data after removing prefix: {base64Data}");
                
              // Try multiple Base64 decoding approaches
              byte[] fullData = null;
              
              // First, let's try to understand the data format
              _logger.Information($"Original base64Data length: {base64Data.Length}");
              _logger.Information($"Original base64Data: {base64Data}");
              
              // Check if it's URL-safe Base64
              var urlSafeData = base64Data.Replace('-', '+').Replace('_', '/');
              _logger.Information($"URL-safe converted: {urlSafeData}");
              
              var approaches = new[]
              {
                  base64Data, // Original
                  urlSafeData, // URL-safe conversion
                  base64Data.TrimEnd('='), // Remove existing padding
                  base64Data.Replace("=", ""), // Remove all padding
                  System.Text.RegularExpressions.Regex.Replace(base64Data, @"[^A-Za-z0-9+/=]", ""), // Clean invalid chars
                  System.Text.RegularExpressions.Regex.Replace(urlSafeData, @"[^A-Za-z0-9+/=]", "") // Clean URL-safe
              };

              foreach (var approach in approaches)
              {
                  try
                  {
                      var testData = approach;
                      
                      // Ensure proper padding
                      while (testData.Length % 4 != 0)
                      {
                          testData += "=";
                      }
                      
                      _logger.Information($"Trying Base64 approach: {testData.Substring(0, Math.Min(50, testData.Length))}...");
                      
                      fullData = Convert.FromBase64String(testData);
                      _logger.Information($"Base64 decoding successful with approach: {approach.Substring(0, Math.Min(20, approach.Length))}...");
                      break;
                  }
                  catch (Exception ex)
                  {
                      _logger.Warning($"Base64 approach failed: {ex.Message}");
                      continue;
                  }
              }

              if (fullData == null)
              {
              // Try manual character-by-character analysis
              _logger.Information("Trying manual character analysis...");
              var cleanData = "";
              foreach (char c in base64Data)
              {
                  if (char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=')
                  {
                      cleanData += c;
                  }
              }
              
              _logger.Information($"Cleaned data length: {cleanData.Length}");
              _logger.Information($"Cleaned data: {cleanData}");
              
              // Check for invalid characters
              var invalidChars = new List<char>();
              for (int i = 0; i < base64Data.Length; i++)
              {
                  char c = base64Data[i];
                  if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
                  {
                      if (!invalidChars.Contains(c))
                      {
                          invalidChars.Add(c);
                      }
                  }
              }
              
              if (invalidChars.Count > 0)
              {
                  _logger.Information($"Found invalid characters: {string.Join(", ", invalidChars.Select(c => $"'{c}' ({(int)c})"))}");
              }
              
              // Log hex representation of the data
              var hexData = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(base64Data)).Replace("-", " ");
              _logger.Information($"Base64 data in hex: {hexData.Substring(0, Math.Min(100, hexData.Length))}...");
              
              // Try to understand what's wrong with the Base64 string
              _logger.Information($"Base64 string length: {base64Data.Length}");
              _logger.Information($"Base64 string ends with: {base64Data.Substring(Math.Max(0, base64Data.Length - 10))}");
              
              // Check for invalid characters in detail
              var invalidPositions = new List<int>();
              for (int i = 0; i < base64Data.Length; i++)
              {
                  char c = base64Data[i];
                  if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '+' || c == '/' || c == '='))
                  {
                      invalidPositions.Add(i);
                      _logger.Information($"Invalid character at position {i}: '{c}' (ASCII: {(int)c})");
                  }
              }
              
              if (invalidPositions.Count == 0)
              {
                  _logger.Information("No invalid characters found in Base64 string");
                  
                  // Try a different approach - maybe the issue is with padding
                  _logger.Information("Trying alternative Base64 decoding approach...");
                  
                  // Try to decode without any modifications first
                  try
                  {
                      var testDecode = Convert.FromBase64String(base64Data);
                      _logger.Information($"Direct Base64 decode successful! Length: {testDecode.Length}");
                      fullData = testDecode;
                  }
                  catch (Exception ex)
                  {
                      _logger.Information($"Direct decode failed: {ex.Message}");
                      
                      // Try with different padding approaches
                      var paddingTests = new[]
                      {
                          base64Data,
                          base64Data.TrimEnd('='),
                          base64Data + "=",
                          base64Data + "==",
                          base64Data + "===",
                          // Try removing all padding and adding proper padding
                          base64Data.Replace("=", ""),
                          base64Data.Replace("=", "") + "=",
                          base64Data.Replace("=", "") + "==",
                          base64Data.Replace("=", "") + "==="
                      };
                      
                      foreach (var test in paddingTests)
                      {
                          try
                          {
                              _logger.Information($"Trying padding test: {test.Substring(Math.Max(0, test.Length - 10))}");
                              var result = Convert.FromBase64String(test);
                              _logger.Information($"Padding test successful! Length: {result.Length}");
                              fullData = result;
                              break;
                          }
                          catch (Exception ex2)
                          {
                              _logger.Information($"Padding test failed: {ex2.Message}");
                          }
                      }
                      
                      // If all padding tests failed, try a completely different approach
                      if (fullData == null)
                      {
                          _logger.Information("All padding tests failed. Trying manual Base64 reconstruction...");
                          
                          // Try to manually reconstruct the Base64 string
                          var manualBase64 = "";
                          for (int i = 0; i < base64Data.Length; i += 4)
                          {
                              var chunk = base64Data.Substring(i, Math.Min(4, base64Data.Length - i));
                              while (chunk.Length < 4)
                              {
                                  chunk += "=";
                              }
                              manualBase64 += chunk;
                          }
                          
                          try
                          {
                              _logger.Information($"Trying manual reconstruction: {manualBase64.Substring(Math.Max(0, manualBase64.Length - 10))}");
                              var result = Convert.FromBase64String(manualBase64);
                              _logger.Information($"Manual reconstruction successful! Length: {result.Length}");
                              fullData = result;
                          }
                          catch (Exception ex3)
                          {
                              _logger.Information($"Manual reconstruction failed: {ex3.Message}");
                          }
                          
                          // If manual reconstruction also failed, try a completely different approach
                          if (fullData == null)
                          {
                              _logger.Information("Manual reconstruction also failed. Trying alternative encoding detection...");
                              
                              // Maybe it's not Base64 at all - try to detect the actual encoding
                              var bytes = System.Text.Encoding.UTF8.GetBytes(base64Data);
                              _logger.Information($"Raw bytes length: {bytes.Length}");
                              _logger.Information($"First 20 bytes: {string.Join(" ", bytes.Take(20).Select(b => b.ToString("X2")))}");
                              
                              // Try to treat it as raw bytes instead of Base64
                              try
                              {
                                  _logger.Information("Trying to treat as raw bytes...");
                                  fullData = bytes;
                                  _logger.Information("Raw bytes approach successful!");
                                  
                                  // But we still need to decode it as Base64
                                  _logger.Information("Now trying to decode the raw bytes as Base64...");
                                  try
                                  {
                                      var base64String = System.Text.Encoding.UTF8.GetString(bytes);
                                      _logger.Information($"Base64 string from raw bytes: {base64String.Substring(0, Math.Min(50, base64String.Length))}...");
                                      
                                      // Try to decode this Base64 string
                                      var decodedBytes = Convert.FromBase64String(base64String);
                                      _logger.Information($"Base64 decode from raw bytes successful! Length: {decodedBytes.Length}");
                                      fullData = decodedBytes;
                                  }
                                  catch (Exception ex5)
                                  {
                                      _logger.Information($"Base64 decode from raw bytes failed: {ex5.Message}");
                                  }
                              }
                              catch (Exception ex4)
                              {
                                  _logger.Information($"Raw bytes approach failed: {ex4.Message}");
                              }
                          }
                      }
                  }
              }
              
              try
              {
                  while (cleanData.Length % 4 != 0)
                  {
                      cleanData += "=";
                  }
                  
                  _logger.Information($"Manual clean data: {cleanData.Substring(0, Math.Min(50, cleanData.Length))}...");
                  fullData = Convert.FromBase64String(cleanData);
                  _logger.Information("Manual character analysis successful!");
              }
              catch (Exception ex)
              {
                  _logger.Error($"Manual character analysis failed: {ex.Message}");
                  
                  // Try to fix common Base64 issues
                  _logger.Information("Trying to fix common Base64 issues...");
                  
                  // Remove all non-Base64 characters and try again
                  var fixedData = System.Text.RegularExpressions.Regex.Replace(base64Data, @"[^A-Za-z0-9+/=]", "");
                  
                  try
                  {
                      while (fixedData.Length % 4 != 0)
                      {
                          fixedData += "=";
                      }
                      
                      _logger.Information($"Fixed data: {fixedData.Substring(0, Math.Min(50, fixedData.Length))}...");
                      fullData = Convert.FromBase64String(fixedData);
                      _logger.Information("Fixed data successful!");
                  }
                  catch (Exception ex2)
                  {
                      _logger.Error($"Fixed data also failed: {ex2.Message}");
                      
                      // Try one more approach - manual Base64 validation
                      _logger.Information("Trying manual Base64 validation...");
                      
                      // Check if the string is valid Base64 by trying to decode it character by character
                      var validBase64 = "";
                      for (int i = 0; i < base64Data.Length; i++)
                      {
                          char c = base64Data[i];
                          if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '+' || c == '/' || c == '=')
                          {
                              validBase64 += c;
                          }
                      }
                      
                      _logger.Information($"Valid Base64 characters only: {validBase64.Substring(0, Math.Min(50, validBase64.Length))}...");
                      
                      try
                      {
                          while (validBase64.Length % 4 != 0)
                          {
                              validBase64 += "=";
                          }
                          
                          _logger.Information($"Final attempt: {validBase64.Substring(0, Math.Min(50, validBase64.Length))}...");
                          fullData = Convert.FromBase64String(validBase64);
                          _logger.Information("Manual validation successful!");
                      }
                      catch (Exception ex3)
                      {
                          _logger.Error($"Manual validation also failed: {ex3.Message}");
                          throw new FormatException("All Base64 decoding approaches failed");
                      }
                  }
              }
              }
                
                _logger.Information($"داده‌های Base64 تبدیل شد - طول: {fullData.Length} بایت");

                // Check minimum length
                if (fullData.Length < 24)
                {
                    _logger.Error($"داده‌های ناکافی - طول: {fullData.Length} بایت");
                    return encryptedData;
                }

                // Extract salt (first 8 bytes)
                var salt = new byte[8];
                Array.Copy(fullData, 0, salt, 0, 8);
                _logger.Information($"Salt استخراج شد - طول: {salt.Length} بایت");

                // Extract encrypted data (remaining bytes)
                var encrypted = new byte[fullData.Length - 8];
                Array.Copy(fullData, 8, encrypted, 0, encrypted.Length);
                _logger.Information($"داده‌های رمز شده استخراج شد - طول: {encrypted.Length} بایت");

                // Derive key and IV from password and salt
                var keyIv = DeriveKeyAndIV(aesKey, salt);
                var derivedKey = keyIv.Item1;
                var iv = keyIv.Item2;
                
                _logger.Information($"کلید و IV تولید شد - کلید: {derivedKey.Length} بایت، IV: {iv.Length} بایت");

                // Decrypt with AES-256-CBC
                using (var aes = Aes.Create())
                {
                    aes.Key = derivedKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(encrypted))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        var decryptedText = srDecrypt.ReadToEnd();
                        _logger.Information($"داده‌های AES رمزگشایی شد. طول: {decryptedText.Length} کاراکتر");
                        return decryptedText;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی داده‌های AES");
                return $"خطا در رمزگشایی AES: {ex.Message}";
            }
        }

        /// <summary>
        /// Try direct RSA decryption (fallback method)
        /// </summary>
        /// <param name="encryptedData">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        private async Task<string> DecryptDirectRSA(string encryptedData)
        {
            try
            {
                _logger.Information("شروع رمزگشایی مستقیم RSA...");
                
                using (var rsa = RSA.Create(2048))
                {
                    var privateKeyBytes = Convert.FromBase64String(_privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var encryptedBytes = Convert.FromBase64String(encryptedData);
                    
                    // Try different padding methods
                    try
                    {
                        // Try OAEP SHA-256 first as specified
                        var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                        var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                        _logger.Information($"رمزگشایی مستقیم RSA با OAEP SHA-256 موفقیت‌آمیز بود");
                        return decryptedText;
                    }
                    catch (Exception ex1)
                    {
                        _logger.Warning($"OAEP SHA-256 failed: {ex1.Message}");
                        
                        try
                        {
                            // Try OAEP SHA-1 as fallback
                            var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA1);
                            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                            _logger.Information($"رمزگشایی مستقیم RSA با OAEP SHA-1 موفقیت‌آمیز بود");
                            return decryptedText;
                        }
                        catch (Exception ex2)
                        {
                            _logger.Warning($"Direct RSA decryption failed: {ex2.Message}");
                            return "رمزگشایی مستقیم RSA ناموفق بود";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در رمزگشایی مستقیم RSA");
                return $"خطا در رمزگشایی مستقیم RSA: {ex.Message}";
            }
        }

        /// <summary>
        /// Derive key and IV from password and salt (CryptoJS compatible)
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Key and IV</returns>
        private (byte[], byte[]) DeriveKeyAndIV(byte[] password, byte[] salt)
        {
            try
            {
                _logger.Information($"شروع تولید کلید و IV - طول password: {password.Length}, طول salt: {salt.Length}");
                
                using (var md5 = MD5.Create())
                {
                    var key = new byte[32]; // 256 bits
                    var iv = new byte[16];   // 128 bits
                    
                    var derived = new List<byte>();
                    var currentHash = new byte[0];
                    
                    // CryptoJS uses multiple rounds of MD5 hashing
                    while (derived.Count < 48) // 32 bytes key + 16 bytes IV
                    {
                        var toHash = new List<byte>();
                        
                        // Add previous hash if exists
                        if (currentHash.Length > 0)
                        {
                            toHash.AddRange(currentHash);
                        }
                        
                        // Add password
                        toHash.AddRange(password);
                        
                        // Add salt
                        toHash.AddRange(salt);
                        
                        // Compute MD5 hash
                        currentHash = md5.ComputeHash(toHash.ToArray());
                        derived.AddRange(currentHash);
                        
                        _logger.Information($"MD5 round completed - derived length: {derived.Count}");
                    }
                    
                    // Extract key and IV
                    var derivedArray = derived.ToArray();
                    Array.Copy(derivedArray, 0, key, 0, 32);
                    Array.Copy(derivedArray, 32, iv, 0, 16);
                    
                    _logger.Information($"کلید و IV تولید شد - کلید: {Convert.ToBase64String(key)}, IV: {Convert.ToBase64String(iv)}");
                    
                    return (key, iv);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تولید کلید و IV");
                throw;
            }
        }

        /// <summary>
        /// Test key pair compatibility by encrypting with public key and decrypting with private key
        /// </summary>
        private async Task TestKeyPairCompatibility()
        {
            try
            {
                _logger.Information("شروع تست سازگاری کلیدها...");
                
                var testMessage = "Test message for key pair verification";
                _logger.Information($"پیام تست: {testMessage}");
                
                // Encrypt with public key
                using (var rsa = RSA.Create(2048))
                {
                    var publicKeyBytes = Convert.FromBase64String(_publicKey);
                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    
                    var messageBytes = Encoding.UTF8.GetBytes(testMessage);
                    var encryptedBytes = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA256);
                    var encryptedBase64 = Convert.ToBase64String(encryptedBytes);
                    
                    _logger.Information($"پیام با کلید عمومی رمز شد: {encryptedBase64}");
                    
                    // Decrypt with private key
                    var privateKeyBytes = Convert.FromBase64String(_privateKey);
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    
                    var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                    var decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
                    
                    _logger.Information($"پیام با کلید خصوصی رمزگشایی شد: {decryptedMessage}");
                    
                    if (testMessage == decryptedMessage)
                    {
                        _logger.Information("✅ کلیدها سازگار هستند - تست موفقیت‌آمیز");
                        Console.WriteLine("✅ کلیدها سازگار هستند - تست موفقیت‌آمیز");
                    }
                    else
                    {
                        _logger.Warning("❌ کلیدها سازگار نیستند");
                        Console.WriteLine("❌ کلیدها سازگار نیستند");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "خطا در تست سازگاری کلیدها");
                Console.WriteLine($"خطا در تست سازگاری کلیدها: {ex.Message}");
            }
        }
    }
}
