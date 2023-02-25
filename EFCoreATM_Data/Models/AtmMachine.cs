
namespace EFCoreATM_Data.Models;

public class AtmMachine
{
    public int Id { get; set; }
    public decimal AtmBalance { get; set; }
    public DateTime LoadDate { get; set; }

    public ICollection<TransactionDetail> Transactions { get; set; }
}
