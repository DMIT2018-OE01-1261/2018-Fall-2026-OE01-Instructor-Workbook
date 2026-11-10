using Microsoft.AspNetCore.Components;
using OLTPSystem.BLL;
using OLTPSystem.Helpers;
using OLTPSystem.ViewModels;

namespace OLTPWebApp.Components.Pages.SamplePages
{
    public partial class Customers
    {
        #region Fields
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private List<CustomerSearchView> customerList = [];

        private string feedbackMessage = string.Empty;
        // general error message
        private string errorMessage = string.Empty;
        // collect error details
        private List<string> errorDetails = [];
        #endregion

        #region Properties
        // [Inject] asks Blazor for the service we registered in Program.cs (OLTPDependencies)
        //  This replaces the "new CustomerService(Context)" we wrote in the tests.
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        #endregion

        #region Methods
        // The service method takes parameters; here they come from the page's field
        private void GetCustomer()
        {
            // ALWAYS starts by clearing messages (feedback and errors)
            //  so old message don't stay on the page
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

            try
            {
                var result = CustomerService.GetCustomers(lastName, phoneNumber);
                if (result.IsSuccess)
                    customerList = result.Value ?? [];
                else
                {
                    customerList = [];
                    errorDetails = ResultHelpers.GetErrorMessages(result.Errors);
                }
            }
            catch(Exception ex)
            {
                //capture any unexpected exceptions
                errorMessage = ex.Message;
            }
        }
        #endregion
    }
}
