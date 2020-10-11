using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace CMS.Middlewares
{
    public static class LanguageExtension
    {
        public static IApplicationBuilder UseLanguage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LanguageMiddleware>();
        }
    }

    public class LanguageMiddleware
    {
        private const string LangFaIr = "fa-IR";
        private const string LangAr = "ar";
        private const string LangEnUs = "en-US";

        private readonly List<string> _supportedLanguages = new List<string> { LangFaIr, LangEnUs, LangAr };

        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(context))
                if (!SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(context))
                    SetCurrentThreadCulture(new RequestCulture(new CultureInfo(LangFaIr), new CultureInfo(LangFaIr)));

            await _next(context);
        }

        private static void SetCurrentThreadCulture(RequestCulture requestCulture)
        {
            CultureInfo.CurrentCulture = requestCulture.Culture;
            CultureInfo.CurrentUICulture = requestCulture.UICulture;
        }

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpContext context)
        {
            var acceptLanguage = context.Request.Headers["Accept-Language"].ToList();
            foreach (var lang in acceptLanguage)
            {
                if (!_supportedLanguages.Contains(lang)) continue;
                SetCurrentThreadCulture(new RequestCulture(new CultureInfo(lang), new CultureInfo(lang)));
                return true;
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpContext context)
        {
            var acceptLanguage = context.Request.Headers["Accept-Language"].ToList();
            foreach (var lang in acceptLanguage)
            {
                var globalLang = lang.Substring(0, 2);
                if (!_supportedLanguages.Any(t => t.StartsWith(globalLang))) continue;

                var supportedLanguage = _supportedLanguages.FirstOrDefault(i => i.StartsWith(globalLang));
                if (supportedLanguage == null) return false;
                SetCurrentThreadCulture(new RequestCulture(new CultureInfo(supportedLanguage), new CultureInfo(supportedLanguage)));
                return true;
            }

            return false;
        }
    }
}
