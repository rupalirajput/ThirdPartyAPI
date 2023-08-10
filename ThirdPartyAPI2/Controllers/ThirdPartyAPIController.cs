using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThirdPartyAPI2.Entities;
using ThirdPartyAPI2.Services;

namespace ThirdPartyAPI2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThirdPartyAPIController : ControllerBase
    {
        private IThirdPartyAPIService _thirdPartyAPIService;

        // this coordination map helps with incoming objects which require third
        // api service to be invoked before the object is registered with the
        // database. This prevents an edge case where if the service calls us
        // back faster than we can insert the object into the database.
        private ConcurrentDictionary<string, Mutex> incomingBodyObjectLocks;

        public ThirdPartyAPIController(IThirdPartyAPIService thirdPartyAPIService)
        {
            _thirdPartyAPIService = thirdPartyAPIService;

            incomingBodyObjectLocks = new ConcurrentDictionary<string, Mutex>(Environment.ProcessorCount, 10);
        }

        /* An example implementation for the third party */
        private void sendServerCallbackRequest(string id, string body)
        {
            // Shortcircuiting the example implementation and return early to indicate success
            return;

            #region commented out example implementation code
            //var req = (HttpWebRequest)WebRequest.Create("http://example.com/request");
            //req.ContentType = "application/json";
            //req.Method = "POST";

            //using (var w = new StreamWriter(req.GetRequestStream()))
            //{
            //    string json = JsonSerializer.Serialize(new
            //    {
            //        body = body,
            //        callback = "/callback/" + id
            //    });

            //    w.Write(json);
            //}

            //var resp = (HttpWebResponse)req.GetResponse();
            //if (resp.StatusCode != HttpStatusCode.OK)
            //{
            //    throw new Exception(); // TODO throw a custom exception
            //}
            #endregion
        }

        /**
         * POST request
         * 
         * The request handler accepts the following structure:
         * 
         * POST /request
         * BODY: Object {
         *   "body": String
         * }
         * RETURNS String
         *
         * This accepts a JSON body consisting of a one key, "body", which is a string. Doing 
         * this will initiate a request to the third-party service. It will also create a unique 
         * identifer for this request we can later reference. This unique identifier string is returned by this API call.
         */
        [Route("/request"), HttpPost]
        public IActionResult PostRequest(RequestObject req)
        {
            string id = Guid.NewGuid().ToString();

            // Lock the requested object in-case the third party server issues a
            // callback request before we call AddBodyObject().
            Mutex mtx = new Mutex();
            this.incomingBodyObjectLocks[id] = mtx;
            mtx.WaitOne();

            try
            {
                // This sends the request to the third party API server and registers the request.
                this.sendServerCallbackRequest(id, req.Body);
            }
            catch (Exception ex)
            {
                mtx.ReleaseMutex();
                return new BadRequestObjectResult(ex); // TODO return a 500 status with a custom message
            }

            _thirdPartyAPIService.AddBodyObject(new BodyObject { Id = id, Body = req.Body });

            // remove the lock object when we are done
            mtx.ReleaseMutex();
            this.incomingBodyObjectLocks.TryRemove(id, out _);
            mtx.Dispose();

            return Ok(id);
        }

        /**
         * POST callback
         * 
         * The request handler accepts the following structure:
         * 
         * POST /callback/{id}
         * BODY String
         * RETURNS 204
         * 
         * This URL is sent in the original /request handler. The third-party-service service is expected to send an
         * initial POST with the text string `STARTED` on this API to indicate that they have received the request.
         * 
         */
        [Route("/callback/{id}"), HttpPost]
        public async Task<IActionResult> PostCallbackAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest("bad id");
            }

            string status;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                status = await reader.ReadToEndAsync();

                if (status != "STARTED")
                {
                    return BadRequest("Bad status value: " + status);
                }
            }

            // wait for the thrird party api server POST method to finish registering the body object
            Mutex mtx;
            if (this.incomingBodyObjectLocks.TryGetValue(id, out mtx))
            {
                mtx.WaitOne();
                mtx.ReleaseMutex();
            }

            try
            {
                _thirdPartyAPIService.UpdateBodyObject(new BodyObject { Id = id, Status = status });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);  // TODO return a 500/404 status with a custom message
            }

            return this.NoContent();
        }

        /**
         * PUT callback
         * 
         * The request handler accepts the following structure:
         * 
         * PUT /callback/{id}
         * BODY Object {
         * "status": String,
         * "detail": String
         * }
         * RETURNS 204
         *
         * The third-party-api service is expected to PUT status updates to this callback URL. Each which will have 
         * a json object with the keys of `status` and `detail`. The status will be one of `PROCESSED`,
         * `COMPLETED` or `ERROR`. The detail is expected to be a text string.
         */
        [Route("/callback/{id}"), HttpPut]
        public IActionResult PutCallback(string id, CallbackRequestObject body)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("bad id");
            }

            string[] validStatus = { "PROCESSED", "COMPLETED", "ERROR" };
            if (Array.IndexOf(validStatus, body.Status) < 0)
            {
                return BadRequest("bad status: " + body.Status);
            }

            try
            {
                _thirdPartyAPIService.UpdateBodyObject(new BodyObject { Id = id, Status = body.Status, Detail = body.Detail });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);  // TODO return a 500/404 status with a custom message
            }

            return this.NoContent();
        }


        /**
         * GET status
         * 
         * The request handler accepts the following structure:
         * 
         * GET /status/{id}
         * RETURNS Object {
         *    "status": String,
         *     "detail": String,
         *     "body": String,
         *     "created_at": String, // UTC iso8601 Time Format
         *     "updated_at": String, // UTC iso8601 Time Format
         * } 
         *
         * Using the unique ID in the query arg, this API will return the status of the request
         * from the service. It will return the status, detail and original body, as well as timestamps 
         * for when the request was initiated and when the latest update occurred.
         *   
         */

        [Route("/status/{id}"), HttpGet]
        public IActionResult GetStatus(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return BadRequest("Bad Id");
            }

            BodyObject bo;
            try
            {
                bo = _thirdPartyAPIService.GetBodyObject(id);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);  // TODO return a 500/404 status with a custom message
            }

            return this.Ok(new GetStatusResponseObject
            {
                Body = bo.Body,
                Status = bo.Status,
                Detail = bo.Detail,
                CreatedAt = bo.CreatedAt,
                UpdatedAt = bo.UpdatedAt
            });
        }
    }
}
