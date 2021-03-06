using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Models.Response;
using Xunit;

namespace Web.Api.IntegrationTests.SignalR
{
    public class ChatHubTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly TestServer _testServer;
        public ChatHubTests(CustomWebApplicationFactory<Startup> factory) => _testServer = factory.Server;
        private async Task<string> AccessTokenProvider()
        {
            HttpClient client = _testServer.CreateClient();
            Assert.NotNull(client);
            var httpResponse = await client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<LoginResponse>(await httpResponse.Content.ReadAsStringAsync());
            Assert.NotNull(response);
            Assert.NotNull(response.AccessToken);
            Assert.False(string.IsNullOrEmpty(response.AccessToken.Token));
            Assert.False(string.IsNullOrEmpty(response.RefreshToken));
            Assert.Equal(7200,(int)response.AccessToken.ExpiresIn);
            return response.AccessToken.Token;
        }
        [Fact]
        public async Task ReceiveMessageTest()
        {
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            string echo = string.Empty;
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ =>  _testServer.CreateHandler();
                                o.AccessTokenProvider = async () => await AccessTokenProvider();
                                //o.Transports = HttpTransportType.WebSockets; Websockets is currently unmockable. https://github.com/dotnet/aspnetcore/issues/28108
                                //o.SkipNegotiation = true;
                            }).Build();
            connection.On<string>("ReceiveMessage", i => {
                echo = i;
                messageReceivedEvent.Set();
            });
            await connection.StartAsync();
            Assert.Equal(HubConnectionState.Connected, connection.State);
            await connection.InvokeAsync("ReceiveMessage", message);
            messageReceivedEvent.WaitOne();
            Assert.False(string.IsNullOrEmpty(echo));
            Assert.Equal(message, echo);
        }
        [Fact]
        public async Task ReceiveMessageFromUserTest()
        {
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            string user = string.Empty;
            string echo = string.Empty;
            string sender = "Mickey Mouse";
            string message = "Integration Testing in Microsoft AspNetCore SignalR";
            HubConnection connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost/chatHub", o => {
                                o.HttpMessageHandlerFactory = _ => _testServer.CreateHandler();
                                o.AccessTokenProvider = async () => await AccessTokenProvider();
                                //o.Transports = HttpTransportType.WebSockets; Websockets is currently unmockable. https://github.com/dotnet/aspnetcore/issues/28108
                                //o.SkipNegotiation = true;
                            }).Build();
            connection.On<string, string>("ReceiveMessageFromUser", (u, i) => {
                user = u;
                echo = i;
                messageReceivedEvent.Set();
            });
            await connection.StartAsync();
            Assert.Equal(HubConnectionState.Connected, connection.State);
            await connection.InvokeAsync("ReceiveMessageFromUser", sender, message);
            messageReceivedEvent.WaitOne();
            Assert.False(string.IsNullOrEmpty(user));
            Assert.False(string.IsNullOrEmpty(echo));
            Assert.Equal(message, echo);
            Assert.Equal(sender, user);
        }
    }
}