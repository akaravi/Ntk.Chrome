using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ntk.Chrome
{
    public class TestDecrypt_RSA
    {
        /// <summary>
        /// Generates a new RSA key pair with OAEP padding and SHA-256 hash
        /// Equivalent to window.crypto.subtle.generateKey in JavaScript
        /// </summary>
        /// <returns>Tuple containing public and private key as base64 strings</returns>
        public static (string PublicKey, string PrivateKey) GenerateKeyPair()
        {
            using (RSA rsa = RSA.Create()) // 2048-bit key length
            {
                // Export public key in SPKI format
                byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                string publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

                // Export private key in PKCS#8 format
                byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
                string privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

                return (publicKeyBase64, privateKeyBase64);
            }
        }

        /// <summary>
        /// Imports a public key from JWK format
        /// Equivalent to window.crypto.subtle.importKey in JavaScript
        /// </summary>
        /// <param name="jwk">JWK object containing key data</param>
        /// <returns>RSA instance with imported public key</returns>
        public static RSA ImportPublicKeyFromJWK(JWK jwk)
        {
            RSA rsa = RSA.Create();
            
            // Convert JWK to RSA parameters
            var rsaParams = new RSAParameters
            {
                Modulus = Convert.FromBase64String(jwk.n),
                Exponent = Convert.FromBase64String(jwk.e)
            };
            
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        /// <summary>
        /// Exports a public key to JWK format
        /// Equivalent to window.crypto.subtle.exportKey in JavaScript
        /// </summary>
        /// <param name="publicKey">RSA public key</param>
        /// <returns>JWK object</returns>
        public static JWK ExportPublicKeyToJWK(RSA publicKey)
        {
            var parameters = publicKey.ExportParameters(false);
            
            return new JWK
            {
                kty = "RSA",
                e = Convert.ToBase64String(parameters.Exponent ?? Array.Empty<byte>()),
                n = Convert.ToBase64String(parameters.Modulus ?? Array.Empty<byte>()),
                alg = "RSA-OAEP-256",
                ext = true
            };
        }

        /// <summary>
        /// Encrypts data using RSA-OAEP with SHA-256
        /// Equivalent to window.crypto.subtle.encrypt in JavaScript
        /// </summary>
        /// <param name="publicKey">RSA public key</param>
        /// <param name="data">Data to encrypt</param>
        /// <returns>Encrypted data as byte array</returns>
        public static byte[] Encrypt(RSA publicKey, byte[] data)
        {
            return publicKey.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        /// <summary>
        /// Decrypts data using RSA-OAEP with SHA-256
        /// Equivalent to window.crypto.subtle.decrypt in JavaScript
        /// </summary>
        /// <param name="privateKey">RSA private key</param>
        /// <param name="encryptedData">Encrypted data</param>
        /// <returns>Decrypted data as byte array</returns>
        public static byte[] Decrypt(RSA privateKey, byte[] encryptedData)
        {
            return privateKey.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
        }
 
        /// <summary>
        /// Wraps a key using RSA-OAEP
        /// Equivalent to window.crypto.subtle.wrapKey in JavaScript
        /// </summary>
        /// <param name="publicKey">RSA public key for wrapping</param>
        /// <param name="keyToWrap">Key to be wrapped</param>
        /// <returns>Wrapped key as byte array</returns>
        public static byte[] WrapKey(RSA publicKey, byte[] keyToWrap)
        {
            return publicKey.Encrypt(keyToWrap, RSAEncryptionPadding.OaepSHA256);
        }

        /// <summary>
        /// Unwraps a key using RSA-OAEP
        /// Equivalent to window.crypto.subtle.unwrapKey in JavaScript
        /// </summary>
        /// <param name="privateKey">RSA private key for unwrapping</param>
        /// <param name="wrappedKey">Wrapped key</param>
        /// <returns>Unwrapped key as byte array</returns>
        public static byte[] UnwrapKey(RSA privateKey, byte[] wrappedKey)
        {
            return privateKey.Decrypt(wrappedKey, RSAEncryptionPadding.OaepSHA256);
        }

        /// <summary>
        /// Test method to demonstrate all RSA-OAEP operations
        /// </summary>
        public static void TestAllOperations()
        {
            try
            {
                Console.WriteLine("=== تست کامل عملیات RSA-OAEP ===");

                // 1. Generate key pair
                Console.WriteLine("\n1. تولید کلید:");
                //var (publicKeyBase64, privateKeyBase64) = GenerateKeyPair();
                var publicKeyBase64 = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxo9gBgRy4bpZcl+d74hrU8LH9EiIEbOCnrCSWyqFUYMYLbROeEiigX1vGb/ntdIDda3+pWN0JlbwQ/bKxWtpJiDFGC6i3PmyV+5mBPA+EVMVxIyW0frZqlrVVTnCmnZKLbgp/UM9wf167GopGaZhgqdkITa0+KxSzWRGg5KXzNHu80HWICSsVkMiC5weVte8t30o32qYhqc3Csk5Yiz1osAaQiwSD/SgEnMqlH5TCAvYdJ98aKQkfIeyOUwaFegDM5MkSv27Gnq7/dkMXW/pPmL7k59UiFphuFWQ/dEMAmf4y7wlboW6JVr9eTX7Dj4rofgS95Xi1A9wTKpRySTbDQIDAQAB";
                var privateKeyBase64 = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";
                Console.WriteLine($"کلید عمومی: {publicKeyBase64}");
                Console.WriteLine($"کلید خصوصی: {privateKeyBase64}");

                // 2. Import keys
                Console.WriteLine("\n2. Import کلیدها:");
                using (RSA publicKey = RSA.Create())
                using (RSA privateKey = RSA.Create())
                {
                    //publicKey.ImportRSAPublicKey(Convert.FromBase64String(publicKeyBase64), out _);
                    publicKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);

                    //privateKey.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
                    privateKey.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);

                    // 3. Export to JWK
                    Console.WriteLine("\n3. Export به JWK:");
                    JWK jwk = ExportPublicKeyToJWK(publicKey);
                    Console.WriteLine($"JWK: {jwk}");

                    // 4. Test encryption/decryption
                    Console.WriteLine("\n4. تست رمزگذاری/رمزگشایی:");
                    string testData = "سلام، این یک متن تست است";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(testData);
                    Console.WriteLine($"متن اصلی: {testData}");

                    byte[] encrypted = Encrypt(publicKey, dataBytes);
                    Console.WriteLine($"رمزگذاری شده: {Convert.ToBase64String(encrypted)}");
                    
                    //encrypted = Convert.FromBase64String("U2FsdGVkX195i9WtFcoUJEd4Ja+SJMn+Ff9yyAsvc/6unZbpslHisJ7pT67p9eu5wwzSryy55NKCdsSXIh6jFr8PdUmjPit5OEvatXzu+O/qO6NO53jUq6tMjX4CvW0Hw+SPhLeK3qLOvKSscYH4TEuBXnwuwp3C62NGh2CAnH+fh+CkZyF8QeoiE3LqF8ultYg8VPA2jT2fFL1z4ZM8wKOEAGsAZU4UtppM7k9CZsU=");
                    byte[] decrypted = Decrypt(privateKey, encrypted);
                    string decryptedText = Encoding.UTF8.GetString(decrypted);
                    Console.WriteLine($"رمزگشایی شده: {decryptedText}");
                    Console.WriteLine($"موفقیت‌آمیز: {testData == decryptedText}");

                    // 5. Test key wrapping/unwrapping
                    Console.WriteLine("\n5. تست Key Wrapping:");
                    byte[] keyToWrap = Encoding.UTF8.GetBytes("SecretKey123");
                    Console.WriteLine($"کلید برای wrap: {Convert.ToBase64String(keyToWrap)}");

                    byte[] wrappedKey = WrapKey(publicKey, keyToWrap);
                    Console.WriteLine($"کلید wrapped: {Convert.ToBase64String(wrappedKey)}");

                    byte[] unwrappedKey = UnwrapKey(privateKey, wrappedKey);
                    Console.WriteLine($"کلید unwrapped: {Convert.ToBase64String(unwrappedKey)}");
                    Console.WriteLine($"Key wrapping موفقیت‌آمیز: {Convert.ToBase64String(keyToWrap) == Convert.ToBase64String(unwrappedKey)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا: {ex.Message}");
            }
        }



        /// <summary>
        /// Test method to demonstrate all RSA-OAEP operations
        /// </summary>
        public static void TestDecryptOperations()
        {
            try
            {
                var privateKeyBase64 = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCzJ8PwsGzhdDz+PwewygYAzCUy/VPU3rJ0ta3jxcTHnuNacxbaQwc9MxgAvJWE1DHcUgNYs3SzoqGifiGxbB7At8YlVgAb5N8fjhjXsc0VN/C+jEaRQGRI/BmyEIpZR8+VSdo6m+0VrTAPkkykF0Bk7gMo07c6hqD/C2442GTtukSUiPaPReHnlHtzTmoCSgeDQP4CO3hEH1c0gEjvunhCqEW9ifZk+RM4JVBeBsPPBz+Uf7R6V0z02KtLEr4eKGxi6UC7MuJr6H82G6kRvlAFLkSi1Etc2f8VihPLsvh9O7yxminPQz1LztnyrgFgfbl53wZJ0WufBVGNmPdHMhfVAgMBAAECggEAJV/eWI/1pvMA5mlvyUncBr6P5BtFKdtrjz13kVTowFw9QdlQoyfokrPeBglRh+xcmoHhgNevOOpsneGCVekgYUP1akSOsUMF6SdTt2u4RPzulFHfRt4QDcnJ8oPQ2N9KRvKpPCDbTPJcXGNA6dqP7H5a2mGQj/0WCR7xV5qNM6qWFK2NLWmP5Iln4PN0z/X0iV26UuKotz3QQjYjrlsRsizWafoMzw4/xVv/V1lRTVO3OX2DoIi71g+BZ1kjqhrydWQAWv70mb2P4lmWdgk8y5lsDz8rN5JQePpRBZ2RMyynBzve6HUMHOFRtfEUGpIlP63i/111Sdk/P2V7jsZM4QKBgQD0ImIdtMRRO6nuTAuoKyMfJrlge0nlDCciDFGBgd9nJuwOMPby/XELnbuzjAFnjnTaVbUKHMfRDp0yBV59Yr0prCZXCkmA80ZoYOlovDfxqoyj809XbBoErsXS0WJAcmJT9nx5VGvRDWSgLevdjnKkCcDUxZuJLKbvAxEahr2/wwKBgQC73ONBNI5/ew8Ey6xnmYjKHZLH6F31mTbU1pMIDnWnjegYkE96eIz+spvbEpIBgHDBkknlRYL4GiYQIzQfHgKeO4I+WBiS4/7uih8P9Q/6M0TC048+HNX/SHSs1OSWxiIrF6KXrr8SB/AE55vBCYATwYw+JpR3zjh3WcI6JO6ohwKBgQDxQTksmgKXNBrNvqCWY2ql0iLHUY7IpqXVY8736FvZGAGWVJT1s7cO/6UJ3YVVzNV1HdV2VNKxqXt2fw/NYNIGaHTK9wOERuSBKaP/OGEglKW/LyZtAgsELaKYnwo1HdRFnQOM8vxI7q9OC5NWsvpfWLQSj+UQPewJrkIssJK6+QKBgCW4cWz7R41zQQ2+c4yNuHiUvY8kKhGRRQAxYW5hsOAGz053U24M3IqbhE3VibmBd6J2ZB4D+gsk/PWKjAGfffkVi85G1BBSdTKiSyBiHWYoeyr/XaikE5fhjYPSb1+SwvOSGFSKgtT1AQ2LD6wP40aUOzuTdYYkwxO70xLnrX/rAoGAdjz/4Xm1N77MJ3U0+xEKIT0pRSJnwQFUykq4X4QvbWBSZ4hObRCT1v4VKB6zlF9B3vfF7CXiyrymr7gDbJzpzUnBZTBYMTaRHRCqEtoZWgkjZbpEPtdPqiTbjTHNgb8e7OfeGjcu+uZAXKPuStni340OFJzrVzZtVoWSyIasEt8=";
                Console.WriteLine($"کلید خصوصی: {privateKeyBase64}");

                // 2. Import keys
                Console.WriteLine("\n2. Import کلیدها:");

                using (RSA privateKey = RSA.Create())
                {
                    //privateKey.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
                    privateKey.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);


                    var encrypted = Convert.FromBase64String("U2FsdGVkX195i9WtFcoUJEd4Ja+SJMn+Ff9yyAsvc/6unZbpslHisJ7pT67p9eu5wwzSryy55NKCdsSXIh6jFr8PdUmjPit5OEvatXzu+O/qO6NO53jUq6tMjX4CvW0Hw+SPhLeK3qLOvKSscYH4TEuBXnwuwp3C62NGh2CAnH+fh+CkZyF8QeoiE3LqF8ultYg8VPA2jT2fFL1z4ZM8wKOEAGsAZU4UtppM7k9CZsU=");
                    byte[] decrypted = Decrypt(privateKey, encrypted);
                    string decryptedText = Encoding.UTF8.GetString(decrypted);
                    Console.WriteLine($"رمزگشایی شده: {decryptedText}");
                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا: {ex.Message}");
            }
        }

        /// <summary>
        /// Simple test method that can be called from Main
        /// </summary>
        public static void RunSimpleTest()
        {
            Console.WriteLine("=== تست ساده RSA-OAEP ===");
            TestDecryptOperations();
            TestAllOperations();
        }
    }

    /// <summary>
    /// JWK (JSON Web Key) structure for key exchange
    /// </summary>
    public class JWK
    {
        public string kty { get; set; } = "RSA";
        public string e { get; set; } = string.Empty;
        public string n { get; set; } = string.Empty;
        public string alg { get; set; } = "RSA-OAEP-256";
        public bool ext { get; set; } = true;

        public override string ToString()
        {
            return $"{{kty: {kty}, e: {e}, n: {n}, alg: {alg}, ext: {ext}}}";
        }
    }
}
