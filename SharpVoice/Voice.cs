using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SharpVoice
{
    public class Voice : IDisposable
    {   
        private static String rnrSEE = null;
	    String user = null;
	    String pass = null;
		private static CookieCollection cookies = new CookieCollection();
        private static CookieContainer cookiejar = new CookieContainer();

        

        /*
		 * Links Imported from https://pygooglevoice.googlecode.com/hg/googlevoice/settings.py
		 * Made to be more compatible with porting future python code.
		*/
		//string LOGIN = "https://www.google.com/accounts/ServiceLoginAuth?service=grandcentral";
		//string LOGIN = "https://www.google.com/accounts/ClientLogin";

        //string[] FEEDS = new String[]{"inbox", "starred", "all", "spam", "trash", "voicemail", "sms",
        //        "recorded", "placed", "received", "missed"};

		const string BASE = "https://www.google.com/voice/";
        const string XML_RECENT = BASE + "inbox/recent/";

        public static Dictionary<string, string> dict = new Dictionary<string, string>(){
            {"BASE","https://www.google.com/voice/"},
            {"RNRSE","https://accounts.google.com/ServiceLogin?service=grandcentral&continue=https://www.google.com/voice/&followup=https://www.google.com/voice/&ltmpl=open"},
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

        /// <summary>
        /// Login with email address and password
        /// </summary>
        /// <param name="user">Email address for account</param>
        /// <param name="pass">Password for account</param>
		public Voice(string user, string pass)
		{
            if (!string.IsNullOrEmpty(rnrSEE))
                Logout();
			this.user = user;
			this.pass = pass;
            this.Login();
		}
		
		~Voice(){
			Logout();
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
        private String getInbox(){
            return Request("xml_inbox");
	    }

        Folder _inbox;

        public Folder Inbox
        {
            get
            {
                if (_inbox == null || _inbox.NeedsUpdate)
                {
                    string inbox = this.getInbox();
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(inbox);
                    string json = xd.SelectSingleNode("//response/json").InnerText;

                    _inbox = JsonConvert.DeserializeObject<Folder>(json);
                    _inbox.LastUpdate = DateTime.Now;
                }
                return _inbox;
            }
        }

	    public String getStarred(){
            return Request("XML_STARRED");
	    }
	    public String getRecent(){
            return Request("XML_ALL");
	    }
	    public String getSpam(){
        	return Request("XML_SPAM");
	    }
	    public String getRecorded(){
            return Request("XML_RECORDED");
	    }
	    public String getPlaced(){
            return Request("XML_PLACED");
	    }
	    public String getReceived(){
            return Request("XML_RECEIVED");
	    }
	    public String getMissed(){
            return Request("XML_MISSED");
	    }
	    private String getSMS(){
            return Request("XML_SMS");
	    }

        Folder _sms;

        public Folder SMS
        {
            get
            {
                if (_sms == null || _sms.NeedsUpdate)
                {
                    string box = this.getSMS();
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(box);
                    string json = xd.SelectSingleNode("//response/json").InnerText;
                    _sms = JsonConvert.DeserializeObject<Folder>(json);
                }
                return _sms;
            }
        }

		#endregion

		private Dictionary<string,string> get_post_vars()
        {
            Dictionary<string,string> Post_Vars = new Dictionary<string,string>(){};
            string response = Request("base");
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
        private void Login(){
            Debug.WriteLine("get:Login");
            if (user == null)
				throw new InvalidOperationException("No email defined");
            if (pass == null)
                throw new InvalidOperationException("No password defined");
            
            Dictionary<string, string> loginData = get_post_vars();
            
            if (loginData.ContainsKey("Email"))
                loginData["Email"] = user;
            else
                loginData.Add("Email", user);

            if (loginData.ContainsKey("Passwd"))
                loginData["Passwd"] = pass;
            else
                loginData.Add("Passwd", pass);
            
            rnrSEE = get_rnrse(Request("rnrse",loginData));
			
			if (string.IsNullOrEmpty(rnrSEE))
			{
				throw new Exception("Could not login");
			}
	    }

        /// <summary>
        /// Deauth oneself with this method.
        /// </summary>
		public void Logout()
		{
            Debug.WriteLine("get:Logout");
			Request("logout");
			rnrSEE = null;
		}
		
		private string get_rnrse(string t)
        {
            t = t.Replace("\n", "");
            Match m = Regex.Match(t, "<input.*?name=[\'\"]_rnr_se[\'\"].*?(value=[\'\"](.*?)[\'\"]|/>)");
            return m.Groups[2].ToString().Trim();
        }

        public string Call(string callTo, string callFrom){
            return Call(callTo, callFrom, "undefined");
        }

        public string Call(String callTo, String callFrom, String subscriberNumber){
            Dictionary<string, string> callData = new Dictionary<string, string>(){
                {"outgoingNumber",callTo},
                {"forwardingNumber",callFrom},
                {"subscriberNumber",subscriberNumber},
                {"remember","1"},
                {"phoneType","2"}
            };

            return this.Request("call", callData);
	    }

        public string SendSMS(String destinationNumber, String txt){
		    
            Dictionary<string, string> smsData = new Dictionary<string, string>(){
                //{"auth",authToken},
                {"phoneNumber",destinationNumber},
                {"text",txt}
            };
			
            return this.Request("sms", smsData);
	    }
		
		public string markRead(string msgID, bool read){
			Dictionary<string,string> data = new Dictionary<string, string>();
			data.Add("_msgID", msgID);
			data.Add("read", read.ToString());
			return this.Request("mark",data);
		}

        internal object makeRequest(String page, object data)
        {
            Debug.WriteLine("request:" + page);
            string url = dict[page.ToUpper()];
            string dataString = "";
            
            if(data is string){
	            if(page.ToUpper() == "DOWNLOAD")
    	            url += data;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = @"sharpVoice / 0.1";
                request.CookieContainer = cookiejar;
                
                request.Method = "GET";
                
                if (data != null)
                {
                    if (data is Dictionary<string, string>)
                    {
                        Dictionary<string,string> dicdata = (Dictionary<string,string>)data;
                        //Dictionary<string, string> temp = new Dictionary<string, string>();
                        if(!string.IsNullOrEmpty(rnrSEE))
                        	dicdata.Add("_rnr_se", rnrSEE);
                        
                        foreach (KeyValuePair<string, string> h in dicdata)
                        {
                            dataString += h.Key + "=" + HttpUtility.UrlEncode(h.Value, Encoding.UTF8);
                            dataString += "&";
                        }
                        dataString.TrimEnd(new char[] { '&' });
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                        request.ContentLength = dataString.Length;
                    }
                }
                
                if (request.ContentLength > 0)
                {
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] bytes = encoding.GetBytes(dataString);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (WebResponse response = request.GetResponse())
                {
                    if (request.CookieContainer != null)
                    {
                        cookiejar = request.CookieContainer;
                    }

                    Stream s = response.GetResponseStream();

                    switch(page.ToUpper()){
                        default:
                            using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                                return reader.ReadToEnd();
                        case "DOWNLOAD":
                            MemoryStream m = new MemoryStream();
                            byte[] buffer = new byte[1024];
                            int bytesSize = 0;
                            while ((bytesSize = s.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                m.Write(buffer, 0, bytesSize);
                            }
                            return m.ToArray();
                        case "LOGOUT":
                            return "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public string Request(string page){
        	return Request(page, null);
        }
        
        public string Request(string page, object data){
        	return (string)makeRequest(page,data);
        }

        public byte[] Download(string page, object data){
            return (byte[])makeRequest(page,data);
        }
        
		public void SaveVoicemail(string voiceID, string location){
			File.WriteAllBytes(location,Download("download", voiceID));
		}


        #region IDisposable Members

        public void Dispose()
        {
            this.Logout();
        }

        #endregion
    }
}