using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidationTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FluentValidationTest.Controllers
{
    [ApiController]
    public class MvcApiAttributeExampleController : Controller
    {
        private readonly ILogger<MvcApiAttributeExampleController> _logger;

        public MvcApiAttributeExampleController(ILogger<MvcApiAttributeExampleController> logger)
        {
            _logger = logger;
        }

        [HttpPost("mvcapiattribute/dostuff")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        // ReSharper disable once UnusedParameter.Global
        public IActionResult DoStuff([Required, FromBody] DoStuffModel doStuffModel)
        {
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"Entered {GetType().Name} method {MethodBase.GetCurrentMethod().Name}");
            return Ok();
        }
    }
}