using Bogus;
using FluentAssertions;
using Projeto02.Services.Api.Test.Helper;
using Projeto02.Services.Api.ViewModels;
using System.Net;
using Xunit;

namespace Projeto02.Services.Api.Test
{
    public class LoginTests
    {
        private readonly string _endpoint;
        public LoginTests()
        {
            _endpoint = "/api/login";
        }

        [Fact]
        public async Task Login_Post_Returns_OK()
        {
            var usuario = await new RegisterTests().Register_Post_Returns_OK();

            var model = new LoginViewModel
            {
                Email = usuario.Email,
                Senha = usuario.Senha,
            };

            var client = TestsHelper.CreateClient();
            var result = await client.PostAsync(_endpoint, TestsHelper.CreateContent(model));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Login_Post_Returns_BadRequest()
        {
            var faker = new Faker("pt_BR");

            var model = new LoginViewModel
            {
                Email = faker.Person.Email,
                Senha = faker.Internet.Password(8)
            };

            var client = TestsHelper.CreateClient();
            var result = await client.PostAsync(_endpoint, TestsHelper.CreateContent(model));

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);   
        }
    }
}