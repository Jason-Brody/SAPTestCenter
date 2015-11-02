using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.DirectoryServices;
using SAPLogonClient.AccountService;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Xml;
using System.IO;
//using TestRemote;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SAPLogonClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ServiceHost _host;

        private AccountServiceClient _client;

        public static ImageSource BingImageUrl = null;

        public string ThemeConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.xml");

        public User SAPUser { get; set; }

        public AccountServiceClient Client 
        { 
            get
            {
                if (_client == null || _client.State == System.ServiceModel.CommunicationState.Faulted || _client.State == System.ServiceModel.CommunicationState.Closed)
                {
                    _client = new AccountServiceClient();
                }
                return _client; 
            } 
        }

        

        public App()
        {
            SAPUser = new User();
            //_client = new AccountServiceClient();
            //_client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(@"ASIAPACIFIC\yanzhou", "zhya.kav2222");
            //Uri[] baseAddresses = new Uri[]{
            //    new Uri("http://localhost:9001/"),
            //    new Uri("net.tcp://localhost:9002/")
            //};
            //try
            //{


            //    _host = new ServiceHost(typeof(RemoteExecutionService), baseAddresses);
            //    ServiceMetadataBehavior behavior = new ServiceMetadataBehavior() { HttpGetEnabled = true };
            //    _host.Description.Behaviors.Add(behavior);

            //    _host.AddServiceEndpoint(typeof(TestRemote.IRemoteExecutionService), new BasicHttpBinding(), "RemoteTestService");
            //    _host.Open();
                
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            
        }

        public void SendMail(string mailto,string subject,string body)
        {

            string contact = string.Format("mailto:{0}?subject={1}&body={2}", mailto, subject,body);

            System.Diagnostics.Process.Start(contact);
        }


        private string getInfo(ResultPropertyValueCollection rpv)
        {
            if(rpv.Count>0)
            {
                if(rpv[0] is byte[])
                {
                    return System.Text.Encoding.Default.GetString(rpv[0] as byte[]);
                }
                else
                {
                    return rpv[0].ToString();
                }
            }
            return "";
        }

        public Task GetUserDetail(User user,bool isByEmail=true)
        {
            return Task.Run(() =>
            {
                string strServerDNS = "ldap.hp.com:389";
                string strSearchBaseDN = "ou=People,o=hp.com";
                string strLDAPPath;
                strLDAPPath = "LDAP://" + strServerDNS + "/" + strSearchBaseDN;
                DirectoryEntry objDirEntry = new DirectoryEntry(strLDAPPath, null, null, AuthenticationTypes.Anonymous);
                DirectorySearcher searcher = new DirectorySearcher(objDirEntry);
                SearchResult result = null; ;

                if(isByEmail)
                {
                    searcher.Filter = "(uid=" + user.Email + ")";
                }
                else
                {
                    string ntAccount = user.NTAccount.Replace(@"\", ":");
                    searcher.Filter = "(ntUserDomainId=" + ntAccount + ")";
                }

                searcher.PropertiesToLoad.Add("ntUserDomainId");
                searcher.PropertiesToLoad.Add("hpPictureOneHpURI");
                searcher.PropertiesToLoad.Add("hpBusinessUnit");
                searcher.PropertiesToLoad.Add("uid");
                searcher.PropertiesToLoad.Add("EmployeeNumber");
                searcher.PropertiesToLoad.Add("manager");
                searcher.PropertiesToLoad.Add("sn");
                searcher.PropertiesToLoad.Add("GivenName");
                searcher.PropertiesToLoad.Add("hpDisplayNameExtension");
                searcher.ClientTimeout = TimeSpan.FromSeconds(20);

                try
                {
                    result = searcher.FindOne();
                    if(isByEmail)
                    {
                        user.NTAccount = result.Properties["ntUserDomainId"][0].ToString().Replace(":",@"\");
                    }
                    else
                    {
                        user.Email = result.Properties["uid"][0].ToString();
                    }
                    if (result.Properties["sn"].Count > 0 || result.Properties["GivenName"].Count > 0 || result.Properties["hpDisplayNameExtension"].Count > 0)
                    {
                        
                        string name = "";
                        name += getInfo(result.Properties["sn"]);
                        name += ","+getInfo(result.Properties["GivenName"]);
                        string ext = getInfo(result.Properties["hpDisplayNameExtension"]);
                        if(ext != "")
                        {
                            name += "(" + ext + ")";
                        }
                        
                        user.UserName = name;
                    }
                    if (result.Properties["uid"].Count > 0)
                    {
                        user.Email = result.Properties["uid"][0].ToString();
                    }
                    if (result.Properties["hpBusinessUnit"].Count > 0)
                    {
                        user.BusinessUnit = getInfo(result.Properties["hpBusinessUnit"]);
                    }
                    if (result.Properties["hpPictureOneHpURI"].Count > 0)
                    {
                        user.PhotoUrl = result.Properties["hpPictureOneHpURI"][0].ToString();
                    }
                    if (result.Properties["manager"].Count > 0)
                    {
                        user.Manager = result.Properties["manager"][0].ToString().Split(',').First().ToLower().Replace("uid=", "");
                    }
                    if (result.Properties["EmployeeNumber"].Count > 0)
                    {
                        user.EmployeeId = int.Parse(result.Properties["EmployeeNumber"][0].ToString());
                    }
                }
                catch
                {

                }

                finally
                {
                    searcher.Dispose();
                }
            });
        }
    }

    public class ThemeData
    {
        public byte A { get; set; }

        public byte B { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }

        public string ThemeURI { get; set; }
    }
}
