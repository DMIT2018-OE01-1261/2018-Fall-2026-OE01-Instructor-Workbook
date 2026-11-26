using Microsoft.AspNetCore.Components;
using MudBlazor;
using OLTPSystem.BLL;
using OLTPSystem.Helpers;
using OLTPSystem.ViewModels;

namespace OLTPWebApp.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        private InvoiceView invoice = new();
        private List<InvoicePartView> parts = [];
        private List<(int CategoryID, string CategoryName)> categories = [];

        // The "Add a part" controls
        private int? categoryID;
        private InvoicePartView? selectedPart;
        private int quantity;

        // true once anything on the invoice has changes and is not saved
        private bool edited;

        private string feedbackMessage = string.Empty;
        // general error message
        private string errorMessage = string.Empty;
        // collect error details
        private List<string> errorDetails = [];
        #endregion

        #region Parameters 
        [Parameter]
        public int InvoiceID { get; set; }
        [Parameter]
        public int CustomerID { get; set; }
        [Parameter]
        public int EmployeeID { get; set; }
        #endregion

        #region Properties
        [Inject]
        protected InvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        protected PartService PartService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        // The lines to show: deleted lines stay in the main InvoiceView entity
        //  so they can be deleted (logically) in the database
        //  We make an ActiveLines list to show only the not deleted lines
        private List<InvoiceLineView> ActiveLines => invoice.InvoiceLines
            .Where(x => !x.RemoveFromViewFlag)
            .ToList();

        // Parts in the selected category that are NOT already on the invoice
        private List<InvoicePartView> AvailableParts => parts
            .Where(x => x.PartCategoryID == categoryID
                && !ActiveLines.Select(l => l.PartID).Contains(x.PartID))
            .ToList();
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            ClearMessages();

            try
            {
                // InvoiceID 0 returns a new, empty invoice for this customer and employee
                var invoiceResult = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                if (invoiceResult.IsSuccess)
                    invoice = invoiceResult.Value ?? new();
                else
                    errorDetails.AddRange(ResultHelpers.GetErrorMessages(invoiceResult.Errors));

                var partResult = PartService.GetParts();
                if(partResult.IsSuccess)
                {
                    parts = partResult.Value ?? [];
                    categories = parts
                        .Select(x => (x.PartCategoryID, x.CategoryName))
                        .Distinct()
                        .ToList();
                }
                else
                {
                    errorDetails.AddRange(ResultHelpers.GetErrorMessages(partResult.Errors));
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }

        private void CategoryChanged(int? newCategoryID)
        {
            categoryID = newCategoryID;
            selectedPart = null;
            quantity = 0;
        }

        // Add a child to the parent. Nothing is saved until Save is clicked.
        private void AddPart()
        {
            if (selectedPart == null || quantity <= 0)
                return;

            // If the part was on the invoice AND deleted, bring back that line
            //  (a part should only be added to an invoice once, even a deleted one)
            InvoiceLineView? deletedLine = invoice.InvoiceLines
                .FirstOrDefault(x => x.PartID == selectedPart.PartID && x.RemoveFromViewFlag);
            if(deletedLine != null)
            {
                deletedLine.RemoveFromViewFlag = false;
                deletedLine.Quantity = quantity;
                // We are resetting to the default price of the product
                deletedLine.Price = selectedPart.Price;
            }
            else
            {
                invoice.InvoiceLines.Add(new InvoiceLineView
                {
                    PartID = selectedPart.PartID,
                    PartDescription = selectedPart.Description,
                    Price = selectedPart.Price,
                    Quantity = quantity,
                    Taxable = selectedPart.Taxable
                });
            }

            //reset the "add a part" controls
            categoryID = null;
            selectedPart = null;
            quantity = 0;
            UpdateTotal();
        }

        private void QuantityEdited(InvoiceLineView line, int newQualtity)
        {
            line.Quantity = newQualtity;
            UpdateTotal();
        }
        private void PriceEdited(InvoiceLineView line, decimal newPrice)
        {
            line.Price = newPrice;
            UpdateTotal();
        }

        // The page shows the totals as the user works. The service recalculates them when it saves.
        private void UpdateTotal()
        {
            invoice.SubTotal = ActiveLines.Sum(x => x.ExtendedPrice);
            invoice.Tax = ActiveLines.Sum(x => x.Taxable ? x.ExtendedPrice * 0.05m : 0);
            edited = true;
        }
        private void ClearMessages()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;
        }
        #endregion
    }
}
