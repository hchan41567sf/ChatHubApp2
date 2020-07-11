using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.Models
{
    public class ChatMessage
    {
        public int MsgID { get; set; }
        public string PhoneNumOfWriter { get; set; }
        public string Msg { get; set; }
        public string Date { get; set; }
        public string Writer { get; set; }
        public string SeenByRecipient { get; set; }
    }
}
