﻿#nullable enable
using GovUk.Education.ExploreEducationStatistics.Common.Model;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Extensions
{
    public static class ReleaseFileExtensions
    {
        public static string BatchesPath(this ReleaseFile releaseFile)
        {
            return releaseFile.File.BatchesPath();
        }

        public static string Path(this ReleaseFile releaseFile)
        {
            return releaseFile.File.Path();
        }

        public static string PublicPath(this ReleaseFile releaseFile)
        {
            return releaseFile.File.PublicPath(releaseFile.Release);
        }

        public static FileInfo ToPublicFileInfo(this ReleaseFile releaseFile, BlobInfo blobInfo)
        {
            return new FileInfo
            {
                Id = releaseFile.FileId,
                FileName = releaseFile.File.Filename,
                Name = releaseFile.Name ?? releaseFile.File.Filename,
                Size = blobInfo.Size,
                Type = releaseFile.File.Type,
            };
        }

        public static FileInfo ToFileInfo(this ReleaseFile releaseFile, BlobInfo blobInfo)
        {
            var info = releaseFile.ToPublicFileInfo(blobInfo);

            info.Created = releaseFile.File.Created;
            info.UserName = releaseFile.File.CreatedBy?.Email;

            return info;
        }

        // TODO: Remove after completion of EES-2343
        public static FileInfo ToFileInfoNotFound(this ReleaseFile releaseFile)
        {
            return new FileInfo
            {
                Id = releaseFile.FileId,
                FileName = releaseFile.File.Filename,
                Name = releaseFile.Name ?? releaseFile.File.Filename,
                Size = FileInfo.UnknownSize,
                Type = releaseFile.File.Type,
                UserName = releaseFile.File.CreatedBy?.Email,
                Created = releaseFile.File.Created
            };
        }
    }
}