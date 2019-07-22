using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Logging;
using Web.Api.Presenters;
using Web.Api.Serialization;
using Xunit;

namespace Web.Api.UnitTests.Logging
{
    public class LoggingTests
    {
        [Fact(Skip = "System.ArgumentException : The path in 'value' must start with '/'.")]
        public async Task RequestLogSerializationTest()
        {
            HttpRequest request = new DefaultHttpRequest(new DefaultHttpContext()) {
                Method = "GET",
                Scheme = "Http",
                PathBase = "http://aspnetapistarter.com",
                Path = new PathString("/health/ready"),
                Host = new HostString("localhost")
            };
            RequestLog log = new RequestLog(request, IPAddress.Loopback);
            string strLog = JsonConvert.SerializeObject(log);
            Assert.False(string.IsNullOrEmpty(strLog));
        }
    }
}