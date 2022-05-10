using MySocket.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket.Objects.Events
{
    public class Event
    {   
        public string Name{ get; set; }

        public Action<Message> Delegate{ get; set; }

        public bool Continue { get; }

        public bool OnlyFromServer { get; }


        public Event(string name, Action<Message> @delegate, bool @continue, bool @public)
        {
            Name = name;
            Delegate = @delegate;
            Continue = @continue;
            OnlyFromServer = !@public;
        }

        public bool Execute(Message msg)
        {
            Delegate.Invoke(msg);

            return Continue;
        }
    }
}
