using EFCoreATM_Data;
using EFCoreATM_Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreATM_Domain.Admin;

public class AdminLogic
{
    AtmDbContextFactory atmDbContextFactory = new AtmDbContextFactory();

    private static EFCoreATM_Data.Models.Admin currentAdmin;

    public void Login()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.WriteLine("\n\t Enter Username:");
        string username = Console.ReadLine();

        Console.WriteLine("Enter Password:");
        string password = Console.ReadLine();

        var admin = context.Admins.FirstOrDefault(a => a.UserName == username && a.Password == password);
        if (admin != null)
        {
            Console.WriteLine("\n\t Login Successful!");
            currentAdmin = admin;

            //ShowAdminMenu();
        }
        else
        {
            Console.WriteLine("Invalid Username or Password. Try again.");
            Login();
        }
    }


    public void LoadCash()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.WriteLine("\n\t Enter Amount to Load: \n\t ");
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount))
        {
            Console.WriteLine("\n\t Invalid input. Enter a valid Amount:\n\t ");
        }

        var atm = context.AtmMachine.FirstOrDefault();
        if (atm != null)
        {
            atm.AtmBalance += amount;
            atm.LoadDate = DateTime.Now;

            context.SaveChanges();
            Console.WriteLine($"\n\t ATM Loaded with ${amount} successfully.");
        }
        else
        {
            Console.WriteLine("\n\t ATM not found. Try again later.");
        }
    }


    public void CheckAtmBalance()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var atm = context.AtmMachine.FirstOrDefault();
        if (atm != null)
        {
            Console.WriteLine($"\n\t ATM Balance: ${atm.AtmBalance}");
        }
        else
        {
            Console.WriteLine("\n\t ATM not found. Try again later.");
        }
    }


    public void RegisterCustomer()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.WriteLine("\n\t Enter First Name: \n\t ");
        string firstName = Console.ReadLine();

        Console.WriteLine("\n\t Enter Last Name:\n\t ");
        string lastName = Console.ReadLine();

        Console.WriteLine("\n\t Enter Email Address:\n\t ");
        string email = Console.ReadLine();

        Console.WriteLine("\n\t Enter Address:\n\t ");
        string address = Console.ReadLine();

        Console.WriteLine("\n\t Enter Phone Number:\n\t ");
        string phone = Console.ReadLine();

        string accountNumber;
        do
        {
            accountNumber = "30000" + new Random().Next(10000, 99999).ToString();
        } while (context.Customers.Any(c => c.AccountNumber == accountNumber));

        string defaultPin = new Random().Next(10000, 99999).ToString();

        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Address = address,
            PhoneNumber = phone,
            AccountNumber = accountNumber,
            AccountType = "Savings",
            AccountBalance = 0,
            DefaultAtmPin = defaultPin,
            NewAtmPin = "",
            IsActive = false,
            RegisteredDate = DateTime.Now,
        };

        context.Customers.Add(customer);
        context.SaveChanges();

        Console.WriteLine($"\n\t Customer {firstName} {lastName} with Account Number {accountNumber} has been registered successfully.");
    }


    public void EditCustomer()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.Write("\n\t Enter customer account number: \n\t ");
        string accountNumber = Console.ReadLine();


        Customer customer = context.Customers.FirstOrDefault(c => c.AccountNumber == accountNumber);

        if (customer == null)
        {
            Console.WriteLine($"\n\t Customer with account number {accountNumber} does not exist.");
            return;
        }

        Console.Write("\n\t Enter updated first name (leave empty to skip): \n\t ");
        string firstName = Console.ReadLine();
        if (!string.IsNullOrEmpty(firstName))
        {
            customer.FirstName = firstName;
        }

        Console.Write("\n\t Enter updated last name (leave empty to skip): \n\t ");
        string lastName = Console.ReadLine();

        if (!string.IsNullOrEmpty(lastName))
        {
            customer.LastName = lastName;
        }

        Console.Write("\n\t Enter updated email address (leave empty to skip): \n\t ");
        string email = Console.ReadLine();

        if (!string.IsNullOrEmpty(email))
        {
            customer.Email = email;
        }

        Console.Write("\n\t Enter updated phone number (leave empty to skip): \n\t ");
        string phone = Console.ReadLine();

        if (!string.IsNullOrEmpty(phone))
        {
            customer.PhoneNumber = phone;
        }

        try
        {
            context.Customers.Update(customer);
            context.SaveChanges();
            Console.WriteLine("\n\t Customer details updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n\t An error occurred while updating customer details: {ex.Message}");
        }
    }


    public void DepositToCustomerAccount(int customerId, decimal amount)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var customer = context.Customers.Find(customerId);
        if (customer == null)
        {
            Console.WriteLine("\n\t Customer not found.");
            return;
        }

        customer.AccountBalance += amount;

        var transaction = new TransactionDetail
        {
            TransactionType = "Deposit",
            Sender = "ATM",
            Receiver = customer.AccountNumber,
            TransactionDate = DateTime.Now,
            TransactedAmount = amount
        };

        context.Transactions.Add(transaction);
        context.SaveChanges();

        Console.WriteLine($"\n\t {amount:C} deposited successfully to {customer.FirstName} {customer.LastName}'s account.");
    }


    public void ViewAllCustomers()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var customers = context.Customers.ToList();

        Console.WriteLine("\n\t List of all registered customers:");
        foreach (var customer in customers)
        {
            Console.WriteLine($"\n\t {customer.FirstName} {customer.LastName} ({customer.Email})");
        }
    }


    public void ViewAllTransactions()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var transactions = context.Transactions
            .Where(t => t.TransactionType != "ATM Load")
            .ToList();

        Console.WriteLine("\n\t List of all transactions:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine($"\n\t {transaction.TransactionType} of {transaction.TransactedAmount:C} on {transaction.TransactionDate:MM/dd/yyyy} between {transaction.Sender} and {transaction.Receiver}");
        }
    }


    public void RenderCustomerAccountInactive(int customerId)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var customer = context.Customers.Find(customerId);
        if (customer == null)
        {
            Console.WriteLine("\n\t Customer not found.");
            return;
        }

        customer.IsActive = false;
        context.SaveChanges();

        Console.WriteLine($"\n\t {customer.FirstName} {customer.LastName}'s account has been rendered inactive.");
    }
}
