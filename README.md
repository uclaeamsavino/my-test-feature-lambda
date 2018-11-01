# C# Starter Lambda 

Use this code as boilerplate to develop new lambdas. 

This README file for a C# lambda should contain the basic logic, and input and output of your lambda handler(s). Also include any AWS resources accessed and any external libraries used. 

**Example README Format:**

## Business Logic
This lambda takes an email and/or phone number and returns success if one CRM user is found and failure if 0 or multiple users are found.

### Input
1. Email address and/or mobile phone number (at least one is required, both can be supplied)
2. This will be supplied to the lambda as input.lookupParams  (based on this)
3. Example: input.lookupParams = {email:'x@y.com', mobile_phone: {country_code: '1', number:'3105557890'}} }

### Lookup logic - all criteria is used:
1. If a single match is found, return success object with user profile, CRM IDs and types (= SourceID and SourceType?), and single user found msgkey key.
    * If email and phone number are supplied, then single match is based on both.
    * If only one is supplied, then single match is based on that.
2. If nothing is found return an error object with no users msgkey key
3. If more than one entry is found, return an error object with multiple users msgkey key

### Responses:
***Single match:*** 
```
{ 
  success: { 
    result: crmUserProfile, 
    msgkey: 'SINGLE_USER_FOUND' 
  }
}
```
**No match:** 
```
{ 
  error: {
    msgkey: 'NO_USERS_FOUND' 
  } 
}
```
**More than one match:**
```
{ 
  error: {
    msgkey: 'MULTIPLE_USERS_FOUND' 
  } 
}
```

## Libraries Used
* UCLA.EA.Blackbaud
* UCLA.EA.TestingLib

