using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PublicWebSms.Models;
using System.Configuration;

namespace PublicWebSms.Controllers
{
    public class MyController : Controller
    {
        public MyController()
        {
            ReportData report = new ReportData();
            report = ReportData.GetCurrentReportData();
            ViewBag.GatewayStatus = report.IsOn();
        }

    }
}
