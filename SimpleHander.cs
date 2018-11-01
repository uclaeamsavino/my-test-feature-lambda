using Amazon.Lambda.Core;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

/**
* Every Lambda should live in its own namespace - to prevent accidental dependencies between lambdas
**/
namespace UCLA.EA.Lambda.SimpleStarter
{
    /**
    * This is the basic handler boilerplate that serverless generates, with a couple changes:
    * 1. The Handler Hello has been changed to async to demonstrate using async/await 
    * 2. The simple input object has been changed into a nested object to demonstrate how Lambda Core deserializes the input\n
    * \n
    * See CSharpStarterHandler.cs for more detailed boilerplate
    */
    public class SimpleHandler
    {
        /** 
        * * Handler can be invoked by API Gateway, a step function, or another lambda
        * * Handler function is specified in serverless.yml
        * * Using async/await example
        */
        public async Task<Response> Hello(Request request, ILambdaContext context)
        {
            logObject(request);
            Response response = await Task.Run(() => {
                LambdaLogger.Log("Test1");
                return new Response("Go Serverless v1.0! Your function executed successfully!", request);
            });
            return response;
        }

        /**
        * Simple log function, will be moved to a library, also logging will be standardized/automated in the near future
        **/
        public static void logObject(object input) {
            LambdaLogger.Log("\nObject type: " + input.GetType().Name + "\n" 
                   + JsonConvert.SerializeObject(input, Formatting.Indented) + "\n");
        }
    }

    /**
    * Lambda Core serializes this into a JSON object that's returned to the invoking entity 
    **/
    public class Response
    {
        public string Message {get; set;}
        public Request Request {get; set;}

        public Response(string message, Request request){
            Message = message;
            Request = request;
        }
    }

    /**
    * Lambda Core deserializes an input JSON object into this Request object
    **/
    public class Request
    {
        public string Email {get; set;}
        public PhoneNumber PhoneNumber {get; set;}

        public Request(string email, PhoneNumber phoneNumber){
            LambdaLogger.Log("Full phone number = +" + phoneNumber.toString() + "\n");
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }

    /**
    * Example of how to parse a nested object input
    **/
    public class PhoneNumber
    {
        public string CountryCode { get; set; }
        public string Number { get; set; }

        /**
        * Example of how to parse an object input
        * the JSON object is deserialized by Lambda Core into the constructor params
        **/
         public PhoneNumber(string countryCode, string phoneNumber) {
            CountryCode = countryCode;
            Number = phoneNumber;
        }

        /**
        * For logging
        **/
        public string toString() 
        {
            return this.CountryCode + " " + this.Number;
        }
    }
}
