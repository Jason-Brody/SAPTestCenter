using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SAPTest.Model
{
    //this.Configuration.ProxyCreationEnabled = false;
    public static class ReportReader
    {
        public static Report ReadReport(XmlDocument xDoc)
        {
            Report report = new Report();
            report.TestName = readContent(xDoc, "ReportRoot/Summary/TestName", "No Test Name found");
            //report.Executor = readContent(xDoc, "ReportRoot/Summary/Executor", "No Executor Found");
            string result = readContent(xDoc, "ReportRoot/Summary/OverallStatus", "No OverallStatus Found");
            report.CompanyCode = readContent(xDoc, "ReportRoot/Summary/CompanyCode", "No CompanyCode Found");
            report.CaseNum = report.TestName.Split('_')[1];
            if(result.ToLower()=="pass")
            {
                report.TestResult = true;
            }
            else
            {
                report.TestResult = false;
            }
            
            report.SubmitDt = DateTime.Now;
            return report;
        }

        private static string readContent(XmlDocument xDoc, string xPath,string ErrorMessage)
        {
            XmlNode node = xDoc.SelectSingleNode(xPath);
            if(node!=null)
            {
                return node.InnerText;
            }
            else
            {
                throw new Exception(ErrorMessage);
            }
        }
    }
}
