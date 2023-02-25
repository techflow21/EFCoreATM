
namespace EFCoreATM_Data.Models;

public class TransactionDetail
{
    public int Id { get; set; }
    public string TransactionType { get; set; }
    public string Sender { get; set; }
    public string Receiver { get; set; }
    public decimal TransactedAmount { get; set; }
    public DateTime TransactionDate { get; set; }

    public Admin Admin { get; set; }
    public AtmMachine AtmMachine { get; set; }
    public Customer SenderCustomer { get; set; }
    public Customer ReceiverCustomer { get; set; }
}
