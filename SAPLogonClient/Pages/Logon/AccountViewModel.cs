using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPLogonClient.AccountService;
using SAPLogonClient.ViewModel;

namespace SAPLogonClient.Pages.Logon
{
    class AccountViewModel
    {
        public bool IsNew { get; set; }

        public MyAccount Account { get; set; }
    }
}
