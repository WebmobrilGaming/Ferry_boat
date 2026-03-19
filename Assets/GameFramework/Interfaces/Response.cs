
using static GF.UnityWebService;

namespace GF
{
    public interface Response
    {
        long code { get; }
        HttpCodes status { get; }
        string message { get; }
    }


}