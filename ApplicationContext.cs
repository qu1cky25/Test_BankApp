using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankBackend 
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options_builder)
        {
            options_builder.UseSqlite("Data Source=transactins.db");
        }

        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            model_builder.Entity<Account>()
                .HasKey(a => a.Id);

            model_builder.Entity<Account>()
                .Property(a => a.Id)
                .HasMaxLength(20)
                .IsFixedLength();

            model_builder.Entity<Account>()
                .ToTable(t => t.HasCheckConstraint("ck_account_id_length", "length(Id) = 20"));

            model_builder.Entity<Account>()
                .ToTable(t => t.HasCheckConstraint("ck_account_bank", "Bank IN ('Сбербанк', 'Альфа-банк', 'Т-банк')"));

            model_builder.Entity<Account>()
                .Property(a => a.Balance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18,2)");

            model_builder.Entity<Transaction>()
                .HasKey(t => t.Id);

            model_builder.Entity<Transaction>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            model_builder.Entity<Transaction>()
                .ToTable(t => t.HasCheckConstraint("ck_transaction_type", "TypeTransaction IN ('Списание', 'Пополнение')"));

            model_builder.Entity<Transaction>()
                .Property(t => t.amount)
                .HasColumnType("decimal(18,2)");

            model_builder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);
        }
    }
}
