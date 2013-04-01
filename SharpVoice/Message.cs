using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpVoice
{
    [DataContract]
    public class Message
    {
        internal Voice connection;

        public enum Direction
        {
            Incoming = 10,
            Outgoing = 11
        }

        public void Delete()
        {
            Delete(true);
        }

        public void Delete(bool trash)
        {
            //Moves this message to the Trash. Use message.delete(0) to move it out of the Trash.
            throw new NotImplementedException();
        }

        public string Download()
        {
            return Download(System.IO.Path.Combine(Environment.CurrentDirectory, this.ID) + ".mp3");
        }

        public string Download(string adir)
        {
            //Download the message MP3 (if any). Saves files to adir (defaults to current directory). Message hashes can be found in self.voicemail().messages for example. Returns location of saved file.
            if (this.HasMP3 && connection != null)
            {
                return Voice.SaveVoicemail(this.ID, adir);
            }
            else if (!this.HasMP3)
            {
                throw new InvalidOperationException("No MP3 available");
            }
            return "";
        }

        public void MarkRead()
        {
            MarkRead(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection">Voice object</param>
        /// <param name="read">True = mark as read; False = mark as unread</param>
        public void MarkRead(bool read)
        {
            Voice.MarkRead(this.ID, read);
        }

        public override string ToString()
        {
            return ID;
        }

        [DataMember(Name = "id")]
        internal string id { get; set; }
        public string ID { get { return this.id; } }

        [DataMember(Name = "phoneNumber")]
        public string phoneNumber { get; set; }

        [DataMember(Name = "displayNumber")]
        internal string displayNumber { get; set; }

        public string DisplayNumber { get { return this.displayNumber; } }

        [DataMember]
        public string startTime { get; set; }

        [DataMember]
        public string displayStartDateTime { get; set; }

        [DataMember]
        public string displayStartTime { get; set; }

        [DataMember]
        public string relativeStartTime { get; set; }

        [DataMember(Name = "note")]
        public string Note { get; set; }

        [DataMember(Name = "isRead")]
        public bool IsRead { get; set; }

        [DataMember(Name = "isSpam")]
        public bool IsSpam { get; set; }

        [DataMember(Name = "isTrash")]
        public bool IsTrash { get; set; }

        [DataMember(Name = "star")]
        internal bool star { get; set; }
        public bool Star
        {
            get { return this.star; }
            set
            {
                Dictionary<string,string> data = new Dictionary<string,string>();
                data.Add("messages",this.ID);
                data.Add("star",value.ToString());
                Voice.Request("star", data);
                this.star = value;
            }
        }

        [DataMember(Name = "messageText")]
        public string Text { get; set; }

        [DataMember(Name = "labels")]
        public string[] Labels { get; set; }

        [DataMember(Name = "hasMp3")]
        internal bool hasMp3 { get; set; }
        public bool HasMP3 { get { return this.hasMp3; } }

        [DataMember(Name = "duration")]
        internal int duration { get; set; }
        public int Duration { get { return this.duration; } }

        [DataMember(Name = "type")]
        internal Direction type { get; set; }
        public Direction Type { get { return this.type; } }

        [DataMember(Name = "children")]
        internal string children { get; set; }
        public string Children { get { return this.children; } }
    }
}
