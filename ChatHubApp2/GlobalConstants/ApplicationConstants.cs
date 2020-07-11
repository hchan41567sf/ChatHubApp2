using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.GlobalConstants
{
    static class ApplicationConstants
    {
        //DATABASE RELATED
        //chat messages database columns
        public const string MSG_ID = "MsgID";
        public const string MSG_PHONE_NUM_WRITER = "PhoneNumOfWriter";
        public const string MSG_COLUMN = "Msg";
        public const string DATE_COLUMN = "Date";
        public const string WRITER_COLUMN = "Writer";
        public const string SEEN_COLUMN = "SeenByRecipient";
    }
}
