using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_back.entities
{
    internal class UserReturn : User
    {
        private int[] accounts;

        public UserReturn(int id, string first_name, string last_name, int[] accounts) : base(id, first_name, last_name)
        {
            this.accounts = accounts;
        }

        public int[] Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }

        public string ToStringAccounts()
        {
            var s = "";
            for (var i = 0; i < accounts.Length; i++)
            {
                if (i != 0)
                {
                    s += ", ";
                }
                s += accounts[i];
            }
            return s;
        }
    }
}
