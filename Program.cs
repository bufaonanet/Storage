﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

const string blobServiceEndpoint = "";
const string storageAccountName = "";
const string storageAccountKey = "";

var accountCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);

var serviceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), accountCredentials);

AccountInfo info = await serviceClient.GetAccountInfoAsync();

await Console.Out.WriteLineAsync($"Connected to Azure Storage Account");
await Console.Out.WriteLineAsync($"Account name:\t{storageAccountName}");
await Console.Out.WriteLineAsync($"Account kind:\t{info?.AccountKind}");
await Console.Out.WriteLineAsync($"Account sku:\t{info?.SkuName}");

await EnumerateContainersAsync(serviceClient);

string existingContainerName = "raster-graphics";
await EnumerateBlobsAsync(serviceClient, existingContainerName);

string newContainerName = "vector-graphics";
BlobContainerClient containerClient = await GetContainerAsync(serviceClient, newContainerName);

string uploadedBlobName = "graph.svg";
BlobClient blobClient = await GetBlobAsync(containerClient, uploadedBlobName);

static async Task EnumerateContainersAsync(BlobServiceClient client)
{
    await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
    {
        await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
    }
}

static async Task EnumerateBlobsAsync(BlobServiceClient client, string containerName)
{
    BlobContainerClient container = client.GetBlobContainerClient(containerName);

    await Console.Out.WriteLineAsync($"Searching:\t{container.Name}");

    await foreach (BlobItem blob in container.GetBlobsAsync())
    {
        await Console.Out.WriteLineAsync($"Existing Blob:\t{blob.Name}");
    }
}

static async Task<BlobContainerClient> GetContainerAsync(BlobServiceClient client, string containerName)
{
    BlobContainerClient container = client.GetBlobContainerClient(containerName);

    await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

    await Console.Out.WriteLineAsync($"New Container:\t{container.Name}");

    return container;
}

static async Task<BlobClient> GetBlobAsync(BlobContainerClient client, string blobName)
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