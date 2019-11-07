using System.Collections.Generic;

namespace GovUk.Education.ExploreEducationStatistics.Data.Model
{
    public class FilterItem
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public FilterGroup FilterGroup { get; set; }
        public long FilterGroupId { get; set; }
        public ICollection<FilterItemFootnote> Footnotes { get; set; }

        public FilterItem()
        {
        }

        public FilterItem(string label, FilterGroup filterGroup)
        {
            Label = label;
            FilterGroup = filterGroup;
            FilterGroupId = filterGroup.Id;
        }
    }
}