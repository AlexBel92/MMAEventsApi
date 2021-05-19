using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MMAEvents.Api.Models
{
    public class EventModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder = new EventDTOModelBinder();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(EventDTO) ? binder : null;
        }
    }
}