using BookStoreUI.Contracts;
using BookStoreUI.Models;
using BookStoreUI.Static;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreUI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;

        public AuthenticationRepository(IHttpClientFactory client)
        {
            _client = client;
        }

        public Task<bool> Login(LoginModel user)
        {
            throw new System.NotImplementedException();
        }

        public Task Logout()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post
                , Endpoints.RegisterEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user)
                , Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
