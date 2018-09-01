///////////////////////////////////////////////////////////////////////
// CheckInControl.xaml.cs - Control GUI for Local CheckIn navigation  //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides a WPF-based control GUI for Remote repository.  It's 
* responsibilities are to:
* - Provide a CheckIn functionalities to user
*   
* Required Files:
* ---------------
* CheckInControl.xaml, CheckInControl.xaml.cs
* 
* Maintenance History:
* --------------------
* ver 1.0 : 22 Apr 2018
* - first release
*/

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
using MsgPassingCommunication;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for CheckInControl.xaml
    /// </summary>
    /// 

    public partial class CheckInControl : UserControl
    {
        internal Stack<string> pathStack_ = new Stack<string>();
        internal string localStorageRoot_;
        internal CsEndPoint navEndPoint_;
        internal Stack<string> pathStackRemote_ = new Stack<string>();

        public CheckInControl()
        {
            InitializeComponent();
        }
        //----< function for refresh display>----------------

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DirList.Items.Clear();
            string path = localStorageRoot_ + pathStack_.Peek();
            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                if (dir != "." && dir != "..")
                {
                    string itemDir = System.IO.Path.GetFileName(dir);
                    DirList.Items.Add(itemDir);
                }
            }
            DirList.Items.Insert(0, "..");

            FileList.Items.Clear();
            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string file in files)
            {
                string itemFile = System.IO.Path.GetFileName(file);
                FileList.Items.Add(itemFile);
            }
        }
        //----< function for refresh display>----------------

        internal void refreshDisplay()
        {
            Refresh_Click(this, null);
        }

        //----< function for refresh display>----------------

        internal void refreshDisplayRemote()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(navEndPoint_));
            msg.add("command", "getDirs");
            msg.add("path", pathStackRemote_.Peek());
            win.translater.postMessage(msg);
            msg.remove("command");
            msg.add("command", "getFiles");
            win.translater.postMessage(msg);
        }
        //----< strip off name of first part of path >---------------------

        public string removeFirstDir(string path)
        {
            string modifiedPath = path;
            int pos = path.IndexOf("/");
            modifiedPath = path.Substring(pos + 1, path.Length - pos - 1);
            return modifiedPath;
        }
        //----< function for refresh display>----------------

        internal void clearDirs()
        {
            DirList.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

        internal void addDir(string dir)
        {
            DirList.Items.Add(dir);
        }
        //----< function dispatched by child thread to main thread >-------

        internal void insertParent()
        {
            DirList.Items.Insert(0, "..");
        }
        //----< function dispatched by child thread to main thread >-------

        internal void clearFiles()
        {
            FileList.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

        internal void addFile(string file)
        {
            FileList.Items.Add(file);
        }

        internal void clearDirsR()
        {
            DirListRemote.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

        internal void addDirR(string dir)
        {
            DirListRemote.Items.Add(dir);
        }
        //----< function dispatched by child thread to main thread >-------

        internal void insertParentR()
        {
            DirListRemote.Items.Insert(0, "..");
        }
        //----< function dispatched by child thread to main thread >-------

        internal void clearFilesR()
        {
            FileListRemote.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

        internal void addFileR(string file)
        {
            FileListRemote.Items.Add(file);
        }
        //----< respond to mouse double-click on dir name >----------------

        private void DirList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);

            // build path for selected dir
            string selectedDir = (string)DirList.SelectedItem;
            string path;
            if (selectedDir == "..")
            {
                if (pathStack_.Count > 1)  // don't pop off "LocalStorage"
                    pathStack_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStack_.Peek() + "/" + selectedDir;
                pathStack_.Push(path);
            }
            // display path in Dir TextBlock
            // May use statement below later:
            // PathTextBlock.Text = win.removeFirstDir("LocalStorage/" + pathStack_.Peek());
            PathTextBlock.Text = "LocalStorage" + pathStack_.Peek();

            refreshDisplay();
        }

        //----< function to respond to CheckIn button-click>----------------

        private void CheckIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            CheckIn();
        }

        //----< function for checkIn>----------------

        void CheckIn()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending CheckIn request to server...";
            try
            {
                string fileName = (string)FileList.SelectedItem;
                string srcFile = localStorageRoot_ + pathStack_.Peek() + "/" + fileName;
                srcFile = System.IO.Path.GetFullPath(srcFile);
                string dstFile = win.sendFilesPath + "/" + fileName;
                System.IO.File.Copy(srcFile, dstFile, true);
                // prepare message to send as a request to server and send it
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                string status, category = "", dependency = "", author = "";
                author = AuthText.Text;
                foreach (ListBoxItem item in CatList.SelectedItems)
                    category = category + item.Content.ToString() + "-";
                if (OpenBtn.IsChecked == true)
                    status = "Open";
                else
                    status = "Close";
                string pathDep = pathStackRemote_.Peek();
                string pkg = pathDep.Substring(pathDep.LastIndexOf('/') + 1);
                if (pkg == "Storage")
                    pkg = "";
                foreach (string item in FileListRemote.SelectedItems)
                    dependency = dependency + pkg + "::" + item.ToString() + "-";
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(win.endPoint_));
                msg.add("command", "checkInFile");
                msg.add("sendingFile", fileName);
                msg.add("path", pathStack_.Peek());
                msg.add("fileName", fileName);
                msg.add("verbose", "sending checkin file");
                msg.add("author", author);
                msg.add("dependency", dependency);
                msg.add("status", status);
                msg.add("category", category);
                msg.show();
                win.translater.postMessage(msg);
            }
            catch (Exception e)
            {
                win.txtStatusBar.Text = "Something is wrong..." + e;
            }
        }

        //----< function to respond to CheckIn button-click>----------------
        void CheckInMsg(string fileName, string path, string author, string dependency, string status, string category)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending CheckIn request to server...";
            string srcFile = localStorageRoot_ + path + "/" + fileName;
            srcFile = System.IO.Path.GetFullPath(srcFile);
            string dstFile = win.sendFilesPath + "/" + fileName;
            System.IO.File.Copy(srcFile, dstFile, true);
            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "checkInFile");
            msg.add("sendingFile", fileName);
            msg.add("path", path);
            msg.add("fileName", fileName);
            msg.add("verbose", "sending checkin file");
            msg.add("author", author);
            msg.add("dependency", dependency);
            msg.add("status", status);
            msg.add("category", category);
            msg.show();
            win.translater.postMessage(msg);
        }

        //----< function for demonstartion>----------------

        public void demoreqClient()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            Console.WriteLine("\n  ----> Case 1: Check-in File which is present and in Open status:");
            Console.WriteLine("\n File Name: Comm.h");
            Console.WriteLine("\n Existing file will be overwritten");
            CheckInMsg("Comm.h", "/Comm", "John", "Message::Message.h.1-", "Open", "Header File-Comm Feature-");
            Console.WriteLine("\n  ----> Case 2: Check-in File which is not present");
            Console.WriteLine("\n File Name: NewFile.cpp");
            Console.WriteLine("\n File will be checkin and so new entry in repository");
            CheckInMsg("NewFile.h", "", "Chetan", "", "Open", "Source File-Functionality-");
            Console.WriteLine("\n  ----> Case 3: Check-in File which is in Close status. User has given new dependency and category here");
            Console.WriteLine("\n File Name: Message.cpp");
            Console.WriteLine("\n New version of file will be created.");
            CheckInMsg("Message.cpp", "/Message", "Parker", "Message::Message.h.1-", "Close", "Source File-Message Feature-Functionality-");
            Console.WriteLine("\n  ----> Case 4: Check-in File which is present but CheckIn person is not author of the file");
            Console.WriteLine("\n File Name: FileSystem.cpp");
            Console.WriteLine("\n So CheckIn will be denied..!!");
            CheckInMsg("FileSystem.cpp", "", "Kadam", "::Sockets.cpp.1-", "Close", "Source File-Functionality-");
        }

        //----< respond to mouse double-click on dir name >----------------

        private void DirListRemote_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);

            // build path for selected dir
            string selectedDir = (string)DirListRemote.SelectedItem;
            string path;
            if (selectedDir == "..")
            {
                if (pathStackRemote_.Count > 1)  // don't pop off "Storage"
                    pathStackRemote_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStackRemote_.Peek() + "/" + selectedDir;
                pathStackRemote_.Push(path);
            }
            // display path in Dir TextBlcok
            PathTextBlock.Text = win.removeFirstDir(pathStackRemote_.Peek());

            // build message to get dirs and post it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(navEndPoint_));
            msg.add("command", "getDirs");
            msg.add("path", pathStackRemote_.Peek());
            win.translater.postMessage(msg);

            // build message to get files and post it
            msg.remove("command");
            msg.add("command", "getFiles");
            win.translater.postMessage(msg);
        }

        //----< function for clearing all selection>----------------

        public void clearAll()
        {
            AuthText.Clear();
            OpenBtn.IsChecked = false;
            CloseBtn.IsChecked = false;
            CatList.UnselectAll();
            FileListRemote.UnselectAll();
            refreshDisplayRemote();

        }


    }
}
