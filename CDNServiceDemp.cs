using System;
using System.IO;
using System.Net.Http;

class CDNServiceDemp
{
    public static void Main()
    {
        CDNService cDNService = new CDNService();

        string cmdKey = string.Empty;
        do
        {
            if (cmdKey == null) { cmdKey = Console.ReadLine(); }
            switch (cmdKey)
            {
                case "U":
                    {
                        foreach (string filePath in Directory.GetFiles(@".\images"))
                        {
                            Console.WriteLine(filePath);

                            // Extract the file name from the full path
                            string fileName = Path.GetFileName(filePath);

                            cDNService.Upload(filePath);

                            // Print the file name
                            Console.WriteLine($"https://my-cdn-app.b-cdn.net/{fileName}");
                        }

                        cmdKey = string.Empty;
                        break;
                    }
                case "D":
                    {
                        foreach (string filePath in Directory.GetFiles(@".\images"))
                        {
                            Console.WriteLine(filePath);

                            // Extract the file name from the full path
                            string fileName = Path.GetFileName(filePath);

                            cDNService.Delete(filePath);

                            // Print the file name
                            Console.WriteLine($"https://my-cdn-app.b-cdn.net/{fileName}");
                        }

                        cmdKey = string.Empty;
                        break;
                    }
                default:
                    {
                        Console.WriteLine();
                        Console.WriteLine("Enter U to upload all images from CDN");
                        Console.WriteLine("Enter D to delete all images from CDN");
                        Console.WriteLine("Enter E to Exit");
                        cmdKey = Console.ReadLine();
                        break;
                    }
            }

        } while (cmdKey != "E");
    }

    public class CDNService
    {
        const string CDN_Endpoint = "https://sg.storage.bunnycdn.com/my-serv-app";
        const string CDN_AccessKey = "f8cd6197-29b0-4bd5-9e8378b4306e-45f6-48dc";
        private readonly HttpClient httpClient;

        public CDNService()
        {
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("AccessKey", CDN_AccessKey);
        }


        public bool Delete(string filePath)
        {
            string cdnError = string.Empty;
            string cdnErrorDetail = string.Empty;

            try
            {
                var response = this.httpClient.DeleteAsync($"{CDN_Endpoint}/{Path.GetFileName(filePath)}").Result;

                if (response.IsSuccessStatusCode) return true;

                cdnError = $"CDN Delete failed: {response.StatusCode} - {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                cdnError = ex.Message;
                cdnErrorDetail = ex.StackTrace;
            }

            Console.WriteLine(cdnError + "\n" + cdnErrorDetail);
            return false;
        }

        public bool Upload(string filePath)
        {
            string cdnError = string.Empty;
            string cdnErrorDetail = string.Empty;

            try
            {
                using var fileStream = File.OpenRead(filePath);
                if (fileStream == null) return false;

                using var content = new StreamContent(fileStream);
                var response = this.httpClient.PutAsync($"{CDN_Endpoint}/{Path.GetFileName(filePath)}", content).Result;

                if (response.IsSuccessStatusCode) return true;

                cdnError = $"CDN Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                cdnError = ex.Message;
                cdnErrorDetail = ex.StackTrace;
            }

            Console.WriteLine(cdnError + "\n" + cdnErrorDetail);

            return false;
        }
    }
}