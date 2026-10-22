using Microsoft.EntityFrameworkCore;
using OLTPSystem.BLL;
using OLTPSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.Tests
{
    public class LookupServiceTests : TestBase
    {
        private readonly LookupService _service;

        //Runs before every test: creates the service with the test's context
        public LookupServiceTests(ITestOutputHelper output) : base(output)
        {
            _service = new LookupService(Context);
        }

        [Fact]
        public void GetLookupValues_InvalidCategoryID_ReturnsMissingInfoError()
        {
            var result = _service.GetLookupValues(0);

            Assert.True(result.IsFailure);
            Assert.Contains("Missing Information: CategoryID must be provided."
                , ResultHelpers.GetErrorMessages(result.Errors));
        }

        [Fact]
        public void GetLookupValues_InvalidCategoryName_ReturnsMissingInfoError()
        {
            var result = _service.GetLookupValues(string.Empty);

            Assert.True(result.IsFailure);
            Assert.Contains("Missing Information: Category name must be provided."
                , ResultHelpers.GetErrorMessages(result.Errors));
        }

        // Testing the returned values, check if they are correct
        [Fact]
        public void GetLookupValues_ByName_ReturnsSortedValues()
        {
            var result = _service.GetLookupValues("Country");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count);
            Assert.Equal("Canada", result.Value[0].Name);
            Dump(result.Value, "Countries");
        }

        [Fact]
        public void GetLookupValues_ByName_ReturnsNoLookupError()
        {
            var value = "Bob";
            var result = _service.GetLookupValues(value);

            Assert.True(result.IsFailure);
            Assert.Contains($"No Lookups: No lookup values found for the Category Name {value}",
                ResultHelpers.GetErrorMessages(result.Errors));
        }
    }
}
