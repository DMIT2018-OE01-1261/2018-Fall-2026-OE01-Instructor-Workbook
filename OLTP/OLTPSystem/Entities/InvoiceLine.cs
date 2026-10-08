using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLTPSystem.Entities;

[Table("InvoiceLine")]
public partial class InvoiceLine
{
    [Key]
    public int InvoiceLineID { get; set; }

    public int InvoiceID { get; set; }

    public int PartID { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("InvoiceID")]
    [InverseProperty("InvoiceLines")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("PartID")]
    [InverseProperty("InvoiceLines")]
    public virtual Part Part { get; set; } = null!;
}
