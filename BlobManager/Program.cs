using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
public class Program()
{
    private const string blobServiceEndpoint = "https://az204lab3storageaccount.blob.core.windows.net";
    private const string storageAccountName = "az204lab3storageaccount";
    private static string? storageAccountKey;

    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();

        storageAccountKey = configuration["StorageAccountKey"]; //Gets Secret

        StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        BlobServiceClient serviceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), accountCredentials);
        AccountInfo account = await serviceClient.GetAccountInfoAsync();

        await Console.Out.WriteLineAsync($"Connected to Azure Storage Account");
        await Console.Out.WriteLineAsync($"Account name:\t{storageAccountName}");
        await Console.Out.WriteLineAsync($"Account kind:\t{account?.AccountKind}");
        await Console.Out.WriteLineAsync($"Account sku:\t{account?.SkuName}");

        await EnumerateContainerAsync(serviceClient);

        var containers = serviceClient.GetBlobContainersAsync();  //Gets first container name
        var enumerator = containers.GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        string containerName = enumerator.Current.Name;
        var task = enumerator.DisposeAsync();

        await EnumerateBlobsAsync(serviceClient, containerName);

        string newContainerName = "az204lab3container3vector-graphics";
        BlobContainerClient vectorGraphicsContainer = await GetContainerAsync(serviceClient, newContainerName);
        string uploadedBlobName = "graph.svg";
        BlobClient blobClient = await GetBlobAsync(vectorGraphicsContainer, uploadedBlobName);
        await Console.Out.WriteLineAsync($"Blob Url:\t{blobClient.Uri}");

        await task;
    }

    private static async Task EnumerateContainerAsync(BlobServiceClient client)
    {
        await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
        {
            await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
        }
    }

    private static async Task EnumerateBlobsAsync(BlobServiceClient client, string containerName)
    {
        BlobContainerClient container = client.GetBlobContainerClient(containerName);

        await Console.Out.WriteLineAsync($"Searching:\t{container.Name}");

        await foreach (BlobItem blob in container.GetBlobsAsync())
        {
            await Console.Out.WriteLineAsync($"Existing Blob:\t{blob.Name}");
        }
    }

    private static async Task<BlobContainerClient> GetContainerAsync(BlobServiceClient client, string containerName)
    {
        BlobContainerClient container = client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
        await Console.Out.WriteLineAsync($"New Container:\t{container.Name}");
        return container;
    }

    private static async Task<BlobClient> GetBlobAsync(BlobContainerClient client, string blobName)
    {
        BlobClient blob = client.GetBlobClient(blobName);
        bool exists = await blob.ExistsAsync();
        if (!exists)
        {
            await Console.Out.WriteLineAsync($"Blob {blob.Name} not found!");

        }
        else
            await Console.Out.WriteLineAsync($"Blob Found, URI:\t{blob.Uri}");
        return blob;
    }
}
