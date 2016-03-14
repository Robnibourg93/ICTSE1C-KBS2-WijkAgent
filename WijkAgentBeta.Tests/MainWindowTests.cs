using Microsoft.VisualStudio.TestTools.UnitTesting;
using WijkAgentBeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using Microsoft.Maps.MapControl.WPF;
using System.Threading.Tasks;

namespace UnitTests.Tests
{
    [TestClass()]
    public class MainWindowTests
    {


        [TestMethod()]
        public void GetLocationEventTest()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            bool succes;
            bool expected;

            //Hier wordt de applicatie gestart. Als deze stert niet lukt dan wordt hier een melding van gegeven.
            bool started = watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
            if (!started)
            {
                succes = false;
            }
            else succes = true;

            expected = true;
            Assert.AreEqual(expected, succes);

        }

        [TestMethod()]
        public void PrintPositionTest()
        {
            Location location = new Location();
            double Longitude = 52.1234;
            double Latitude = 6.09123;
            bool succes;
            bool expected;

            //Dit maakt van de komma in de double Latitude en Longtitude een punt waardoor deze gegevens als gps coordinaat gebruikt kunnen worden.
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            location.Longitude = Longitude;
            location.Latitude = Latitude;

            if (location != null)
            {
                succes = true;
            }
            else succes = false;

            expected = true;
            Assert.AreEqual(expected, succes);
        }
    }
}