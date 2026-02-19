using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_back.Entities
{
    public class Account
    {
        private int id;
        private double deposit;
        private int user_id;

        public Account(int id, double deposit, int user_id)
        {
            this.id = id;
            this.deposit = deposit;
            this.user_id = user_id;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public double Deposit
        {
            get { return deposit; }
            set { deposit = value; }
        }

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
    }
}
