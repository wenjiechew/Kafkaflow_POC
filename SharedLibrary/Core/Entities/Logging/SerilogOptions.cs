namespace SharedLibrary.Core.Entities.Logging;

public class SerilogOptions
{
    public class FileSinkOptions
    {
        public string FilePath { get; set; }
        public string OutputTemplate { get; set; }
        public int RetainedFileCountLimit { get; set; }
        
    }
}