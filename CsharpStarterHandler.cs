using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using UCLA.EA.Blackbaud;

/// NOTE: - only using this so the handler will return mock data, normally a live Handler wouldn't reference the testing 
using UCLA.EA.TestingLib;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

/**
* Every Lambda should live in its own namespace - to prevent accidental dependencies between lambdas
**/
namespace UCLA.EA.Lambda.CsharpStarter
{
    /** This boilerplate code demonstrates:\n
    * * Initializing a client in the class constructor\n
    * * Using async/await\n
    * * Lambda handler method signature: LookupCrmUser(Request Request, ILambdaContext context)\n
    * * Using built-in lambda JSON deserializers\n
    * * Accessing Blackbaud library to make a back end call\n
    * * Creating a virtual overridable method to retrieve the Blackbaud DataList class (so the test class can mock the Request to Blackbaud)\n
    * * Wrapping Request and response in custom csharp objects\n
    * * Logging; WORK IN PROGRESS\n
    * \n
    * See SimpleHandler.cs for more basic examples\n
    */
    public class CsharpStarterHandler
    {
        static AppFxClient BBClient;

        public CsharpStarterHandler()
        {
            BBClient = new AppFxClient(UCLA.EA.Blackbaud.Environment.Get("TEST4"), "UCLA API");
        }
        
        /**
         * Handler can be invoked by API Gateway, a step function, or another lambda
         * Handler function is specified in serverless.yml
        **/
        public async Task<Response> LookupCrmUser(Request Request, ILambdaContext context)
        {
            List<LookupOutput> testResultsList = await findUser(Request);
            // logObject(testResultsList);
            
            if (testResultsList.Count() == 1) {
                return new Response(testResultsList[0], "SINGLE_USER_FOUND", false);
            }
            else if (testResultsList.Count() > 1) {
                return new Response(null, "MULTIPLE_USERS_FOUND", true);
            }
            else {
                return new Response(null, "NO_USERS_FOUND", true);
            }
        }

        public async Task<List<LookupOutput>> findUser(Request input) {

            IDataList<EmailInput, LookupOutput> dataList = getDataList(BBClient, new Guid("2f559fc5-9e76-4cfe-8697-4e57cc2f7c9d"));
            IEnumerable<LookupOutput> results = await dataList.LoadAsync(new EmailInput{email = input.email}, null);
            List<LookupOutput> testResultsList = results.ToList();

            return testResultsList;       
        }

        /**
        * Creating a virtual function so the HandlerTest class can override it to return mock results
        **/
        public virtual IDataList<EmailInput, LookupOutput> getDataList(AppFxClient client, Guid callId) 
        {
            // return new DataList<EmailInput, LookupOutput>(client, callId);
            
            // TODO - using mock for now until the real DataList is ready   
            var mockDataList = new MockDataList<EmailInput, LookupOutput>(client, callId);
            mockDataList.HandlerTest = new CsharpStarterHandlerTest();
            return mockDataList;
        }

        /**
        * Simple log function, will be moved to a library, also logging will be standardized/automated in the near future
        **/
        public static void logObject(object input) {
            LambdaLogger.Log("\nObject type: " + input.GetType().Name + "\n" 
                   + JsonConvert.SerializeObject(input, Formatting.Indented) + "\n");
        }
    }
    
    public class EmailInput
    {
        public string email { get; set; }
    }

    public class LookupOutput
    {
        public string LOOKUPID { get; set; }
        public string UID { get; set; }
    }

    public class Response
    {
        public ErrorResponse Error {get; set;}
        public SuccessResponse Success {get; set;}

        public Response(object payload, string msgkey, bool isError){
            if (isError) 
            {
                Error = new ErrorResponse{msgkey=msgkey};
            }
            else 
            {
                Success = new SuccessResponse{payload=payload, msgkey=msgkey};
            }
        }
    }

    public class SuccessResponse
    {
        public object payload {get; set;}
        public string msgkey {get; set;}
    }

    public class ErrorResponse
    {
        public string msgkey {get; set;}
    }

    public class Request
    {
        public string email {get; set;}
        public PhoneNumber phoneNumber {get; set;}
    }

    public class PhoneNumber
    {
        public string countryCode { get; set; }
        public string number { get; set; }

        public string toString() 
        {
            return this.countryCode + " " + this.number;
        }
    }
}
