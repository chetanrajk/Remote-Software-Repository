///////////////////////////////////////////////////////////////////////
// BrowseControl.xaml.cs - Control GUI for Local Browse navigation  //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides a WPF-based control GUI for Remote repository.  It's 
* responsibilities are to:
* - Provide a browse functionalities to user
*   
* Required Files:
* ---------------
* BrowseControl.xaml, BrowseControl.xaml.cs
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
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for BrowseControl.xaml
    /// </summary>
    public partial class BrowseControl : UserControl
    {
        internal CsEndPoint navEndPoint_;
        internal Stack<string> pathStack_ = new Stack<string>();

        //----< function for refresh display>----------------

        public BrowseControl()
        {
            InitializeComponent();
        }

        //----< function for refresh display>----------------

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(navEndPoint_));
            msg.add("command", "getDirs");
            msg.add("path", pathStack_.Peek());
            win.translater.postMessage(msg);
            msg.remove("command");
            msg.add("command", "getFiles");
            win.translater.postMessage(msg);
        }

        //----< function for refresh display>----------------

        internal void refreshDisplay()
        {
            Refresh_Click(this, null);
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
        //----< show file text on MouseDoubleClick >-----------------------
        //----< respond to mouse double-click on dir name >----------------

        private void DirList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);

            // build path for selected dir
            string selectedDir = (string)DirList.SelectedItem;
            string path;
            if (selectedDir == "..")
            {
                if (pathStack_.Count > 1)  // don't pop off "Storage"
                    pathStack_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStack_.Peek() + "/" + selectedDir;
                pathStack_.Push(path);
            }
            // display path in Dir TextBlcok
            PathTextBlock.Text = win.removeFirstDir(pathStack_.Peek());

            // build message to get dirs and post it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(navEndPoint_));
            msg.add("command", "getDirs");
            msg.add("path", pathStack_.Peek());
            win.translater.postMessage(msg);

            // build message to get files and post it
            msg.remove("command");
            msg.add("command", "getFiles");
            win.translater.postMessage(msg);
        }

        //----< function to respond to Browse button-click>----------------

        private void Browse_ButtonClick(object sender, RoutedEventArgs e)
        {
            Browse();
        }
        //----< function to send Browse message request>----------------

        public void demoreqClient()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            Console.WriteLine("\n  ----> Case A: Browse through repository and view full file text and metadata of a particular file");
            Console.WriteLine("\n  File Name- Comm.h.1");
            Console.WriteLine("\n  New window will be opened with full file text and metadata");
            ViewFullFileTextMsg("Comm.h.1", "../Storage/Comm");
            Console.WriteLine("\n  ----> Case B: Browse through repository by using filters");
            Console.WriteLine("\n  ----> Query 1: Return files matching file name as \"Message\"");
            BrowseMsg("Message", "", "", "", "");
            Console.WriteLine("\n  ----> Query 2: Return files matching file name as \"Comm\" and with Version \"1\"");
            BrowseMsg("Comm", "", "", "1", "");
            Console.WriteLine("\n  ----> Query 3: Return files whose dependency is \"Sockets.cpp.1\"");
            BrowseMsg("", "", "", "Sockets.cpp.1", "");
            Console.WriteLine("\n  ----> Query 4: Return files in a \"Header File\" category");
            BrowseMsg("", "Header File", "", "", "");
            Console.WriteLine("\n  ----> Query 5: Return all files in any category that have no parents ");
            BrowseMsg("", "", "", "", "Yes");
        }

        //----< function for browse>----------------

        void Browse()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            try
            {
                win.txtStatusBar.Text = "Sending browseFiles request to server...";

                // prepare message to send as a request to server and send it
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();

                string qFileName = txtFname.Text;
                string qDep = txtDep.Text;
                string qVer = txtVer.Text;
                string qParent = "";
                if ((bool)qChkParent.IsChecked)
                    qParent = "Yes";
                string category = "";
                foreach (ListBoxItem item in qCatList.SelectedItems)
                    category = category + item.Content.ToString();

                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(win.endPoint_));
                msg.add("command", "browseFiles");
                msg.add("verbose", "browse files by queries");
                msg.add("qFileName", qFileName);
                msg.add("qCategory", category);
                msg.add("qDependency", qDep);
                msg.add("qVersion", qVer);
                msg.add("qParent", qParent);
                msg.show();
                win.translater.postMessage(msg);
            }
            catch (Exception ex)
            {
                win.txtStatusBar.Text = "Something is wrong..." + ex;
            }
        }

        //----< function for browse message>----------------

        void BrowseMsg(string qFileName, string category, string qDep, string qVer, string qParent)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending browseFiles request to server...";

            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "browseFiles");
            msg.add("verbose", "browse files by queries");
            msg.add("qFileName", qFileName);
            msg.add("qCategory", category);
            msg.add("qDependency", qDep);
            msg.add("qVersion", qVer);
            msg.add("qParent", qParent);
            msg.show();
            win.translater.postMessage(msg);

        }

        //----< function to respond to fileList item double-click>----------------

        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            try
            {
                ViewFullFileText(FileList.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                win.txtStatusBar.Text = "Something is wrong..." + ex;
            }
        }

        //----< function to send fileText message request>----------------

        void ViewFullFileText(String fileName)
        {
            // prepare message to send as a request to server and send it
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending FileText request to server...";
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "fileText");
            msg.add("fileName", fileName);
            msg.add("verbose", "view full file text and metadata");
            msg.add("path", pathStack_.Peek());
            msg.show();
            win.translater.postMessage(msg);
        }
        //----< function for browse full file text>----------------
        void ViewFullFileTextMsg(String fileName, string path)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending FileText request to server...";
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "fileText");
            msg.add("fileName", fileName);
            msg.add("verbose", "view full file text and metadata");
            msg.add("path", path);
            msg.show();
            win.translater.postMessage(msg);
        }
        //----< function to popup new window and display full file text>----------------

        public void showFile(string fileName, CsMessage msg)
        {
            // reads file into string and display it in new popup window
            MainWindow win = (MainWindow)Window.GetWindow(this);
            string path = win.saveFilesPath + "\\" + fileName;
            string fileContents = File.ReadAllText(path);
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(fileContents));
            CodePopUpWindow popUp = new CodePopUpWindow();
            popUp.codeView.Blocks.Clear();
            popUp.codeLabel.Text = fileName;
            popUp.codeView.Blocks.Add(paragraph);
            string auth, stat, date, cat, dep, ver;
            msg.attributes.TryGetValue("author", out auth);
            msg.attributes.TryGetValue("status", out stat);
            msg.attributes.TryGetValue("datetime", out date);
            msg.attributes.TryGetValue("categories", out cat);
            msg.attributes.TryGetValue("dependencies", out dep);
            msg.attributes.TryGetValue("version", out ver);
            popUp.dauth.Text = auth;
            popUp.dstat.Text = stat;
            popUp.ddate.Text = date;
            popUp.dcat.Text = cat;
            popUp.ddep.Text = dep;
            popUp.dver.Text = ver;
            popUp.Show();
        }
        //----< function for showing file list>----------------
        public void showFileList(string resultFiles)
        {
            string[] resFiles = resultFiles.Split('-');
            FileListRes.Items.Clear();
            Console.WriteLine("\n  Received Files as a result of a Query:\n");
            foreach (string file in resFiles)
            {
                FileListRes.Items.Add(file);
                Console.WriteLine("  File: " + file);
            }
            Console.WriteLine("\n  Result File List updated on GUI");
            clearAll();
        }
        //----< function for clear all selections>----------------
        public void clearAll()
        {
            txtFname.Clear();
            txtDep.Clear();
            txtVer.Clear();
            qChkParent.IsChecked = false;
            qCatList.UnselectAll();
        }
        //----< function for double click>----------------
        private void FileListRes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            try
            {
                ViewFullFileText(FileListRes.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                win.txtStatusBar.Text = "Something is wrong..." + ex;
            }
        }
    }
}
