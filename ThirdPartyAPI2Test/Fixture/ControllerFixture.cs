using System;
using ThirdPartyAPI2.Controllers;
using ThirdPartyAPI2.Services;
using ThirdPartyAPI2Test.Entities.ModelDb;

namespace ThirdPartyAPI2Test.Fixture
{
    public class ControllerFixture : IDisposable
    {
        private bool disposed = false;

        private ModelDbContextMock dbContextMock { get; set; }

        private ThirdPartyAPIService thirdPartyAPIService { get; set; }

        public ThirdPartyAPIController thirdPartyAPIController { get; private set; }

        public ControllerFixture()
        {
            dbContextMock = new ModelDbContextMock();

            dbContextMock.BObject.AddRange(new ThirdPartyAPI2.Entities.BodyObject[]
            {      
                new ThirdPartyAPI2.Entities.BodyObject()
                {
                  Id = "ac81af8c-8863-4a22-b2d2-9083a4c10830",
                  Body = "sample body # 1",
                  Status = "STARTED",
                  Detail = "started",
                  CreatedAt = DateTime.Now,
                  UpdatedAt = null,
                },

                new ThirdPartyAPI2.Entities.BodyObject()
                {
                 Id = "bcd1af8c-8863-4a22-b2d2-9083a4c10830",
                  Body = "sample body # 2",
                  Status = "STARTED",
                  Detail = "started",
                  CreatedAt = DateTime.Now,
                  UpdatedAt = null,
                }
            });

            dbContextMock.SaveChanges();

            // Create ThirdPartyAPIService with Memory Database
            thirdPartyAPIService = new ThirdPartyAPIService(dbContextMock);

            // Create Controller
            thirdPartyAPIController = new ThirdPartyAPIController(thirdPartyAPIService);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // nothing else to finalize other than dispose(false)
        ~ControllerFixture()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContextMock.Dispose();
                    dbContextMock = null;

                    thirdPartyAPIService = null;
                    thirdPartyAPIController = null;

                    this.disposed = true;
                }
            }
        }
    }
}
