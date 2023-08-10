using ThirdPartyAPI2.Entities;

namespace ThirdPartyAPI2.Services
{
    public interface IThirdPartyAPIService
    {
        void AddBodyObject(BodyObject body);

        void UpdateBodyObject(BodyObject body);

        BodyObject GetBodyObject(string id);
    }
}
