using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using OLTPSystem.BLL;
using OLTPSystem.Helpers;
using OLTPSystem.ViewModels;

namespace OLTPWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        private CustomerEditView editCustomer = new();
        // Lists to hold the options for the select fields on the Customer Edit form
        private List<LookupView> countries = [];
        private List<LookupView> provinces = [];
        private List<LookupView> statuses = [];

        private string feedbackMessage = string.Empty;
        // general error message
        private string errorMessage = string.Empty;
        // collect error details
        private List<string> errorDetails = [];

        // @ref fields are assigned by Blazor after the component first renders.
        // default! tell the compiler "this starts null, but Blazor will set it before I use it"/
        private MudForm customerForm = default!;
        // bound to the form: true when every field has a valid value
        private bool isFormValid;
        // bound to the form: true once the user has changed a field
        private bool hasFormChanged;

        // No login yet, every invoice is made by employee 1 (Nole Body)
        private const int EMPLOYEE_ID = 1;
        #endregion

        #region Properties
        //Must match the name in the @page directive (capitalization matters!)
        [Parameter]
        public int CustomerID { get; set; }
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        [Inject]
        protected LookupService LookupService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;


        private string closeButtonText => hasFormChanged ? "Cancel" : "Close";
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            try
            {
                // A Customer of 0 is a new customer: nothing to get
                //  editCustomer is already new() when we create the field
                if (CustomerID != 0)
                {
                    var result = CustomerService.GetCustomer_ByID(CustomerID);
                    if (result.IsSuccess)
                        editCustomer = result.Value ?? new();
                    else
                        errorDetails = ResultHelpers.GetErrorMessages(result.Errors);
                }

                //Fill the drop-downs
                provinces = GetLookup("Province");
                countries = GetLookup("Country");
                statuses = GetLookup("Customer Status");
            }
            catch(Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }
        private List<LookupView> GetLookup(string categoryName)
        {
            var result = LookupService.GetLookupValues(categoryName);
            if (result.IsSuccess)
                return result.Value ?? [];

            errorDetails.AddRange(ResultHelpers.GetErrorMessages(result.Errors));
            return [];
        }
        private async Task CancelAsync()
        {
            //Anytime the user could lose changes, ask first
            if (await ConfirmLeave("Are you sure you want to leave? All unsaved changes will be lost."))
                NavigationManager.NavigateTo("/SamplePages/Customers");
            else
                return;
        }

        private void SaveCustomer()
        {
            ClearMessages();

            // Format the values before they are saved, so the database is consistent.
            //  Trim() removes spaces at the start and end, ToLower() makes Bob@Mail.com bob@mail.com
            editCustomer.Email = editCustomer.Email.Trim().ToLower();

            try
            {
                bool isNew = editCustomer.CustomerID == 0;
                var result = isNew
                    ? CustomerService.AddCustomer(editCustomer)
                    : CustomerService.UpdateCustomer(editCustomer);

                if (result.IsSuccess)
                {
                    editCustomer = result.Value ?? new();
                    feedbackMessage = isNew
                        ? $"Customer {editCustomer.CustomerID} was added."
                        : "Customer was saved.";

                    // The form now matches the database, so reset the "changed" tracker for the form
                    hasFormChanged = false;
                    customerForm.ResetTouched();
                }
                else
                    errorDetails = ResultHelpers.GetErrorMessages(result.Errors);
            }
            catch(Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }

        //A method that shows a dialog must be async and return a Task
        private async Task DeleteCustomer()
        {
            ClearMessages();

            bool? answer = await DialogService.ShowMessageBoxAsync(
                "Confirm Delete",
                $"Are you sure you want to delete {editCustomer.FirstName} {editCustomer.LastName}?",
                yesText: "Delete",
                cancelText: "Cancel");
            //answer is true for Yes, null if the user cancels or closes the dialog
            if (answer != true)
                return;

            try
            {
                var result = CustomerService.DeleteCustomer(editCustomer.CustomerID);
                if (result.IsSuccess)
                {
                    Snackbar.Add($"{editCustomer.FirstName} {editCustomer.LastName} was deleted.", Severity.Success);
                    NavigationManager.NavigateTo("/SamplePages/Customers");
                }
                else
                    errorDetails = ResultHelpers.GetErrorMessages(result.Errors);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
        }

        private async Task NewInvoice()
        {
            if (await ConfirmLeave("Are you sure you want to start an invoice? All unsaved customer changes will be lost."))
                NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/0/{editCustomer.CustomerID}/{EMPLOYEE_ID}");
            else
                return;
        }
        private async Task EditInvoice(int invoiceID)
        {
            if (await ConfirmLeave("Are you sure you want to edit the invoice? All unsaved customer changes will be lost."))
                NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/{invoiceID}/{editCustomer.CustomerID}/{EMPLOYEE_ID}");
            else
                return;
        }

        private void ClearMessages()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;
        }

        #region Validation
        // Validation Functions
        //  MudBlazor calls these as the user types (because of Immediate).
        //  Return null when the value is valid, or the error message to show under the field.

        // A regular expression describes the pattern the whole value must match:
        //  ^ start, [A-Z] one letter, \d one digit, " ?" an optional space, $ end
        //  The first letter can't be D, F, I, O, Q, U, W or Z, and later letters
        //  can't be D, F, I, O, Q or U (Canada Post rules).
        private string? ValidatePostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return null; // Required already shows "must be provided"

            return Regex.IsMatch(postalCode, @"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z] ?\d[ABCEGHJ-NPRSTV-Z]\d$")
                ? null
                : "Postal Code must look like T5G 2T5.";
        }

        // After the mask, the phone value is digits only, so it is valid when all 10 are typed.
        //  Counting digits (not Length) also accepts older data saved as 780-555-1234.
        private string? ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;    //Required already shows "must be provided"

            return phone.Count(char.IsDigit) == 10
                ? null
                : "Phone number must be 10 digits, like (780) 555-1234.";
        }
        //Email pattern, read left to right:
        //  ^[^@\s]+         the name: one or more characters that are not @ or a space
        //  @                exactly one @
        //  ([^@\s.]+\.)+    one or more domain parts, each followed by a real dot
        //                   (nait. or mail.nait.). Without the \ a dot means "any character".
        //  [A-Za-z]{2,}$    the ending: 2 or more letters (ca, com) at the very end
        //  So bob@test fails (no dot), and bob@test.ca passes.
        private string? ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;    //Required already shows "must be provided"

            return Regex.IsMatch(email.Trim(), @"^[^@\s]+@([^@\s.]+\.)+[A-Za-z]{2,}$")
                ? null
                : "Email must look like name@example.com.";
        }


        // Postal code: letter-digit-letter space digit-letter-digit, always uppercase
        private PatternMask postalCodeMask = new("a0a 0a0")
        {
            Transformation = c => char.ToUpper(c)
        };
        private PatternMask phoneMask = new("(000) 000-0000")
        {
            CleanDelimiters = true
        };
        #endregion

        private async Task<bool> ConfirmLeave(string message)
        {
            if (!hasFormChanged)
                return true;

            bool? answer = await DialogService.ShowMessageBoxAsync("Unsaved Changes", message,
                yesText: "Yes", noText: "No");
            return answer == true;
        }

        #endregion
    }
}
