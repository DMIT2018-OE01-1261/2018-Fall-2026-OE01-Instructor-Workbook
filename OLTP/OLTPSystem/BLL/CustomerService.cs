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

            //rule: (Standard Rule) RemoveFromViewFlag cannot be true
            // This is a typical rule to follow when returning data
            var customers = _context.Customers
                .Where(x => x.RemoveFromViewFlag == false
                    &&
                    (
                        (!string.IsNullOrWhiteSpace(lastName)
                        &&
                        x.LastName.ToLower().Contains(lastName.ToLower())
                        )
                        ||
                        (!string.IsNullOrWhiteSpace(phone)
                        &&
                        x.Phone.Contains(phone)
                        )
                    )
                )
                .Select(x => new CustomerSearchView
                {
                    CustomerID = x.CustomerID,
                    FullName = x.FirstName + " " + x.LastName,
                    Address = x.Address1 + " " + x.City + ", " + x.ProvState.Name + " " + x.Country.Name + " " + x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    Status = x.Status.Name,
                    TotalSales = x.Invoices.Sum(i => i.SubTotal + i.Tax)
                }).ToList();

            if(!customers.Any())
            {
                result.AddError(new Error("No customers were found"));
                return result;
            }

            //Happy Path - we got customers and no business rules were brokens
            //Remember to return the happy path results WithValue!
            return result.WithValue(customers);
        }

        public Result<CustomerEditView> GetCustomer_ByID(int customerID)
        {
            var result = new Result<CustomerEditView>();

            if(customerID <= 0)
            {
                result.AddError(new Error("Missing Information", "A valid CustomerID must be provided"));
                return result;
            }

            var customer = _context.Customers
                .Where(x => !x.RemoveFromViewFlag
                    && x.CustomerID == customerID)
                .Select(x => new CustomerEditView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address1 = x.Address1,
                    // Address2 can be null in the database, but our view model isn't nullable
                    //      We did set a default, but we should still add a default in our code when
                    //      setting the value
                    // ?? checks if the value before is null, if it is then it returns the value after ??
                    Address2 = x.Address2 ?? string.Empty,
                    City = x.City,
                    ProvStateID = x.ProvStateID,
                    CountryID = x.CountryID,
                    PostalCode = x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();

            if(customer == null)
            {
                result.AddError(new Error("No Customer", $"No customer was found with ID: {customerID}"));
                return result;
            }

            return result.WithValue(customer);
        }
    }
}
