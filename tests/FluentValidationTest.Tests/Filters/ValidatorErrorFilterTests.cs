using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentAssertions;
using FluentValidationTest.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Xunit;

namespace FluentValidationTest.Tests.Filters
{
    [SuppressMessage("ReSharper", "CA1806")]
    public class ValidatorErrorFilterTests
    {

        [Fact]
        public void Order_Should_Be_Set_To_Minus_1500()
        {
            // Arrange
            // Act
            // Assert
            GetFilter()
                .Order
                .Should()
                .Be(-1500);
        }

        [Fact]
        public void OnResultExecuting_Should_Do_Nothing_When_OkObjectResult()
        {
            // Arrange
            var actionResult = new OkObjectResult(new { MyValue = "Thing" });
            var context = GetExecutingContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuting_Should_Do_Nothing_When_OkResult()
        {
            // Arrange
            var actionResult = new OkResult();
            var context = GetExecutingContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuting_Should_Do_Nothing_When_NotFoundResult()
        {
            // Arrange
            var actionResult = new NotFoundResult();
            var context = GetExecutingContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuting_Should_Generate_ValidationProblemDetails()
        {
            // Arrange
            const string path = "/api/path";
            const string queryString = "?q1=a&q2=b";
            var pathAndQuery = $"{path}{queryString}";

            var context = GetExecutingContext(new OkResult(), path, queryString);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            var badRequestResult = context
                .Result
                .Should()
                .BeOfType<BadRequestObjectResult>();

            var validationProblemDetailsType = badRequestResult
                .Subject
                .Value
                .Should()
                .BeOfType<ValidationProblemDetails>();

            var validationProblemDetails = validationProblemDetailsType.Subject;

            validationProblemDetails
                .Errors
                .Should()
                .BeEquivalentTo(new Dictionary<string, string[]> { { "Username", new[] { "Invalid Username" } } });

            validationProblemDetails
                .Instance
                .Should()
                .Be(pathAndQuery);

            validationProblemDetails
                .Title
                .Should()
                .Be("One or more validation errors occurred.");

            validationProblemDetails
                .Type
                .Should()
                .Be("https://tools.ietf.org/html/rfc7231#section-6.5.1");

            validationProblemDetails
                .Extensions
                .Should()
                .ContainKey("traceId")
                .And
                .NotBeEmpty();
        }

        [Fact]
        public void Should_Throw_ArgumentNullException_When_ApiBehaviorOptions_Is_Null()
        {
            // Act
            Action constructor = () => new ValidatorErrorFilter(default);

            // Assert
            constructor
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'apiBehaviourOptions')");
        }

        [Fact]
        public void Should_NotThrow_Exception_When_ApiBehaviorOptions_Is_Not_Null()
        {
            // Act
            Action constructor = () => new ValidatorErrorFilter(GetOptions());

            // Assert
            constructor
                .Should()
                .NotThrow();
        }

        [Fact]
        public void OnResultExecuted_Should_Do_Nothing_When_OkObjectResult()
        {
            // Arrange
            var actionResult = new OkObjectResult(new { MyValue = "Thing" });
            var context = GetExecutedContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuted(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuted_Should_Do_Nothing_When_OkResult()
        {
            // Arrange
            var actionResult = new OkResult();
            var context = GetExecutedContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuted(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuted_Should_Do_Nothing_When_NotFoundResult()
        {
            // Arrange
            var actionResult = new NotFoundResult();
            var context = GetExecutedContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuted(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }

        [Fact]
        public void OnResultExecuted_Should_Do_Nothing_When_BadRequestResult()
        {
            // Arrange
            var actionResult = new BadRequestResult();
            var context = GetExecutedContext(actionResult, includeModelStateError: false);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuted(context);

            // Assert
            context
                .Result
                .Should()
                .BeEquivalentTo(actionResult);
        }


        private static ValidatorErrorFilter GetFilter(IOptions<ApiBehaviorOptions> options = null)
        {
            var apiBehaviorOptions = options ?? GetOptions();
            return new ValidatorErrorFilter(apiBehaviorOptions);
        }

        private static IOptions<ApiBehaviorOptions> GetOptions()
        {
            var apiBehaviorOptions = new ApiBehaviorOptions();
            apiBehaviorOptions.ClientErrorMapping.Add((int)HttpStatusCode.BadRequest, new ClientErrorData
            {
                Title = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest),
                Link = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

            return Options.Create(apiBehaviorOptions);
        }

        private static ResultExecutingContext GetExecutingContext(IActionResult actionResult,
            string path = "/api/path",
            string queryString = null,
            bool includeModelStateError = true)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            httpContext.Request.QueryString = new QueryString(queryString);

            var context = new ResultExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                Array.Empty<IFilterMetadata>(),
                actionResult,
                new object());

            if (includeModelStateError)
            {
                context.ModelState.AddModelError("Username", "Invalid Username");
            }

            return context;
        }

        private static ResultExecutedContext GetExecutedContext(IActionResult actionResult, string path = "/api/path",
            string queryString = null,
            bool includeModelStateError = true)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            httpContext.Request.QueryString = new QueryString(queryString);

            var context = new ResultExecutedContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                Array.Empty<IFilterMetadata>(),
                actionResult,
                new object());

            if (includeModelStateError)
            {
                context.ModelState.AddModelError("Username", "Invalid Username");
            }

            return context;
        }
    }
}