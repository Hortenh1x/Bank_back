using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bank_business.entities
{
    internal class Transaction
    {
        private int id;
        private string date_time;
        private double deposit;
        private int from_id;
        private int to_id;

        public Transaction(int id, string date_t, double deposit, int from_id, int to_id)
        {
            this.id = id;
            this.date_time = date_t;
            this.deposit = deposit;
            this.from_id = from_id;
            this.to_id = to_id;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Date_time
        {
            get { return date_time; }
            set { date_time = value; }
        }

        public double Deposit
        {
            get { return deposit; }
            set { deposit = value; }
        }

        public int From_id
        {
            get { return from_id; }
            set { from_id = value; }
        }

        public int To_id
        {
            get { return to_id; }
            set { to_id = value; }
        }
    }
}
