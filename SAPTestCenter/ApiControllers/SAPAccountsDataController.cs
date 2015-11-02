using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SAPTest.Model;
using System.Net.Mail;
using System.Configuration;
using SAPTestCenter.Helpers;

namespace SAPTestCenter.Controllers
{

    public class SAPAccountsDataController : ApiController
    {
        private SAPTestContext db = new SAPTestContext();

        private User u = null;

        public SAPAccountsDataController()
        {
            u = getUser();
        }


        private User getUser()
        {
            //return db.Users.Where(u => u.NTAccount == @"ASIAPACIFIC\yanzhou").FirstOrDefault();
            return db.Users.Where(u => u.NTAccount == User.Identity.Name).FirstOrDefault();
        }



        [HttpPost]
        [ResponseType(typeof(Account))]
        public IHttpActionResult GetAccount(Account myAccount)
        {
            var user = getUser();
            Account acct = null;
            if (user != null)
            {
                acct = (from account in db.Accounts
                        join accountUser in db.AccountUsers on account.Id equals accountUser.AcctId
                        where accountUser.Uid == user.Id && account.Id == myAccount.Id && account.IsAvailable
                        select account).FirstOrDefault();

            }
            if (acct == null)
            {
                return NotFound();
            }
            else
            {
                AcctUsageLog log = new AcctUsageLog();
                log.AcctId = acct.Id;
                log.LogDt = DateTime.Now;
                log.IsManual = true;
                log.Machine = "";
                log.IsExecute = true;
                log.Uid = user.Id;
                db.AcctUsageLogs.Add(log);
                db.SaveChanges();
            }
            return Ok(acct);
        }

        //PUT: api/SAPAccountsData/5
        [HttpPost]
        [ResponseType(typeof(Account))]
        public IHttpActionResult UpdateAccount(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = Request.RequestUri;

            var user = getUser();

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
                    account.AccountUsers.Add(au);
                    db.Accounts.Add(account);
                    db.SaveChanges();

                    //if not clear accout users, it will throw an Serializable error.
                    account.AccountUsers.Clear();
                    return Ok(account);

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

                        return Ok(account);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public IHttpActionResult GetAccess(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (u != null)
            {
                Access accs = null;
                var accesses = db.Accesses.Where(c => c.AcctId == account.Id && c.Uid == u.Id).ToList();

                if (accesses.Count > 0)
                {
                    accs = accesses.First();
                }
                else
                {
                    accs = new Access();
                    accs.AcctId = account.Id;
                    accs.CreateDt = DateTime.Now;
                    accs.Uid = u.Id;
                    db.Accesses.Add(accs);
                    

                    AccessLog log = new AccessLog();
                    log.AcctId = account.Id;
                    log.Uid = u.Id;
                    log.CreateDt = DateTime.Now;
                    db.AccessLogs.Add(log);

                    db.SaveChanges();

                }

                var aus = db.AccountUsers.Include(au => au.User).Where(au => au.AcctId == account.Id && au.IsOwner).ToList();
                if (aus.Count > 0)
                {
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("SAPTesting@hp.com");
                    foreach (var au in aus)
                    {
                        msg.To.Add(au.User.Email);
                    }
                    msg.CC.Add(u.Email);
                    MailHelper.AddAdminMail(msg);

                    var htmlLink = "http://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/SAPAccounts/SetAccess/" + accs.Id;

                    msg.Subject = "Applying access for SAP Box :" + account.BoxName;

                    msg.Body = "<h3>Hi Owners,</h3>";
                    msg.Body += "<p>" + u.UserName + " is applying the access of SAP Box:" + account.BoxName + "</p>";
                    msg.Body += "<p>You can grant access for her/him by <a href='" + htmlLink + "'>Click Me</a></p>";
                    msg.Body += "<p>If you don't want to do this,please ignore the mail</p>";
                    msg.IsBodyHtml = true;
                    MailHelper.SendMail(msg);
                    return Ok();
                }
            }
            return NotFound();
        }

        // DELETE: api/SAPAccountsData/5
        [HttpPost]
        [ResponseType(typeof(Account))]
        public IHttpActionResult DeleteAccount(Account acct)
        {
            var user = getUser();
            if (user == null)
                return BadRequest();

            var au = db.AccountUsers.Where(c => c.Uid == user.Id && c.IsPrimary && c.AcctId == acct.Id).FirstOrDefault();

            if (au == null)
                return NotFound();

            var account = db.Accounts.Find(acct.Id);
            var delTrans = db.Database.BeginTransaction();
            try
            {
                db.AccountUsers.RemoveRange(db.AccountUsers.Where(c => c.AcctId == acct.Id));
                db.Accounts.Remove(account);
                db.SaveChanges();
                delTrans.Commit();
            }
            catch
            {
                delTrans.Rollback();
            }
            delTrans.Dispose();



            return Ok(account);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult RemoveUser(AccountUser acctUser)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (u != null && u.Id != acctUser.Uid)
            {
                if (db.AccountUsers.Any(o => o.AcctId == acctUser.AcctId && o.Uid == u.Id && o.IsOwner))
                {
                    db.Entry(acctUser).State = EntityState.Modified;
                    db.AccountUsers.Remove(acctUser);
                    db.SaveChanges();
                }
            }
            return Ok();
        }

        [HttpPost]
        [ResponseType(typeof(AccountUser))]
        public IHttpActionResult UpdateAccountUser(AccountUser acctUser)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (u != null && u.Id != acctUser.Uid)
            {
                if (db.AccountUsers.Any(o => o.AcctId == acctUser.AcctId && o.Uid == u.Id && o.IsOwner))
                {
                    acctUser.CreateDt = DateTime.Now;
                    var entity = db.Entry(acctUser);
                    entity.State = EntityState.Modified;
                    entity.Property(p => p.IsOwner).IsModified = true;
                    entity.Property(p => p.CreateDt).IsModified = true;
                    entity.Property(p => p.IsPrimary).IsModified = false;
                    db.SaveChanges();
                }
            }
            return Ok(acctUser);
        }

