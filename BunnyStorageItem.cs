using System;

public class BunnyStorageItem
{
    public Guid Guid { get; set; }
    public string StorageZoneName { get; set; }
    public string Path { get; set; }
    public string ObjectName { get; set; }
    public long Length { get; set; }
    public DateTime LastChanged { get; set; }
    public int ServerId { get; set; }
    public int ArrayNumber { get; set; }
    public bool IsDirectory { get; set; }
    public Guid UserId { get; set; }
    public string ContentType { get; set; }
    public DateTime DateCreated { get; set; }
    public int StorageZoneId { get; set; }
    public string Checksum { get; set; }
    public string ReplicatedZones { get; set; }
}