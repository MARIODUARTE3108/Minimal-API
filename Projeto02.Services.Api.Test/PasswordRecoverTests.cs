using Bogus;
using FluentAssertions;
using Projeto02.Services.Api.Test.Helper;
using Projeto02.Services.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Projeto02.Services.Api.Test
{
    public class PasswordRecoverTests
    {
        private readonly string _endpoint;
        public PasswordRecoverTests()
        {
            _endpoint = "/api/password-recover";
        }
        [Fact]
        public async Task PasswordRecover_Post_Returns_OK()
        {
            var usuario = await new RegisterTests().Register_Post_Returns_OK();

            var model = new PasswordRecoverViewModel
            {
                Email = usuario.Email,
            };

            var client = TestsHelper.CreateClient();
            var result = await client.PostAsync(_endpoint, TestsHelper.CreateContent(model));

            result.StatusCode.Should().Be(HttpStatusCode.OK);

        }
        [Fact]
        public async Task PasswordRecover_Post_Returns_BadRequest()
        {
            var faker = new Faker("pt_BR");

            var model = new PasswordRecoverViewModel
            {
                Email = faker.Person.Email.ToLower(),
            };

            var client = TestsHelper.CreateClient();
            var result = await client.PostAsync(_endpoint, TestsHelper.CreateContent(model));

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
    }
}
