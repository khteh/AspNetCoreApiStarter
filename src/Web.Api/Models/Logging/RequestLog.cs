using System.Net;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Web.Api.Models.Logging
{
    public class RequestLog
    {
        [JsonProperty]
        public string Method {get; private set;}
        [JsonProperty]
        public string Scheme {get; private set;}
        [JsonProperty]
        public string PathBase {get; private set;}
        [JsonProperty]
        public string Path {get; private set;}
        [JsonProperty]
        public IPAddress IP {get; private set;}
        [JsonProperty]
        public HostString Host {get; private set;}
        [JsonProperty]
        public long ContentLength {get; private set;}
        public RequestLog(HttpRequest request, IPAddress ip)
        {
            Method = request.Method;
            Scheme = request.Scheme;
            PathBase = request.PathBase;
            Path = request.Path;
            IP = ip;
            Host = request.Host;
            ContentLength = request.ContentLength ?? 0;
        }
    }
}