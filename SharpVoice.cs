using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Media;
using System.Xml;
using System.Configuration;

namespace SharpVoice
{
    class Voice
    {   
        public String rnrSEE = null;
	    String source = null;
	    String user = null;
	    String pass = null;
	    String authToken = null;
		String settingsPath = "sharpVoice.xml";
		private CookieCollection cookies = new CookieCollection();
        private CookieContainer cookiejar = new CookieContainer();
        /*
		 * Links Imported from https://pygooglevoice.googlecode.com/hg/googlevoice/settings.py
		 * Made to be more compatible with porting future python code.
		*/
		//string LOGIN = "https://www.google.com/accounts/ServiceLoginAuth?service=grandcentral";
		//string LOGIN = "https://www.google.com/accounts/ClientLogin";

        //string[] FEEDS = new String[]{"inbox", "starred", "all", "spam", "trash", "voicemail", "sms",
        //        "recorded", "placed", "received", "missed"};

        public Util.Folder inbox;
        public Util.Folder starred;
        public Util.Folder all;
        //continue for each FEED

		

		const string BASE = "https://www.google.com/voice/";
        const string XML_RECENT = BASE + "inbox/recent/";

        Dictionary<string, string> dict = new Dictionary<string, string>(){
            {"BASE","https://www.google.com/voice/"},
            {"RNRSE","https://accounts.google.com/ServiceLogin?service=grandcentral&continue=https://www.google.com/voice/&followup=https://www.google.com/voice/&ltmpl=open"},
    		{"LOGIN","https://www.google.com/accounts/ClientLogin"},
            {"LOGOUT",BASE + "account/signout"},
    		{"INBOX",BASE + "#inbox"},
    		{"CALL",BASE + "call/connect/"},
    		{"CANCEL",BASE + "call/cancel/"},
    		{"DEFAULT_FORWARD",BASE + "settings/editDefaultForwarding/"},
    		{"FORWARD",BASE + "settings/editForwarding/"},
    		{"DELETE",BASE + "inbox/deleteMessages/"},
    		{"MARK",BASE + "inbox/mark/"},
    		{"STAR",BASE + "inbox/star/"},
    		{"SMS",BASE + "sms/send/"},
    		{"DOWNLOAD",BASE + "media/send_voicemail/"},
    		{"BALANCE",BASE + "settings/billingcredit/"},
    		{"XML_SEARCH",BASE + "inbox/search/"},
    		{"XML_CONTACTS",BASE + "contacts/"},
    		{"XML_INBOX",XML_RECENT + "inbox/"},
    		{"XML_STARRED",XML_RECENT + "starred/"},
    		{"XML_ALL",XML_RECENT + "all/"},
    		{"XML_SPAM",XML_RECENT + "spam/"},
    		{"XML_TRASH",XML_RECENT + "trash/"},
    		{"XML_VOICEMAIL",XML_RECENT + "voicemail/"},
    		{"XML_SMS",XML_RECENT + "sms/"},
    		{"XML_RECORDED",XML_RECENT + "recorded/"},
    		{"XML_PLACED",XML_RECENT + "placed/"},
    		{"XML_RECEIVED",XML_RECENT + "received/"},
    		{"XML_MISSED",XML_RECENT + "missed/"}
        };
        

        public Voice()
        {
            //ConfigurationManager.AppSettings[""] = "";
        }

        /// <summary>
        /// Set email address and password.
        /// </summary>
        /// <param name="user">Email address for account</param>
        /// <param name="pass">Password for account</param>
		public Voice(string user, string pass)
		{
			this.user = user;
			this.pass = pass;
		}


        /// <summary>
        /// provide login information and rnr_se key. Login immediately.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="source"></param>
        /// <param name="rnrSee"></param>
		public Voice(String user, String pass, String source, String rnrSee){

		    this.user = user;
		    this.pass = pass;
		    this.rnrSEE = rnrSee;
		    this.source = source;

		    login();
	    }
		/*
		inbox - Recent, unread messages
		starred - Starred messages
		all - All messages
		spam - Messages likely to be spam
		trash - Deleted messages
		voicemail - Voicemail messages
		sms - Text messages
		recorded - Recorced messages
		placed - Outgoing messages
		received - Incoming messages
		missed - Messages not received
		*/

