using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket.Messages.Body
{
    public class ChangeChannelBody
    {
        public string From { get; set; }

        public string To { get; set; }

        public ChangeChannelBody() { }
    }
}
