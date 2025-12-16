using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;

public class BunnyCDNService
{
    ImageConversionService imageService = new ImageConversionService();
    const string Bunny_PullZone_Endpoint = "https://m-p-z.b-cdn.net";
    const string Bunny_StorageZone_Base_Endpoint = "https://storage.bunnycdn.com";
    const string Bunny_StorageZone_Endpoint = $"{Bunny_StorageZone_Base_Endpoint}/my-storage-zone-name";
    const string Bunny_StorageZone_AccessKey = "------------secret----------------";
    private readonly HttpClient bunnyStorageZoneHttpClient;

    public BunnyCDNService()
    {
        this.bunnyStorageZoneHttpClient = new HttpClient();
        this.bunnyStorageZoneHttpClient.DefaultRequestHeaders.Add("AccessKey", Bunny_StorageZone_AccessKey);
    }

    public bool GetF(string filePath = "")
    {
        try
        {
            var response = this.bunnyStorageZoneHttpClient.GetAsync($"{Bunny_StorageZone_Endpoint}/{filePath}").GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode}");
                return false;
            }

            var json = response.Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            var items = JsonSerializer.Deserialize<List<BunnyStorageItem>>(json);

            foreach (var item in items)
            {
                Console.WriteLine($"Name: {item.ObjectName}, Dir: {item.IsDirectory}, Size: {item.Length}");

                if (item.IsDirectory) this.GetF($"{filePath}/{item.ObjectName}/");
                else
                {
                    //download image
                    this.DownloadImageAsFileFromStorage2($"{item.Path}", item.ObjectName);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    public bool GetB(string filePath = "")
    {
        try
        {
            var response = this.bunnyStorageZoneHttpClient.GetAsync($"{Bunny_StorageZone_Endpoint}/{filePath}").GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode}");
                return false;
            }

            var json = response.Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            var items = JsonSerializer.Deserialize<List<BunnyStorageItem>>(json);

            foreach (var item in items)
            {
                Console.WriteLine($"Name: {item.ObjectName}, Dir: {item.IsDirectory}, Size: {item.Length}");

                if (!item.IsDirectory)
                {
                    //download image
                    this.DownloadImageAsBase64FromStorage2($"{item.Path}", item.ObjectName);
                }
                else this.GetB($"{filePath}/{item.ObjectName}/");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    public bool Delete(string filePath)
    {
        string cdnError = string.Empty;
        string cdnErrorDetail = string.Empty;

        try
        {
            var response = this.bunnyStorageZoneHttpClient.DeleteAsync($"{Bunny_StorageZone_Endpoint}/{NormalizeFilePath(filePath)}").Result;

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
            var response = this.bunnyStorageZoneHttpClient.DeleteAsync($"{Bunny_StorageZone_Endpoint}/{NormalizeFolderPath(filePath)}").Result;

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

            var response = this.bunnyStorageZoneHttpClient.PutAsync($"{Bunny_StorageZone_Endpoint}/{NormalizeFilePath(filePath)}", content).Result;

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

    public void DownloadImageAsFileFromStorage2(string filePath, string filename = "")
    {
        try
        {
            byte[] imageBytes = this.bunnyStorageZoneHttpClient.GetByteArrayAsync($"{Bunny_StorageZone_Base_Endpoint}{filePath}{filename}").Result;
            var path = @$"C:\Users\NAROT\AppData\Local\Temp\downloaded{filePath}{filename}";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, imageBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }

    public void DownloadImageAsBase64FromStorage2(string filePath, string filename = "")
    {
        try
        {
            byte[] imageBytes = this.bunnyStorageZoneHttpClient.GetByteArrayAsync($"{Bunny_StorageZone_Base_Endpoint}{filePath}{filename}").Result;
            string base64String = Convert.ToBase64String(imageBytes);
            Console.WriteLine(base64String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }


    public void DownloadImageAsFileFromStorage(string filePath)
    {
        try
        {
            byte[] imageBytes = this.bunnyStorageZoneHttpClient.GetByteArrayAsync($"{Bunny_StorageZone_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            var path = @$"C:\Users\NAROT\AppData\Local\Temp\downloaded/{NormalizeFilePath(filePath)}";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, imageBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }

    public void DownloadImageAsBase64FromStorage(string filePath)
    {
        try
        {
            byte[] imageBytes = this.bunnyStorageZoneHttpClient.GetByteArrayAsync($"{Bunny_StorageZone_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            string base64String = Convert.ToBase64String(imageBytes);
            Console.WriteLine(base64String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }

    public void DownloadImageAsFileFromPullZone(string filePath)
    {
        try
        {
            var httpClient = new HttpClient();
            byte[] imageBytes = httpClient.GetByteArrayAsync($"{Bunny_PullZone_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            var path = @$"C:\Users\NAROT\AppData\Local\Temp\downloaded_pz/{NormalizeFilePath(filePath)}";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, imageBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }

    public void DownloadImageAsBase64FromPullZone(string filePath)
    {
        try
        {
            var httpClient = new HttpClient();
            byte[] imageBytes = httpClient.GetByteArrayAsync($"{Bunny_PullZone_Endpoint}/{NormalizeFilePath(filePath)}").Result;
            string base64String = Convert.ToBase64String(imageBytes);
            Console.WriteLine(base64String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
        }
    }
}
