using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLTPSystem.Entities;

[Keyless]
[Table("UnitTest")]
public partial class UnitTest
{
    public int UnitTestID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TableName { get; set; } = null!;

    public int ReferenceID { get; set; }
}
