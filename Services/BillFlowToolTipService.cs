using Microsoft.AspNetCore.Components;
using Radzen;

namespace BillFlow.Services
{
    public class BillFlowToolTipService
    {
        private readonly Radzen.TooltipService _toolTipService;
        public BillFlowToolTipService(Radzen.TooltipService toolTipService) 
        {
            _toolTipService = toolTipService;
        }

        public void ShowTooltip(ElementReference elementReference, string toolTipText, TooltipOptions options = null)
        {
            _toolTipService.Open(elementReference, toolTipText, options);
        }
    }
}
