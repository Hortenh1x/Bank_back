using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bank_back.entities
{
    public class TransactionResponce : Transaction
    {
        private string to_owner_name;

        private string from_owner_name;


        public TransactionResponce(int id, string date_time, double deposit, int from_id, int to_id, TransactionType type, string from_owner_name, string to_owner_name) : base(id, date_time, deposit, from_id, to_id, type)
        {
            this.from_owner_name = from_owner_name;
            this.to_owner_name = to_owner_name;
        }

        public string To_owner_name
        {
            get { return to_owner_name; }
            set { to_owner_name = value; }
        }

        public string From_owner_name
        {
            get { return from_owner_name; }
            set { from_owner_name = value; }
        }
    }
}
