using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=166174sawus02;AccountKey=wWdK71LCwN+elxaApObKU6CMfPN+MiMtEOxXvIKPHoWGR8xCUTFeelpCTucCT6g/W6J8b1jQrCNU+AStEvXOCQ==;EndpointSuffix=core.windows.net";
        string containerName = "blob";
        string blobName = "myFile";
        string localFilePath = "D:/myText.txt";

        // Initialize a BlobServiceClient using the connection string
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        // Create a BlobContainerClient
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Upload a blob to the container
        BlobClient blobClient = containerClient.GetBlobClient(blobName);
        await UploadBlobAsync(blobClient, "Hello, Azure Blob!");

        // Download and save the blob locally
        await DownloadBlobAsync(blobClient, localFilePath);

        // Get blob metadata
        BlobProperties properties = await GetBlobMetadataAsync(blobClient);

        // Take a snapshot of the blob
        BlobSnapshotInfo snapshotInfo = await CreateBlobSnapshotAsync(blobClient);

        // Delete the blob
        await DeleteBlobAsync(blobClient);

        // Delete the container (optional)
        // await DeleteContainerAsync(containerClient);

        Console.WriteLine("Operations completed successfully.");
    }

    static async Task UploadBlobAsync(BlobClient blobClient, string content)
    {
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
        {
            await blobClient.UploadAsync(stream, true);
        }
    }

    static async Task DownloadBlobAsync(BlobClient blobClient, string localFilePath)
    {
        BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();
        using (FileStream fs = File.OpenWrite(localFilePath))
        {
            await blobDownloadInfo.Content.CopyToAsync(fs);
            fs.Close();
        }
    }

    static async Task<BlobProperties> GetBlobMetadataAsync(BlobClient blobClient)
    {
        BlobProperties properties = await blobClient.GetPropertiesAsync();
        Console.WriteLine($"Content Type: {properties.ContentType}");
        Console.WriteLine($"Content Length: {properties.ContentLength}");
        // Add more metadata properties as needed
        return properties;
    }

    static async Task<BlobSnapshotInfo> CreateBlobSnapshotAsync(BlobClient blobClient)
    {
        BlobSnapshotInfo snapshotInfo = await blobClient.CreateSnapshotAsync();
        Console.WriteLine($"Snapshot ID: {snapshotInfo.Snapshot}");
        return snapshotInfo;
    }

    static async Task DeleteBlobAsync(BlobClient blobClient)
    {
        await blobClient.DeleteAsync();
        Console.WriteLine("Blob deleted successfully.");
    }

    static async Task DeleteContainerAsync(BlobContainerClient containerClient)
    {
        await containerClient.DeleteAsync();
        Console.WriteLine("Container deleted successfully.");
    }
}
