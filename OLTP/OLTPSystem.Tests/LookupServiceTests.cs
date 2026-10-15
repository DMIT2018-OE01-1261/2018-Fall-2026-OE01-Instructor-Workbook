using Microsoft.EntityFrameworkCore;
using OLTPSystem.BLL;
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

        }
    }
}
