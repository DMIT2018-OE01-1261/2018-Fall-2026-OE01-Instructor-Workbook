using Microsoft.AspNetCore.Components;

namespace OLTPWebApp.Components.Custom
{
    public partial class DMIT2018FeedbackAndErrorDisplay
    {
        //Parameters for Custom Components are used
        //  to pass data from the parent (page) to the component
        #region Parameters
        [Parameter]
        public List<string> ErrorDetails { get; set; } = [];
        [Parameter]
        public string ErrorMessage { get; set; } = string.Empty;
        [Parameter]
        public string FeedbackMessage { get; set; } = string.Empty;
        #endregion

        private bool hasError => !string.IsNullOrEmpty(ErrorMessage) || ErrorDetails.Any();
    }
}
