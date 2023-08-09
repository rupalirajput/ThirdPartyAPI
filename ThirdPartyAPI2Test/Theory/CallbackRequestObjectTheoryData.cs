using System;
using ThirdPartyAPI2.Entities;
using Xunit;

namespace ThirdPartyAPI2Test.Theory
{
    public class CallbackRequestObjectTheoryData : TheoryData<CallbackRequestObject>
    {
        public CallbackRequestObjectTheoryData()
        {
            Add(new CallbackRequestObject()
            {
                Status = "PROCESSED",
                Detail = "processed.."
            });
        }
    }
}
