using Microsoft.AspNetCore.Mvc.Filters;

namespace XmlTvGrabberWebGui.Helpers.ModelStateSerialization
{
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected const string Key = nameof(ModelStateTransfer);
    }
}
