namespace SharedLibrary.SeriLogging.Configurations;

public class SerilogConfiguration
{
    public class FileSinkOptions
    {
        public string FilePath { get; set; } = "Logs/log-.txt";
        public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        public int RetainedFileCountLimit { get; set; }
        
    }
}