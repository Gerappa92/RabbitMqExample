namespace ProcessingService;

public class QueueOptions
{
    public const string Location = "QueueOptions";
    public string QueueName { get; set; }
    public string DataCaptureFolderPath { get; set; }
    public string ProcessDataFolderPath { get; set; }
    public string FileExtension { get; set; }
}
