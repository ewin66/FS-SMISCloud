namespace FreeSun.FS_SMISCloud.Server.CloudApi.Log
{
    using System;

    public class LogInfoAttribute : Attribute
    {
        public LogInfoAttribute(string description, bool isVisible)
        {
            this.Description = description;
            this.IsVisible = isVisible;
        }

        public string Description { get; set; }

        public bool IsVisible { get; set; }
    }
}