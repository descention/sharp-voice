using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVoice
{
    public class Folder
    {//Folder wrapper for feeds from Google Voice
        int totalSize;//(aka __len__)
        Dictionary<string, int> unreadCounts;
        int resultsPerPage;
        List<Message> messages;//list of Message instances

        public Folder(Voice v, string name, string data)
        {
            //Folder(voice, name, data)
        }

        public List<Message> Messages()
        {//Returns a list of all messages in this folder
            return messages;
        }
    }
}
