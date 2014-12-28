/////////////////////////////////////////////////////////////////////
// Window2.xaml.cs - WPF User Interface for Server-1                //
// ver 2.2                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2008 //
//Modified: sarika shrishail kudagi, 726869838, Syracuse University //
//              (551) 998-2431, sskudagi@syr.edu                    //
/////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ----------------------
 * ThreadProc and ThreadProcFiles: get message or filesNames out of the blocking queue.
 * 
 * displayFileName: Diplay the fileName sent by the client to server to perform Type and Package Analysis.
 * 
 * listenPortServer1: Dynamically send the Server Port numbers that are actively listening.
 * 
 * displayFileList: Displays the file List of the Server.
 * 
 * ListentButton_Click: function that handles the Listen button click on the WPF of Server-1.
 * 
 * 
 * Maintenance History:
 * ====================
 * ver 2.2 : 30 Oct 11
 * - added send thread to keep UI from freezing on slow sends
 * - added more comments to clarify what code is doing
 * ver 2.1 : 16 Oct 11
 * - cosmetic changes, posted to the college server but not
 *   distributed in class
 * ver 2.0 : 06 Nov 08
 * - fixed bug that had local and remote ports swapped
 * - made Receive thread background so it would not keep 
 *   application alive after close button is clicked
 * - now closing sender and receiver channels on window
 *   unload
 * ver 1.0 : 17 Jul 07
 * - first release
 */

using System.Threading.Tasks;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using System.Text;

using System.Threading;
using SWTools;
using System.IO;
using CodeAnalysis;
//using System.Xml.Linq.XElement;



using System.Runtime.Serialization;


using CodeAnalysis;
namespace WPF_GUI_Server
{
    public partial class Window2 : Window
    {
        WCF_Peer_Comm.Receiver recvr;
        WCF_Peer_Comm.Sender sndr;
        Repository repRelation = new Repository();
        //CodeAnalysis.Analyzer an = new CodeAnalysis.Analyzer();
        //CodeAnalysis.Executable ex = new CodeAnalysis.Executable();
        string rcvdMsg = "";
        List<string> rcvdFiles = null;
        int MaxMsgCount = 100;
        List<string> patterns = new List<string>();
        List<string> files = new List<string>();

        Thread rcvThrd = null;
        Thread rcvThrd1 = null;

        delegate void NewMessage(string msg);
        delegate void NewMessage1(List<string> file);
        event NewMessage OnNewMessage;
        event NewMessage1 OnNewMessageFiles;

        //----< receive thread processing >------------------------------

        void ThreadProc()
        {
            while (true)
            {
                // get message out of receive queue - will block if queue is empty
                rcvdMsg = recvr.GetMessage();
                // call window functions on UI thread
                this.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  OnNewMessage,
                  rcvdMsg);
                
            }
        }
        void ThreadProcFiles()
        {
            while (true)
            {
                rcvdFiles = recvr.GetFMessage();
                this.Dispatcher.BeginInvoke(
           System.Windows.Threading.DispatcherPriority.Normal,
           OnNewMessageFiles,
           rcvdFiles);
            }
        }
        public void displayFileName(string msg)
        {
            listBox1.Items.Insert(0, msg);
            if (listBox1.Items.Count > MaxMsgCount)
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
        }
        public string listenPortServer1()
        {
            string port=RemotePortTextBox.Text;
            return port;
        }
        public void addPattern(string pattern) // method to find files of .cs type
        {
            patterns.Add(pattern);
        }

        public void displayFileList(List<string> files)
        {
            foreach (string file in files)
            {
                listBox1.Items.Insert(0, file);
                if (listBox1.Items
                    .Count > MaxMsgCount)
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
            }
        }
       

        //----< called by UI thread when dispatched from rcvThrd >-------

        void OnNewMessageHandler(string msg)
        {
            listBox1.Items.Insert(0, msg);
            if (listBox1.Items.Count > MaxMsgCount)
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
        }
        void OnNewMessageHandlerFile(List<string> files)
        {
            foreach (string file in files)
            {
                listBox1.Items.Insert(0, file);
                if (listBox1.Items
                    .Count > MaxMsgCount)
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
            }
        }

        //----< subscribe to new message events >------------------------

        public Window2()
        {
            InitializeComponent();
            Title = "Server-1";
            OnNewMessageFiles += new NewMessage1(OnNewMessageHandlerFile);
            OnNewMessage += new NewMessage(OnNewMessageHandler);

        }
        //----< start listener >-----------------------------------------

        private void ListentButton_Click(object sender, RoutedEventArgs e)
        {

            listBox1.Items.Insert(0,"Server is Listening");
            string localPort = RemotePortTextBox.Text;
            string endpoint = "http://localhost:" + localPort + "/ICommunicator";

            try
            {
                recvr = new WCF_Peer_Comm.Receiver();
                recvr.CreateRecvChannel(endpoint);
                //ThreadProc();
                //ThreadProcFiles();
                 //create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();
                //ThreadProcFiles();
                rcvThrd1 = new Thread(new ThreadStart(this.ThreadProcFiles));
                rcvThrd1.IsBackground = true;
                rcvThrd1.Start();

               // ConnectButton.IsEnabled = true;
                ListenButton.IsEnabled = true;

            }
            catch (Exception ex)
            {
                Window temp = new Window();
                StringBuilder msg = new StringBuilder(ex.Message);
                msg.Append("\nport = ");
                msg.Append(localPort.ToString());
                temp.Content = msg.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }
        }

        //----< connect to remote listener >-----------------------------


        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            sndr.PostMessage("quit");
            sndr.Close();
            recvr.Close();
        }

        
    }
}
