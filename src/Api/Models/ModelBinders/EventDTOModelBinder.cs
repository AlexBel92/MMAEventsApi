using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace MMAEvents.Api.Models
{
    public class EventDTOModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var name = bindingContext.ValueProvider.GetValue("EventName");

            bindingContext.Result = ModelBindingResult.Success(
                new EventDTO { Name = name.FirstValue }
                );

            return Task.CompletedTask;
        }
    }
}