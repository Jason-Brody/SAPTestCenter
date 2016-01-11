using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAPTest.Model;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.IO.Compression;
using System.Xml;
using SAPTestCenter.Models;
using SAPTestCenter.Filters;

namespace SAPTestCenter.Controllers
{
    public class ReportController : SAPBaseController
    {
        private const int _numInPage = 20;
        // GET: Report
        public ActionResult Index(MyReportFilter Filter,int page = 1)
        {
            ReportInfo ri = getReports(Filter,page);
            ri.Request = Request.Url;
            return View(ri);
        }


        private ReportInfo getReports(MyReportFilter Filter,int page)
        {
            
                
            ReportInfo ri = new ReportInfo();
            ri.Filter = Filter;
            var u = InternalAttribute.GetUser();
            if (u != null)
                ri.IsVaildUser = true;


            string user = User.Identity.Name.Trim().ToLower();

            ViewBag.Message = "";
            
            using (var db = new SAPTestContext())
            {
                IQueryable<Report> reportQuery = db.Reports.Include(r => r.Asset.Release.Project).Include(r => r.User);
                if(Filter.qs != null)
                {
                    reportQuery = reportQuery.Where(r=>r.TestName.Contains(Filter.qs));
                    ViewBag.Message += " Test Name contains:" + Filter.qs +"\n";
                }
                if (Filter.Pid > 0)
                {
                    reportQuery = reportQuery.Where(r=>r.Asset.Release.Project.Id == Filter.Pid);
                    ViewBag.Message += "Project:" + db.Projects.Find(Filter.Pid).Name + "\n";
                }
                if (Filter.Rid > 0)
                {
                    reportQuery = reportQuery.Where(r=>r.Asset.Release.Id == Filter.Rid);
                    ViewBag.Message += "Release:" + db.Releases.Find(Filter.Rid).Name + "\n";
                }
                if (Filter.Aid > 0)
                {
                    reportQuery = reportQuery.Where(r=>r.Asset.Id == Filter.Aid);
                    ViewBag.Message += "Asset:" + db.Assets.Find(Filter.Aid).Name + "\n";
                }


                ri.TotalReportNum = reportQuery.Count();

                if (Filter.isMyReport)
                {
                    ViewBag.PageCount = Math.Ceiling((double)ri.MyReportNum / _numInPage);
                    ri.IsMyReport = true;

                    reportQuery = reportQuery.Where(r => r.Executor.Trim().ToLower() == user);
                    ri.MyReportNum = reportQuery.Count();
                    //ri.Reports = reportQuery.Where(r => r.Executor.Trim().ToLower() == user).OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                }
                else
                {
                    ri.MyReportNum = reportQuery.Where(r => r.Executor.Trim().ToLower() == user).Count();
                    //ri.Reports = reportQuery.OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                    ViewBag.PageCount = Math.Ceiling((double)ri.TotalReportNum / _numInPage);
                }

                

                if (Filter.Sort == -1)
                {
                    reportQuery = reportQuery.OrderByDescending(c => c.SubmitDt);
                }
                else if (Filter.Sort == 0)
                {
                    reportQuery = reportQuery.OrderBy(c => c.CaseNum).ThenByDescending(c => c.SubmitDt);
                }
                else
                {
                    reportQuery = reportQuery.OrderByDescending(c => c.CaseNum).ThenByDescending(c => c.SubmitDt);
                }

                ri.Reports = reportQuery.Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                

                
            }

            
            ViewBag.CurrentPage = page;

            return ri;
            
        }

        
        public ActionResult Upload()
        {
            var u = InternalAttribute.GetUser();
            if (u!=null)
            {
                HttpPostedFileBase file = Request.Files["file"];
                string guid = Guid.NewGuid().ToString();
                string path = HttpContext.Server.MapPath("/Report1/Temp");
                string backupPath = HttpContext.Server.MapPath("/Report1/Backup");
                string reportFolder = HttpContext.Server.MapPath("/Report1/ReportFiles");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                string fileName = Path.Combine(path, guid + "_" + file.FileName.Split('\\').Last());
                string type = file.ContentType;
                file.SaveAs(fileName);
                FileInfo fi = new FileInfo(fileName);
                int atId;
                try
                {
                    if (string.Compare(fi.Extension, ".zip", true) == 0 && int.TryParse(Request.Form["asset"], out atId) && atId > 0)
                    {
                        ZipFile.ExtractToDirectory(fileName, Path.Combine(reportFolder, guid));
                        string reportFile = Path.Combine(reportFolder, guid, "report.xml");
                        if (System.IO.File.Exists(reportFile))
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(reportFile);
                            Report rp = ReportReader.ReadReport(xDoc);
                            rp.Executor = User.Identity.Name;
                            rp.Uid = u.Id;
                            using (var db = new SAPTestContext())
                            {
                                rp.Url = guid;
                                // "/Report1/ReportFiles/" + guid + "/report.xml";
                                Asset at = db.Assets.Find(atId);
                                if (at != null)
                                {
                                    rp.Asset = at;
                                }
                                db.Reports.Add(rp);
                                db.SaveChanges();
                            }
                            ViewBag.Flag = true;
                        }
                        else
                        {
                            Directory.Delete(Path.Combine(reportFolder, guid), true);
                            ViewBag.Flag = false;
                        }
                    }
                    else
                    {
                        ViewBag.Flag = false;
                    }
                    System.IO.File.Move(fileName, Path.Combine(backupPath, guid + ".zip"));
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMsg = ex.Message;
                    fi.Delete();
                    if (Directory.Exists(Path.Combine(reportFolder, guid)))
                    {
                        Directory.Delete(Path.Combine(reportFolder, guid), true);
                    }
                    throw new Exception();

                }
                MyReportFilter filter = new MyReportFilter();
                filter.isMyReport = true;
                return RedirectToAction("Index", filter);
            }
            return RedirectToAction("Index");
        }


