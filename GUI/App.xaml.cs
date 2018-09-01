using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
        void startWin(object sender, StartupEventArgs arg)
        {
            MainWindow win8082 = new WpfApp1.MainWindow(8082);
            win8082.Show();

         //   MainWindow win8084 = new WpfApp1.MainWindow(8084);
         //   win8084.Show();
        }
  }
}
