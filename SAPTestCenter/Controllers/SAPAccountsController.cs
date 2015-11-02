using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAPTest.Model;
using System.Configuration;
using SAPTestCenter.Helpers;
using System.Net.Mail;


namespace SAPTestCenter.Controllers
{
    public class SAPAccountsController : SAPBaseController
    {
        private SAPTestContext db = new SAPTestContext();


        // GET: SAPAccounts
        public ActionResult Index()
        {
            List<Account> myAccounts = new List<Account>();

            User u = getUser();



            if (u != null && u.IsValid)
            {

                myAccounts = (from act in db.Accounts
                              join au in db.AccountUsers on act.Id equals au.AcctId
                              where act.IsAvailable
                              && au.Uid == u.Id
                              orderby act.BoxName
                              select act).ToList();


            }


            return View(myAccounts);
        }

        public ActionResult MyAccounts()
        {
            List<AccountUser> myAccounts = new List<AccountUser>();
            //List<Account> myAccounts = new List<Account>();

            User u = getUser();

            if (u != null && u.IsValid)
            {

                myAccounts = db.AccountUsers.Include(c => c.Account).Where(au => au.Uid == u.Id && au.IsOwner).OrderBy(a=>a.Account.BoxName).ToList();
                //myAccounts = (from act in db.Accounts
                //              join au in db.AccountUsers on act.Id equals au.AcctId
                //              where au.Uid == u.Id && au.IsOwner
                //              select act).ToList();


            }


            return View(myAccounts);
        }


        public ActionResult GetAccess()
        {
            List<Account> accounts = new List<Account>();
            User u = getUser();

            if(u!=null && u.IsValid)
            {
                var myAccounts = from act in db.Accounts
                              join au in db.AccountUsers on act.Id equals au.AcctId
                              where au.Uid == u.Id
                              select act;

                accounts = (from acct in db.Accounts
                           where acct.IsAvailable && !acct.IsPrivate && !myAccounts.Contains(acct)
                           orderby acct.BoxName
                           select acct).ToList();
               
            }
            return View(accounts);
        }

        // GET: SAPAccounts/Details/5
        public ActionResult Details(int id)
        {
            var user = getUser();

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IEnumerable<AccountUser> accounts = db.AccountUsers.Include(c => c.User).Include(c => c.Account).Where(c => c.AcctId == id).ToList();

            var account = new Account();

            if (accounts.Count() > 0)
            {
                account = accounts.First().Account;
            }


            return View(account);
        }

        // GET: SAPAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SAPAccounts/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BoxName,UserName,Password,Client,ServerAddress,IsAvailable,UpdateDt,IsPrivate")] Account account)
        {
            if (ModelState.IsValid)
            {
                var user = getUser();

                var accoutNum = db.Accounts.Where(c => c.BoxName == account.BoxName).Count();

                if (accoutNum > 0)
                {
                    ViewBag.ErrorMessage = string.Format("The Box {0} has already exist in DB, please choose another one", account.BoxName);
                    return View(account);
                }


                if (user != null)
                {
                    if (account.Id == 0)
                    {
                        AccountUser au = new AccountUser()
                        {
                            CreateDt = DateTime.Now,
                            IsOwner = true,
                            IsPrimary = true,
                            Uid = user.Id,
                            Account = account
                        };
                        account.UpdateDt = DateTime.Now;
                        account.AccountUsers.Add(au);
                        db.Accounts.Add(account);
                        db.SaveChanges();
                    }
                    else
                    {
                        var myAct = from at in db.Accounts
                                    join au in db.AccountUsers on at.Id equals au.AcctId
                                    where at.Id == account.Id && au.IsOwner && au.Uid == user.Id
                                    select at;

                        if (myAct.Count() > 0)
                        {
                            db.Entry(account).State = EntityState.Modified;
                            account.UpdateDt = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("MyAccounts");
            }

            return View(account);
        }

        //// GET: SAPAccounts/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Account account = db.Accounts.Find(id);
        //    if (account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(account);
        //}

        //// POST: SAPAccounts/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,BoxName,UserName,Password,Client,ServerAddress,IsAvailable,UpdateDt,IsVisible,IsPublic,IsWebLogin")] Account account)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(account).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(account);
        //}

        //// GET: SAPAccounts/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Account account = db.Accounts.Find(id);
        //    if (account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(account);
        //}

        //// POST: SAPAccounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Account account = db.Accounts.Find(id);
        //    db.Accounts.Remove(account);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        [HttpGet]
        public ActionResult SetAccess(int id)
        {
            var u = getUser();
            if (u != null)
            {
                var access = db.Accesses.Where(c => c.Id == id).FirstOrDefault();
                if (access != null)
                {
                    var aus = db.AccountUsers.Include(a => a.User).Include(a => a.Account).Where(a => a.AcctId == access.AcctId && a.IsOwner).ToList();

                    if (aus.Exists(a => a.Uid == u.Id))
                    {

                        AccountUser newAu = new AccountUser();
                        newAu.Uid = access.Uid;
                        newAu.AcctId = access.AcctId;
                        newAu.CreateDt = DateTime.Now;

                        db.AccountUsers.Add(newAu);
                        db.Accesses.Remove(access);
                        db.SaveChanges();

                        

                        var targetUser = db.Users.Find(access.Uid);
                        ViewBag.UserName = targetUser.UserName;
                        ViewBag.BoxName = aus.First().Account.BoxName;

                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("SAPTesting@hp.com");

                        

                        msg.To.Add(targetUser.Email);


                        foreach (var au in aus)
                        {
                            msg.CC.Add(au.User.Email);
                        }

                        MailHelper.AddAdminMail(msg);

                        msg.Subject = "You now have access to SAP Box:" + aus.First().Account.BoxName;
                        msg.Body = "<p>Hi,</p>";
                        msg.Body += "<P>" + u.UserName + " has granted the access to SAP Box for you</p>";
                        msg.IsBodyHtml = true;
                        MailHelper.SendMail(msg);

                    }
                    else
                    {
                        ViewBag.ErrorMessage = "You don't have permisson to do this";
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "Can not find the request.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invaild User";
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
