using Microsoft.EntityFrameworkCore;
using EFCoreATM_Data.Models;
using EFCoreATM_Data;

namespace EFCoreATM_Domain.Customer_BLL;

public class CustomerLogic
{
    AtmDbContextFactory atmDbContextFactory = new AtmDbContextFactory();
    public void CustomerMenu()
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.WriteLine("\n\t Welcome to the Customer Menu");

        Console.WriteLine("\n\t Please enter your Account Number: ");
        string accountNumber = Console.ReadLine();

        Console.WriteLine("\n\t Please enter your Default ATM PIN: ");
        string atmPin = Console.ReadLine();

        var customer = context.Customers.FirstOrDefault(c => c.AccountNumber == accountNumber);

        if (customer != null && customer.DefaultAtmPin == atmPin)
        {
            Console.WriteLine("\n\t Login successful!");
            Console.WriteLine("\n\t What would you like to do?");

            bool continueOperations = true;

            while (continueOperations)
            {
                Console.WriteLine("\n\t 1. View Account Details\n\t 2. Check Account Balance\n\t 3. Transfer Money\n\t 4. Withdraw Money\n\t 5. View Account Transactions\n\t 6. Change Default ATM PIN \n\t 7. Logout\n");

                Console.Write("\n\t Enter your choice: \n\t ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAccountDetails(customer);
                        break;
                    case "2":
                        CheckAccountBalance(customer);
                        break;
                    case "3":
                        TransferMoney(customer);
                        break;
                    case "4":
                        WithdrawMoney(customer);
                        break;
                    case "5":
                        ViewAccountTransactions(customer);
                        break;
                    case "6":
                        ChangeDefaultAtmPin(customer);
                        break;
                    case "7":
                        Console.WriteLine("\n\t Logging out of Customer menu...");
                        continueOperations = false;
                        break;
                    default:
                        Console.WriteLine("\n\t Invalid choice, please try again.");
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine("\n\t Invalid Account Number or ATM PIN.");
        }
    }

    public static void ViewAccountDetails(Customer customer)
    {
        Console.WriteLine($"\n\t Customer Details: ");
        Console.WriteLine($"\n\t Name: {customer.FirstName} {customer.LastName}");
        Console.WriteLine($"\n\t Email: {customer.Email}");
        Console.WriteLine($"\n\t Address: {customer.Address}");
        Console.WriteLine($"\n\t Phone Number: {customer.PhoneNumber}");
        Console.WriteLine($"\n\t Account Number: {customer.AccountNumber}");
        Console.WriteLine($"\n\t Account Type: {customer.AccountType}");
        Console.WriteLine($"\n\t Account Balance: {customer.AccountBalance:C}");
        Console.WriteLine($"\n\t Registered Date: {customer.RegisteredDate.ToShortDateString()}");
        Console.WriteLine($"\n\t Is Active: {customer.IsActive}");
    }

    public static void CheckAccountBalance(Customer customer)
    {
        Console.WriteLine($"\n\t Your Account Balance: {customer.AccountBalance:C}");
    }

    public void Transfer(Customer sender, List<Customer> customers, decimal atmBalance)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.Write("\n\t Enter Receiver's Account Number: \n\t ");
        string receiverAccountNumber = Console.ReadLine();

        Customer receiver = customers.FirstOrDefault(c => c.AccountNumber == receiverAccountNumber);

        if (receiver == null)
        {
            Console.WriteLine("\n\t Receiver's Account Number not found.");
            return;
        }

        Console.WriteLine("\n\t Enter Amount to Transfer: \n\t ");
        decimal transferAmount;

        while (!decimal.TryParse(Console.ReadLine(), out transferAmount) || transferAmount <= 0)
        {
            Console.WriteLine("\n\t Invalid amount. Please try again.");
        }

        if (sender.AccountBalance < transferAmount)
        {
            Console.WriteLine("\n\t Insufficient funds.");
            return;
        }


        if (atmBalance < transferAmount)
        {
            Console.WriteLine("\n\t ATM does not have enough cash.");
            return;
        }

        sender.AccountBalance -= transferAmount;

        receiver.AccountBalance += transferAmount;

        TransactionDetail senderTransaction = new TransactionDetail()
        {
            TransactionType = TransactionType.Transfer.ToString(),
            Sender = $"{sender.FirstName} {sender.LastName}",
            Receiver = receiverAccountNumber,
            TransactionDate = DateTime.Now,
            TransactedAmount = transferAmount
        };

        sender.Transactions.Add(senderTransaction);

        TransactionDetail receiverTransaction = new TransactionDetail()
        {
            TransactionType = TransactionType.Transfer.ToString(),
            Sender = $"{sender.FirstName} {sender.LastName}",
            Receiver = receiverAccountNumber,
            TransactionDate = DateTime.Now,
            TransactedAmount = transferAmount
        };

        receiver.Transactions.Add(receiverTransaction);

        context.SaveChanges();

        Console.WriteLine("\n\t Transfer successful.");
    }


