using MySocket.DTO;

namespace MySocket.Messages.Body
{
    public class RequestChannelsInfoBody
    {
        public List<ChannelDTO> Channels { get; set; }

        public RequestChannelsInfoBody() { }
    }
}
