using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentValidationTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FluentValidationTest.Controllers
{
    public class MvcExampleController : Controller
    {
        private readonly ILogger<MvcExampleController> _logger;

        public MvcExampleController(ILogger<MvcExampleController> logger)
        {
            _logger = logger;
        }

        // ReSharper disable once StringLiteralTypo
        [HttpPost("mvc/dostuff")]
        // ReSharper disable once UnusedParameter.Global
        public IActionResult DoStuff([Required, FromBody] DoStuffModel doStuffModel)
        {
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            _logger.LogInformation($"Entered {GetType().Name} method {MethodBase.GetCurrentMethod().Name}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}