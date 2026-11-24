using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.ViewModels
{
    public class InvoicePartView
    {
        public int PartID { get; set; }
        public int PartCategoryID { get; set; }
        public string Description { get; set; } = string.Empty;
        public int QOH { get; set; }
        public decimal Price { get; set; }
        public bool Taxable { get; set; }

        // Calculated Fields
        public string CategoryName { get; set; } = string.Empty;
    }
}