    public void WithdrawCash(Customer customer)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.Write("\n\t Enter amount to withdraw: \n\t ");
        decimal amountToWithdraw = decimal.Parse(Console.ReadLine());

        if (amountToWithdraw > customer.AccountBalance)
        {
            Console.WriteLine("\n\t Insufficient balance!");
            return;
        }

        if (amountToWithdraw > atmMachine.AtmBalance)
        {
            Console.WriteLine("\n\t ATM machine does not have enough cash!");
            return;
        }

        customer.AccountBalance -= amountToWithdraw;
        atmMachine.AtmBalance -= amountToWithdraw;

        TransactionDetail transaction = new TransactionDetail
        {
            TransactionType = "Withdrawal",
            Sender = $"{customer.FirstName} {customer.LastName}",
            Receiver = $"{customer.AccountNumber}",
            TransactedAmount = amountToWithdraw,
            TransactionDate = DateTime.Now
        };

        context.Transactions.Add(transaction);
        context.SaveChanges();

        Console.WriteLine($"\n\t Withdrawal successful. Your new balance is ${customer.AccountBalance}.");
    }

    public void ChangeAtmPin(Customer customer)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        Console.Write("\n\t Enter current ATM pin: \n\t ");
        string currentPin = Console.ReadLine();

        if (currentPin != customer.NewAtmPin)
        {
            Console.WriteLine("\n\t Invalid current ATM pin!");
            return;
        }

        Console.Write("\n\t Enter new ATM pin: \n\t ");
        string newPin = Console.ReadLine();

        Console.Write("\n\t Confirm new ATM pin: \n\t ");
        string confirmPin = Console.ReadLine();

        if (newPin != confirmPin)
        {
            Console.WriteLine("\n\t New pin does not match confirmation! \n\t ");
            return;
        }

        customer.NewAtmPin = newPin;

        context.Customers.Update(customer);
        context.SaveChanges();

        Console.WriteLine("\n\t ATM pin updated successfully!");
    }

    public void ViewPersonalDetails(Customer customer)
    {
        Console.WriteLine($"\n\t Full Name: {customer.FirstName} {customer.LastName}");
        Console.WriteLine($"\n\t Email: {customer.Email}");
        Console.WriteLine($"\n\t Address: {customer.Address}");
        Console.WriteLine($"\n\t Phone Number: {customer.PhoneNumber}");
        Console.WriteLine($"\n\t Account Number: {customer.AccountNumber}");
        Console.WriteLine($"\n\t Account Type: {customer.AccountType}");
        Console.WriteLine($"\n\t Account Balance: ${customer.AccountBalance}");
    }


    public void ViewPersonalTransactions(Customer customer)
    {
        var context = atmDbContextFactory.CreateDbContext(null);

        var transactions = context.Transactions
                                .Where(t => t.Sender == $"{customer.FirstName} {customer.LastName}"
                                            || t.Receiver == $"{customer.AccountNumber}")
                                .OrderByDescending(t => t.TransactionDate)
                                .ToList();

        Console.WriteLine("\n\t Transaction History");

        Console.WriteLine($"{"Type",-20} {"Sender/Receiver",-40} {"Amount",-20} {"Date/Time",-20}");

        foreach (var transaction in transactions)
        {
            string transactionType = transaction.TransactionType;
            string senderOrReceiver = "";

            if (transaction.Sender == $"{customer.FirstName} {customer.LastName}")
            {
                senderOrReceiver = $"To {transaction.Receiver}";
            }
            else
            {
                senderOrReceiver = $"From {transaction.Sender}";
            }

            Console.WriteLine($"{transactionType,-20} {senderOrReceiver,-40} ${transaction.TransactedAmount,-20} {transaction.TransactionDate,-20}");
        }
    }
}
