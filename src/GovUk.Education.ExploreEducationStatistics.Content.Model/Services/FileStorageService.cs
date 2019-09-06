using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Common.Services;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Services.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using MimeTypes;
using FileInfo = GovUk.Education.ExploreEducationStatistics.Common.Model.FileInfo;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Services
{
    public class FileStorageService : IFileStorageService
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum FileSizeUnit : byte
        {
            B,
            Kb,
            Mb,
            Gb,
            Tb
        }

        private readonly string _storageConnectionString;

        private const string ContainerName = "downloads";

        public FileStorageService(IConfiguration config)
        {
            _storageConnectionString = config.GetConnectionString("PublicStorage");
        }

        public IEnumerable<FileInfo> ListPublicFiles(string publication, string release)
        {
            var files = new List<FileInfo>();
            
            files.AddRange(ListFiles(publication, release, ReleaseFileTypes.Data));
            files.AddRange(ListFiles(publication, release, ReleaseFileTypes.Ancillary));

            return files.OrderBy(f => f.Name);

        }

        public IEnumerable<FileInfo> ListFiles(string publication, string release, ReleaseFileTypes type)
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(ContainerName);
            blobContainer.CreateIfNotExists();
            
            var result = blobContainer.ListBlobs(FileStoragePathUtils.PublicReleaseDirectoryPath(publication, release, type), true, BlobListingDetails.Metadata)
                .OfType<CloudBlockBlob>()
                .Where(IsFileReleased)
                .Select(file => new FileInfo
                {
                    Extension = GetExtension(file),
                    Name = GetName(file),
                    Path = file.Name,
                    Size = GetSize(file)
                })
                .OrderBy(info => info.Name).ToList();
            return result;
        }

        private static bool IsFileReleased(CloudBlob blob)
        {
            if (!blob.Exists())
            {
                throw new ArgumentException("File not found: {filename}", blob.Name);
            }

            if (blob.Metadata.TryGetValue("releasedatetime", out var releaseDateTime))
            {
                return DateTime.Compare(ParseDateTime(releaseDateTime), DateTime.Now) <= 0;
            }

            return false;
        }

        private static string GetExtension(CloudBlob blob)
        {
            var contentType = blob.Properties.ContentType;
            contentType = contentType.Replace("; charset=utf-8", "");
            return MimeTypeMap.GetExtension(contentType).TrimStart('.');
        }

        private static string GetName(CloudBlob blob)
        {
            return blob.Metadata.TryGetValue("name", out var name) ? name : "Unknown file name";
        }

        private static string GetSize(CloudBlob blob)
        {
            var fileSize = blob.Properties.Length;
            var unit = FileSizeUnit.B;
            while (fileSize >= 1024 && unit < FileSizeUnit.Tb)
            {
                fileSize /= 1024;
                unit++;
            }

            return $"{fileSize:0.##} {unit}";
        }

        private static DateTime ParseDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "o", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}