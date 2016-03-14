using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WijkAgentBeta.ContentHandling;

namespace WijkAgentBeta
{
    /*
      Author: Rob Nibourg
      Modified by: Rob Nibourg & Joshua van Gelder

      Explain the main thing this class does.
     */
    public partial class MapUserControl : UserControl
    {
        AlertController alertcontroller = new AlertController();
        public List<Pushpin> PinsAddedWithMouse = new List<Pushpin>();

        public MapUserControl()
        {
            InitializeComponent();
        }
        //Made by Joshua van Gelder
        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mouseposition = e.GetPosition(this);
            Location pinlocation = Map.ViewportPointToLocation(mouseposition);

            Pushpin pin = new Pushpin();
            pin.Location = pinlocation;

            PinsAddedWithMouse.Add(pin);
            Map.Children.Add(pin);
        }
    }
}