        public ActionResult MyReport(int page = 1)
        {
            ReportInfo ri = new ReportInfo();

            using (var db = new SAPTestContext())
            {

                ri.TotalReportNum = db.Reports.AsQueryable().Count();
                ri.MyReportNum = db.Reports.AsQueryable().Where(c => c.Executor.Trim().ToLower() == User.Identity.Name.Trim().ToLower()).Count();
                ViewBag.PageCount = Math.Ceiling((double)ri.MyReportNum / _numInPage);
                ViewBag.CurrentPage = page;
                ri.Reports = db.Reports.Include(c => c.Asset.Release.Project).Where(c => c.Executor.Trim().ToLower() == User.Identity.Name.Trim().ToLower()).OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();

            }

            return View(ri);
        }


        public ActionResult Filter(int id, int level, string qs, int page = 1)
        {

            ViewBag.Level = level;
            ViewBag.Id = id;
            ViewBag.Qs = qs;
            ReportInfo ri = new ReportInfo();

            using (var db = new SAPTestContext())
            {
                ri.MyReportNum = db.Reports.AsQueryable().Where(c => c.Executor.Trim().ToLower() == User.Identity.Name.Trim().ToLower()).Count();
                ViewBag.CurrentPage = page;

                switch (level)
                {
                    case 1:
                        ri.TotalReportNum = db.Reports.AsQueryable().Where(c => c.Asset.Release.Pid == id).Count();
                        ViewBag.Message = "Project=" + db.Projects.Find(id).Name;
                        ri.Reports = db.Reports.Include(c => c.Asset.Release.Project).Where(c => c.Asset.Release.Pid == id && c.TestName.Contains(qs)).OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                        break;
                    case 2:
                        ri.TotalReportNum = db.Reports.AsQueryable().Where(c => c.Asset.Rid == id).Count();
                        ViewBag.Message = "Release=" + db.Releases.Find(id).Name;
                        ri.Reports = db.Reports.Include(c => c.Asset.Release.Project).Where(c => c.Asset.Rid == id && c.TestName.Contains(qs)).OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                        break;
                    case 3:
                        ri.TotalReportNum = db.Reports.AsQueryable().Where(c => c.AssetId == id).Count();
                        ViewBag.Message = "Asset=" + db.Assets.Find(id).Name;
                        ri.Reports = db.Reports.Include(c => c.Asset.Release.Project).Where(c => c.AssetId == id && c.TestName.Contains(qs)).OrderByDescending(c => c.SubmitDt).Skip(_numInPage * (page - 1)).Take(_numInPage).ToList();
                        break;
                    default:
                        ri.TotalReportNum = -1;
                        break;
                }


                ViewBag.PageCount = Math.Ceiling((double)ri.TotalReportNum / _numInPage);



            }

            if (qs.Trim() != "")
            {
                ViewBag.Message += " And Test Name =" + qs;
            }

            if (ri.TotalReportNum == -1)
            {
                return RedirectToAction("Index");
            }

            return View(ri);
        }
    }

    public class ReportInfo
    {
        public List<Asset> Assets { get; set; }

        public List<Report> Reports { get; set; }

        public int TotalReportNum { get; set; }

        public int MyReportNum { get; set; }

        public Uri Request { get; set; }

        public bool IsMyReport { get; set; }

        public string Project { get; set; }

        public string Release { get; set; }

        public string Asset { get; set; }

        public string QueryString { get; set; }

        public MyReportFilter Filter { get; set; }

        public bool IsVaildUser { get; set; }

    }
}