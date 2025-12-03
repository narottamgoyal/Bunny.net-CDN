using System;
using System.IO;
using System.Net.Http;

partial class CDNServiceDemp
{
    public static void Main()
    {
        CDNService cDNService = new CDNService();

        string cmdKey = string.Empty;
        do
        {
            if (cmdKey == null) { cmdKey = Console.ReadLine(); }
            switch (cmdKey.ToUpper())
            {
                case "U":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.Upload(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "D":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.Delete(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "DF":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            cDNService.DeleteF(dirPath);
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                default:
                    {
                        Console.WriteLine();
                        Console.WriteLine("Enter U to upload all images from CDN");
                        Console.WriteLine("Enter D to delete image from CDN");
                        Console.WriteLine("Enter DF to delete all images from CDN");
                        Console.WriteLine("Enter E to Exit");
                        cmdKey = Console.ReadLine();
                        break;
                    }
            }

        } while (cmdKey != "E");
    }

    public class CDNService
    {
        ImageConversionService imageService = new ImageConversionService();
        const string CDN_Endpoint = "https://storage.bunnycdn.com/m-s-z";
        const string CDN_AccessKey = "2ed46a31-2a0c-4900-89f6453c63d8-879a-4c70";
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
                var response = this.httpClient.DeleteAsync($"{CDN_Endpoint}/{NormalizeFilePath(filePath)}").Result;

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

        public bool DeleteF(string filePath)
        {
            string cdnError = string.Empty;
            string cdnErrorDetail = string.Empty;

            try
            {
                var response = this.httpClient.DeleteAsync($"{CDN_Endpoint}/{NormalizeFolderPath(filePath)}").Result;

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
                //using var fileStream = File.OpenRead(filePath);
                //if (fileStream == null) return false;

                using var ms = imageService.ConvertImage(filePath);
                using var content = new StreamContent(ms);

                var response = this.httpClient.PutAsync($"{CDN_Endpoint}/{NormalizeFilePath(filePath)}", content).Result;

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

        public string NormalizeFilePath(string path)
        {
            path = path.Trim()
                .Replace("\\", "/")
                .TrimStart('.')
                .TrimStart('/');

            while (path.Contains("//"))
                path = path.Replace("//", "/");

            return Path.ChangeExtension(path, ".webp");
        }

        public string NormalizeFolderPath(string path)
        {
            // Trim all prepending & tailing whitespace, fix windows-like paths then remove prepending slashes
            path = path.Trim()
                .Replace("\\", "/")
                .TrimStart('.')
                .TrimStart('/');



            while (path.Contains("//"))
                path = path.Replace("//", "/");


            return path + "/.";
        }
    }
}