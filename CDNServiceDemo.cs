using System;
using System.IO;

public class CDNServiceDemo
{
    /// <summary>
    /// https://docs.bunny.net/reference/storage-api
    /// </summary>
    public static void Main()
    {
        BunnyCDNService cDNService = new BunnyCDNService();

        string cmdKey = string.Empty;
        do
        {
            if (cmdKey == null) { cmdKey = Console.ReadLine(); }
            switch (cmdKey.ToUpper())
            {
                case "GF":
                    {
                        cDNService.GetF();
                        cmdKey = string.Empty;
                        break;
                    }
                case "GB":
                    {
                        cDNService.GetB();
                        cmdKey = string.Empty;
                        break;
                    }
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
                case "DSF":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsFileFromStorage(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "DSB":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsBase64FromStorage(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "DPF":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsFileFromPullZone(filePath);

                                // Print the file name
                                Console.WriteLine($"https://m-s-z.b-cdn.net/{fileName}");
                            }
                        }
                        cmdKey = string.Empty;
                        break;
                    }
                case "DPB":
                    {
                        foreach (string dirPath in Directory.GetDirectories(@".\images"))
                        {
                            foreach (string filePath in Directory.GetFiles(@dirPath))
                            {
                                Console.WriteLine(filePath);

                                // Extract the file name from the full path
                                string fileName = Path.GetFileName(filePath);

                                cDNService.DownloadImageAsBase64FromPullZone(filePath);

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
                        Console.WriteLine("Enter GB to get all images from Bunny Storage Zone");
                        Console.WriteLine("Enter GF to get all images from Bunny Storage Zone");
                        Console.WriteLine("Enter U to upload all images to Bunny Storage Zone");
                        Console.WriteLine("Enter DL to delete image from Bunny Storage Zone");
                        Console.WriteLine("Enter DSF to download image from Bunny Storage Zone");
                        Console.WriteLine("Enter DSB to download image from Bunny Storage Zone");
                        Console.WriteLine("Enter DPF to download image from Bunny Pull Zone");
                        Console.WriteLine("Enter DPB to download image from Bunny Pull Zone");
                        Console.WriteLine("Enter DF to delete all images from Bunny Storage Zone");
                        Console.WriteLine("Enter E to Exit");
                        cmdKey = Console.ReadLine();
                        break;
                    }
            }

        } while (cmdKey != "E");
    }
}