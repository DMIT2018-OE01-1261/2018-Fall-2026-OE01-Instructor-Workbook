using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLTPSystem.Entities;

[Table("Customer")]
public partial class Customer
{
    [Key]
    public int CustomerID { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Address1 { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    public int ProvStateID { get; set; }

    public int CountryID { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PostalCode { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    public int StatusID { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("CountryID")]
    [InverseProperty("CustomerCountries")]
    public virtual Lookup Country { get; set; } = null!;

    [InverseProperty("Customer")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("ProvStateID")]
    [InverseProperty("CustomerProvStates")]
    public virtual Lookup ProvState { get; set; } = null!;

    [ForeignKey("StatusID")]
    [InverseProperty("CustomerStatuses")]
    public virtual Lookup Status { get; set; } = null!;
}
