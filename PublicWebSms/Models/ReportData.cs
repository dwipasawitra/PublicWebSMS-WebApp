using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PublicWebSms.Models
{
    public class ReportData
    {
        public DateTime LastUpdate { get; set; }
        public string ServerString { get; set; }
        public bool IsOn()
        {
            TimeSpan tressholdTime = LastUpdate - DateTime.Now;
            if (tressholdTime.Minutes > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static ReportData GetCurrentReportData()
        {
            ReportData report = new ReportData();
            report.LastUpdate = DateTime.Parse(ConfigurationManager.AppSettings["ServerLastUpdate"]);
            report.ServerString = ConfigurationManager.AppSettings["ServerString"];
            return report;
        }
    }
}