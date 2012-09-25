using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpVoice
{
    [DataContract]
    public class Message
    {
        //Wrapper for all call/sms message instances stored in Google Voice Attributes are:

        public Message() { }

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

        public override string ToString()
        {
            return ID;
        }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }

        [DataMember]
        public string displayNumber { get; set; }

        [DataMember]
        public string startTime { get; set; }

        [DataMember]
        public string displayStartDateTime { get; set; }

        [DataMember]
        public string displayStartTime { get; set; }

        [DataMember]
        public string relativeStartTime { get; set; }

        [DataMember]
        public string note { get; set; }

        [DataMember]
        public bool isRead { get; set; }

        [DataMember]
        public bool isSpam { get; set; }

        [DataMember]
        public bool isTrash { get; set; }

        [DataMember(Name = "star")]
        public bool Star { get; set; }

        [DataMember]
        public string messageText { get; set; }

        [DataMember]
        public string[] labels { get; set; }

        [DataMember]
        public bool hasMp3 { get; set; }

        [DataMember]
        public int duration { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string children { get; set; }
    }
}
