using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Content.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Data.Processor.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static GovUk.Education.ExploreEducationStatistics.Content.Model.DataImportStatus;
using static GovUk.Education.ExploreEducationStatistics.Data.Processor.Utils.DbUtils;

namespace GovUk.Education.ExploreEducationStatistics.Data.Processor.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly DbContextOptions<ContentDbContext> _contentDbContextOptions;
        private readonly ILogger<DataImportService> _logger;

        public DataImportService(
            DbContextOptions<ContentDbContext> contentDbContextOptions,
            ILogger<DataImportService> logger)
        {
            _contentDbContextOptions = contentDbContextOptions;
            _logger = logger;
        }

        public async Task FailImport(Guid id, List<DataImportError> errors)
        {
            await using var contentDbContext = new ContentDbContext(_contentDbContextOptions);
            var import = await contentDbContext.DataImports.FindAsync(id);
            if (import.Status != COMPLETE && import.Status != FAILED)
            {
                contentDbContext.Update(import);
                import.Status = FAILED;
                import.Errors.AddRange(errors);
                await contentDbContext.SaveChangesAsync();
            }
        }

        public async Task FailImport(Guid id, params string[] errors)
        {
            await FailImport(id, errors.Select(error => new DataImportError(error)).ToList());
        }

        public async Task<DataImport> GetImport(Guid id)
        {
            await using var contentDbContext = new ContentDbContext(_contentDbContextOptions);
            return await contentDbContext.DataImports
                .AsNoTracking()
                .Include(import => import.Errors)
                .Include(import => import.File)
                .Include(import => import.MetaFile)
                .Include(import => import.ZipFile)
                .SingleAsync(import => import.Id == id);
        }

        public async Task<DataImportStatus> GetImportStatus(Guid id)
        {
            await using var contentDbContext = new ContentDbContext(_contentDbContextOptions);
            var import = await contentDbContext.DataImports
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == id);

            if (import == null)
            {
                return NOT_FOUND;
            }

            return import.Status;
        }

        public async Task Update(Guid id, int rowsPerBatch, int totalRows, int numBatches)
        {
            await using var contentDbContext = new ContentDbContext(_contentDbContextOptions);
            var import = await contentDbContext.DataImports.FindAsync(id);
            contentDbContext.Update(import);

            import.RowsPerBatch = rowsPerBatch;
            import.TotalRows = totalRows;
            import.NumBatches = numBatches;

            await contentDbContext.SaveChangesAsync();
        }

        public async Task UpdateStatus(Guid id, DataImportStatus newStatus, double percentageComplete)
        {
            await using var contentDbContext = new ContentDbContext(_contentDbContextOptions);
            await ExecuteWithExclusiveLock(
                contentDbContext,
                $"LockForImport-{id}",
                async context =>
                {
                    var import = await context.DataImports
                        .Include(i => i.File)
                        .SingleAsync(i => i.Id == id);

                    var filename = import.File.Filename;

                    var percentageCompleteBefore = import.StagePercentageComplete;
                    var percentageCompleteAfter = (int) Math.Clamp(percentageComplete, 0, 100);

                    // Ignore updating if already finished
                    if (import.Status.IsFinished())
                    {
                        _logger.LogWarning(
                            $"Update: {filename} {import.Status} ({percentageCompleteBefore}%) -> " +
                            $"{newStatus} ({percentageCompleteAfter}%) ignored as this import is already finished");
                        return;
                    }

                    // Ignore updating if already aborting and the new state is not aborting or finishing
                    if (import.Status.IsAborting() && !newStatus.IsFinishedOrAborting())
                    {
                        _logger.LogWarning(
                            $"Update: {filename} {import.Status} ({percentageCompleteBefore}%) -> " +
                            $"{newStatus} ({percentageCompleteAfter}%) ignored as this import is already aborting or is finished");
                        return;
                    }

                    // Ignore updates if attempting to downgrade from a normal importing state to a lower normal importing state,
                    // or if the percentage is being set lower or the same as is currently and is the same state
                    if (!newStatus.IsFinishedOrAborting() &&
                        (import.Status.CompareTo(newStatus) > 0 ||
                         import.Status == newStatus && percentageCompleteBefore > percentageCompleteAfter))
                    {
                        _logger.LogWarning(
                            $"Update: {filename} {import.Status} ({percentageCompleteBefore}%) -> " +
                            $"{newStatus} ({percentageCompleteAfter}%) ignored");
                        return;
                    }

                    // Ignore updating to an equal percentage complete (after rounding) at the same status without logging it
                    if (import.Status == newStatus && percentageCompleteBefore == percentageCompleteAfter)
                    {
                        return;
                    }

                    _logger.LogInformation(
                        $"Update: {filename} {import.Status} ({percentageCompleteBefore}%) -> {newStatus} ({percentageCompleteAfter}%)");

                    import.StagePercentageComplete = percentageCompleteAfter;
                    import.Status = newStatus;
                    context.DataImports.Update(import);
                    await context.SaveChangesAsync();
                }
            );
        }
    }
}