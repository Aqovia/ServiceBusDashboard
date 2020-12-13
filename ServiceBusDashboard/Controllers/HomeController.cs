using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceBusDashboard.Code;

namespace ServiceBusDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(SbConnectionStringsLoader.Instance.ConnectionStrings);
        }

        public ActionResult ReloadList()
        {
            SbConnectionStringsLoader.Instance.Reload();
            return RedirectToAction("Index");
        }

        public ActionResult ServiceBus(string groupName, string name)
        {
            return View(SbConnectionStringsLoader.Instance.FindConnectionString(groupName, name));
        }
    }
}
