﻿using GovUk.Education.ExploreEducationStatistics.Content.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.Models.Api;
using GovUk.Education.ExploreEducationStatistics.Common.Model;
using UserId = System.Guid;
using TopicId = System.Guid;
using PublicationId = System.Guid;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces
{
    public interface IPublicationService
    {
        List<Publication> List();

        Task<Publication> GetAsync(Guid id);

        Publication Get(string slug);

        Task<List<PublicationViewModel>> GetByTopicAndUserAsync(TopicId topicId, UserId userId);
        Task<Either<ValidationResult, PublicationViewModel>> CreatePublicationAsync(CreatePublicationViewModel publication);
        
        Task<PublicationViewModel> GetViewModelAsync(PublicationId publicationId);
        
    }
}
