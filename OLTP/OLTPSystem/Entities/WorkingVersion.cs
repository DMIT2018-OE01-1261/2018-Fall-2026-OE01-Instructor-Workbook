using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLTPSystem.Entities;

public partial class WorkingVersion
{
    [Key]
    public int VersionId { get; set; }

    public int Major { get; set; }

    public int Minor { get; set; }

    public int Build { get; set; }

    public int Revision { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AsOfDate { get; set; }

    [StringLength(50)]
    public string? Comments { get; set; }
}
