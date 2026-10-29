using System;
using System.Collections.Generic;
using System.Text;
using BYSResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OLTPSystem.DAL;
using OLTPSystem.ViewModels;

namespace OLTPSystem.BLL
{
    // Invoices are the parent records, invoice lines are the children.
    //  The children are always handles through their parent, so they share this service.
    public class InvoiceService
    {
        #region Data Context Setup
        private readonly OLTPContext _context;

        // The context is passed in (injected). Right now the tests will pass it in for you.
        //  one in Blazor this happens almost automatically
        public InvoiceService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        // This method will return an existing Invoice if it is found OR a new invoice (not saved to the database)
        //  When existing - will return the existing record as a InvoiceView
        //      when existing - we will update the employee or customer info if it is different
        //  When new - will return a InvoiceView with the customer and employee info provided.
        // NOTE: We will not save the new invoice to the database!

        public Result<InvoiceView> GetInvoice(int invoiceID, int customerID, int employeeID)
        {
            var result = new Result<InvoiceView>();
            //rule: parameters are provided
            if (customerID <= 0)
                result.AddError(new Error("Missing Information", "Customer ID must be provided."));
            if (employeeID <= 0)
                result.AddError(new Error("Missing Information", "Employee ID must be provided."));
            //We will not check Invoice, because if it is 0 we will return a new invoice
            if (result.IsFailure)
                return result;

            //Handle noth new and existing invoices
            //  For new invoices only the customer and employee id are needed
            InvoiceView? invoice;

            //Check if the invoice is new
            //  If it is new, we will give it the default date as today and add the provided info
            if (invoiceID == 0)
            {
                invoice = new InvoiceView
                {
                    CustomerID = customerID,
                    EmployeeID = employeeID,
                    // This is a Date ONLY field
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Now)
                };
            }
            else
            {
                invoice = _context.Invoices
                    .Where(x => x.InvoiceID == invoiceID
                        && !x.RemoveFromViewFlag)
                    .Select(x => new InvoiceView
                    {
                        InvoiceID = x.InvoiceID,
                        //Using the provided customer and employee IDs
                        CustomerID = customerID,
                        EmployeeID = employeeID,
                        InvoiceDate = x.InvoiceDate,
                        SubTotal = x.SubTotal,
                        Tax = x.Tax,
                        RemoveFromViewFlag = x.RemoveFromViewFlag,
                        //The child record: only get the lines that are not logically deleted
                        InvoiceLines = x.InvoiceLines
                            .Where(i => !i.RemoveFromViewFlag)
                            .Select(i => new InvoiceLineView
                            {
                                InvoiceLineID = i.InvoiceLineID,
                                InvoiceID = i.InvoiceID,
                                PartID = i.PartID,
                                Price = i.Price,
                                Quantity = i.Quantity,
                                RemoveFromViewFlag = i.RemoveFromViewFlag,
                                //Calculated Fields
                                PartDescription = i.Part.Description,
                                Taxable = i.Part.Taxable
                            }).ToList()
                    }).FirstOrDefault();
            }

            // invoice could be null if it didn't exist in the database or the invoice was logically deleted
            //  Check BEFORE we use it, or the next lines would crash on a null invoice
            if(invoice == null)
            {
                result.AddError(new Error("No results", $"No invoice was found for the invoice ID {invoiceID}"));
                return result;
            }

            // Because they can change the customer and employee ID of an existing record
            // We will populare the Names after the initial LINQ query
            // ?? here means if the result of the query before the ?? is null,
            //      use what is after the ?? as the value
            invoice.CustomerName = _context.Customers
                    .Where(x => x.CustomerID == customerID)
                    .Select(x => x.FirstName + " " + x.LastName).FirstOrDefault() ?? string.Empty;
            invoice.EmployeeName = _context.Employees
                    .Where(x => x.EmployeeID == employeeID)
                    .Select(x => x.FirstName + " " + x.LastName).FirstOrDefault() ?? string.Empty;

            // NOTE: NOTHING WAS ADDED OR SAVED TO THE DATABASE IN THIS METHOD
            //  We only return an InvoiceView so we can add to it or edit it
            //  AddInvoice or UpdateInvoice saves it afterwards
            return result.WithValue(invoice);
        }

        private void AddInvoice(InvoiceView invoice)
        {
            var result = new Result<InvoiceView>();
            ValidateInvoice(invoice, result);
        }

        // Private Helpers
        private void ValidateInvoice(InvoiceView invoiceView, Result<InvoiceView> result)
        {
            //Parent rules (collect all)
            if (invoiceView.CustomerID <= 0)
                result.AddError(new Error("Missing Information", "A valid customer ID is required."));
            if (invoiceView.EmployeeID <= 0)
                result.AddError(new Error("Missing Information", "A valid employee ID is required."));
            //rule: there must be at least 1 invoice Line
            if (invoiceView.InvoiceLines.Count == 0)
                result.AddError(new Error("Missing Information", "Invoice details are required."));

            //child rules: check EVERY child record
            foreach(var invoiceLine in invoiceView.InvoiceLines)
            {
                //rule: a part is required
                //  stop checking right away: the other checks need the part
                if(invoiceLine.PartID <= 0)
                {
                    result.AddError(new Error("Missing Information", "Missing Part ID"));
                    return;
                }

                //rule: price cannot be less than zero
                if(invoiceLine.Price < 0)
                {
                    string? partName = _context.Parts
                        .Where(x => x.PartID == invoiceLine.PartID)
                        .Select(x => x.Description)
                        .FirstOrDefault();
                    result.AddError(new Error("Invalid Price",
                        $"Part {partName} has a price that is less than zero."));
                }

                //result: quantity must be at least 1
                if(invoiceLine.Quantity < 1)
                {
                    string? partName = _context.Parts
                        .Where(x => x.PartID == invoiceLine.PartID)
                        .Select(x => x.Description)
                        .FirstOrDefault();
                    result.AddError(new Error("Invalid Quantity",
                        $"Part {partName} has a quantity that is less than one."));
                }
            }

            //OUTSIDE the loop: rules about the lines as a group
            // rule: a part can only be on the invoice once
            List<string?> duplicatedParts = invoiceView.InvoiceLines
                .GroupBy(x => x.PartID)
                //more than 1 line for the same part
                .Where(x => x.Count() > 1)
                //We cannot use navigational properties here because of the Group By
                //  so we look up the part's name in the Parts table
                .Select(x => _context.Parts
                    .Where(p => p.PartID == x.Key)
                    .Select(p => p.Description)
                    .FirstOrDefault())
                .ToList();

            foreach (var partName in duplicatedParts)
            {
                result.AddError(new Error("Duplicate Invoice Line",
                    $"Part {partName} can only be added to the invoice once."));
            }  
        }
    }
}
