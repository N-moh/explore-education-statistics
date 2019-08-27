using System;
using GovUk.Education.ExploreEducationStatistics.Data.Model.Query;
using GovUk.Education.ExploreEducationStatistics.Data.Services.ViewModels;

namespace GovUk.Education.ExploreEducationStatistics.Data.Api.ViewModels
{
    public class PermalinkViewModel
    {
        public Guid Id { get; set; }

        public TableBuilderConfiguration Configuration { get; set; }

        public DateTime Created { get; set; }

        public TableBuilderResultViewModel FullTable { get; set; }
    }
}