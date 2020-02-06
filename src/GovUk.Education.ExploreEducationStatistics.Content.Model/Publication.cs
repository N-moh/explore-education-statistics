using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GovUk.Education.ExploreEducationStatistics.Content.Model
{
    public class Publication
    {
        public Guid Id { get; set; }

        public string Slug { get; set; }

        [Required] public string Title { get; set; }

        public string Description { get; set; }

        public string DataSource { get; set; }

        public string Summary { get; set; }

        public DateTime? NextUpdate { get; set; }

        public List<Release> Releases { get; set; }

        public Guid? MethodologyId { get; set; }
        
        public Methodology Methodology { get; set; }

        public Uri MethodologyUrl { get; set; }

        public Uri LegacyPublicationUrl { get; set; }

        public List<Link> LegacyReleases { get; set; }

        public Guid TopicId { get; set; }

        public Topic Topic { get; set; }
        
        public Guid? ContactId { get; set; }

        public Contact Contact { get; set; }
        
        public Release LatestRelease()
        {
            var latest = Releases?.Where(r => r.Published != null)
                .OrderBy(r => r.Order)
                .ThenBy(r => r.Published)
                .LastOrDefault();
            return latest;
        }
    }
}