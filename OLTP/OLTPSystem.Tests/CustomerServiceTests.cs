using BYSResults;
using OLTPSystem.BLL;
using OLTPSystem.Helpers;
using OLTPSystem.ViewModels;

namespace OLTPSystem.Tests
{
    // Change to public and inherit (:) the TestBase
    public class CustomerServiceTests : TestBase
    {
        private readonly CustomerService _service;

        //Runs before every test: creates the service with the test's context
        public CustomerServiceTests(ITestOutputHelper output) : base(output)
        {
            _service = new CustomerService(Context);
        }

        #region GetCustomers Tests

        // Error Tests - Negatives
        // rule: either last name or phone must be provided
        [Theory]
        [InlineData("","")]
        [InlineData(null, null)]
        [InlineData("     ", "      ")]
        public void GetCustomers_BlankNameAndPhone_ReturnsMissingInfoError(string? lastName, string? phone)
        {
            // Arrage - Not needed)

            // Act - what we are doing
            var result = _service.GetCustomers(lastName!, phone!);

            // Asset - Check what happened
            // We expect it to fail
            Assert.True(result.IsFailure);
            // We expect this error message
            Assert.Contains("Missing Information: Either a partial phone number or a last name must be provided",
                ResultHelpers.GetErrorMessages(result.Errors));
        }

        [Fact]
        public void GetCustomers_NoMatches_ReturnsNoCustomersError()
        {
            //Arrange - No need for this
            //Act
            var result = _service.GetCustomers("zzzzzzz", "9999999");

            //Assert
            Assert.True(result.IsFailure);
            Assert.Contains("No customers were found",
                ResultHelpers.GetErrorMessages(result.Errors));
        }

        // Value Tests - Success
        [Fact]
        public void GetCustomers_ByLastName_ReturnsMatchingCustomers()
        {
            var results = _service.GetCustomers("Fos", "");

            Assert.True(results.IsSuccess);
            Assert.NotNull(results.Value);
            Assert.Equal(14, results.Value.Count);
            Dump(results.Value, "Customers with Last Name containing 'Fos'");
        }

        [Fact]
        public void GetCustomer_ByExactPhone_ReturnsOneCustomer()
        {
            var result = _service.GetCustomers("", "7804326565");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
            Dump(result.Value, "Customer with phone 7804326565");
        }
        #endregion

        #region GetCustomer_ByID Tests
        // rule: CustomerID must be provided and valid
        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void GetCustomer_ByID_InvalidID_ReturnsMissingInfoError(int customerID)
        {
            var result = _service.GetCustomer_ByID(customerID);

            Assert.True(result.IsFailure);
            Assert.Contains("Missing Information: A valid CustomerID must be provided",
                ResultHelpers.GetErrorMessages(result.Errors));
        }

        [Fact]
        public void GetCustomer_ByID_BadID_ReturnsNoCustomerError()
        {
            int customerID = 999999999;
            var result = _service.GetCustomer_ByID(customerID);

            Assert.True(result.IsFailure);
            Assert.Contains($"No Customer: No customer was found with ID: {customerID}",
                ResultHelpers.GetErrorMessages(result.Errors));
        }

        [Fact]
        public void GetCustomer_ByID_ValidID_ReturnsCustomer()
        {
            var result = _service.GetCustomer_ByID(6);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Fred", result.Value.FirstName);
            Assert.Equal("Foster", result.Value.LastName);

            Dump(result.Value, "Customer 6");
        }
        #endregion
    }
}
