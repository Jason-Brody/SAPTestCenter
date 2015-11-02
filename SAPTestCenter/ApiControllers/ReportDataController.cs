using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAPTest.Model;
using System.Web.Hosting;
using System.IO;


namespace SAPTestCenter.Controllers
{
    public class ReportDataController : ApiController
    {
        [HttpGet]
        public List<Project> GetProjects()
        {
            List<Project> projects = null;
            using(var db = new SAPTestContext())
            {
                projects = db.Projects.AsQueryable().ToList();
            }
            return projects;
        }

        [HttpGet]
        public List<Release> GetReleases(int id)
        {
            List<Release> releases = null;
            using (var db = new SAPTestContext())
            {
                releases = db.Releases.Where(c => c.Pid == id).AsQueryable().ToList();
            }
            return releases;
        }

        [HttpGet]
        public List<Asset> GetAssets(int id)
        {
            List<Asset> assets = null;
            using (var db = new SAPTestContext())
            {
                assets = db.Assets.Where(c => c.Rid == id).AsQueryable().ToList();
            }
            return assets;
        }

        [HttpPost]
        public void DeleteReport(Report DelReport)
        {
            string deleteFolder = string.Empty;
            
            var id = DelReport.Id;

            using (var db = new SAPTestContext())
            {
                var report = db.Reports.Find(id);
                if(report!=null && report.Executor == User.Identity.Name)
                {
                    deleteFolder = Path.Combine(HostingEnvironment.MapPath("/Report1/ReportFiles"), report.Url);
                    db.Reports.Remove(report);
                    db.SaveChanges();
                }
            }

            if (deleteFolder != "" && deleteFolder != HostingEnvironment.MapPath("/Report1/ReportFiles"))
            {
                if (Directory.Exists(deleteFolder))
                {
                    Directory.Delete(deleteFolder, true);
                }
            }
        }

        [HttpPost]
        public void UpdateReport(Report rp)
        {
            var id = rp.Id;
            var atId = rp.AssetId;
            using (var db = new SAPTestContext())
            {
                var report = db.Reports.Find(id);
                var asset = db.Assets.Find(atId);
                if(report!=null && asset != null && report.Executor == User.Identity.Name)
                {
                    report.AssetId = atId;
                    db.SaveChanges();
                }
            }
        }
    }

    
}
