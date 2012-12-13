using System;// I'm pretty sure this is required...
using System.Windows.Forms;//Console app, but I like to show message boxes for errors...
using System.IO;// this is needed.
using System.Threading;// yay threads!
using System.Diagnostics;//Helpful in getting this code working.
using SharpVoice;
using System.Xml;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GoogleTests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            try {
                Visual Form = new Visual();
                Form.ShowDialog();
		    } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                MessageBox.Show(e.Message);//more Debug help
		    }
        }
    }
}
