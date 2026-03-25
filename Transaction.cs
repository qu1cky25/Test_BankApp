using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankBackend
{
    public class Transaction
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string TypeTransaction { get; set; }
        public decimal amount { get; set; }
        public DateTime date_transaction { get; set; }
        public Account Account { get; set; }

    }
}
