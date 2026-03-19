
using static GF.UnityWebService;

namespace GF
{
    public interface Response
    {
        long code { get; }
        bool status { get; }
        string message { get; }
    }


}