using System;
using System.Collections.Generic;
using System.Text;
using BYSResults;
using OLTPSystem.DAL;
using OLTPSystem.ViewModels;

namespace OLTPSystem.BLL
{
    public class PartService
    {
        #region Data Context Setup
        private readonly OLTPContext _context;

        // The context is passed in (injected). Right now the tests will pass it in for you.
        //  one in Blazor this happens almost automatically
        public PartService(OLTPContext context)
        {
            _context = context
                        ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion

        //Every active part, sorted by category then description
        public Result<List<InvoicePartView>> GetParts()
        {
            var result = new Result<List<InvoicePartView>>();

            var parts = _context.Parts
                .Where(x => !x.RemoveFromViewFlag)
                .Select(x => new InvoicePartView
                {
                    PartID = x.PartID,
                    CategoryName = x.PartCategory.Name,
                    Description = x.Description,
                    PartCategoryID = x.PartCategoryID,
                    Price = x.Price,
                    QOH = x.QOH,
                    Taxable = x.Taxable
                })
                .OrderBy(x => x.CategoryName)
                .ThenBy(x => x.Description)
                .ToList();

            // rule: there must be parts to sell
            if(parts.Count == 0)
            {
                result.AddError(new Error("No Parts", "No parts were found."));
                return result;
            }

            return result.WithValue(parts);
        }
    }
}
