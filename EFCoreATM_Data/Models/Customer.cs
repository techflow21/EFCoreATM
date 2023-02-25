
using System.ComponentModel.DataAnnotations;

namespace EFCoreATM_Data.Models;

public class Customer : User
{
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public decimal AccountBalance { get; set; }
    public string DefaultAtmPin { get; set; }
    public string NewAtmPin { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegisteredDate { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
    public ICollection<TransactionDetail> Transactions { get; set; }
}
