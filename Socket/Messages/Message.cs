﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Socket.Messages
{
    public class Message
    {
        public DateTime DateTime { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string FGUID { get; set; }
        public string To { get; set; }
        public string TGUID { get; set; }
        public string Channel { get; set; }

        public List<UserMessageDTO> ChannelsParts { get; set; } = new List<UserMessageDTO>();

        public Message() { }


        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message ToMessage(string jMsg)
        {
            return JsonSerializer.Deserialize<Message>(jMsg);
        }

    }

    public class UserMessageDTO 
    {
        public string Name { get; set; }
        public string GUID { get; set; }

        public UserMessageDTO() { }
    }
}
