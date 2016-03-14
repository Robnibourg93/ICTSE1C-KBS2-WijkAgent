using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using WijkAgentBeta.Models;
namespace WijkAgentBeta.ContentHandling
{

    class BackgroundProcessesController
    {
        /*
            Author: Ronne Timmerman
            Modified: Ronne Timmerman & Rob Nibourg
            Description: This class starts handles all the background proccesses.

        */
        RSSfeed Rss = new RSSfeed();
        PanicController panicController = new PanicController();
        Panic panic;
        public BackgroundProcessesController()
        {

        }
        //Made by Ronne Timmerman
        public void StartBackgroundProcesses()
        {
            while (true)
            {
                this.Rss.getRSSfeed();

                Thread.Sleep(30000);
            }

        }
        //Made by Ronne Timmerman
        //Edited bu Rob Nibourg
        //edited By Ronne Timmerman
        internal void StartBackgroundProcesses(MapController mapController)
        {
            while (true)
            {
                this.Rss.getRSSfeed();
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    //Console.WriteLine("Refreshing Map");
                    mapController.RefreshMap();
                    //Console.WriteLine("Map Refreshed");

                    //check if panic is active in database
                    if (this.panicController.checkPanic())
                    {
                        this.panic = this.panicController.panicModel;
                        if (this.panic.checkIfRelevand())
                        {
                            mapController.PanicStart(this.panic);
                        }
                    }
                    else
                    {
                        mapController.PanicStop();
                    }
                }));

                Thread.Sleep(2000);
            }

        }
    }
}
