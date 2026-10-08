using OLTPSystem.DAL;
using OLTPSystem.Entities;
using OLTPSystem.ViewModels;
using BYSResults;

namespace OLTPSystem.BLL
{
    //Change to a public class
    public class CustomerService
    {
        #region Data Context Setup
        private readonly OLTPContext _context;

        // The context is passed in (injected). Right now the tests will pass it in for you.
        //  one in Blazor this happens almost automatically
        public CustomerService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        //Business Logic Methods
        public Result<List<CustomerSearchView>> GetCustomers(string lastName, string phone)
        {
            // Always set up your result at the top of the method
            //  Give the result the same type as the method return type
            var result = new Result<List<CustomerSearchView>>();

            // rule: either last name or phone must be provided
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
            {
                result.AddError(new Error("Missing Information", "Either a partial phone number or a last name must be provided"));
                // Since this is missing information, there is no point in continuing
                //  So we return right away
                return result;
            }

            return new Result<List<CustomerSearchView>>();
        }
    }
}
