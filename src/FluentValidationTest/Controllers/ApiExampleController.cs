using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentValidationTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FluentValidationTest.Controllers
{
    [ApiController]
    public class ApiExampleController : ControllerBase
    {
        private readonly ILogger<ApiExampleController> _logger;

        public ApiExampleController(ILogger<ApiExampleController> logger)
        {
            _logger = logger;
        }

        // ReSharper disable once StringLiteralTypo
        [HttpPost("api/dostuff")]
        // ReSharper disable once UnusedParameter.Global
        public IActionResult DoStuff([Required, FromBody]DoStuffModel doStuffModel)
        {
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"Entered {GetType().Name} method {MethodBase.GetCurrentMethod().Name}");
            return Ok();
        }
    }
}