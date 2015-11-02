using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Xml;
using SAPFEWSELib;
using SAPAutoLogon.AccountService;
using SAPAutomation;

namespace SAPAutoLogon
{
    public class Logon
    {
        private AccountServiceClient _client;
        public Logon() 
        {
            _client = new AccountServiceClient(new System.ServiceModel.NetTcpBinding(), new System.ServiceModel.EndpointAddress("net.tcp://c0040597.itcs.hp.com:9002/AccountService"));

            
        }

        public void RemoteLogon(string BoxId,string Language="EN")
        {
            //string ntAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //WebClient client = new WebClient();
            //client.Headers.Add("content-type", "application/xml");
            //client.Credentials = CredentialCache.DefaultCredentials;
            ////http://c0049289.itcs.hp.com:8018/api/Account/AccountInfoByName?usr=ASIAPACIFIC\yanzhou&box=LH7_Andy&PC=test
            ////http://c0049289.itcs.hp.com:8018/api/account/AccountInfoByName?name={0}&pc={1}
            //string api = string.Format("http://c0040597.itcs.hp.com:8018/api/Account/AccountInfoByName?usr={0}&box={1}&PC={2}", ntAccount, BoxId, Environment.MachineName);
            //string str = client.DownloadString(api);
            //XmlDocument xml = new XmlDocument();
            //xml.LoadXml(str);

            //XmlNamespaceManager xmlspace = new XmlNamespaceManager(xml.NameTable);
            //xmlspace.AddNamespace("act", "http://schemas.datacontract.org/2004/07/SAPAccountService");
            //XmlNode id = xml.SelectSingleNode("//act:Account/act:Id", xmlspace);
            //XmlNode userName = xml.SelectSingleNode("//act:Account/act:UserName", xmlspace);
            //XmlNode password = xml.SelectSingleNode("//act:Account/act:Password", xmlspace);
            //XmlNode Client = xml.SelectSingleNode("//act:Account/act:Client", xmlspace);
            //XmlNode Server = xml.SelectSingleNode("//act:Account/act:Server", xmlspace);


            var account = _client.AccountInfoByName(BoxId, Environment.MachineName);

            SAPTestHelper.Current.SetSession();
            SAPTestHelper.Current.GetElementById<GuiTextField>("wnd[0]/usr/txtRSYST-BNAME").Text = account.UserName;
            SAPTestHelper.Current.GetElementById<GuiTextField>("wnd[0]/usr/pwdRSYST-BCODE").Text = account.Password;
            SAPTestHelper.Current.GetElementById<GuiTextField>("wnd[0]/usr/txtRSYST-MANDT").Text = account.Client;
            SAPTestHelper.Current.GetElementById<GuiTextField>("wnd[0]/usr/txtRSYST-LANGU").Text = Language;
            GuiFrameWindow window = SAPTestHelper.Current.GetElementById<GuiFrameWindow>("wnd[0]");
            window.SendVKey(0);


            
        }

        public void StartLogon(string BoxId,string Language = "EN")
        {
            var account = _client.AccountInfoByName(BoxId, Environment.MachineName);


            //string ntAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //WebClient client = new WebClient();
            //client.Headers.Add("content-type", "application/xml");
            //client.Credentials = CredentialCache.DefaultCredentials;
            ////http://c0049289.itcs.hp.com:8018/api/Account/AccountInfoByName?usr=ASIAPACIFIC\yanzhou&box=LH7_Andy&PC=test
            ////http://c0049289.itcs.hp.com:8018/api/account/AccountInfoByName?name={0}&pc={1}
            //string api = string.Format("http://c0049289.itcs.hp.com:8018/api/Account/AccountInfoByName?usr={0}&box={1}&PC={2}", ntAccount, BoxId, Environment.MachineName);
            //string str = client.DownloadString(api);
            //XmlDocument xml = new XmlDocument();
            //xml.LoadXml(str);

            //XmlNamespaceManager xmlspace = new XmlNamespaceManager(xml.NameTable);
            //xmlspace.AddNamespace("act", "http://schemas.datacontract.org/2004/07/SAPAccountService");
            //XmlNode id = xml.SelectSingleNode("//act:Account/act:Id", xmlspace);
            //XmlNode userName = xml.SelectSingleNode("//act:Account/act:UserName", xmlspace);
            //XmlNode password = xml.SelectSingleNode("//act:Account/act:Password",xmlspace);
            //XmlNode Client = xml.SelectSingleNode("//act:Account/act:Client", xmlspace);
            //XmlNode Server = xml.SelectSingleNode("//act:Account/act:Server", xmlspace);



            SAPLogon logon = new SAPLogon();
            logon.AfterLogin += (s, e) => {
                GuiPasswordField gpf = SAPTestHelper.Current.TryGetElementById<GuiPasswordField>("wnd[1]/usr/pwdRSYST-NCODE");
                GuiTextField txtF = SAPTestHelper.Current.TryGetElementById<GuiTextField>("wnd[0]/usr/pwdRSYST-BCODE");
                if (gpf != null || txtF != null)
                {
                    GuiConnection gc = s.Parent as GuiConnection;
                    gc.CloseSession(s.Id);
                    _client.FailLogin(account);
                    throw new Exception("Fail to Login, an email has will send to owner shortly");
                }
                else
                {
                    _client.AddLog(account.Id, Environment.MachineName, false);
                    //string newapi = string.Format("http://c0049289.itcs.hp.com:8018/api/Account/addlog?usr={0}&id={1}&pc={2}&m=false", ntAccount, id.InnerText, Environment.MachineName);
                    //client.DownloadString(newapi);
                }
                //http://c0049289.itcs.hp.com:8080/api/account/AddLog?id={0}&pc={1}&m=false
                //http://c0049289.itcs.hp.com:8018/api/Account/addlog?usr=ASIAPACIFIC\yanzhou&id=55&pc=test&m=false
                
            };
            logon.StartProcess();
            logon.OpenConnection(account.Server);
            SAPTestHelper.Current.SetSession(logon);
            logon.Login(account.UserName, account.Password, account.Client,Language);

            //logon.OpenConnection(Server.InnerText);
            //SAPTestHelper.Current.SetSession(logon.SapGuiSession);
            //logon.Login(userName.InnerText, password.InnerText,Client.InnerText,Language);
        }
    }
}
