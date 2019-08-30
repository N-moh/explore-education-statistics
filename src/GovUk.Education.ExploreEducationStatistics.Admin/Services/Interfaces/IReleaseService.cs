﻿using GovUk.Education.ExploreEducationStatistics.Content.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Models;
using GovUk.Education.ExploreEducationStatistics.Admin.Models.Api;
using ReleaseId = System.Guid;
using PublicationId  = System.Guid;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces
{
    public interface IReleaseService
    {
        List<Release> List();

        Release Get(Guid id);
        
        Task<Release> GetAsync(Guid id);

        Release Get(string slug);

        Task<Either<ValidationResult, ReleaseViewModel>> CreateReleaseAsync(CreateReleaseViewModel release);

        Task<ReleaseViewModel> GetReleaseForIdAsync(ReleaseId id);
        
        Task<ReleaseSummaryViewModel> GetReleaseSummaryAsync(ReleaseId releaseId);
        
        Task<Either<ValidationResult, ReleaseViewModel>> EditReleaseSummaryAsync(ReleaseSummaryViewModel model);

        Task<List<ReleaseViewModel>> GetReleasesForPublicationAsync(PublicationId publicationId);

        Task<List<ReleaseViewModel>> GetReleasesForReleaseStatusesAsync(params ReleaseStatus[] releaseStatuses);

        Task<Either<ValidationResult, ReleaseSummaryViewModel>> UpdateReleaseStatusAsync(ReleaseId releaseId, ReleaseStatus status, string internalReleaseNote);
    }
}
