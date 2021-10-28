using APIFrame.Web.Request.Interfaces;
using System.Diagnostics;

namespace APIFrame.Web.Request
{
    public class RequestProfiler : IRequestProfiler
    {
        private Stopwatch _stopWatch;

        public void Start()
        {
            _stopWatch = Stopwatch.StartNew();
        }

        public long Stop()
        {
            _stopWatch.Stop();

            return _stopWatch.ElapsedMilliseconds;
        }
    }
}
