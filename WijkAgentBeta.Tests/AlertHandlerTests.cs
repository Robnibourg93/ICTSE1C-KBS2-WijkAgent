using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WijkAgentBeta;
using WijkAgentBeta.ContentHandling;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class AlertHandlerTests
    {
        [TestMethod]
        public void AlertHandler_getAllAlerts_request()
        {
            AlertController alertlist = new AlertController();
            List<Alert> List;
            Boolean result = true;

            List = alertlist.getAllAlerts();

            foreach (Alert a in List)
            {
                if (a.report == "")
                {
                    result = false;
                }
            }

            Assert.IsTrue(result);
        }
    }
}
