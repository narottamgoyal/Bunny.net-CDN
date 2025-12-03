using System;
using System.IO;

public class CDNServiceDemo
{
    public static void Main()
    {
        BunnyCDNService cDNService = new BunnyCDNService();

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
                case "DL":
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
                case "DWF":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsFileAsync(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "DWB":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsBase64Async(filePath);

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
                        Console.WriteLine("Enter DL to delete image from CDN");
                        Console.WriteLine("Enter DWF to download image from CDN");
                        Console.WriteLine("Enter DWB to download image from CDN");
                        Console.WriteLine("Enter DF to delete all images from CDN");
                        Console.WriteLine("Enter E to Exit");
                        cmdKey = Console.ReadLine();
                        break;
                    }
            }

        } while (cmdKey != "E");
    }
}