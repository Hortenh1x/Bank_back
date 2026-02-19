using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_back.entities
{
    public class User
    {
        private int id;
        private string first_name;
        private string last_name;
        private string password_hash;

        public User(int id, string first_name, string last_name)
        {
            this.id = id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.password_hash = "";
        }

        public User(int id, string first_name, string last_name, string password_hash)
        {
            this.id = id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.password_hash = password_hash;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string First_name
        {
            get { return first_name; }
            set { first_name = value; }
        }

        public string Last_name
        {
            get { return last_name; }
            set { last_name = value; }
        }

        public string Password_hash
        {
            get { return password_hash; }
            set { password_hash = value; }
        }
    }
}
