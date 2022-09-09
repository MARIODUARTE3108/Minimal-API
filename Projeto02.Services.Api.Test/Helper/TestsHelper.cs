using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto02.Services.Api.Test.Helper
{
    public class TestsHelper
    {
        public static HttpClient CreateClient()
        {
            var applicarion = new WebApplicationFactory<Program>();
            return applicarion.CreateClient();
        }

        public static StringContent CreateContent<TRequest>(TRequest request)
        {
            return new StringContent(JsonConvert.SerializeObject(request),
                Encoding.UTF8,"application/json");
        }
        public static TResponse CreateResponse<TResponse>(HttpResponseMessage message)
        {
            return JsonConvert.DeserializeObject<TResponse>(message.Content.ReadAsStringAsync().Result);
        }
    }
}
