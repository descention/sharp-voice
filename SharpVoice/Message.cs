using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpVoice
{
    [DataContract]
    public class Message
    {
        public enum Direction{
            Incoming = 10,
            Outgoing = 11
        }

        Voice connection;

        public void Delete()
        {
            Delete(true);
        }

        public void Delete(bool trash)
        {
            //Moves this message to the Trash. Use message.delete(0) to move it out of the Trash.
            throw new NotImplementedException();
        }

        public void Download()
        {
            Download(Environment.CurrentDirectory);
        }

        public void Download(string adir)
        {
            //Download the message MP3 (if any). Saves files to adir (defaults to current directory). Message hashes can be found in self.voicemail().messages for example. Returns location of saved file.
            if (this.HasMp3 && connection != null)
            {
                connection.SaveVoicemail(this.ID, adir);
            }
            throw new NotImplementedException();
        }

        public void MarkRead()
        {
            MarkRead(true);
        }

        public void MarkRead(bool read)
        {
            //Mark this message as read. Use message.mark(0) to mark it as unread.
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return ID;
        }

        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "phoneNumber")]
        public string phoneNumber { get; set; }

        [DataMember(Name = "displayNumber")]
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

        [DataMember(Name = "messageText")]
        public string Text { get; set; }

        [DataMember(Name = "labels")]
        public string[] Labels { get; set; }

        [DataMember(Name = "hasMp3")]
        public bool HasMp3 { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember]
        public Direction type { get; set; }

        [DataMember(Name = "children")]
        public string Children { get; set; }
    }
}
