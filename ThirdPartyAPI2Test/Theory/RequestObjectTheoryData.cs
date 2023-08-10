using ThirdPartyAPI2.Entities;
using Xunit;

namespace ThirdPartyAPI2Test.Theory
{
    public class RequestObjectTheoryData : TheoryData<RequestObject>
    {
        public RequestObjectTheoryData()
        {
            Add(new RequestObject()
            {
                Body="sample body # 2"
            });
        }
    }
}
