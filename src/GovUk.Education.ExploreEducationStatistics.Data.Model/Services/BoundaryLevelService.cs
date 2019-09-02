using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ExploreEducationStatistics.Data.Model.Database;
using GovUk.Education.ExploreEducationStatistics.Data.Model.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ExploreEducationStatistics.Data.Model.Services
{
    public class BoundaryLevelService : AbstractRepository<BoundaryLevel, long>, IBoundaryLevelService
    {
        public BoundaryLevelService(StatisticsDbContext context, ILogger<BoundaryLevelService> logger)
            : base(context, logger)
        {
        }

        private IEnumerable<BoundaryLevel> FindByGeographicLevel(GeographicLevel geographicLevel)
        {
            return FindMany(level => level.Level.Equals(geographicLevel))
                .OrderByDescending(level => level.Published);
        }

        public IEnumerable<BoundaryLevel> FindByGeographicLevels(IEnumerable<GeographicLevel> geographicLevels)
        {
            return FindMany(level => geographicLevels.Contains(level.Level))
                .OrderByDescending(level => level.Published);
        }

        public BoundaryLevel FindLatestByGeographicLevel(GeographicLevel geographicLevel)
        {
            return FindMany(level => level.Level.Equals(geographicLevel))
                .OrderByDescending(level => level.Published)
                .First();
        }

        public IEnumerable<BoundaryLevel> FindRelatedByBoundaryLevel(long boundaryLevelId)
        {
            var boundaryLevel = Find(boundaryLevelId);
            if (boundaryLevel == null)
            {
                throw new ArgumentException("Boundary Level does not exist", nameof(boundaryLevelId));
            }

            return FindByGeographicLevel(boundaryLevel.Level);
        }
    }
}