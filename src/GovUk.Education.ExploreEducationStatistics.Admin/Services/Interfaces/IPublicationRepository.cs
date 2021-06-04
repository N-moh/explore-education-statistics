using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ExploreEducationStatistics.Admin.ViewModels;

namespace GovUk.Education.ExploreEducationStatistics.Admin.Services.Interfaces
{
    public interface IPublicationRepository
    {
        Task<List<MyPublicationViewModel>> GetAllPublicationsForTopicAsync(Guid topicId);

        Task<List<MyPublicationViewModel>> GetPublicationsForTopicRelatedToUser(Guid topicId, Guid userId);

        Task<MyPublicationViewModel> GetPublicationForUser(Guid publicationId, Guid userId);

        Task<MyPublicationViewModel> GetPublicationWithAllReleases(Guid publicationId);
    }
}
