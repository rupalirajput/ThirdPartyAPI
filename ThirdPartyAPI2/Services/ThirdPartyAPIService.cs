using System;
using System.Linq;
using ThirdPartyAPI2.Entities;
using ThirdPartyAPI2.Entities.ModelDb;

namespace ThirdPartyAPI2.Services
{
    public class ThirdPartyAPIService : IThirdPartyAPIService
    {
        private ModelDbContext _dbCtx;

        public ThirdPartyAPIService(ModelDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        /**
         * AddBodyObject inserts the BodyObject entity into the database.
         * The entities are not guaranteed to be unique, it is the callers
         * responsibility to ensure that the BodyObject.Id is a unique
         * identifier.
         */
        public void AddBodyObject(BodyObject bo)
        {
            bo.CreatedAt = DateTime.Now;
            _dbCtx.BObject.Add(bo);
            _dbCtx.SaveChanges();
        }

        /**
         * UpdateBodyObject will update the existing object by BodyObject.Id.
         * 
         * If the object is not found by its ID then a NotFoundException is thrown.
         */
        public void UpdateBodyObject(BodyObject bo)
        {
            BodyObject objBO = _dbCtx.BObject.Where(w => w.Id == bo.Id).FirstOrDefault();
            if (String.IsNullOrEmpty(objBO.Id))
            {
                throw new Exception(); // TODO throw custom NotFoundException
            }

            objBO.Status = bo.Status;
            objBO.Detail = bo.Detail;
            objBO.UpdatedAt = DateTime.Now;
            _dbCtx.SaveChanges();
        }

        /**
         * GetBodyObject will retrieve the BodyObject entity by the given ID.
         * 
         * If it is not found then NotFoundException exception is thrown.
         */
        public BodyObject GetBodyObject(string id)
        {
            BodyObject bo = _dbCtx.BObject.Where(w => w.Id == id).FirstOrDefault();

            if (String.IsNullOrEmpty(bo.Id))
            {
                throw new Exception(); // TODO throw custom NotFoundException
            }

            return bo;
        }
    }
}
