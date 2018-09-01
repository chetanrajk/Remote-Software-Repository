///////////////////////////////////////////////////////////////////////
// CodePopUpWindow.xaml.cs - Window for viewing full file text       //
// ver 1.0                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides a WPF-based window for displaying full file text upon
 * double clicking on file from filelist in GUI window
 * Required Files:
 * ---------------
 * CodePopUpWindow.xaml, CodePopUpWindow.xaml.cs
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 30 Mar 2018
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class CodePopUpWindow : Window
    {
        public CodePopUpWindow()
        {
            InitializeComponent();
        }
    }
}
