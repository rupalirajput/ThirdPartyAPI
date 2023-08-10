using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ThirdPartyAPI2.Controllers;
using ThirdPartyAPI2.Entities;
using ThirdPartyAPI2Test.Fixture;
using ThirdPartyAPI2Test.Theory;
using Xunit;

namespace ThirdPartyAPI2Test
{
    public class ThirdPartyAPIControllerTest : IClassFixture<ControllerFixture>
    {
        ThirdPartyAPIController thirdPartyAPIController;

        /**
         * xUnit constructor runs before each test. 
         */
        public ThirdPartyAPIControllerTest(ControllerFixture fixture)
        {
            thirdPartyAPIController = fixture.thirdPartyAPIController;
        }

        [Theory] // ye sab galat hai!
        [InlineData("")]
        public void GetStatus_WithoutParam_ThenBadRequest_Test(string id)
        {
            var result = thirdPartyAPIController.GetStatus(id) as BadRequestObjectResult;

            Assert.Equal(400, result.StatusCode); // TODO: should be 404
            Assert.Equal("Bad Id", result.Value);
        }

        [Theory]
        [InlineData("ac81af8c-8863-4a22-b2d2-9083a4c10830")]
        public void GetStatusById_WithParam_ThenOk_Test(string id)
        {
            //Arrange
            var expected = "sample body # 1";

            //Act
            var okObjectResult = thirdPartyAPIController.GetStatus(id) as OkObjectResult;

            //Assert     
            Assert.Equal(200, okObjectResult.StatusCode);

            var model = okObjectResult.Value as GetStatusResponseObject;
            Assert.NotNull(model);

            var actual = model.Body;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [ClassData(typeof(RequestObjectTheoryData))]
        public void PostRequest_WithTestData_ThenOk_Test(RequestObject req)
        {
            var okObjectResult = thirdPartyAPIController.PostRequest(req) as OkObjectResult;

            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(JsonConvert.SerializeObject(okObjectResult.Value));
        }

        [Fact]
        public void PostRequest_WithoutTestData_ThenBadRequest_Test()
        {
            //Arrange
            RequestObject req = null;

            //Act
            var badRequestObjectResult = thirdPartyAPIController.PostRequest(req) as BadRequestObjectResult;

            //Assert
            Assert.Equal(400, badRequestObjectResult.StatusCode);
        }


        [Theory]
        [InlineData("ac81af8c-8863-4a22-b2d2-9083a4c10830")]
        public async void PostCallbackAsync_WithParam_ThenNoContent_Test(string id)
        {
            //Arrange
            var callbackRequestObject = new CallbackRequestObject();
            callbackRequestObject.Status = "STARTED";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(callbackRequestObject.Status));
            var httpContext = new DefaultHttpContext()
            {
                Request = { Body = stream, ContentLength = stream.Length }
            };

            var controllerContext = new ControllerContext { HttpContext = httpContext };
            thirdPartyAPIController.ControllerContext = controllerContext;

            //Act
            var noContentResult = await thirdPartyAPIController.PostCallbackAsync(id) as NoContentResult;

            //Assert     
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async void PostCallbackAsync_WithEmptyId_ThenBadRequest_Test()
        {
            //Arrange
            string id = "";
            var callbackRequestObject = new CallbackRequestObject();
            callbackRequestObject.Status = "STARTED";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(callbackRequestObject.Status));
            var httpContext = new DefaultHttpContext()
            {
                Request = { Body = stream, ContentLength = stream.Length }
            };

            var controllerContext = new ControllerContext { HttpContext = httpContext };
            thirdPartyAPIController.ControllerContext = controllerContext;

            //Act
            var badRequestObjectResult = await thirdPartyAPIController.PostCallbackAsync(id) as BadRequestObjectResult;

            //Assert     
            Assert.Equal("bad id", badRequestObjectResult.Value);
        }

        [Theory]
        [InlineData("ac81af8c-8863-4a22-b2d2-9083a4c10830")]
        public async void PostCallbackAsync_WithInvalidStatusCallbackRequestObject_ThenBadRequest_Test(string id)
        {
            //Arrange
            var callbackRequestObject = new CallbackRequestObject();
            callbackRequestObject.Status = "invalid status";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(callbackRequestObject.Status));
            var httpContext = new DefaultHttpContext()
            {
                Request = { Body = stream, ContentLength = stream.Length }
            };

            var controllerContext = new ControllerContext { HttpContext = httpContext };
            thirdPartyAPIController.ControllerContext = controllerContext;

            //Act
            var badRequestObjectResult = await thirdPartyAPIController.PostCallbackAsync(id) as BadRequestObjectResult;

            //Assert     
            Assert.Equal("Bad status value: " + callbackRequestObject.Status, badRequestObjectResult.Value);
        }

        [Theory]
        [InlineData("bcd1af8c-8863-4a22-b2d2-9083a4c10830")]
         public void PutCallback_WithParamTestData_ThenNoContent_Test(string id)
        {
            //Arrange
            var callbackRequestObject = new CallbackRequestObject();
            callbackRequestObject.Status = "PROCESSED";
            callbackRequestObject.Detail = "processed";

            //Act
            var noContentResult = thirdPartyAPIController.PutCallback(id, callbackRequestObject) as NoContentResult;

            //Assert
            Assert.Equal(204, noContentResult.StatusCode);
        }
    }
}
