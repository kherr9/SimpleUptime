using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.WebApi.ModelBinders
{
    public class HttpMonitorIdBinder : IModelBinder, IModelBinderProvider
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            // Specify a default argument name if none is set by ModelBinderAttribute
            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = nameof(HttpMonitorId);
            }

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
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

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(HttpMonitorId))
            {
                return new BinderTypeModelBinder(typeof(HttpMonitorIdBinder));
            }

            return null;
        }
    }
}
