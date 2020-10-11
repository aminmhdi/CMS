using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMS.Common.ModelBinder
{
    /// <summary>
    /// Comma Separator Model Binder Extensions
    /// </summary>
    public static class CommaSeparatorModelBinderExtensions
    {
        /// <summary>
        /// Inserts YeKeModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
        /// </summary>
        public static MvcOptions UseCommaSeparatorModelBinder(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ModelBinderProviders.Insert(0, new CommaSeparatorModelBinderProvider());
            return options;
        }
    }

    /// <summary>
    /// Persian Ye Ke Model Binder Provider
    /// </summary>
    public class CommaSeparatorModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Creates an IModelBinder based on ModelBinderProviderContext.
        /// </summary>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsComplexType)
            {
                return null;
            }

            if (context.Metadata.ModelType == typeof(int) || context.Metadata.ModelType == typeof(int?) || context.Metadata.ModelType == typeof(Int64) || context.Metadata.ModelType == typeof(Int64?))
            {
                return new CommaSeparatorModeBinder();
            }

#if !NETSTANDARD1_6
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
#else
            return new SimpleTypeModelBinder(context.Metadata.ModelType);
#endif
        }
    }


    /// <summary>
    /// Persian Ye Ke Model Binder
    /// </summary>
    public class CommaSeparatorModeBinder : IModelBinder
    {

        /// <summary>
        /// Attempts to bind a model.
        /// </summary>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

#if !NETSTANDARD1_6
            var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);
#else
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType);
#endif

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valueAsString = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            if (bindingContext.ModelMetadata.ModelType == typeof(int) || bindingContext.ModelMetadata.ModelType == typeof(int?))
            {
                if (valueAsString.Contains(","))
                    bindingContext.Result = ModelBindingResult.Success(int.Parse(valueAsString.Replace(",", "")));
                else
                    bindingContext.Result = ModelBindingResult.Success(int.Parse(valueAsString));
            }
            else
            {
                if (valueAsString.Contains(","))
                    bindingContext.Result = ModelBindingResult.Success(long.Parse(valueAsString.Replace(",", "")));
                else
                    bindingContext.Result = ModelBindingResult.Success(long.Parse(valueAsString));
            }

            return Task.CompletedTask;
        }
    }
}
