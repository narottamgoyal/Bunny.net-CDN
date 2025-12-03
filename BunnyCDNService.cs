using System;
using System.IO;
using System.Net.Http;

public class BunnyCDNService
{
    ImageConversionService imageService = new ImageConversionService();
    const string CDN_Endpoint = "https://storage.bunnycdn.com/my-storage-zone-name";
    const string CDN_AccessKey = "d31a73db-4f18-ad24c7a11549-7026-06d6-44d2";
    private readonly HttpClient httpClient;

    public BunnyCDNService()
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
        path = path.Trim()
            .Replace("\\", "/")
            .TrimStart('.')
            .TrimStart('/');



        while (path.Contains("//"))
            path = path.Replace("//", "/");


        return path + "/.";
    }

    public void DownloadImageAsFileAsync(string filePath)
    {
        try
        {
            byte[] imageBytes = this.httpClient.GetByteArrayAsync($"{CDN_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            var path = @$"C:\Users\NAROT\AppData\Local\Temp\downloaded/{NormalizeFilePath(filePath)}";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, imageBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }

    public void DownloadImageAsBase64Async(string filePath)
    {
        try
        {
            byte[] imageBytes = this.httpClient.GetByteArrayAsync($"{CDN_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            string base64String = Convert.ToBase64String(imageBytes);
            Console.WriteLine(base64String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }
}