		#region java translated functions
        public String getInbox(){
            return makeRequest(dict["xml_inbox"])[0];
	    }
	    public String getStarred(){
            return makeRequest(dict["XML_STARRED"])[0];
	    }
	    public String getRecent(){
            return makeRequest(dict["XML_ALL"])[0];
	    }
	    public String getSpam(){
            return makeRequest(dict["XML_SPAM"])[0];
	    }
	    public String getRecorded(){
            return makeRequest(dict["XML_RECORDED"])[0];
	    }
	    public String getPlaced(){
            return makeRequest(dict["XML_PLACED"])[0];
	    }
	    public String getReceived(){
            return makeRequest(dict["XML_RECEIVED"])[0];
	    }
	    public String getMissed(){
            return makeRequest(dict["XML_MISSED"])[0];
	    }
	    public String getSMS(){
            return makeRequest(dict["XML_SMS"])[0];
	    }
		#endregion

		public Dictionary<string,string> get_post_vars()
        {
            Dictionary<string,string> Post_Vars = new Dictionary<string,string>(){};
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(dict["BASE"]);
            request.CookieContainer = cookiejar;
            request.Method = "GET";
            request.UserAgent = "sharpVoice / 0.1";


            WebResponse answer = request.GetResponse();
            cookiejar = request.CookieContainer;

            StreamReader reader = new StreamReader(answer.GetResponseStream(), Encoding.UTF8);
            string response = reader.ReadToEnd();
            response = response.Replace("\n", "");
            

            MatchCollection vars = Regex.Matches(response, "name=['\"](.*?)['\"].*?(value=['\"](.*?)['\"]|>)", RegexOptions.Multiline);
            int num_matches = vars.Count;
            string name = "";
            string value = "";
            for (int i = 0; i < num_matches; i++)
            {
                //clear
                name = "";
                value = "";

                //set
                name = vars[i].Groups[1].ToString().Trim();
                value = vars[i].Groups[3].ToString().Trim();

                /*/trim trailing " or '
                name = name.Remove(name.Length - 1, 1);
                value = value.Remove(value.Length - 1, 1);
                */
                if (!Post_Vars.ContainsKey(name))
                    Post_Vars.Add(name, value);
            }

            return Post_Vars;
        }
		
        /// <summary>
        /// Login with and email address and password.  Retrieve the auth and __rnr_se keys.
        /// </summary>
        public void login(){
            if (user == null)
				throw new IOException("No User Defined");
            if (pass == null)
				throw new IOException("No Pass Defined");
            
            Dictionary<string, string> loginData = get_post_vars();
            
            if (loginData.ContainsKey("Email"))
                loginData["Email"] = user;
            else
                loginData.Add("Email", user);

            if (loginData.ContainsKey("Passwd"))
                loginData["Passwd"] = pass;
            else
                loginData.Add("Passwd", pass);
            
            rnrSEE = get_rnrse(makeRequest("rnrse",loginData)[0]);
			
			if (string.IsNullOrEmpty(rnrSEE))
			{
				try
				{
					special();
				}
				catch
				{
					throw new IOException("No rnr key Defined");
				}
			}
	    }

        /// <summary>
        /// Deauth oneself with this method.
        /// </summary>
		public void logout()
		{
			makeRequest("logout");
			rnrSEE = null;
		}

        /// <summary>
        /// The method retrieves the __rnr_se value from the website so you don't have to.
        /// </summary>
		private void special()
		{
			Regex regex = new Regex("<input.*name=\"_rnr_se\".*value=\"(.*)\"/>",RegexOptions.None);
			try
			{
                string t = makeRequest("base")[0];
                MatchCollection m = regex.Matches(t);
                rnrSEE = m[0].Result("$2");
			}
			catch
			{
				throw new IOException();
			}
		}
		
		public string get_rnrse(string t)
        {
            t = t.Replace("\n", "");
            Match m = Regex.Match(t, "<input.*?name=[\'\"]_rnr_se[\'\"].*?(value=[\'\"](.*?)[\'\"]|/>)");
            return m.Groups[2].ToString().Trim();
        }

        public string call(string callTo, string callFrom){
            return call(callTo, callFrom, "undefined");
        }

        public string call(String callTo, String callFrom, String subscriberNumber){
            Dictionary<string, string> callData = new Dictionary<string, string>(){
                {"outgoingNumber",callTo},
                {"forwardingNumber",callFrom},
                {"subscriberNumber",subscriberNumber},
                {"remember","1"},
                {"phoneType","2"}
            };

            return this.makeRequest("call", callData)[0];
	    }

        public string sendSMS(String destinationNumber, String txt){
		    
            Dictionary<string, string> smsData = new Dictionary<string, string>(){
                //{"auth",authToken},
                {"phoneNumber",destinationNumber},
                {"text",txt}
            };
			
            return this.makeRequest("sms", smsData)[0];
	    }
		
		public string markSMS(string smsID, bool read){
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add("_msgID", smsID);
			data.Add("read",read.ToString());
			
			return this.makeRequest("mark",data)[0];
		}

