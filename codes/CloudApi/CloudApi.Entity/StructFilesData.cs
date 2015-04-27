namespace FreeSun.FS_SMISCloud.Server.CloudApi.Entity
{
    public class StructFilesData
    {
        private SearchReturnData[] dayFiles;

        public SearchReturnData[] DayFiles
        {
            get { return dayFiles; }
            set { dayFiles = value; }
        }
        private SearchReturnData[] weekFiles;

        public SearchReturnData[] WeekFiles
        {
            get { return weekFiles; }
            set { weekFiles = value; }
        }
        private SearchReturnData[] monthFiles;

        public SearchReturnData[] MonthFiles
        {
            get { return monthFiles; }
            set { monthFiles = value; }
        }
        private SearchReturnData[] yearFiles;

        public SearchReturnData[] YearFiles
        {
            get { return yearFiles; }
            set { yearFiles = value; }
        }
    }
}