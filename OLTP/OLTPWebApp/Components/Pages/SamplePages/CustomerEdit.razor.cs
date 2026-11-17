using Microsoft.AspNetCore.Components;

namespace OLTPWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Properties
        //Must match the name in the @page directive (capitalization matters!)
        [Parameter]
        public int CustomerID { get; set; }
        #endregion
    }
}
