using NeonatalPhysiologyGUI.ViewModels;
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

namespace NeonatalPhysiologyGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;


        public MainWindow()
        {
            InitializeComponent();

            // first find the properties of the current screen for adpative purposes
            double screen_x = SystemParameters.PrimaryScreenWidth;
            double screen_y = SystemParameters.PrimaryScreenHeight;
            DpiScale dpi = VisualTreeHelper.GetDpi(this);
            double dpi_scale = dpi.DpiScaleX;

            // instatiate the mainwindow viewmodel
            mainViewModel = new MainViewModel(screen_x, screen_y, dpi_scale);

            // set the datacontext
            DataContext = mainViewModel;

        }
    }
}
