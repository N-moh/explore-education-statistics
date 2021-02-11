using System;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.ViewModels;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces
{
    public interface IDataImportService
    {
        Task<DataImportStatus> GetStatus(Guid fileId);

        Task<Either<ActionResult, Unit>> CancelImport(Guid releaseId, Guid fileId);

        Task DeleteImport(Guid fileId);

        Task<bool> HasIncompleteImports(Guid releaseId);

        Task<DataImportViewModel> GetImport(Guid fileId);

        Task Import(Guid subjectId, File dataFile, File metaFile, IFormFile formFile);

        Task ImportZip(Guid subjectId, File dataFile, File metaFile, File zipFile);
    }
}
