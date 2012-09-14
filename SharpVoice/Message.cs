using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpVoice
{
    public class Message
    {
        //Wrapper for all call/sms message instances stored in Google Voice Attributes are:

        string id;//SHA1 identifier
        bool isTrash;
        //displayStartDateTime: datetime
        bool star;
        bool isSpam;
        //startTime: gmtime
        List<string> labels;
        // displayStartTime: time
        string children;
        string note;
        bool isRead;
        string displayNumber;
        string relativeStartTime;
        string phoneNumber;
        int type;

        public Message(string folder, int id, string data)
        {
            //Message(folder, id, data)
            /*
            def __init__(self, voice, name, data):
                self.voice = voice
                self.name = name
                super(AttrDict, self).__init__(data)
            */
        }

        public void Delete()
        {
            Delete(true);
        }

        public void Delete(bool trash)
        {
            //Moves this message to the Trash. Use message.delete(0) to move it out of the Trash.
        }

        public void Download()
        {
            Download(Environment.CurrentDirectory);
        }

        public void Download(string adir)
        {
            //Download the message MP3 (if any). Saves files to adir (defaults to current directory). Message hashes can be found in self.voicemail().messages for example. Returns location of saved file.
        }

        public void MarkRead()
        {
            MarkRead(true);
        }

        public void MarkRead(bool read)
        {
            //Mark this message as read. Use message.mark(0) to mark it as unread.

        }

        public void Star()
        {
            Star(true);
        }

        public void Star(bool star)
        {
            //Star this message. Use message.star(0) to unstar it.
        }

        public string toString()
        {
            return id;
        }

    }
}
