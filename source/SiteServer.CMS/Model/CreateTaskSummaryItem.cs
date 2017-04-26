namespace SiteServer.CMS.Model
{
    public class CreateTaskSummaryItem
    {
        public CreateTaskSummaryItem(string type, string name, string timeSpan, bool isOver, bool isSuccess, string errorMessage)
        {
            this.type = type;
            this.name = name;
            this.timeSpan = timeSpan;
            this.isOver = isOver;
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
        }

        public string type { get; set; }
        public string name { get; set; }
        public string timeSpan { get; set; }
        public bool isOver { get; set; }
        public bool isSuccess { get; set; }
        public string errorMessage { get; set; }
    }
}
