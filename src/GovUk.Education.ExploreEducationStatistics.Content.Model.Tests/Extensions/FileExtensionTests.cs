﻿using System;
using System.Collections.Generic;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Extensions;
using Xunit;
using static GovUk.Education.ExploreEducationStatistics.Common.Model.BlobInfo;
using static GovUk.Education.ExploreEducationStatistics.Common.Model.FileType;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.FileStoragePathUtils;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model.Tests.Extensions
{
    public class FileExtensionTests
    {
        private const string PublicationSlug = "publication-slug";
        private const string ReleaseSlug = "release-slug";

        private readonly File _file = new File
        {
            Id = Guid.NewGuid(),
            RootPath = Guid.NewGuid(),
            Filename = "ancillary.pdf",
            Type = Ancillary
        };

        [Fact]
        public void Batches_ReleaseFile()
        {
            var releaseFile = new ReleaseFile
            {
                File = new File
                {
                    Id = Guid.NewGuid(),
                    RootPath = Guid.NewGuid(),
                    Filename = "data.csv",
                    Type = Data
                }
            };
 
            Assert.Equal(releaseFile.File.BatchesPath(), releaseFile.BatchesPath());
        }

        [Fact]
        public void BatchesPath()
        {
            var dataFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "data.csv",
                Type = Data
            };

            Assert.Equal($"{dataFile.RootPath}/data/batches/{dataFile.Id}/", dataFile.BatchesPath());
        }

        [Fact]
        public void BatchPath()
        {
            var dataFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "data.csv",
                Type = Data
            };

            Assert.Equal($"{dataFile.RootPath}/data/batches/{dataFile.Id}/data.csv_000999", dataFile.BatchPath(999));
        }

        [Fact]
        public void Path_ReleaseFile()
        {
            var releaseFile = new ReleaseFile
            {
                File = _file
            };

            Assert.Equal(_file.Path(), releaseFile.Path());
        }

        [Fact]
        public void Path()
        {
            var ancillaryFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "ancillary.pdf",
                Type = Ancillary
            };

            var chartFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "chart.png",
                Type = Chart
            };

            var dataFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "data.csv",
                Type = Data
            };

            var imageFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "image.png",
                Type = Image
            };

            var metaFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "data.meta.csv",
                Type = Metadata
            };

            var zipFile = new File
            {
                Id = Guid.NewGuid(),
                RootPath = Guid.NewGuid(),
                Filename = "data.zip",
                Type = DataZip
            };

            Assert.Equal($"{ancillaryFile.RootPath}/ancillary/{ancillaryFile.BlobStorageName}", ancillaryFile.Path());
            Assert.Equal($"{chartFile.RootPath}/chart/{chartFile.BlobStorageName}", chartFile.Path());
            Assert.Equal($"{dataFile.RootPath}/data/{dataFile.BlobStorageName}", dataFile.Path());
            Assert.Equal($"{imageFile.RootPath}/image/{imageFile.BlobStorageName}", imageFile.Path());
            Assert.Equal($"{metaFile.RootPath}/data/{metaFile.BlobStorageName}", metaFile.Path());
            Assert.Equal($"{zipFile.RootPath}/zip/{zipFile.BlobStorageName}", zipFile.Path());
        }

        [Fact]
        public void PublicPath_ReleaseFile()
        {
            var release = new Release
            {
                Publication = new Publication
                {
                    Slug = PublicationSlug
                },
                Slug = ReleaseSlug
            };

            var releaseFile = new ReleaseFile
            {
                Release = release,
                File = _file
            };

            Assert.Equal(_file.PublicPath(release), releaseFile.PublicPath());
        }

        [Fact]
        public void PublicPath()
        {
            var release = new Release
            {
                Publication = new Publication
                {
                    Slug = PublicationSlug
                },
                Slug = ReleaseSlug
            };
            
            Assert.Equal(PublicReleasePath(PublicationSlug, ReleaseSlug, _file.Type, _file.BlobStorageName),
                _file.PublicPath(release));
        }

        [Fact]
        public void ToFileInfo()
        {
            var result = _file.ToFileInfo(new BlobInfo
            (
                path: "Ignored",
                size: "400 B",
                contentType: "Ignored",
                contentLength: -1L,
                meta: new Dictionary<string, string>
                {
                    {FilenameKey, "Ignored"},
                    {NameKey, "Test ancillary file"}
                }
            ));

            Assert.Equal(_file.Id, result.Id);
            Assert.Equal("pdf", result.Extension);
            Assert.Equal("ancillary.pdf", result.FileName);
            Assert.Equal("Test ancillary file", result.Name);
            Assert.Equal(_file.Path(), result.Path);
            Assert.Equal("400 B", result.Size);
            Assert.Equal(Ancillary, result.Type);
        }

        [Fact]
        // TODO EES-1815 Remove this when the name is changed to use the database Subject name
        public void ToFileInfo_BlobWithNoNameKey()
        {
            // Name is retrieved from the blob meta properties due to EES-1637 (Subject didn't exist early on and Name on Data files is Subject name persisted on blob)
            // Chart files don't have any meta properties added to avoid duplicating data unnecessarily since nothing requires the filename or name of them
            // Nevertheless test that we populate the name from the database File as a fallback when the blob name property is missing

            var result = _file.ToFileInfo(new BlobInfo
            (
                path: "Ignored",
                size: "400 B",
                contentType: "Ignored",
                contentLength: -1L,
                meta: new Dictionary<string, string>()
            ));

            Assert.Equal(_file.Id, result.Id);
            Assert.Equal("pdf", result.Extension);
            Assert.Equal("ancillary.pdf", result.FileName);
            Assert.Equal("ancillary.pdf", result.Name);
            Assert.Equal(_file.Path(), result.Path);
            Assert.Equal("400 B", result.Size);
            Assert.Equal(Ancillary, result.Type);
        }

        [Fact]
        public void ToFileInfoNotFound()
        {
            var result = _file.ToFileInfoNotFound();

            Assert.Equal(_file.Id, result.Id);
            Assert.Equal("pdf", result.Extension);
            Assert.Equal("ancillary.pdf", result.FileName);
            Assert.Equal("Unknown", result.Name);
            Assert.Null(result.Path);
            Assert.Equal("0.00 B", result.Size);
            Assert.Equal(Ancillary, result.Type);
        }

        [Fact]
        public void ToPublicFileInfo()
        {
            var release = new Release
            {
                Publication = new Publication
                {
                    Slug = PublicationSlug
                },
                Slug = ReleaseSlug
            };

            var result = _file.ToPublicFileInfo(new BlobInfo
            (
                path: _file.PublicPath(release),
                size: "400 B",
                contentType: "Ignored",
                contentLength: -1L,
                meta: new Dictionary<string, string>
                {
                    {FilenameKey, "Ignored"},
                    {NameKey, "Test ancillary file"}
                }
            ));

            Assert.Equal(_file.Id, result.Id);
            Assert.Equal("pdf", result.Extension);
            Assert.Equal("ancillary.pdf", result.FileName);
            Assert.Equal("Test ancillary file", result.Name);
            Assert.Equal(_file.PublicPath(release), result.Path);
            Assert.Equal("400 B", result.Size);
            Assert.Equal(Ancillary, result.Type);
        }
    }
}