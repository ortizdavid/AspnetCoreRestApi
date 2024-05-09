namespace AspNetCoreRestApi.Helpers
{
    public class FileExtensions
    {
        public static readonly string[] Images = {".jpg", ".jpeg", ".png", ".gif"}; 
        public static readonly string[] Documents = {".txt", ".pdf", ".docx", ".ppt", ".pptx", ".xls", ".xlsx"};
        public static readonly string[] CsvTxts = {".txt", ".csv"};
        public static readonly string[] Archives = {".zip", ".rar", ".7z", ".tar", ".gz"};  
    }
}