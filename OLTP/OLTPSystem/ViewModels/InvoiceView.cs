using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.ViewModels
{
    public class InvoiceView
    {
        public int InvoiceID { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public int CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public bool RemoveFromViewFlag { get; set; }

        // Calculated Fields
        //  These are not stored in the invoice table. We fill them in when we get the data.
        public string CustomerName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        // Read-only Field (get only)
        //  After the lambda (=>) is what is returned when this fields is called
        public decimal Total => SubTotal + Tax;

        public List<InvoiceLineView> InvoiceLines { get; set; } = [];
    }
}
