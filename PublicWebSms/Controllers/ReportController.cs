using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PublicWebSms.Models;
using System.Configuration;

namespace PublicWebSms.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            ReportData report = new ReportData();

            try
            {
                report = ReportData.GetCurrentReportData();
            }
            catch
            {
                return View("Error");
            }

            return View(report);
            
        }

        public ActionResult ReportAPI(ReportData reportData, SMSAPIInput apiData)
        {
            // Process wtih some secret data
            if (apiData.APIId == "hahaha" && apiData.APISecretCode == "hihihi")
            {
                ConfigurationManager.AppSettings["ServerLastUpdate"] = reportData.LastUpdate.ToString();
                ConfigurationManager.AppSettings["ServerString"] = reportData.ServerString;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}
