using System;
using System.IO;
using System.Net.Http;

public class BunnyCDNService
{
    ImageConversionService imageService = new ImageConversionService();
    const string CDN_Endpoint = "https://storage.bunnycdn.com/m-s-z";
    const string CDN_AccessKey = "2ed46a31-2a0c-4900-89f6453c63d8-879a-4c70";
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
