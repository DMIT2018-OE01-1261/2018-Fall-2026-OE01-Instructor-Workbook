using Microsoft.AspNetCore.Components;
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
        #endregion

        #region Properties
        //Must match the name in the @page directive (capitalization matters!)
        [Parameter]
        public int CustomerID { get; set; }
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        [Inject]
        protected LookupService LookupService { get; set; } = default!;
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
        #endregion
    }
}
