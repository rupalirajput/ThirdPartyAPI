# Third Party API Server

This project implements an application that can be used as an intermediary for a third-party service. 
The third party service accepts a document request payload which includes a callback url and will respond to that url when the document is ready, which may be up to 10 business days later.

## REST Endpoints

The application has four endpoints which are implemented as a stateful service connected to a store. The state is be stored in-memory with a note on how to store it at rest. Each API responds 
to an http request and follows the REST conventions for methods. The endpoints meet the following requirements:

### 1. POST a request

The request handler accepts the following structure:

POST /request
BODY: Object {
  "body": String
}
RETURNS String
          
This accepts a JSON body consisting of a one key, "body", which is a string. Doing this will initiate a request to the third-party service. It will also create a unique identifer for this request we can later reference. This unique identifier string is returned by this API call.

The request to the third party service is a stubbed call to "http://example.com/request"
with the following payload:

BODY Object: {
  "body": {body},
  "callback": "/callback/{id}"
}

Comments or other indicators are present on how one could actually call this service, as well as any additional concerns around error handling/logging.

### 2. POST callback

The request handler accepts the following structure:

POST /callback/{id}
BODY String
RETURNS 204

This URL is sent in the original /request handler. The third-party-service service is expected to send an initial POST with the text string `STARTED` on this API to indicate that they have received the request.

### 3. PUT callback

The request handler accepts the following structure:

PUT /callback/{id}
BODY Object {
  "status": String,
  "detail": String
}
RETURNS 204
                                                                                                          
The third-party-api service is expected to PUT status updates to this callback URL. Each which will have a json object with the keys of `status` and `detail`. The status will be one of `PROCESSED`,
`COMPLETED` or `ERROR`. The detail is expected to be a text string.

### 4. GET status

The request handler accepts the following structure:

GET /status/{id}
RETURNS Object {
   "status": String,
    "detail": String,
    "body": String,
    "created_at": String, // UTC iso8601 Time Format
    "updated_at": String, // UTC iso8601 Time Format
} 
                                                                                              
Using the unique ID in the query arg, this API will return the status of the request
from the service. It will return the status, detail and original body, as well as timestamps 
for when the request was initiated and when the latest update occurred.

# Technologies Used

The primary tools used for this work are C# (.NET Core) and the Visual Studio IDE.
