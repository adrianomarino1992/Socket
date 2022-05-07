using Socket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket.Messages.Body
{
    public class RequestChannelsInfoBody
    {
        public List<ChannelDTO> Channels { get; set; }        

        public RequestChannelsInfoBody() { }
    }
}
