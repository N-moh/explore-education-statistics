namespace GovUk.Education.ExploreEducationStatistics.Content.Api.Models
{
    public class FileInfo
    {
        public string Extension { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        
        public int Rows { get; set; }
    }
}