using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using ThirdPartyAPI2.Entities;
using ThirdPartyAPI2.Services;

namespace ThirdPartyAPI2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThirdPartyAPIController : ControllerBase
    {
        private IThirdPartyAPIService _thirdPartyAPIService;

        public ThirdPartyAPIController(IThirdPartyAPIService thirdPartyAPIService)
        {
            _thirdPartyAPIService = thirdPartyAPIService;
        }
        private readonly ILogger<ThirdPartyAPIController> _logger;

        private void sendServerCallbackRequest(string id, string body)
        {
            return;

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
            //    throw new Exception(); // TODO
            //}
        }
       
        // POST request
        [Route("/request"), HttpPost]
        public IActionResult PostRequest(RequestObject req)
        {
            string id = Guid.NewGuid().ToString();

            try
            {
                this.sendServerCallbackRequest(id, req.Body);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex); // TODO
            }

            _thirdPartyAPIService.AddBodyObject(new BodyObject {Id = id, Body = req.Body});

            return Ok(id);
        }

        // POST callback
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

            try
            {
                _thirdPartyAPIService.UpdateBodyObject(new BodyObject { Id = id, Status = status });

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex); // TODO
            }

            return this.NoContent();
        }

        // PUT callback
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
                return new BadRequestObjectResult(ex); // TODO
            }

            return this.NoContent();
        }


        // GET status
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
                return new BadRequestObjectResult(ex); // TODO
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
