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

        public async Task ShowTooltip(ElementReference elementReference, string toolTipText, TooltipOptions options = null)
        {
            await Task.Yield();

            options = options ?? new TooltipOptions() {Position = TooltipPosition.Top };

            _toolTipService.Open(elementReference, toolTipText, options);
        }
    }
}