        // api/SAPAccountsData/FailLogin
        [HttpPost]
        public void FailLogin(Account acct)
        {
            var u = getUser();

            if (u != null && acct.Id > 0)
            {
                if (db.AccountUsers.Where(au => au.Uid == u.Id && au.AcctId == acct.Id).Count() > 0)
                {
                    var aus = db.AccountUsers.Include(au => au.Account).Include(au => au.User).Where(au => au.AcctId == acct.Id && au.IsOwner).ToList();

                    if (aus.Count > 0)
                    {
                        var account = aus.First().Account;
                        account.IsAvailable = false;
                        db.Entry(account).State = EntityState.Modified;
                        db.SaveChanges();

                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("SAPTesting@hp.com");
                        foreach (var au in aus)
                        {
                            msg.To.Add(au.User.Email);
                        }
                        msg.CC.Add(u.Email);
                        MailHelper.AddAdminMail(msg);

                        msg.Subject = "Fail to login" + account.BoxName;

                        msg.Body = "<h3>Hi Owners,</h3>";
                        msg.Body += string.Format("<p>Fail to login SAP Box:{0}</p>", account.BoxName);
                        msg.Body += "<p>This box has been set to unavailable in SAP Logon tool</p>";
                        msg.Body += "<p>Please change/reset your password first and then update the lastest info in SAP Logon tool</p>";
                        msg.IsBodyHtml = true;

                        MailHelper.SendMail(msg);
                    }
                }

            }
        }

        [HttpPost]
        public void RequestAccess(Account acct)
        {
            if (u != null)
            {
                var aus = db.AccountUsers.Include(au => au.Account).Include(au => au.User).Where(au => au.AcctId == acct.Id && au.IsOwner).ToList();

                if (aus.Count > 0)
                {
                    var accout = aus.First().Account;

                    Access acs = new Access();
                    acs.AcctId = accout.Id;
                    acs.Uid = u.Id;
                    acs.CreateDt = DateTime.Now;
                    db.Accesses.Add(acs);

                    AccessLog log = new AccessLog();
                    log.AcctId = accout.Id;
                    log.CreateDt = DateTime.Now;
                    log.Uid = u.Id;

                    db.AccessLogs.Add(log);
                    db.SaveChanges();

                    string _basicURL = "http://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port;
                    string htmlLink = _basicURL + "?index=";// +returnId.ToString() + "&token=" + md5;
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("SAPTesting@hp.com");
                    foreach (var au in aus)
                    {
                        msg.To.Add(au.User.Email);
                    }
                    msg.CC.Add(u.Email);


                    MailHelper.AddAdminMail(msg);


                    msg.Subject = "Applying access for SAP Box :" + accout.BoxName;

                    msg.Body = "<h3>Hi Owners,</h3>";
                    msg.Body += "<p>" + u.UserName + " is applying the access of SAP Box:" + accout.BoxName + "</p>";
                    msg.Body += "<p>You can grant access for her/him by <a href='" + htmlLink + "'>Click Me</a></p>";
                    msg.Body += "<p>If you don't want to do this,please ignore the mail</p>";
                    msg.IsBodyHtml = true;
                    MailHelper.SendMail(msg);
                }
            }
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