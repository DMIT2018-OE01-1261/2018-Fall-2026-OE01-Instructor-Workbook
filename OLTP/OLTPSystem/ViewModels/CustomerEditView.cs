using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.ViewModels
{
    public class CustomerEditView
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Address1 { get; set; } = String.Empty;
        public string Address2 { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public int ProvStateID { get; set; }
        public int CountryID { get; set; }
        public string PostalCode { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public int StatusID { get; set; }
        public bool RemoveFromViewFlag { get; set; }

        // Invoice Information
        public bool HasInvoices { get; set; }
        // Whenever a view has a collection, make sure to default it to an empty collection!
        public List<InvoiceListView> Invoices { get; set; } = [];
    }
}
