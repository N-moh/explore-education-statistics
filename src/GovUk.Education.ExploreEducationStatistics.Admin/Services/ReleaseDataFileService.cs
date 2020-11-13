﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Extensions;
using GovUk.Education.ExploreEducationStatistics.Admin.Models;
using GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces.Security;
using GovUk.Education.ExploreEducationStatistics.Common.Extensions;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Common.Services;
using GovUk.Education.ExploreEducationStatistics.Common.Services.Interfaces;
using GovUk.Education.ExploreEducationStatistics.Common.Services.Interfaces.Security;
using GovUk.Education.ExploreEducationStatistics.Common.Utils;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Data.Model.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.FileStorageUtils;
using static GovUk.Education.ExploreEducationStatistics.Common.BlobContainerNames;
using static GovUk.Education.ExploreEducationStatistics.Common.Model.ReleaseFileTypes;
using static GovUk.Education.ExploreEducationStatistics.Common.Services.FileStoragePathUtils;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services
{
    public class ReleaseDataFileService : IReleaseDataFileService
    {
        private readonly ContentDbContext _contentDbContext;
        private readonly IPersistenceHelper<ContentDbContext> _persistenceHelper;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IDataArchiveValidationService _dataArchiveValidationService;
        private readonly IFileUploadsValidatorService _fileUploadsValidatorService;
        private readonly IFileRepository _fileRepository;
        private readonly IReleaseFileRepository _releaseFileRepository;
        private readonly IImportService _importService;
        private readonly IImportStatusService _importStatusService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;

        public ReleaseDataFileService(ContentDbContext contentDbContext,
            IPersistenceHelper<ContentDbContext> persistenceHelper,
            IBlobStorageService blobStorageService,
            IDataArchiveValidationService dataArchiveValidationService,
            IFileUploadsValidatorService fileUploadsValidatorService,
            IFileRepository fileRepository,
            IReleaseFileRepository releaseFileRepository,
            IImportService importService,
            IImportStatusService importStatusService,
            ISubjectService subjectService,
            IUserService userService)
        {
            _contentDbContext = contentDbContext;
            _persistenceHelper = persistenceHelper;
            _blobStorageService = blobStorageService;
            _dataArchiveValidationService = dataArchiveValidationService;
            _fileUploadsValidatorService = fileUploadsValidatorService;
            _fileRepository = fileRepository;
            _releaseFileRepository = releaseFileRepository;
            _importService = importService;
            _importStatusService = importStatusService;
            _subjectService = subjectService;
            _userService = userService;
        }

        public async Task<Either<ActionResult, Unit>> Delete(Guid releaseId,
            Guid id,
            bool forceDelete = false)
        {
            return await Delete(releaseId, new List<Guid>
            {
                id
            }, forceDelete: forceDelete);
        }

        public async Task<Either<ActionResult, Unit>> Delete(Guid releaseId,
            IEnumerable<Guid> ids,
            bool forceDelete = false)
        {
            return await _persistenceHelper
                .CheckEntityExists<Release>(releaseId)
                .OnSuccess(async release => await _userService.CheckCanUpdateRelease(release, ignoreCheck: forceDelete))
                .OnSuccess(async release =>
                    await ids.Select(id => _releaseFileRepository.CheckFileExists(releaseId, id, ReleaseFileTypes.Data))
                        .OnSuccessAll())
                .OnSuccessVoid(async files =>
                {
                    foreach (var file in files)
                    {
                        var metaReleaseFileReference =
                            await GetAssociatedReleaseFileReference(file, Metadata);

                        if (await _releaseFileRepository.FileIsLinkedToOtherReleases(releaseId, file.Id))
                        {
                            await _releaseFileRepository.Delete(releaseId, file.Id);
                            await _releaseFileRepository.Delete(releaseId, metaReleaseFileReference.Id);
                        }
                        else
                        {
                            await _importService.RemoveImportTableRowIfExists(releaseId, file.Filename);
                            await _blobStorageService.DeleteBlob(
                                PrivateFilesContainerName,
                                file.Path()
                            );
                            await _blobStorageService.DeleteBlob(
                                PrivateFilesContainerName,
                                metaReleaseFileReference.Path()
                            );

                            // If this is a replacement then unlink it from the original
                            if (file.ReplacingId.HasValue)
                            {
                                var originalFile = await _fileRepository.Get(file.ReplacingId.Value);
                                originalFile.ReplacedById = null;
                                _contentDbContext.Update(originalFile);
                                await _contentDbContext.SaveChangesAsync();
                            }

                            await _releaseFileRepository.Delete(releaseId, file.Id);
                            await _releaseFileRepository.Delete(releaseId, metaReleaseFileReference.Id);

                            await _fileRepository.Delete(file.Id);
                            await _fileRepository.Delete(metaReleaseFileReference.Id);

                            if (file.SourceId.HasValue)
                            {
                                var sourceRef = await _fileRepository.Get(file.SourceId.Value);
                                await _blobStorageService.DeleteBlob(
                                    PrivateFilesContainerName,
                                    // TODO can this be sourceRef.Path()?
                                    AdminReleasePath(releaseId, DataZip, sourceRef.Filename)
                                );
                                // N.B. No ReleaseFiles row for source links
                                await _fileRepository.Delete(sourceRef.Id);
                            }
                        }
                    }
                });
        }

        public async Task<Either<ActionResult, Unit>> DeleteAll(Guid releaseId, bool forceDelete = false)
        {
            var releaseFiles = await _releaseFileRepository.GetByFileType(releaseId, ReleaseFileTypes.Data);

            return await Delete(releaseId,
                releaseFiles.Select(releaseFile => releaseFile.ReleaseFileReference.Id),
                forceDelete: forceDelete);
        }

        public async Task<Either<ActionResult, DataFileInfo>> GetInfo(Guid releaseId, Guid id)
        {
            return await _persistenceHelper
                .CheckEntityExists<Release>(releaseId)
                .OnSuccess(_userService.CheckCanViewRelease)
                .OnSuccess(
                    async release => await _persistenceHelper
                        .CheckEntityExists<ReleaseFile>(
                            q => q.Include(rf => rf.ReleaseFileReference)
                                .Where(
                                    rf => rf.ReleaseId == release.Id
                                          && rf.ReleaseFileReference.ReleaseFileType == ReleaseFileTypes.Data
                                          && rf.ReleaseFileReferenceId == id
                                )
                        )
                )
                .OnSuccess(async file => await GetDataFileInfo(releaseId, file.ReleaseFileReference));
        }

        public async Task<Either<ActionResult, IEnumerable<DataFileInfo>>> ListAll(Guid releaseId)
        {
            return await _persistenceHelper
                .CheckEntityExists<Release>(releaseId)
                .OnSuccess(_userService.CheckCanViewRelease)
                .OnSuccess(async _ =>
                {
                    var fileList = new List<DataFileInfo>();
                    var files = await _releaseFileRepository.GetByFileType(releaseId, ReleaseFileTypes.Data);

                    // Exclude files that are replacements in progress
                    var filesExcludingReplacements = files.Where(file =>
                        !file.ReleaseFileReference.ReplacingId.HasValue);

                    await filesExcludingReplacements.ForEachAsync(async file =>
                        fileList.Add(await GetDataFileInfo(releaseId, file.ReleaseFileReference)));

                    return fileList
                        .OrderBy(file => file.Name)
                        .AsEnumerable();
                });
        }

        public async Task<Either<ActionResult, DataFileInfo>> Upload(Guid releaseId,
            IFormFile dataFile,
            IFormFile metaFile,
            string userName,
            Guid? replacingFileId = null,
            string subjectName = null)
        {
            return await _persistenceHelper
                .CheckEntityExists<Release>(releaseId)
                .OnSuccess(_userService.CheckCanUpdateRelease)
                .OnSuccess(async release =>
                {
                    return await _persistenceHelper.CheckOptionalEntityExists<ReleaseFileReference>(replacingFileId)
                        .OnSuccess(async replacingFile =>
                        {
                            return await ValidateSubjectName(releaseId, subjectName, replacingFile)
                                .OnSuccess(validSubjectName => _fileUploadsValidatorService
                                    .ValidateDataFilesForUpload(releaseId, dataFile, metaFile)
                                    // First, create with status uploading to prevent other users uploading the same datafile
                                    .OnSuccess(async () =>
                                        await _importService.CreateImportTableRow(releaseId,
                                            dataFile.FileName.ToLower()))
                                    .OnSuccess(async () =>
                                    {
                                        var fileReference = await _fileRepository.Create(
                                            releaseId: releaseId,
                                            filename: dataFile.FileName.ToLower(),
                                            type: ReleaseFileTypes.Data,
                                            replacingFile: replacingFile);

                                        var metaFileReference = await _fileRepository.Create(
                                            releaseId: releaseId,
                                            filename: metaFile.FileName.ToLower(),
                                            Metadata);

                                        var dataInfo = GetDataFileMetaValues(
                                            name: validSubjectName,
                                            metaFileName: metaFile.FileName,
                                            userName: userName,
                                            numberOfRows: CalculateNumberOfRows(dataFile.OpenReadStream())
                                        );
                                        var metaDataInfo = GetMetaDataFileMetaValues(
                                            dataFileName: dataFile.FileName,
                                            userName: userName,
                                            numberOfRows: CalculateNumberOfRows(metaFile.OpenReadStream())
                                        );

                                        await UploadFileToStorage(releaseId, dataFile, ReleaseFileTypes.Data, dataInfo);
                                        await UploadFileToStorage(releaseId, metaFile, Metadata,
                                            metaDataInfo);

                                        await _importService.Import(
                                            releaseId: releaseId,
                                            dataFileName: dataFile.FileName.ToLower(),
                                            metaFileName: metaFile.FileName.ToLower(),
                                            dataFile: dataFile,
                                            isZip: false);

                                        var blob = await _blobStorageService.GetBlob(
                                            PrivateFilesContainerName,
                                            fileReference.Path()
                                        );

                                        return new DataFileInfo
                                        {
                                            Id = fileReference.Id,
                                            Extension = blob.Extension,
                                            Name = blob.Name,
                                            Path = blob.Path,
                                            Size = blob.Size,
                                            MetaFileId = metaFileReference.Id,
                                            MetaFileName = blob.GetMetaFileName(),
                                            Rows = blob.GetNumberOfRows(),
                                            UserName = blob.GetUserName(),
                                            Status = IStatus.QUEUED,
                                            Created = blob.Created
                                        };
                                    }));
                        });
                });
        }

        public async Task<Either<ActionResult, DataFileInfo>> UploadAsZip(Guid releaseId,
            IFormFile zipFile,
            string userName,
            Guid? replacingFileId = null,
            string subjectName = null)
        {
            return await _persistenceHelper
                .CheckEntityExists<Release>(releaseId)
                .OnSuccess(_userService.CheckCanUpdateRelease)
                .OnSuccess(async release =>
                {
                    return await _persistenceHelper.CheckOptionalEntityExists<ReleaseFileReference>(replacingFileId)
                        .OnSuccess(async replacingFile =>
                        {
                            return await ValidateSubjectName(releaseId, subjectName, replacingFile)
                                .OnSuccess(validSubjectName =>
                                    _dataArchiveValidationService.ValidateDataArchiveFile(releaseId, zipFile)
                                        .OnSuccess(async archiveFile =>
                                        {
                                            var dataInfo = GetDataFileMetaValues(
                                                name: validSubjectName,
                                                metaFileName: archiveFile.MetaFileName,
                                                userName: userName,
                                                numberOfRows: 0
                                            );

                                            return await _fileUploadsValidatorService
                                                .ValidateDataArchiveEntriesForUpload(releaseId, archiveFile)
                                                .OnSuccess(async () =>
                                                    await _importService.CreateImportTableRow(
                                                        releaseId,
                                                        archiveFile.DataFileName))
                                                .OnSuccess(async () =>
                                                {
                                                    var source = await _fileRepository.CreateZip(
                                                        filename: zipFile.FileName.ToLower(),
                                                        releaseId: releaseId);

                                                    var dataFileReference = await _fileRepository.Create(
                                                        releaseId: releaseId,
                                                        filename: archiveFile.DataFileName,
                                                        type: ReleaseFileTypes.Data,
                                                        replacingFile: replacingFile,
                                                        source: source);

                                                    var metaFileReference = await _fileRepository.Create(
                                                        releaseId: releaseId,
                                                        filename: archiveFile.MetaFileName,
                                                        type: Metadata,
                                                        source: source);

                                                    await UploadFileToStorage(releaseId, zipFile,
                                                        DataZip,
                                                        dataInfo);

                                                    await _importService.Import(
                                                        releaseId: releaseId,
                                                        dataFileName: archiveFile.DataFileName,
                                                        metaFileName: archiveFile.MetaFileName,
                                                        dataFile: zipFile,
                                                        isZip: true);

                                                    var blob = await _blobStorageService.GetBlob(
                                                        PrivateFilesContainerName,
                                                        source.Path()
                                                    );

                                                    return new DataFileInfo
                                                    {
                                                        // TODO size and rows are for zip file but they need to be for
                                                        // the datafile which isn't extracted yet
                                                        Id = dataFileReference.Id,
                                                        Extension = dataFileReference.Extension,
                                                        Name = blob.Name,
                                                        Path = dataFileReference.Filename,
                                                        Size = blob.Size,
                                                        MetaFileId = metaFileReference.Id,
                                                        MetaFileName = blob.GetMetaFileName(),
                                                        Rows = blob.GetNumberOfRows(),
                                                        UserName = blob.GetUserName(),
                                                        Status = IStatus.QUEUED,
                                                        Created = blob.Created
                                                    };
                                                });
                                        }));
                        });
                });
        }

        private async Task<DataFileInfo> GetDataFileInfo(Guid releaseId, ReleaseFileReference dataFileReference)
        {
            // Files should exists in storage but if not then allow user to delete
            var blobExists =
                await _blobStorageService.CheckBlobExists(PrivateFilesContainerName, dataFileReference.Path());

            if (!blobExists)
            {
                return await GetFallbackDataFileInfo(releaseId, dataFileReference);
            }

            var blob = await _blobStorageService.GetBlob(PrivateFilesContainerName, dataFileReference.Path());

            // If the file does exist then it could possibly be
            // partially uploaded so make sure meta data exists for it
            if (string.IsNullOrEmpty(blob.GetUserName()))
            {
                return await GetFallbackDataFileInfo(releaseId, dataFileReference);
            }

            var metaFileReference =
                await GetAssociatedReleaseFileReference(dataFileReference, Metadata);

            var importStatus = await _importStatusService.GetImportStatus(dataFileReference.ReleaseId, blob.FileName);

            return new DataFileInfo
            {
                Id = dataFileReference.Id,
                Extension = blob.Extension,
                Name = blob.Name,
                Path = blob.Path,
                Size = blob.Size,
                MetaFileId = metaFileReference.Id,
                MetaFileName = blob.GetMetaFileName(),
                ReplacedBy = dataFileReference.ReplacedById,
                Rows = blob.GetNumberOfRows(),
                UserName = blob.GetUserName(),
                Status = importStatus.Status,
                Created = blob.Created
            };
        }

        private async Task<DataFileInfo> GetFallbackDataFileInfo(Guid releaseId, ReleaseFileReference file)
        {
            // Try to get the name from the zip file if existing
            if (file.SourceId != null)
            {
                var source = await _fileRepository.Get(file.SourceId.Value);

                if (await _blobStorageService.CheckBlobExists(PrivateFilesContainerName, source.Path()))
                {
                    var zipBlob = await _blobStorageService.GetBlob(PrivateFilesContainerName, source.Path());
                    var importStatus = await _importStatusService.GetImportStatus(releaseId, file.Filename);

                    return new DataFileInfo
                    {
                        Id = file.Id,
                        Extension = file.Extension,
                        Name = zipBlob.Name,
                        Path = file.Filename,
                        Size = zipBlob.Size,
                        MetaFileId = null,
                        MetaFileName =
                            file.ReleaseFileType == ReleaseFileTypes.Data
                                ? zipBlob.GetMetaFileName()
                                : string.Empty,
                        Rows = 0,
                        UserName = zipBlob.GetUserName(),
                        Status = importStatus.Status,
                        Created = zipBlob.Created
                    };
                }
            }

            var dataFileName = file.ReleaseFileType == ReleaseFileTypes.Data
                ? file.Filename
                : (await GetAssociatedReleaseFileReference(file, ReleaseFileTypes.Data)).Filename;

            // Fail the import if this was a datafile upload
            await _importService.FailImport(
                releaseId,
                dataFileName,
                new List<ValidationError>
                {
                    new ValidationError("Files not uploaded correctly. Please delete and retry")
                }.AsEnumerable()
            );

            var metaFileReference = file.ReleaseFileType == ReleaseFileTypes.Data
                ? await GetAssociatedReleaseFileReference(file, Metadata)
                : null;

            return new DataFileInfo
            {
                Id = file.Id,
                Name = await GetSubjectName(file),
                Extension = file.Extension,
                Path = file.Filename,
                Size = "0.00 B",
                MetaFileId = metaFileReference?.Id,
                MetaFileName = metaFileReference?.Filename ?? "",
                Rows = 0,
                UserName = "",
                Status = IStatus.NOT_FOUND
            };
        }

        private async Task<string> GetSubjectName(ReleaseFileReference file)
        {
            if (file.SubjectId.HasValue)
            {
                var subject = await _subjectService.Get(file.SubjectId.Value);
                return subject.Name;
            }

            return "Unknown";
        }

        private async Task<Either<ActionResult, string>> ValidateSubjectName(Guid releaseId,
            string subjectName, ReleaseFileReference replacingFile)
        {
            if (replacingFile == null)
            {
                return await _fileUploadsValidatorService.ValidateSubjectName(releaseId, subjectName)
                    .OnSuccess(async () => await Task.FromResult(subjectName));
            }

            return await GetSubjectName(replacingFile);
        }

        private async Task UploadFileToStorage(
            Guid releaseId,
            IFormFile file,
            ReleaseFileTypes type,
            IDictionary<string, string> metaValues)
        {
            await _blobStorageService.UploadFile(
                containerName: PrivateFilesContainerName,
                path: AdminReleasePath(releaseId, type, file.FileName.ToLower()),
                file: file,
                options: new IBlobStorageService.UploadFileOptions
                {
                    MetaValues = metaValues
                }
            );
        }

        private async Task<ReleaseFileReference> GetAssociatedReleaseFileReference(
            ReleaseFileReference releaseFileReference,
            ReleaseFileTypes associatedType)
        {
            return await _contentDbContext.ReleaseFileReferences
                .FirstAsync(rfr => rfr.ReleaseId == releaseFileReference.ReleaseId
                                   && rfr.ReleaseFileType == associatedType
                                   && rfr.SubjectId == releaseFileReference.SubjectId);
        }
    }
}