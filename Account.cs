using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankBackend
{
    public class Account
    {
        public string Id { get; set; }
        public string Bank { get; set; }
        public decimal Balance { get; set; }
        public bool is_visible { get; set; } = true;
        public List<Transaction> Transactions { get; set; } = new();
    }
}
