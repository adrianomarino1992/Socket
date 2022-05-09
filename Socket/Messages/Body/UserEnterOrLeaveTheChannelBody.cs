using Socket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket.Messages.Body
{
    public class UserEnterOrLeaveTheChannelBody
    {
        public String Name{ get; set; }
        public String GUID { get; set; }
        public string Channel { get; set; }


    }
}
