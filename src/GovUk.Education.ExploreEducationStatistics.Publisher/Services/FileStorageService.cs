using System;
using System.Globalization;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Common.Functions;
using GovUk.Education.ExploreEducationStatistics.Publisher.Model;
using GovUk.Education.ExploreEducationStatistics.Publisher.Services.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.FileStoragePathUtils;

namespace GovUk.Education.ExploreEducationStatistics.Publisher.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ILogger _logger;

        private readonly string _privateStorageConnectionString = ConnectionUtils.GetAzureStorageConnectionString("CoreStorage");
        private readonly string _publicStorageConnectionString = ConnectionUtils.GetAzureStorageConnectionString("PublicStorage");

        private const string PrivateContainerName = "releases";
        private const string PublicContainerName = "downloads";

        public FileStorageService(IConfiguration config,
            ILogger<FileStorageService> logger)
        {
            _logger = logger;
        }

        public async Task CopyReleaseToPublicContainer(PublishReleaseDataMessage message)
        {
            var privateStorageAccount = CloudStorageAccount.Parse(_privateStorageConnectionString);
            var publicStorageAccount = CloudStorageAccount.Parse(_publicStorageConnectionString);

            var privateBlobClient = privateStorageAccount.CreateCloudBlobClient();
            var publicBlobClient = publicStorageAccount.CreateCloudBlobClient();

            var privateContainer = privateBlobClient.GetContainerReference(PrivateContainerName);
            var publicContainer = publicBlobClient.GetContainerReference(PublicContainerName);


            var sourceDirectoryAddress = AdminReleaseDirectoryPath(message.ReleaseId);
            var destinationDirectoryAddress = PublicReleaseDirectoryPath(message.PublicationSlug, message.ReleaseSlug);
            
            await CopyDirectoryAsync(sourceDirectoryAddress, destinationDirectoryAddress, privateContainer,
                publicContainer, message.ReleasePublished);
        }

        private async Task CopyDirectoryAsync(string sourceDirectoryAddress, string destinationDirectoryAddress,
            CloudBlobContainer sourceContainer, CloudBlobContainer destinationContainer, DateTime releasePublished)
        {
            var sourceDirectory = sourceContainer.GetDirectoryReference(sourceDirectoryAddress);
            var destinationDirectory = destinationContainer.GetDirectoryReference(destinationDirectoryAddress);

            var options = new CopyDirectoryOptions
            {
                Recursive = true
            };

            var context = new DirectoryTransferContext();
            context.FileTransferred += FileTransferredCallback;
            context.FileFailed += FileFailedCallback;
            context.FileSkipped += FileSkippedCallback;
            context.ShouldTransferCallbackAsync += async (source, destination) =>
            {
                // We do not copy the metadata file.
                if (source is CloudBlockBlob blob)
                {
                    blob.FetchAttributes();
                    return !blob.Metadata.ContainsKey(DataFileKey); // Determine if this is a metadata file
                };
                return true;
            };

            context.SetAttributesCallbackAsync += async destination =>
            {
                var releasePublishedString = releasePublished.ToString("o", CultureInfo.InvariantCulture);
                (destination as CloudBlockBlob)?.Metadata.Add("releasedatetime", releasePublishedString);
            };

            await TransferManager.CopyDirectoryAsync(sourceDirectory, destinationDirectory, true, options, context);
        }

        private void FileTransferredCallback(object sender, TransferEventArgs e)
        {
            _logger.LogInformation("Transfer succeeds. {0} -> {1}.", e.Source, e.Destination);
        }

        private void FileFailedCallback(object sender, TransferEventArgs e)
        {
            _logger.LogInformation("Transfer fails. {0} -> {1}. Error message:{2}", e.Source, e.Destination,
                e.Exception.Message);
        }

        private void FileSkippedCallback(object sender, TransferEventArgs e)
        {
            _logger.LogInformation("Transfer skips. {0} -> {1}.", e.Source, e.Destination);
        }
    }
}