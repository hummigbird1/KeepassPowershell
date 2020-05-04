namespace KeepassPSCmdlets
{
    public class KeepassFindEntryParameter
    {
        public string[] Group { get; set; }
        public FilterMode GroupFilterMode { get; set; }
        public bool GroupFilterOnPath { get; set; }
        public TagMatchMode TagMatchMode { get; set; }
        public string[] Tags { get; set; }
        public string[] Title { get; set; }
        public FilterMode TitleFilterMode { get; set; }
    }
}