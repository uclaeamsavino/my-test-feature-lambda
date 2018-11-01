using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

using UCLA.EA.Blackbaud;
using UCLA.EA.TestingLib;

/**
* Every Lambda should live in its own namespace - to prevent accidental dependencies between lambdas
**/
namespace UCLA.EA.Lambda.CsharpStarter
{
    /**
    * This boilerplate test class demonstrates:\n
    * * Extending the base handler and adding overriding a method to mock Blackbaud DataList class\n
    * * Implementing ILambdaTest interface which calls mockLoadAsync method to return specifc mock results for this class only\n
    * * Using Xunit fact/theory tests (https://xunit.github.io/docs/getting-started-dotnet-core)\n
    * * Mock responses based on certain inputs, then assert results\n
    * * Using the generic components in the TestingLib\n
    * \n
    **/
    public class CsharpStarterHandlerTest : CsharpStarterHandler, ILambdaTest
    {
        /**
        * Xunit facts are always true
        **/
        [Fact]
        public async void SingleUserSuccessTest()
        {
            Response user = await (new CsharpStarterHandlerTest()).LookupCrmUser(new Request{email="x@y.com"}, null);

            Assert.NotNull(user.Success);
            Assert.Equal("SINGLE_USER_FOUND", user.Success.msgkey);
            Assert.NotNull(user.Success.payload);
            Assert.Null(user.Error);
        }

        /**
        * Xunit theories can have different results for different inputs
        *! Notice in these cases case the calling function is finding a normal response object from the handler,
        *        not catching an exception - which is reserved for more serious errors like below
        **/
        [Theory]
        [InlineData("testMany@gmail.com", "MULTIPLE_USERS_FOUND")]
        [InlineData("test0@gmail.com", "NO_USERS_FOUND")]
        public async void ErrorTest(string email, string expectedMsgkey)
        {
            Response user = await (new CsharpStarterHandlerTest()).LookupCrmUser(new Request{email=email}, null);

            Assert.Null(user.Success);
            Assert.NotNull(user.Error);
            Assert.Equal(user.Error.msgkey, expectedMsgkey);
        }

        /**
        * System error test is important because it usually won't show up in smoke testing
        * Notice in this case the calling function is catching an error thrown by the lambda handler, 
        *        instead of finding a normal response object
        **/
        [Fact]
        public async void SystemErrorTest()
        {
            try 
            {
                Response user = await (new CsharpStarterHandlerTest()).LookupCrmUser(new Request{email="SYSTEM_ERROR@gmail.com"}, null);
            }
            catch (Exception e) 
            {
                Assert.Contains("SOMETHING VERY BAD HAPPENED", e.ToString());
            }
        }

        /**
        * Overridden method from parent Handler
        * This may become part of a service class that can be overridden to return mock services with env vars
        **/
        public override IDataList<EmailInput, LookupOutput> getDataList(AppFxClient client, Guid callId) 
        {
           var mockDataList = new MockDataList<EmailInput, LookupOutput>(client, callId);
           mockDataList.HandlerTest = this;
           return mockDataList;
        }

        /**
        * Providing specific mock results based on inputs
        * Is invoked by MockDataList in the TestingLib
        **/
        public IEnumerable<object> mockLoadAsync(object inputs)
        {
            EmailInput input = inputs as EmailInput;

            /**
            * Lambdas should reserve throwing errors for serious, unexpected errors
            **/
            if (input.email.StartsWith("SYSTEM_ERROR"))
            {
                throw new System.InvalidOperationException("SOMETHING VERY BAD HAPPENED");
            }

            var results = new List<LookupOutput>();
            
            /**
            * Expected errors like User Not Found, should be handled with standard response object
            **/
            if (!input.email.StartsWith("test0"))
            {
                results.Add(new LookupOutput{LOOKUPID = "SomeLookupId", UID = "SomeUID"});
            }
            
            if (input.email.StartsWith("testMany"))
            {
                results.Add(new LookupOutput{LOOKUPID = "SomeOtherLookupId", UID = "SomeOtherUID"});
            }
            
            IEnumerable<object> enumResults = results;                

            return enumResults;       
        }
   }
}
