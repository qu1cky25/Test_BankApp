using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BankBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BankService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    using var db_context = new ApplicationContext();
    db_context.Database.EnsureCreated();

    if (!db_context.Accounts.Any())
    {
        var visible_accounts = new List<Account>
        {
            new Account { Id = "40817810570000123456", Bank = "Сбербанк", Balance = 15000.00m, is_visible = true },
            new Account { Id = "40817810000000987654", Bank = "Альфа-банк", Balance = 2500.50m, is_visible = true },
            new Account { Id = "40817810221110543210", Bank = "Т-Банк", Balance = 12700.00m, is_visible = false },
            new Account { Id = "40817810889990123456", Bank = "Сбербанк", Balance = 540.25m, is_visible = false },
            new Account { Id = "40817810334440654321", Bank = "Альфа-банк", Balance = 89200.10m, is_visible = false },
            new Account { Id = "40817810556660789012", Bank = "Т-Банк", Balance = 3300.50m, is_visible = false },
            new Account { Id = "40817810112220901234", Bank = "Сбербанк", Balance = 1560.00m, is_visible = false },
            new Account { Id = "40817810778880345678", Bank = "Альфа-банк", Balance = 42150.75m, is_visible = false },
            new Account { Id = "40817810445550234567", Bank = "Т-Банк", Balance = 7100.30m, is_visible = false },
            new Account { Id = "40817810990000876543", Bank = "Сбербанк", Balance = 120.00m, is_visible = false },
            new Account { Id = "40817810667770456789", Bank = "Альфа-банк", Balance = 9500.90m, is_visible = false },
            new Account { Id = "40817810223330567890", Bank = "Т-Банк", Balance = 27430.15m, is_visible = false }
        };
        db_context.Accounts.AddRange(visible_accounts);
        
        db_context.SaveChanges();
    }

    print_database_state(db_context);
}

app.MapGet("/api/accounts", (BankService service) =>
{
    using var db_context = new ApplicationContext();
    return db_context.Accounts.Where(a => a.is_visible).ToList();
});

app.MapPost("/api/accounts", (BankService service, account_dto dto) =>
{
    string result = service.add_account(dto.id, dto.bank);
    using var db_context = new ApplicationContext();
    print_database_state(db_context);
    return result == "OK" ? Results.Ok() : Results.BadRequest(result);
});

app.MapPost("/api/transactions/transfer", (BankService service, transfer_dto dto) =>
{
    string result = service.process_transaction(dto.from_account_id, dto.to_account_id, dto.amount);
    using var db_context = new ApplicationContext();
    print_database_state(db_context);
    return result == "OK" ? Results.Ok() : Results.BadRequest(result);
});

app.MapPost("/api/transactions/topup", (BankService service, transfer_dto dto) =>
{
    string result = service.process_transaction(dto.from_account_id, dto.to_account_id, dto.amount);
    using var db_context = new ApplicationContext();
    print_database_state(db_context);
    return result == "OK" ? Results.Ok() : Results.BadRequest(result);
});

app.Run();

void print_database_state(ApplicationContext db_context)
{
    Console.Clear();
    Console.WriteLine("--- ТАБЛИЦА СЧЕТА (В базе всего) ---");
    Console.WriteLine("Номер счета          | Банк       | Баланс");
    Console.WriteLine("----------------------------------------------------");

    var all_accounts = db_context.Accounts.ToList();
    foreach (var acc in all_accounts)
    {
        string visibility = acc.is_visible ? "ДА" : "НЕТ";
        Console.WriteLine($"{acc.Id} | {acc.Bank,-10} | {acc.Balance,8:F2}");
    }

    Console.WriteLine("\n--- ТАБЛИЦА ТРАНЗАКЦИИ ---");
    var all_transactions = db_context.Transactions.ToList();
    foreach (var trans in all_transactions)
    {
        Console.WriteLine($"{trans.Id,-2} | {trans.AccountId} | {trans.TypeTransaction,-10} | {trans.amount,-7:F2} | {trans.date_transaction}");
    }
}

public class account_dto { public string id { get; set; } public string bank { get; set; } }
public class transfer_dto { public string from_account_id { get; set; } public string to_account_id { get; set; } public decimal amount { get; set; } }
