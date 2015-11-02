using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPLogonClient.AccountService;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SAPLogonClient.ViewModel
{
    [XmlRoot(ElementName="Account")]
    public class MyAccount
    {
        public ObservableCollection<AccountUser> AcctUsers { get; set; }

        public string BoxId { get; set; }

        public string Client { get; set; }

        public int Id { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsPublic { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string UserName { get; set; }

        public bool IsWebLogin { get; set; }

        public Browser Browser { get; set; }

        public MyAccount() { }

        public MyAccount(Account Acct)
        {
            BoxId = Acct.BoxId;
            Client = Acct.Client;
            Id = Acct.Id;
            IsAvailable = Acct.IsAvailable;
            IsPublic = Acct.IsPublic;
            Password = Acct.Password;
            Server = Acct.Server;
            UserName = Acct.UserName;
            IsWebLogin = Acct.IsWebLogin;
            if(Acct.AcctUsers != null)
            {
                AcctUsers = new ObservableCollection<AccountUser>();
                foreach(var u in Acct.AcctUsers)
                {
                    AcctUsers.Add(u);
                }
            }
        }


    }

    public enum Browser
    {
        Chrome = 0,
        IE = 1
    }
}
