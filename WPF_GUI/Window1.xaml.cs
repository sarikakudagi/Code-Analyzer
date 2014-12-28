/////////////////////////////////////////////////////////////////////
// Window1.xaml.cs - WPF User Interface for WCF Communicator       //
// ver 2.2                                                         //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2008  //
//Modified: sarika shrishail kudagi, 726869838, Syracuse University //
//              (551) 998-2431, sskudagi@syr.edu                    //
/////////////////////////////////////////////////////////////////////
/*
 *  Module Operations:
 *  --------------------------------
 *  ConnectButton_Click: function that handles the "Connect" button click on WPF. Prints a Success
 *                        message if client connects with server-1 successfully.
 *                        
 *  ConnectButton_Click2: another function that handles the "Connect" button click on WPF. Prints a Success
 *                        message if client connects with server-2 successfully.
 *                        
 *  SendMessageButton_Click: send the file name selected by the user on WPF to server for Type Analysis.
 *  
 *  GetFilesButton_Click: extracts all the files from the server-1 and server-2 and displays it on the client WPF.
 *  
 * DoAnalysisButton_Click: function that performs Type-Analysis on the selected files.
 * 
 * mergeFile: is a function where repositories from both the server are combined. One file may have dependency on another
 *              file present on another server. This function helps in finding such dependencies.
 *   
 * Analysis across servers: User sends a file each to server-1 and server-2. Perform Type-Analysis on each file and 
 *                          store it in table. Now use mergeFile function to append both the files sent by the user. 
 *                          Perform Type-Analysis of the merged files and store in another table. Compare the table 
 *                          created on file-1 and the mergeFile table and display the output in listbox. This function 
 *                          finds dependenices across servers.
 *                         
 *  
 * 
 * 
 *  
 * Maintenance History:
 * ====================
 * ver 2.2 : 30 Oct 11
 * - added send thread to keep UI from freezing on slow sends
 * - added more comments to clarify what code is doing
 * ver 2.1 : 16 Oct 11
 * 
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



using System.Runtime.Serialization;


using CodeAnalysis;
namespace WPF_GUI
{
  public partial class Window1 : Window
  {
    WCF_Peer_Comm.Receiver recvr;
    WCF_Peer_Comm.Sender sndr;
    WCF_Peer_Comm1.Receiver1 recvr1;
    WCF_Peer_Comm1.Sender1 sndr1;

    Repository repRelation = new Repository();
    Repository2 repRelation1 = new Repository2();
  
    string rcvdMsg = "";
    List<string> rcvdFiles = null;
    int MaxMsgCount = 100;
    WPF_GUI_Server.Window2 server1 = new WPF_GUI_Server.Window2();//server-1
    WPF_GUI_Server2.Window2 server2 = new WPF_GUI_Server2.Window2();//server-2
    Thread rcvThrd = null;
    Thread rcvThrd1 = null;
       
    delegate void NewMessage(string msg);
    delegate void NewMessage1(List<string> file);
    event NewMessage OnNewMessage;
    event NewMessage1 OnNewMessageFiles;

    //----< receive thread processing >------------------------------

    void ThreadProc1()
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
             while (true){
            rcvdFiles = recvr.GetFMessage();
            this.Dispatcher.BeginInvoke(
       System.Windows.Threading.DispatcherPriority.Normal,
       OnNewMessageFiles,
       rcvdFiles);
        }
        }

    

    //----< called by UI thread when dispatched from rcvThrd >-------
        void OnNewMessageHandler1(string msg)
        {
         
             server2.displayFileName(msg);
            
        }
    void OnNewMessageHandler(string msg)
    {
        server1.displayFileName(msg);//display files on the server
       
    }
    void OnNewMessageHandlerFile(List<string> files) //display the list of files on the server
    {
        server1.displayFileList(files);
       
    }
    void OnNewMessageHandlerFile2(List<string> files)
    {
        server2.displayFileList(files);
        
    }
    //----< subscribe to new message events >------------------------

    public Window1()
    {
      InitializeComponent();
      Title = "Peer Comm";
      OnNewMessageFiles += new NewMessage1(OnNewMessageHandlerFile);
      OnNewMessageFiles += new NewMessage1(OnNewMessageHandlerFile2);
      OnNewMessage += new NewMessage(OnNewMessageHandler);
      OnNewMessage += new NewMessage(OnNewMessageHandler1);
     
      ConnectButton.IsEnabled = true;
      SendButton.IsEnabled = false;
      GetFilesButton.IsEnabled = false;
      doAnalysis.IsEnabled = false;
      doAnalysisXML.IsEnabled = false;
    }
    

    
    //----< connect to remote listener >-----------------------------

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        listBox1.Items.Insert(0, "Connection with Server-1 Successful!!!!");
      string remoteAddress = RemoteAddressTextBox.Text;
      string remotePort = RemotePortTextBox.Text;
      string port = server1.listenPortServer1();//get the port number of the listening server
      string endpoint = remoteAddress + ":" + port + "/ICommunicator";

      sndr = new WCF_Peer_Comm.Sender(endpoint);
      SendButton.IsEnabled = false;
      GetFilesButton.IsEnabled = true;
    }
    private void ConnectButton_Click2(object sender, RoutedEventArgs e)
    {
        listBox2.Items.Insert(0, "Connection with Server-2 Successful!!!!");
        string remoteAddress = RemoteAddressTextBox.Text;
        string remotePort = RemotePortTextBox1.Text;
        string port = server2.listenPortServer2();//get the port number of the listening server
        string endpoint = remoteAddress + ":" + port + "/ICommunicator1";

        sndr1 = new WCF_Peer_Comm1.Sender1(endpoint);
        SendButton.IsEnabled = false;
        GetFilesButton.IsEnabled = true;
        doAnalysisXML.IsEnabled = false;
    }
    //------send Message to the remote listener
    private void SendMessageButton_Click(object sender, RoutedEventArgs e) //send the selected file name for analysis
    {
      try
      {
         
         
          
          doAnalysis.IsEnabled = true;
          string text = SendMsgTextBox1.Text;
        listBox1.Items.Insert(0, "Sending File to server: "+ SendMsgTextBox.Text);
        listBox2.Items.Insert(0, "Sending File to server: "+ SendMsgTextBox1.Text);
        if(listBox1.Items.Count > MaxMsgCount)
          listBox1.Items.RemoveAt(listBox1.Items.Count-1);
      sndr.PostMessage(SendMsgTextBox.Text);
        sndr1.PostMessage1(text);
      }
      catch (Exception ex)
      {
        Window temp = new Window();
        temp.Content = ex.Message;
        temp.Height = 100;
        temp.Width = 500;
      }
    }
     //------- Send the Selected file to the remote server
      private void GetFilesButton_Click(object sender, RoutedEventArgs e)
    {
        
        
        try
        {
            SendButton.IsEnabled = true;
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            
            doAnalysisXML.IsEnabled = true;
            string fn=null;
            List<string> fileNameList = new List<string>();
            string[] fileList=Directory.GetFiles("..//..//..//WCF_Peer_Comm//FilesOnServer//");
          
            {
                for (int iFile = 0; iFile < fileList.Length; iFile++)
                {

                    fn = new FileInfo(fileList[iFile]).Name;
                    fileNameList.Add(fn);
                }
                
            }
                foreach(string fileNames in fileNameList)
                    listBox1.Items.Insert(0, fileNames);
                listBox1.Items.Insert(0, "Files on server-1 are:");
            
                List<string> fileNameListServer2 = new List<string>();
                string[] fileListServer2 = Directory.GetFiles("..//..//..//WCF_Peer_Comm1//FilesOnServer2//");
                
                {
                    for (int iFile = 0; iFile < fileListServer2.Length; iFile++)
                    {

                        fn = new FileInfo(fileListServer2[iFile]).Name;
                        fileNameListServer2.Add(fn);
                    }

                }
                foreach (string fileNames in fileNameListServer2)
                    listBox2.Items.Insert(0, fileNames);
            listBox2.Items.Insert(0, "Files on server-2 are:");
        }
          catch(Exception exp)
        {
            Window temp = new Window();
            temp.Content = exp.Message;
            temp.Height = 100;
            temp.Width = 500;
        }

    }
      //-- Performs Type-Analysis on the files Selected
      private void DoAnalysisButton_Click(object sender, RoutedEventArgs e)
      {
          try {

              listBox1.Items.Clear();
              listBox2.Items.Clear();
              List<string> pattern = new List<string>();
              Executable ex = new Executable();

              string[] newfiles = null;

              repRelation = Repository.getInstance(); 
              
              List<string> names = new List<string>();
              List<string> names2 = new List<string>();
            
              pattern.Add("*.cs");
              string temp = Directory.GetCurrentDirectory();
              foreach(string pat in pattern)
              {
                  newfiles = Directory.GetFiles(temp,pat);
              }
              string file = SendMsgTextBox.Text;
              string file2 = SendMsgTextBox1.Text;
              string fileName = "..//..//..//WCF_Peer_Comm//FilesOnServer//";
              string fileName2 = "..//..//..//WCF_Peer_Comm1//FilesOnServer2//";
              
              fileName = string.Concat(fileName, file);
              fileName2 = string.Concat(fileName2, file2);
              ex.mainFunction(fileName);
              XMLOutput xml = new XMLOutput();
              
              repRelation = Repository.getInstance();
              List<Elem> tab11 = repRelation.locations;
              foreach (Elem e11 in tab11)
              {
                  names.Add(e11.name);
              }
              repRelation.locations.Clear();
              repRelation.locationRp.Clear();
            
              mergeFile(fileName, fileName2); // merger files across servers
              fileName = "..//..//..//WCF_Peer_Comm//FilesOnServer//mergeFile.txt";
              ex.mainFunction(fileName);
              
              repRelation = Repository.getInstance();
              
              List<Elem> tab1 = repRelation.locations;
              List<Element> tableRelation = repRelation.locationRp;
          
             
                  foreach (Element el in tableRelation)  //to send the result of type-Analysis from server 1
                  {
                      foreach (string name in names)
                      {
                          if (name == el.class1 )
                          {
                             
                              string relationName = el.class1 + " " + el.relation + " " + el.class2;
                              listBox1.Items.Insert(0, relationName);
                          
                              sndr.PostMessage(relationName);
                              
                          }
                  }
              }
                  listBox1.Items.Insert(0, "Result of Type-Analysis on Sever-1:");
           
             
              repRelation1 = Repository2.getInstance();
              repRelation1.locationRp.Clear();
              repRelation1.locations.Clear();
            
              ex.mainFunction(fileName2);
              List<Elem> tab12 = repRelation1.locations;
             
              foreach (Elem e12 in tab12)
              {
                  names2.Add(e12.name);
              }
              repRelation1.locations.Clear();
              repRelation1.locationRp.Clear();
              ex.mainFunction(fileName);
              repRelation = Repository.getInstance();
              List<Element> tableRelation1 = repRelation1.locationRp;
             
              foreach (Element ee in tableRelation1) //to send the result of type-Analysis from server 1
              {
                  foreach (string name in names2)
                  {
                      if (name == ee.class1)
                      {
                          string relationName = ee.class1 + " " + ee.relation + " " + ee.class2;
                          listBox2.Items.Insert(0, relationName);
                          
                          sndr1.PostMessage1(relationName);
                         
                      }
                  }
              }
               listBox2.Items.Insert(0, "Result of Type-Analysis on Sever-2:");
               repRelation1.locationRp.Clear();
               repRelation1.locations.Clear();
               repRelation.locations.Clear();
               repRelation.locationRp.Clear();
             
          }
          catch (Exception exp)
          {
              Window temp = new Window();
              temp.Content = exp.Message;
              temp.Height = 100;
              temp.Width = 500;
          }
          Mouse.Capture(null);
      }
      
      //----------<LINQ Query XML file>------------
      private void DoAnalysisXMLButton_Click(object sender, RoutedEventArgs e)
      {
          try
          {

              listBox1.Items.Clear();
              listBox2.Items.Clear();
              List<string> pattern = new List<string>();
              Executable ex = new Executable();

              string[] newfiles = null;

              repRelation = Repository.getInstance();

              List<string> names = new List<string>();
              List<string> names2 = new List<string>();

              pattern.Add("*.cs");
              string temp = Directory.GetCurrentDirectory();
              foreach (string pat in pattern)
              {
                  newfiles = Directory.GetFiles(temp, pat);
              }
              string file = SendMsgTextBox.Text;
              string file2 = SendMsgTextBox1.Text;
              string fileName = "..//..//..//WCF_Peer_Comm//FilesOnServer//";
              string fileName2 = "..//..//..//WCF_Peer_Comm1//FilesOnServer2//";

              fileName = string.Concat(fileName, file);
              fileName2 = string.Concat(fileName2, file2);
              ex.mainFunction(fileName);
              XMLOutput xml = new XMLOutput();

              repRelation = Repository.getInstance();
              List<Elem> tab11 = repRelation.locations;
              foreach (Elem e11 in tab11)
              {
                  names.Add(e11.name);
              }
              repRelation.locations.Clear();
              repRelation.locationRp.Clear();

              mergeFile(fileName, fileName2); // merger files across servers
              fileName = "..//..//..//WCF_Peer_Comm//FilesOnServer//mergeFile.txt";
              ex.mainFunction(fileName);

              repRelation = Repository.getInstance();

              List<Elem> tab1 = repRelation.locations;
              List<Element> tableRelation = repRelation.locationRp;


              foreach (Element el in tableRelation)  //to send the result of type-Analysis from server 1
              {
                  foreach (string name in names)
                  {
                      if (name == el.class1)
                      {
                          xml.displayrelationxml(el.class1, el.relation, el.class2);
                          XDocument xdoc = XDocument.Load(@"..//Debug.xml");
                          IEnumerable<XElement> dataParent = (from x in xdoc.Descendants("ParentClass") select x).ToArray();
                          foreach (var data1 in dataParent)
                          {
                              listBox1.Items.Insert(0, data1);
                          }
                          string relationName = el.class1 + " " + el.relation + " " + el.class2;
                      sndr.PostMessage(relationName);

                      }
                  }
              }
              listBox1.Items.Insert(0, "Result of Type-Analysis on Sever-1:");


              repRelation1 = Repository2.getInstance();
              repRelation1.locationRp.Clear();
              repRelation1.locations.Clear();

              ex.mainFunction(fileName2);
              List<Elem> tab12 = repRelation1.locations;

              foreach (Elem e12 in tab12)
              {
                  names2.Add(e12.name);
              }
              repRelation1.locations.Clear();
              repRelation1.locationRp.Clear();
              ex.mainFunction(fileName);
              repRelation = Repository.getInstance();
              List<Element> tableRelation1 = repRelation1.locationRp;

              foreach (Element ee in tableRelation1) //to send the result of type-Analysis from server 1
              {
                  foreach (string name in names2)
                  {
                      if (name == ee.class1)
                      {
                          xml.displayrelationxml(ee.class1, ee.relation, ee.class2);
                          XDocument xdoc = XDocument.Load(@"..//Debug.xml");
                          IEnumerable<XElement> dataParent = (from x in xdoc.Descendants("ParentClass") select x).ToArray();
                          foreach (var data1 in dataParent)
                          {
                              listBox2.Items.Insert(0, data1);
                          }

                      }
                  }
              }
              listBox2.Items.Insert(0, "Result of Type-Analysis on Sever-2:");
              repRelation1.locationRp.Clear();
              repRelation1.locations.Clear();
              repRelation.locations.Clear();
              repRelation.locationRp.Clear();

          }
          catch (Exception exp)
          {
              Window temp = new Window();
              temp.Content = exp.Message;
              temp.Height = 100;
              temp.Width = 500;
          }
          Mouse.Capture(null);
      }
      //
      private void mergeFile(string file1,string file2)
      {
         string[] file_1 = File.ReadAllLines(file1);
         string[] file_2 = File.ReadAllLines(file2);
          
         List<String> fileName = new List<string>();
         fileName.Add(file1);
         fileName.Add(file2);

          string destPath = "..//..//..//WCF_Peer_Comm//FilesOnServer//";
          string destName = "mergeFile.txt";
          string destFileName = string.Concat(destPath, destName);
          using (Stream destStream = File.OpenWrite(destFileName))
          {
              foreach (string srcFileName in fileName)
            {
                using (Stream srcStream = File.OpenRead(srcFileName))
                {
                    srcStream.CopyTo(destStream);
                }
              }

          }
          
      }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {

        sndr.PostMessage("quit");
        sndr.Close();
        recvr.Close();
       
    }

    private void listBox1_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
  }
}
