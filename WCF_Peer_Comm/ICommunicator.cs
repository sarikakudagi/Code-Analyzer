//////////////////////////////////////////////////////////////////////
// ICommunicator.cs - WCF Service Contract                          //
//                                                                  //
// Jim Fawcett, CSE681 - Software Modeling & Analysis, Summer 2008  //
//Modified: sarika shrishail kudagi, 726869838, Syracuse University //
//              (551) 998-2431, sskudagi@syr.edu                    //
/////////////////////////////////////////////////////////////////////

/*Module Operations
 * ---------------------
 * This WCF Service connects the client WCF_GUI to Server-1
 * [ServiceContract]: used to declare a service contract.
 * [OperationContract]: Declare methods which belong to service method.
 * [DataContract]: declare the data being used in the serviceContract
 * 
 * Maintenance History:
 * ====================
 * ver 2.0 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * ver 1.0 : 14 Jul 07
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SWTools;
using System.IO;

namespace WCF_Peer_Comm
{
    [DataContract(Namespace = "WCF_Peer_Comm")] //used to describe the form of data being sent between client and server
    public class SvcMsg
    {
        public enum CommandP { fileName, package1,package2,relationp };
        public enum CommandT {class1, class2, relationt }
        [DataMember]
        public CommandP cmdP;
        [DataMember]
        public CommandT cmdT;
        [DataMember]
        public string fileName;

        [DataMember]
        public string package1;
        [DataMember]
        public string package2;
        [DataMember]
        public string relationp;
        [DataMember]
        public string class1;
        [DataMember]
        public string class2;
        [DataMember]
        public string relationt;

        public void ShowMessage()
        {
            Console.Write("\n  Received Message:");
        //    Console.Write("\n    src = {0}\n    dst = {1}", src.ToString(), dst.ToString());
            Console.Write("\n    cmd = {0}", cmdP.ToString());
           
        }
    }
    
  [ServiceContract]
  public interface ICommunicator
  {
    [OperationContract(IsOneWay = true)]
    void PostMessage(string msg);
    [OperationContract(IsOneWay = true)]
    void PostResult(List<string> fileList);

    // used only locally so not exposed as service method

    string GetMessage();
    List<string> GetFMessage();


  }
}
