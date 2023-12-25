using AirportSimulator.Services.ModelsDTO;
using System.Net.Http.Json;

namespace AirportSimulator.Services
{
    public class DataService
    {
        protected HttpClient client = new() { BaseAddress = new Uri("https://localhost:7129") };
        public async Task GetHub(ControllerType controller) =>
            await client.GetAsync($"api/{controller.ToString()}/hub");
        public async Task<T> GetData<T>(ControllerType controller) =>
            await client.GetFromJsonAsync<T>($"api/{controller.ToString()}");
        public async Task<bool> AddData<T>(ControllerType controller, T data)
        {
            var response = await client.PostAsJsonAsync($"api/{controller.ToString()}", data);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool>UpdateData<T>(ControllerType controller)
        {
            var data = await GetData<T>(controller);
            var response = await client.PutAsJsonAsync($"api/{controller.ToString()}",data);
            return response.IsSuccessStatusCode;
        }
    }
}