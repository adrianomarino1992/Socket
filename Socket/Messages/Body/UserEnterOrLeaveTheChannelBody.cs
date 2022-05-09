namespace MySocket.Messages.Body
{
    public class UserEnterOrLeaveTheChannelBody
    {
        public String Name { get; set; }
        public String GUID { get; set; }
        public string Channel { get; set; }

        public UserEnterOrLeaveTheChannelBody() { }
    }
}
