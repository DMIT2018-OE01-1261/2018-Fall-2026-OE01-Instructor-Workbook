using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.ViewModels
{
    public class InvoiceLineView
    {
        public int InvoiceLineID { get; set; }
        public int InvoiceID { get; set; }
        public int PartID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool RemoveFromViewFlag { get; set; }

        //Calculated Fields
        public string PartDescription { get; set; } = string.Empty;
        public bool Taxable { get; set; }

        //Read-only Field (get only)
        public decimal ExtendedPrice => Quantity * Price;
    }
}
