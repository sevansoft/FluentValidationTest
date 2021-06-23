using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace FluentValidationTest.Filters
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ValidatorActionFilter : IAlwaysRunResultFilter, IOrderedFilter
    {
        private readonly ApiBehaviorOptions _apiBehaviourOptions;

        public ValidatorActionFilter(IOptions<ApiBehaviorOptions> apiBehaviourOptions)
        {
            _apiBehaviourOptions = apiBehaviourOptions?.Value;
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

            var validationProblemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Instance = string.Concat(context.HttpContext.Request.Path,
                    context.HttpContext.Request.QueryString.HasValue ?
                    context.HttpContext.Request.QueryString.Value :
                    string.Empty)
            };

            if (!validationProblemDetails
                .Extensions
                .ContainsKey(Constants.ProblemDetailsElements.TraceId))
            {
                validationProblemDetails
                    .Extensions[Constants.ProblemDetailsElements.TraceId] = context
                                                                                .HttpContext
                                                                                .TraceIdentifier;
            }

            if (!validationProblemDetails
                .Extensions
                .ContainsKey(Constants.ProblemDetailsElements.Type))
            {
                validationProblemDetails
                    .Extensions[Constants.ProblemDetailsElements.Type] = _apiBehaviourOptions
                                                                            .ClientErrorMapping[StatusCodes.Status400BadRequest].Link;
            }
            
            context.Result = new BadRequestObjectResult(validationProblemDetails)
            {
                ContentTypes =
                {
                    Constants.Produces.ApplicationJson,
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
            [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
            public static class ProblemDetailsElements
            {
                public static readonly string TraceId = "traceId";
                public static readonly string Type = "type";
            }

            public static class Produces
            {
                public const string ApplicationJson = MediaTypeNames.Application.Json;
                public const string ApplicationProblemJson = "application/problem+json";
            }
        }
    }
}