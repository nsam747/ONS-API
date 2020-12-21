namespace Acre.Backend.Ons.Models.Configurations
{
    public class DatasetConfig
    {
        public string FolderName { get; set; }
        public OnsConfig[] Ons { get; set; }
        public string CaseFileName { get; set; }
        public string CaseFilePath => $"{FolderName}/{CaseFileName}";
    }

    public class OnsConfig {
        public string FileName { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}