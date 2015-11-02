using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPUserInfo
{
    class Program
    {
        static void Main()
        {
            Dictionary<string, string> filters = new Dictionary<string, string>()
            {
                {"uid","yang.zhou4@hpe.com" }
            };
            Dictionary<string, ResultPropertyValueCollection> formats = new Dictionary<string, ResultPropertyValueCollection>()
            {
                {"sn",null },
                {"manager",null },
                {"ntUserDomainId",null },
                
            };

            HPUserHelper.GetUserInfo(filters, formats).Wait();

        }
    }


    public static class HPUserHelper
    {
        public static Task GetUserInfo(Dictionary<string,string> Filter,Dictionary<string, ResultPropertyValueCollection> ResultFormat)
        {
            
            return Task.Run(() => {
                string strServerDNS = "ldap.hp.com:389";
                string strSearchBaseDN = "ou=People,o=hp.com";
                string strLDAPPath;
                strLDAPPath = "LDAP://" + strServerDNS + "/" + strSearchBaseDN;
                DirectoryEntry objDirEntry = new DirectoryEntry(strLDAPPath, null, null, AuthenticationTypes.Anonymous);
                DirectorySearcher searcher = new DirectorySearcher(objDirEntry);
                SearchResult result = null;

                searcher.Filter = "uid=yang.zhou4@hpe.com";

                foreach(var returnVal in ResultFormat)
                {
                    searcher.PropertiesToLoad.Add(returnVal.Key);
                }
                searcher.ClientTimeout = TimeSpan.FromSeconds(20);
                try
                {
                    result = searcher.FindOne();
                    for(int i = 0;i<ResultFormat.Count;i++)
                    {
                        string key = ResultFormat.ElementAt(i).Key;
                        ResultFormat[key] = result.Properties[key];
                    }
                    
                }
                catch(Exception ex)
                {

                }

                finally
                {
                    searcher.Dispose();
                }

            });
        }
        //public Task GetUserDetail(User user, bool isByEmail = true)
        //{
        //    return Task.Run(() =>
        //    {
        //        string strServerDNS = "ldap.hp.com:389";
        //        string strSearchBaseDN = "ou=People,o=hp.com";
        //        string strLDAPPath;
        //        strLDAPPath = "LDAP://" + strServerDNS + "/" + strSearchBaseDN;
        //        DirectoryEntry objDirEntry = new DirectoryEntry(strLDAPPath, null, null, AuthenticationTypes.Anonymous);
        //        DirectorySearcher searcher = new DirectorySearcher(objDirEntry);
        //        SearchResult result = null; ;

        //        if (isByEmail)
        //        {
        //            searcher.Filter = "(uid=" + user.Email + ")";
        //        }
        //        else
        //        {
        //            string ntAccount = user.NTAccount.Replace(@"\", ":");
        //            searcher.Filter = "(ntUserDomainId=" + ntAccount + ")";
        //        }

        //        searcher.PropertiesToLoad.Add("ntUserDomainId");
        //        searcher.PropertiesToLoad.Add("hpPictureOneHpURI");
        //        searcher.PropertiesToLoad.Add("hpBusinessUnit");
        //        searcher.PropertiesToLoad.Add("uid");
        //        searcher.PropertiesToLoad.Add("EmployeeNumber");
        //        searcher.PropertiesToLoad.Add("manager");
        //        searcher.PropertiesToLoad.Add("sn");
        //        searcher.PropertiesToLoad.Add("GivenName");
        //        searcher.PropertiesToLoad.Add("hpDisplayNameExtension");
        //        searcher.ClientTimeout = TimeSpan.FromSeconds(20);

        //        try
        //        {
        //            result = searcher.FindOne();
        //            if (isByEmail)
        //            {
        //                user.NTAccount = result.Properties["ntUserDomainId"][0].ToString().Replace(":", @"\");
        //            }
        //            else
        //            {
        //                user.Email = result.Properties["uid"][0].ToString();
        //            }
        //            if (result.Properties["sn"].Count > 0 || result.Properties["GivenName"].Count > 0 || result.Properties["hpDisplayNameExtension"].Count > 0)
        //            {

        //                string name = "";
        //                name += getInfo(result.Properties["sn"]);
        //                name += "," + getInfo(result.Properties["GivenName"]);
        //                string ext = getInfo(result.Properties["hpDisplayNameExtension"]);
        //                if (ext != "")
        //                {
        //                    name += "(" + ext + ")";
        //                }

        //                user.UserName = name;
        //            }
        //            if (result.Properties["uid"].Count > 0)
        //            {
        //                user.Email = result.Properties["uid"][0].ToString();
        //            }
        //            if (result.Properties["hpBusinessUnit"].Count > 0)
        //            {
        //                user.BusinessUnit = getInfo(result.Properties["hpBusinessUnit"]);
        //            }
        //            if (result.Properties["hpPictureOneHpURI"].Count > 0)
        //            {
        //                user.PhotoUrl = result.Properties["hpPictureOneHpURI"][0].ToString();
        //            }
        //            if (result.Properties["manager"].Count > 0)
        //            {
        //                user.Manager = result.Properties["manager"][0].ToString().Split(',').First().ToLower().Replace("uid=", "");
        //            }
        //            if (result.Properties["EmployeeNumber"].Count > 0)
        //            {
        //                user.EmployeeId = int.Parse(result.Properties["EmployeeNumber"][0].ToString());
        //            }
        //        }
        //        catch
        //        {

        //        }

        //        finally
        //        {
        //            searcher.Dispose();
        //        }
        //    });
        //}

        //private string getInfo(ResultPropertyValueCollection rpv)
        //{
        //    if (rpv.Count > 0)
        //    {
        //        if (rpv[0] is byte[])
        //        {
        //            return System.Text.Encoding.Default.GetString(rpv[0] as byte[]);
        //        }
        //        else
        //        {
        //            return rpv[0].ToString();
        //        }
        //    }
        //    return "";
        //}
    }
}
