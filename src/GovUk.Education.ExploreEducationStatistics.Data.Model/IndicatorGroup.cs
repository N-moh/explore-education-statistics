using System.Collections.Generic;

namespace GovUk.Education.ExploreEducationStatistics.Data.Model
{
    public class IndicatorGroup
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public Subject Subject { get; set; }
        public long SubjectId { get; set; }
        public ICollection<Indicator> Indicators { get; set; }

        public IndicatorGroup()
        {
        }

        public IndicatorGroup(string label, Subject subject)
        {
            Label = label;
            Subject = subject;
            Indicators = new List<Indicator>();
        }
    }
}