
namespace EFCoreATM_Data.Models;

public class Admin : User
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public ICollection<TransactionDetail> Transactions { get; set; }
}
