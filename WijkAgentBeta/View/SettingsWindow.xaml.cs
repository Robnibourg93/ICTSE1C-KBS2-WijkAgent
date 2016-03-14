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
using System.Windows.Shapes;
using WijkAgentBeta.ContentHandling;

namespace WijkAgentBeta.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static bool ApplyPressed;
        public static string setHours;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainUserControl.WindowOpen = false;
        }

        public void Apply(object sender, RoutedEventArgs e)
        {
            ApplyPressed = true;
            this.Close();
            MainUserControl.WindowOpen = false;
        }

        private void LocationTrackingBox_Click(object sender, RoutedEventArgs e)
        {
            if (LocationTrackingBox.IsChecked.Value)
            {
                MapController.userTrackingOn = true;
            }
            else {
                MapController.userTrackingOn = false;
            }
        }
    }
}
