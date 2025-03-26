# Azure-AZ204-Lab3
Azure AZ 204 Lab 03: Retrieve Azure Storage resources and metadata by using the Azure Storage SDK for .NET

## Instructions

### 1. Setup Azure Storage

To create the storage account the following commands where used.

```bash
tenant=<tenant-id>
az login --tenant $tenant
resource-group-name=<resource-group-name>
az group create --location <location name> --name $resource-group-name
storage-account-name=<storage-account-name>
az storage account create --name $storage-account-name  --resource-group <resource group name> --sku Standard_LRS
az storage account show-connection-string --name $storage-account-name -g <resource group name>
connection-string=<connection-string>
container-name=<container-name>
az storage container create --name $container-name --connection-string $connection-string --resource-group $resource-group-name
az storage blob upload --connection-string $connection-string --container-name $container-name --file <file path>
```

When the account is created, one can specify the following:

```
[--access-tier {Cold, Cool, Hot, Premium}]
[--kind {BlobStorage, BlockBlobStorage, FileStorage, Storage, StorageV2}]
[--public-network-access {Disabled, Enabled, SecuredByPerimeter}]
```

