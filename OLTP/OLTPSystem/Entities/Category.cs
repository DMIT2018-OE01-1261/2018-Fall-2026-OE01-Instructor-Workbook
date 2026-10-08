using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLTPSystem.Entities;

[Table("Category")]
public partial class Category
{
    [Key]
    public int CategoryID { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    public bool RemoveFromViewFlag { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
}
