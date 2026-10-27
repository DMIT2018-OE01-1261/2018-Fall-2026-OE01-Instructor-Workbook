using OLTPSystem.DAL;
using OLTPSystem.Entities;
using OLTPSystem.ViewModels;
using BYSResults;
using OLTPSystem.Helpers;

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

        #region OLTP Methods
        // Public Methods
        public Result<CustomerEditView> AddCustomer(CustomerEditView newCustomer)
        {
            var result = new Result<CustomerEditView>();

            // rule: a customer must be provided
            if (newCustomer == null)
            {
                result.AddError(new Error("Missing Information", "No customer was provided."));
                // return right away
                return result;
            }
            // rule: a new customer cannot already have an ID
            if (newCustomer.CustomerID != 0)
                result.AddError(new Error("Invalid Customer", "A new customer cannot already have a CustomerID."));

            // share rules (mandatory fields and duplicates)
            ValidateCustomer(newCustomer, result);

            // Once there is no more validatation, check if an error happened and if there was one, 
            // return and do not proceed
            if (result.IsFailure)
                return result;

            //Create a new database record, copy the values and stage the add
            Customer customer = new();

            CopyToCustomer(newCustomer, customer);

            _context.Customers.Add(customer);

            // Call our Save Customer Method
            return SaveCustomer(customer, result);
        }

        public Result<CustomerEditView> UpdateCustomer(CustomerEditView editCustomer)
        {
            var result = new Result<CustomerEditView>();

            if(editCustomer == null)
            {
                result.AddError(new Error("Missing Information", "No customer was provided."));
                // return right away
                return result;
            }

            //rule: an existing customer must have an ID (return right away)
            //  This differs from NEW where they could NOT have an ID
            if(editCustomer.CustomerID <= 0)
            {
                result.AddError(new Error("Missing Information", "A valid Customer ID must be provided."));
            }

            //the shared rules
            ValidateCustomer(editCustomer, result);
            if (result.IsFailure)
                return result;

            // rule: the customer must exist (and not be deleted)
            Customer? customer = _context.Customers
                .Where(x => x.CustomerID == editCustomer.CustomerID
                    && !x.RemoveFromViewFlag)
                .FirstOrDefault();
            // check if the customer was found (if not FirstOrDefault() returns null.
            if(customer == null)
            {
                result.AddError(new Error("Cannot find Record to Edit", $"CustomerID {editCustomer.CustomerID} cannot be found, edits cannot be made."));
                return result;
            }

            // Copy the values and stage the Update
            CopyToCustomer(editCustomer, customer);
            _context.Customers.Update(customer);

            return SaveCustomer(customer, result);
        }

        // Delete a Customer (a SOFT delete: set the flag, keep the record)
        //  Returns a plain Result: there is no customer to send back after a delete
        public Result DeleteCustomer(int customerID)
        {
            var result = new Result();

            //rule: CustomerID must be valid
            if (customerID <= 0)
            {
                result.AddError(new Error("Missing Information", "A valid CustomerID must be provided."));
                return result;
            }

            // rule: the customer must exist and is not already deleted
             Customer? customer = _context.Customers
                .Where(x => x.CustomerID == customerID
                    && !x.RemoveFromViewFlag)
                .FirstOrDefault();
            if (customer == null)
            {
                result.AddError(new Error("No Customer", $"No customer was found with ID: {customerID}"));
                return result;
            }

            //soft delete: change the flag, don't remove the record
            customer.RemoveFromViewFlag = true;
            _context.Customers.Update(customer);

            try
            {
                _context.SaveChanges();
                return result; // no errors = success!
            }
            catch(Exception ex)
            {
                _context.ChangeTracker.Clear();
                result.AddError(ResultHelpers.SaveError(ex));
                return result;
            }
        }


        // Private Methods
        private void ValidateCustomer(CustomerEditView editCustomer, Result<CustomerEditView> result)
        {
            // rules: Mandatory Fields were provided (collect all the errors)
            if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
                result.AddError(new Error("Missing Information", "First Name is required."));
            if (string.IsNullOrWhiteSpace(editCustomer.LastName))
                result.AddError(new Error("Missing Information", "Last Name is required."));
            if (string.IsNullOrWhiteSpace(editCustomer.Address1))
                result.AddError(new Error("Missing Information", "Address1 is required."));
            if (string.IsNullOrWhiteSpace(editCustomer.PostalCode))
                result.AddError(new Error("Missing Information", "Postal Code is required."));
            if (string.IsNullOrWhiteSpace(editCustomer.Phone))
                result.AddError(new Error("Missing Information", "Phone is required."));
            if (string.IsNullOrWhiteSpace(editCustomer.Email))
                result.AddError(new Error("Missing Information", "Email is required."));
            if (editCustomer.ProvStateID <= 0)
                result.AddError(new Error("Missing Information", "A Province or State is required."));
            if (editCustomer.CountryID <= 0)
                result.AddError(new Error("Missing Information", "A Country is required."));
            if (editCustomer.StatusID <= 0)
                result.AddError(new Error("Missing Information", "A Customer Status is required."));

            // rule: FirstName, LastName, and Phone must be unique
            bool existingCustomer = _context.Customers
                .Any(x => x.FirstName.ToLower() == editCustomer.FirstName.ToLower()
                    && x.LastName.ToLower() == editCustomer.LastName.ToLower()
                    && x.Phone == editCustomer.Phone
                    && x.CustomerID != editCustomer.CustomerID);

            // Check if there were and add an error if needed
            if(existingCustomer)
            {
                result.AddError(new Error("Existing Customer Data", $"A customer with the name {editCustomer.FirstName} {editCustomer.LastName} and the phone number {editCustomer.Phone} already exists in the database" +
                    $"aned cannot be entered again."));
            }
        }

        private void CopyToCustomer(CustomerEditView from, Customer to)
        {
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Address1 = from.Address1;
            to.Address2 = from.Address2;
            to.Email = from.Email;
            to.Phone = from.Phone;
            to.PostalCode = from.PostalCode;
            to.City = from.City;
            to.ProvStateID = from.ProvStateID;
            to.CountryID = from.CountryID;
            to.StatusID = from.StatusID;
        }
        private Result<CustomerEditView> SaveCustomer(Customer customer, Result<CustomerEditView> result)
        {
            try
            {
                _context.SaveChanges();
                // customer.CustomerID for a NEW Customer will get an ID after it is saved
                // Note: COMMON MISTAKE is to pass the editcustomer or newCustomer to GetCustomer_ByID
                //      This will fail for a new record because ONLY the database record (Customer)
                //      will have the new customerID.
                return GetCustomer_ByID(customer.CustomerID);

            }
            catch(Exception ex)
            {
                // If you don't clear the change tracker the failing Customer record
                //  stay in the tracked changes and will retry every time you call
                //  SaveChanges()
                _context.ChangeTracker.Clear();
                result.AddError(ResultHelpers.SaveError(ex));
                return result;
            }
        }
        #endregion
    }
}
