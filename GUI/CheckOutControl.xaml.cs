///////////////////////////////////////////////////////////////////////
// CheckOutControl.xaml.cs - Control GUI for Local CheckOut navigation  //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides a WPF-based control GUI for Remote repository.  It's 
* responsibilities are to:
* - Provide a CheckOut functionalities to user
*   
* Required Files:
* ---------------
* CheckOutControl.xaml, CheckOutControl.xaml.cs
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
    /// Interaction logic for CheckOutControl.xaml
    /// </summary>
    public partial class CheckOutControl : UserControl
    {
        internal CsEndPoint navEndPoint_;
        internal Stack<string> pathStack_ = new Stack<string>();

        public CheckOutControl()
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

        //----< function to respond to CheckOut button-click>----------------

        private void CheckOut_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            try
            {
                CheckOut(FileList.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                win.txtStatusBar.Text = "Something is wrong..." + ex;
            }
        }

        //----< function for demonstration>----------------

        public void demoreqClient()
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            Console.WriteLine("\n  ----> Case 1: Check-out a particular file from repository");
            Console.WriteLine("\n File Name: Comm.cpp.1");
            Console.WriteLine("\n File will be checked out in local CheckedOut Files folder without version no.");
            checkOutMsg("Comm.cpp.1", "../Storage/Comm");
        }
        //----< function to send CheckOut message request>----------------

        void CheckOut(string fileName)
        {

            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending CheckOut request to server...";

            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "checkOutFile");
            msg.add("fileName", fileName);
            msg.add("verbose", "checkOut a file");
            msg.add("path", pathStack_.Peek());
            msg.show();
            win.translater.postMessage(msg);

        }

        //----< function for check out message>----------------

        void checkOutMsg(string fileName, string path)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.txtStatusBar.Text = "Sending CheckOut request to server...";
            // prepare message to send as a request to server and send it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(win.endPoint_));
            msg.add("command", "checkOutFile");
            msg.add("fileName", fileName);
            msg.add("verbose", "checkOut a file");
            msg.add("path", path);
            msg.show();
            win.translater.postMessage(msg);
        }

    }
}
