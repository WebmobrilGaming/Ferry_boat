using Netconfig;

namespace GF
{
    public interface Request
    {
        public RequestType requestType { get; }

        public string requestData { get; }
    }
}