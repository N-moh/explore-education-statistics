﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using GovUk.Education.ExploreEducationStatistics.Content.Api.ViewModels;
using GovUk.Education.ExploreEducationStatistics.Publisher.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ExploreEducationStatistics.Content.Api.Services.Interfaces
{
    public interface IMethodologyService
    {
        public Task<Either<ActionResult, MethodologyViewModel>> GetLatestMethodologyBySlug(string slug);

        public Task<Either<ActionResult, List<MethodologySummaryViewModel>>> GetSummariesByPublication(Guid publicationId);

        public Task<Either<ActionResult, List<ThemeTree<PublicationMethodologiesTreeNode>>>> GetTree();
    }
}
