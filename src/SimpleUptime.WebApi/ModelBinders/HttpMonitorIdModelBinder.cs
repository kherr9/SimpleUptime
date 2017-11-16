using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.WebApi.ModelBinders
{
    public class HttpMonitorIdModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = nameof(HttpMonitorId);
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            if (Guid.TryParse(value, out var guid))
            {
                try
                {
                    bindingContext.Result = ModelBindingResult.Success(new HttpMonitorId(guid));
                }
                catch (ArgumentException ex)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
                }
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Http Monitor Id must be a guid");
            }

            return Task.CompletedTask;
        }
    }
}
