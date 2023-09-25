using SimpleReddit.FileService.Contracts;
using System.IO;

namespace SimpleReddit.FileService
{
    public class TokenService : ITokenService
    {
        private string _fileName = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "logs\\RDNOauthToken_";

        public String GetTokenAsync()
        {
            string result = string.Empty;
            string tokenPath = _fileName + DateTime.Now.ToString("yyyyMMdd") + ".json";

            try
            {
                using (FileStream fs = new FileStream(tokenPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            return result;
        }

        public void SaveTokenAsync(string content)
        {
            try
            {
                string tokenPath = _fileName + DateTime.Now.ToString("yyyyMMdd") + ".json";

                File.WriteAllText(tokenPath, content);
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }
    }
}