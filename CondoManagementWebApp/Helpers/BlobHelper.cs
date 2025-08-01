using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CondoManagementWebApp.Helpers
{
    public class BlobHelper
    {
        private readonly CloudBlobClient _blobClient;  //isto que me liga ao contentor

        public BlobHelper(IConfiguration configuration)
        {
            //TODO : criar blob no azure e botar connection string
            string keys = configuration["Blob:ConnectionString"]; //usado para buscar dados no appsettings

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);  //manda o connection string para se ligar ao container

            _blobClient = storageAccount.CreateCloudBlobClient(); //ligação ao container
        }

        ////mesmos metodos de upload só variam os parâmetreos: ficheiro, array de bytes e string
        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();//busca ficheiro
            return await UploadStreamAsync(stream, containerName); //manda ficheiro e nome do container onde será feito upload 
        }


        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file); //memory stream usado para bytes
            return await UploadStreamAsync(stream, containerName);
        }


        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }


        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();

            CloudBlobContainer container = _blobClient.GetContainerReference(containerName); //ligação ao blob usando o nome do container

            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}"); //passa o nome gerado pelo guid

            await blockBlob.UploadFromStreamAsync(stream); //faz upload

            return name; //retiorna nome gerado pelo guid
        }
    }
}
