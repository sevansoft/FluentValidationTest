using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace FluentValidationTest.Filters
{
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    public class ValidatorErrorFilter : IAlwaysRunResultFilter, IOrderedFilter
    {
        private readonly ApiBehaviorOptions _apiBehaviourOptions;

        public ValidatorErrorFilter(IOptions<ApiBehaviorOptions> apiBehaviourOptions)
        {
            ArgumentNullException.ThrowIfNull(_apiBehaviourOptions = apiBehaviourOptions?.Value, nameof(apiBehaviourOptions));
        }

        /// <summary>
        /// Gets the filter order. Defaults to -1500 so that it runs early
        /// </summary>
        public int Order => -1500;

        /// <summary>
        ///  Called before the action result executes.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext.</param>
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationValidationProblemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Instance = string.Concat(context.HttpContext.Request.Path,
                    context.HttpContext.Request.QueryString.HasValue ?
                    context.HttpContext.Request.QueryString.Value :
                    string.Empty)
            };

            validationValidationProblemDetails
                .Extensions[Constants.ValidationProblemDetailsElements.TraceId] = Activity.Current?.Id ?? context
                                                                            .HttpContext
                                                                            .TraceIdentifier;
            validationValidationProblemDetails.Type = _apiBehaviourOptions
                                                                        .ClientErrorMapping[StatusCodes.Status400BadRequest]
                                                                        .Link;

            context.Result = new BadRequestObjectResult(validationValidationProblemDetails)
            {
                ContentTypes =
                {
                    Constants.Produces.ApplicationProblemJson
                }
            };
        }

        /// <summary>
        /// Called after the action result executes.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext.</param>
        public void OnResultExecuted(ResultExecutedContext context) { }

        private static class Constants
        {
            public static class ValidationProblemDetailsElements
            {
                public static readonly string TraceId = "traceId";
            }

            public static class Produces
            {
                public const string ApplicationProblemJson = "application/problem+json";
            }
        }
    }
}