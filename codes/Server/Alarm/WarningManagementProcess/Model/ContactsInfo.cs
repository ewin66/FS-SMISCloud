namespace FS.SMIS_Cloud.Alarm.Forwarder.Model
{
    public enum ContactType
    {
        Support = 0,
        Client = 1,
    }

    public class ContactsInfo
    {
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public int FilterLevel { get; set; }
        public int UserNo { get; set; }
        public int RoleId { get; set; }
        public bool ReceiveMode { get; set; }
        public string ReceiverMail { get; set; }
    }
}