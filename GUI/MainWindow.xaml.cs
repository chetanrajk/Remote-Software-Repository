///////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - GUI for RemoteRepository                     //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides a WPF-based GUI for RemoteRepository demo.  It's 
* responsibilities are to:
* - Provide a display of directory contents of a remote ServerPrototype.
* - It provides a subdirectory list and a filelist for the selected directory.
* - You can navigate into subdirectories by double-clicking on subdirectory
*   or the parent directory, indicated by the name "..".
*   
* Required Files:
* ---------------
* Mainwindow.xaml, MainWindow.xaml.cs
* Translater.dll
* 
* Maintenance History:
* --------------------
* ver 2.0 : 22 Apr 2018
* - added tabbed display
* - moved remote file view to CheckOutControl
* - migrated some methods from MainWindow to CheckOutControl
* - added local file view
* ver 1.0 : 30 Mar 2018
* - first release
*/

// Translater has to be statically linked with CommLibWrapper
// - loader can't find Translater.dll dependent CommLibWrapper.dll
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Data.SqlClient;
using MsgPassingCommunication;
using System.IO;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow(int portno)
        {
            endPoint_ = new CsEndPoint();
            endPoint_.machineAddress = "localhost";
            endPoint_.port = portno;
            InitializeComponent();

            Console.Title = "Client side console";
            Console.WriteLine("\n  =====================================");
            Console.WriteLine("  Remote Code Repository - Client ");
            Console.WriteLine("  =====================================");
            this.Title = "GUI for Remote Repository - (Client: " + portno + ")";
            updateStatusBar("Client :" + portno);

        }

        private Stack<string> pathStack_ = new Stack<string>();
        internal Translater translater;
        internal CsEndPoint endPoint_;
        private Thread rcvThrd = null;
        private Dictionary<string, Action<CsMessage>> dispatcher_
        = new Dictionary<string, Action<CsMessage>>();
        internal string saveFilesPath;
        internal string sendFilesPath;

        //----< process incoming messages on child thread >----------------

        private void processMessages()
        {
            ThreadStart thrdProc = () =>
            {
                while (true)
                {
                    CsMessage msg = translater.getMessage();
                    try
                    {
                        if (msg.value("command") != "getFiles" && msg.value("command") != "getDirs")
                            msg.show();
                        string msgId = msg.value("command");
                        if (dispatcher_.ContainsKey(msgId))
                            dispatcher_[msgId].Invoke(msg);
                    }
                    catch (Exception e)
                    {
                        Console.Write("\n {0}", e.Message);
                        msg.show();
                    }
                }
            };
            rcvThrd = new Thread(thrdProc);
            rcvThrd.IsBackground = true;
            rcvThrd.Start();
        }
        //----< function to update status bar of GUI window>-------

        private void updateStatusBar(string msg)
        {
            txtStatusBar.Text = msg;
        }
        //----< add client processing for message with key >---------------

        private void addClientProc(string key, Action<CsMessage> clientProc)
        {
            dispatcher_[key] = clientProc;
        }
        //----< load getDirs processing into dispatcher dictionary >-------

        private void DispatcherLoadGetDirs()
        {
            Action<CsMessage> getDirs = (CsMessage rcvMsg) =>
            {
                Action clrDirs = () =>
    {
                NavChkOut.clearDirs();
                NavBrowse.clearDirs();
                NavChkIn.clearDirsR();
            };
                Dispatcher.Invoke(clrDirs, new Object[] { });
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("dir"))
                    {
                        Action<string> doDir = (string dir) =>
            {
                NavChkOut.addDir(dir);
                NavBrowse.addDir(dir);
                NavChkIn.addDirR(dir);
            };
                        Dispatcher.Invoke(doDir, new Object[] { enumer.Current.Value });
                    }
                }
                Action insertUp = () =>
    {
                NavChkOut.insertParent();
                NavBrowse.insertParent();
                NavChkIn.insertParentR();
            };
                Dispatcher.Invoke(insertUp, new Object[] { });
            };
            addClientProc("getDirs", getDirs);
        }
        //----< load getFiles processing into dispatcher dictionary >------

        private void DispatcherLoadGetFiles()
        {
            Action<CsMessage> getFiles = (CsMessage rcvMsg) =>
            {
                Action clrFiles = () =>
    {
                NavChkOut.clearFiles();
                NavBrowse.clearFiles();
                NavChkIn.clearFilesR();
            };
                Dispatcher.Invoke(clrFiles, new Object[] { });
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("file"))
                    {
                        Action<string> doFile = (string file) =>
            {
                NavChkOut.addFile(file);
                NavBrowse.addFile(file);
                NavChkIn.addFileR(file);
            };
                        Dispatcher.Invoke(doFile, new Object[] { enumer.Current.Value });
                    }
                }
            };
            addClientProc("getFiles", getFiles);
        }
        //----< load checkInFile processing into dispatcher dictionary >------

        private void DispatcherLoadCheckIn()
        {
            Action<CsMessage> checkInFile = (CsMessage rcvMsg) =>
            {
                string str = "Received response from server to CheckIn...!!";
                Action<string> doUpdate = (String str1) =>
    {
                updateStatusBar(str1);
                NavChkIn.clearAll();
            };
                Dispatcher.Invoke(doUpdate, new Object[] { str });
            };
            addClientProc("checkInFile", checkInFile);
        }
        //----< load checkOutFile processing into dispatcher dictionary >------

        private void DispatcherLoadCheckOut()
        {
            Action<CsMessage> checkOutFile = (CsMessage rcvMsg) =>
            {
                string fileName = "";
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("sendingFile"))
                    {
                        fileName = enumer.Current.Value;
                        break;
                    }
                }
                if (fileName.Length > 0)
                {
                    Action<string> act = (string fileNm) =>
        {
                string srcFile = saveFilesPath + "/" + fileName;
                string dstFile = NavChkIn.localStorageRoot_ + "/CheckedOut Files" + "/" + fileName;
                dstFile = System.IO.Path.GetFullPath(dstFile); ;
                System.IO.File.Copy(srcFile, dstFile, true);

            };
                    Dispatcher.Invoke(act, new object[] { fileName });
                    string str = "CheckOut Successful: Please check Local Storage folder..!!";
                    Action<string> doUpdate = (String str1) =>
        {
                updateStatusBar(str1);
            };
                    Dispatcher.Invoke(doUpdate, new Object[] { str });
                }
            };
            addClientProc("checkOutFile", checkOutFile);

        }
        //----< load browseFiles processing into dispatcher dictionary >------

        private void DispatcherLoadBrowse()
        {
            Action<CsMessage> browseFiles = (CsMessage rcvMsg) =>
            {
                string filesResult = "";
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("resultFiles"))
                    {
                        filesResult = enumer.Current.Value; break;
                    }
                }
                if (filesResult.Length > 0)
                {
                    Action<string> act = (string resultFiles) => { NavBrowse.showFileList(resultFiles); };
                    Dispatcher.Invoke(act, new object[] { filesResult });
                }
                else
                {
                    string str = "No files present in respository matching selected filters...";
                    Action<string> doUpdate = (String str1) =>
        {
                clearBrowse(str1);
            };
                    Dispatcher.Invoke(doUpdate, new Object[] { str });

                }
            };
            addClientProc("browseFiles", browseFiles);
        }

        //----< function for clearing browse selections>------

        void clearBrowse(string str1)
        {
            NavBrowse.clearAll();
            NavBrowse.FileListRes.Items.Clear();
            updateStatusBar(str1);
        }

        //----< load connectToServer processing into dispatcher dictionary >------

        private void DispatcherLoadConnectToServer()
        {
            Action<CsMessage> connectToServer = (CsMessage rcvMsg) =>
            {
                string str = "Client is already connected to Server..!!!";
                Action<string> doUpdate = (String str1) =>
    {
                Console.WriteLine("\n  Received response from server to connectToServer request...!!");
                updateStatusBar(str1);
            };
                Dispatcher.Invoke(doUpdate, new Object[] { str });
            };
            addClientProc("connectToServer", connectToServer);
        }

        //----< load fileText processing into dispatcher dictionary >------

        private void DispatcherLoadFileText()
        {
            Action<CsMessage> fileText = (CsMessage rcvMsg) =>
            {
                string fileName = "";
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("sendingFile"))
                    {
                        fileName = enumer.Current.Value;
                        break;
                    }
                }
                if (fileName.Length > 0)
                {
                    Action<string> act = (string fileNm) => { NavBrowse.showFile(fileNm, rcvMsg); };
                    Dispatcher.Invoke(act, new object[] { fileName });
                    string str = "File text received from server and opened in new window";
                    Action<string> doUpdate = (String str1) =>
        {
                updateStatusBar(str1);
            };
                    Dispatcher.Invoke(doUpdate, new Object[] { str });
                }
            };
            addClientProc("fileText", fileText);
        }

        //----< load all dispatcher processing >---------------------------

        private void loadDispatcher()
        {
            DispatcherLoadGetDirs();
            DispatcherLoadGetFiles();
            DispatcherLoadCheckIn();
            DispatcherLoadCheckOut();
            DispatcherLoadBrowse();
            DispatcherLoadConnectToServer();
            DispatcherLoadFileText();
        }
        //----< start Comm, fill window display with dirs and files >------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // start Comm
            try
            {
                NavChkOut.navEndPoint_ = endPoint_;
                NavBrowse.navEndPoint_ = endPoint_;
                NavChkIn.navEndPoint_ = endPoint_;

                translater = new Translater();
                translater.listen(endPoint_);

                // start processing messages
                processMessages();
                // load dispatcher
                loadDispatcher();
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                pathStack_.Push("../Storage");

                NavChkOut.PathTextBlock.Text = "Storage";
                NavChkOut.pathStack_.Push("../Storage");

                NavChkIn.PathTextBlock.Text = "LocalStorage";
                NavChkIn.pathStack_.Push("");
                NavChkIn.localStorageRoot_ = "../../../../LocalStorage";

                NavChkIn.pathStackRemote_.Push("../Storage");

                NavBrowse.PathTextBlock.Text = "Storage";
                NavBrowse.pathStack_.Push("../Storage");

                saveFilesPath = translater.setSaveFilePath("../../../SaveFiles");
                sendFilesPath = translater.setSendFilePath("../../../SendFiles");

                NavChkIn.refreshDisplay();
                NavChkOut.refreshDisplay();
                NavBrowse.refreshDisplay();
                NavChkIn.refreshDisplayRemote();
                //Automated test suit to demonstrate all requirements
                testRequirements();
            }
            catch (Exception ex)
            {
                txtStatusBar.Text = "Something is wrong..." + ex;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //----< strip off name of first part of path >---------------------

        public string removeFirstDir(string path)
        {
            string modifiedPath = path;
            int pos = path.IndexOf("/");
            modifiedPath = path.Substring(pos + 1, path.Length - pos - 1);
            return modifiedPath;
        }

        //----< function to respond to ConnectToServer button-click>----------------

        private void Connect_ButtonClick(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
        }
        //----< function to send ConnectToServer message request>----------------

        void ConnectToServer()
        {
            txtStatusBar.Text = "Sending ConnectToServer request...";

            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "connectToServer");
            msg.add("verbose", "connection request");
            msg.show();
            translater.postMessage(msg);
        }

        //----< function to send ConnectToServer message request>----------------

        void donotReply()
        {
            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "doNotReply");
            msg.add("verbose", "Do not reply message - one way");
            msg.show();
            translater.postMessage(msg);
        }

        //----< function to demonstrate all function requirements>----------------

        public void testRequirements()
        {
            if (endPoint_.port == 8082)
            {
                Console.WriteLine("\n\n  ===============================================");
                Console.WriteLine("            Demonstrating Requirements ");
                Console.WriteLine("  ===============================================");

                Console.WriteLine("\n\n  Demonstrating Requirement No. 1 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Using Visual Studio 2017 and the standard C++ libraries. Also for client C#, WPF and  C++\\CLI is used");

                Console.WriteLine("\n\n  Demonstrating Requirement No. 7 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Following automated test unit is sending requests one by one to demonstrate requirements");

                Console.WriteLine("\n\n  Demonstrating Requirement No. 4 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Sending connect to server message through message-passing communication system");
                ConnectToServer();
                Console.WriteLine("\n  Demonstrating Requirement No. 5 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Sending \"Do Not Reply\" HTTP style one way asynchronous message");
                donotReply();
                Console.WriteLine("\n  Demonstrating Requirement No. 6 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Following CheckIn, CheckOut and Browse requests will show sending or receiving block bytes in file transfer");
                Console.WriteLine("\n  Demonstrating Requirement No. 3");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Provided Client GUI that enables user to Check-in, Check-out and Brwose files");
                Console.WriteLine("\n  Demonstrating Requirement No. 2 ");
                Console.WriteLine("  =====================================");
                Console.WriteLine("\n  Provided a Repository Server that provides functionality to check-in, check-out, and browse");
                Console.WriteLine("\n  Following CheckIn, CheckOut and Browse parts will demonstrate each functionality");
                Console.WriteLine("\n  This console will display Client side and server console will display Server side");
                Console.WriteLine("\n\n  Browse Part");
                Console.WriteLine("  =====================");
                NavBrowse.demoreqClient();
                Console.WriteLine("\n\n  Check-out Part");
                Console.WriteLine("  =====================");
                NavChkOut.demoreqClient();
                Console.WriteLine("\n\n  Check-in Part");
                Console.WriteLine("  =====================");
                NavChkIn.demoreqClient();
                Console.WriteLine("\n\n===== This automated test unit demonstrated main cases in CheckIn, CheckOut and Browse parts  =====");
                Console.WriteLine("\n===== Please use GUI window for your demonstration of Remote Code Repository =====\n");
            }
        }

    }
}
