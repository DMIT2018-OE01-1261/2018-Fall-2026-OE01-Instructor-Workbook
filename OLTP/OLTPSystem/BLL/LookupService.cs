using BYSResults;
using OLTPSystem.DAL;
using OLTPSystem.Entities;
using OLTPSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OLTPSystem.BLL
{
    public class LookupService
    {
        #region Data Context Setup
        private readonly OLTPContext _context;

        // The context is passed in (injected). Right now the tests will pass it in for you.
        //  one in Blazor this happens almost automatically
        public LookupService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        public Result<List<LookupView>> GetLookupValues(int categoryID)
        {
            var result = new Result<List<LookupView>>();
            // rule: CategoryID must have a value
            if(categoryID <= 0)
            {
                result.AddError(new Error("Missing Information","CategoryID must be provided."));
                return result;
            }    

            return result;
        }

        // Created an overloaded method so the user can search by ID or Name
        //  Overloaded method is the same name and return type, but have different parameters
        public Result<List<LookupView>> GetLookupValues(string categoryName)
        {
            var result = new Result<List<LookupView>>();
            if(string.IsNullOrWhiteSpace(categoryName))

            {
                result.AddError(new Error("Missing Information","Category name must be provided."));
                return result;
            }
            var values = _context.Lookups
                .Where(x => x.Category.CategoryName.ToLower() == categoryName.ToLower()
                    && !x.RemoveFromViewFlag)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupID,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();

            if (!values.Any())
            {
                result.AddError(new Error("No Lookups", $"No lookup values found for the Category Name {categoryName}"));
                return result;
            }    

            return result.WithValue(values);
        }
    }
}
