// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace FluentValidationTest.Models
{
    public record DoStuffModel
    {
        public string Username { get; init; }
        public string Password { get; init; }
    }
}