        private string[] makeRequest(string page){
            return makeRequest(page, null);
        }

        private string[] makeRequest(string page, object data){
            return makeRequest(page, data, null);
        }

        private string[] makeRequest(String page, object data, WebHeaderCollection headers)
        {
            string url = dict[page.ToUpper()];

            if (headers == null)
                headers = new WebHeaderCollection();

            List<string> result = new List<string>();
            
            WebResponse response = null;
            StreamReader reader = null;
            String dataString = "";

            try
            {
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = @"sharpVoice / 0.1";
                request.CookieContainer = cookiejar;
                
                if (data != null)
                {
                    if (data is Dictionary<string, string>)
                    {
                        Dictionary<string,string> dicdata = (Dictionary<string,string>)data;
                        //Dictionary<string, string> temp = new Dictionary<string, string>();
                        if(!string.IsNullOrEmpty(this.rnrSEE))
                        	dicdata.Add("_rnr_se", rnrSEE);
                        
                        foreach (KeyValuePair<string, string> h in dicdata)
                        {
                            dataString += h.Key + "=" + HttpUtility.UrlEncode(h.Value, Encoding.UTF8);
                            dataString += "&";
                        }
                        dataString.TrimEnd(new char[] { '&' });
                    }
                    else
                    {
                        //dataString += data + 
                    }
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                }
                else
                {
                    request.Method = "GET";
                }
                //request.Headers.Add(headers);

                request.ContentLength = dataString.Length;
                if (dataString.Length > 0)
                {
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] bytes = encoding.GetBytes(dataString);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
                response = request.GetResponse();
                
                if (request.CookieContainer != null)
                {
                    cookiejar = request.CookieContainer;
                }
                
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                if (page.ToUpper() != "LOGIN")
                {
                    result.Add(reader.ReadToEnd());
                }
                else
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Data);
                throw new IOException(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }

            if (result.Equals(""))
            {
                throw new IOException("No Response Data Received.");
            }

            return result.ToArray();
        }

		private String saveVoicemail(string voiceID){
			//https://www.google.com/voice/media/send_voicemail/[voicemail id]
			// still working on this...
			// 
			throw new NotImplementedException();
		}

	}
	
	namespace Util{
		class Folder
		{//Folder wrapper for feeds from Google Voice
			int totalSize;//(aka __len__)
			Dictionary<string,int> unreadCounts;
			int resultsPerPage;
			List<Message> messages;//list of Message instances

			public Folder(Voice v, string name, string data){
				//Folder(voice, name, data)
			}

			public List<Message> Messages()
			{//Returns a list of all messages in this folder
				return messages;
			}
		}
		
		class Phone
		{// Wrapper for phone objects used for phone specific methods
			int id;
			string phoneNumber;// i18n phone number
			string formattedNumber;// humanized phone number string
			//data dictionary? we;
			//data dictionary? wd;
			bool verified;
			string name;
			bool smsEndabled;
			bool scheduleSet;
			int policyBitmask;
			//List weekdayTimes;
			//dEPRECATEDDisabled: bool
			bool weekdayAllDay;
			//bool telephonyVerified;
			//List weekendTimes
			bool active;
			bool weekendAllDay;
			bool enabledForOthers;
			int type;// (1 - Home, 2 - Mobile, 3 - Work, 4 - Gizmo)

			
			public Phone(Voice v, string data){
				//Phone(voice, data)
			}

			public void Disable()
			{
				//disables phone
			}

			public void Enable()
			{
				//enables phone
			}

			public string toString()
			{
				return phoneNumber;
			}
		}
		
		class Message{
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

			public void MarkRead(){
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

		class XMLParser{
			/*
			class SharpVoice.util.XMLParser(voice, name, datafunc)¶
			XML Parser helper that can dig json and html out of the feeds. The parser takes a Voice instance, page name, and function to grab data from. Calling the parser calls the data function once, sets up the json and html attributes and returns a Folder instance for the given page:

			>>> o = XMLParser(voice, 'voicemail', lambda: 'some xml payload')
			>>> o()
			... <Folder ...>
			>>> o.json
			... 'some json payload'
			>>> o.data
			... 'loaded json payload'
			>>> o.html
			... 'some html payload'
			data
			Returns the parsed json information after calling the XMLParser
			folder
			Returns associated Folder instance for given page (self.name)
			*/
			string json;
			string html;
			string data;

			public XMLParser(Voice v, string name, string datafunc){
				
			}

			public string Data()
			{
				return data;
			}

			public Folder Folder()
			{
				return null;
			}
		}
	}
}