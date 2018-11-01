
using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;
using Xunit;

using UCLA.EA.TestingLib;

/**
* Every Lambda should live in its own namespace - to prevent accidental dependencies between lambdas
**/
namespace UCLA.EA.Lambda.SimpleStarter
{
    /**
    * This boilerplate test class demonstrates:
    * * Using Xunit fact/theory tests (https://xunit.github.io/docs/getting-started-dotnet-core)
    * * See CsharpStarterHandlerTest.cs for more testing examples
    **/
    public class SimpleHandlerTest : SimpleHandler
    {
        /// xunit tests
        [Fact]
        public async void DemonstrationTest()
        {
            Response response = await (new SimpleHandler()).Hello(new Request("x@y.com", new PhoneNumber("1", "3105556677")), null);

            Assert.NotNull(response);
        }
  }
}
