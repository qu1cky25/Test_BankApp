using System;
using System.Collections.Generic;
using System.Linq;

namespace BankBackend
{
    public class BankService
    {
        public List<Account> get_all_accounts()
        {
            using var db_context = new ApplicationContext();
            return db_context.Accounts.ToList();
        }

        public string add_account(string account_id, string bank_name)
        {
            using var db_context = new ApplicationContext();
            if (db_context.Accounts.Any(a => a.Id == account_id)) return "Счет уже существует";

            var new_account = new Account
            {
                Id = account_id,
                Bank = bank_name,
                Balance = 0
            };
            db_context.Accounts.Add(new_account);
            db_context.SaveChanges();
            return "OK";
        }

        public string process_transaction(string from_account_id, string to_account_id, decimal transfer_amount)
        {
            using var db_context = new ApplicationContext();
            var account_from = db_context.Accounts.FirstOrDefault(a => a.Id == from_account_id);
            var account_to = db_context.Accounts.FirstOrDefault(a => a.Id == to_account_id);

            if (account_from == null) return "Счет списания не найден.";
            if (account_to == null) return "Счет зачисления не найден.";
            if (account_from.Balance < transfer_amount) return "Недостаточно средств на счете.";

            account_from.Balance -= transfer_amount;
            account_to.Balance += transfer_amount;

            var time_now = DateTime.Now;

            var transaction_out = new Transaction
            {
                AccountId = from_account_id,
                TypeTransaction = "Списание",
                amount = transfer_amount,
                date_transaction = time_now
            };

            var transaction_in = new Transaction
            {
                AccountId = to_account_id,
                TypeTransaction = "Пополнение",
                amount = transfer_amount,
                date_transaction = time_now
            };

            db_context.Transactions.Add(transaction_out);
            db_context.Transactions.Add(transaction_in);
            db_context.SaveChanges();
            return "OK";
        }
    }
}