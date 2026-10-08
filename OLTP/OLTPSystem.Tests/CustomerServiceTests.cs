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

        #endregion
    }
}
