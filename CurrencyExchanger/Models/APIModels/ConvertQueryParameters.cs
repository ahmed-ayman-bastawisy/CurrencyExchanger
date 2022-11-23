using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchanger.Models.APIModels
{
    public class ConvertQueryParameters
    {
        [BindRequired] public long clientId { get; set; }
        [BindRequired] public string amount { get; set; }
        [BindRequired] public string from { get; set; }
        [BindRequired] public string to { get; set; }
    }
}
