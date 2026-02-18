using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApptScheduler
{

    internal class Client
    {
        HttpClient _httpClient;
        string token = "";

        public Client(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task Start()
        {
            var response = await _httpClient.PostAsync($"https://scheduling.interviews.brevium.com/api/Scheduling/Start?token={token}", null);
        }

        public async Task Stop()
        {
            HttpResponseMessage message = await _httpClient.PostAsync($"https://scheduling.interviews.brevium.com/api/Scheduling/Stop?token={token}", null);
            string jsonMessage = await message.Content.ReadAsStringAsync();
            Console.WriteLine(jsonMessage);
        }

        public async Task<Appointment[]> GetInitSchedule()
        {
            HttpResponseMessage message = await _httpClient.GetAsync($"https://scheduling.interviews.brevium.com/api/Scheduling/Schedule?token={token}");
            string jsonMessage = await message.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Appointment[]>(jsonMessage);
        }

        public async Task<AppointmentRequest?> GetApptRequest()
        {
            HttpResponseMessage message = await _httpClient.GetAsync($"https://scheduling.interviews.brevium.com/api/Scheduling/AppointmentRequest?token={token}");
            if (message.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return null;
            }
            message.EnsureSuccessStatusCode();
            string jsonMessage = await message.Content.ReadAsStringAsync();
            //Console.WriteLine(jsonMessage);
            return JsonSerializer.Deserialize<AppointmentRequest>(jsonMessage);
        }

        public async Task Schedule(Appointment appt)
        {
            Console.WriteLine("Appointment date: " + appt.appointmentTime);
            string json = JsonSerializer.Serialize(appt);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage message = await _httpClient.PostAsync($"https://scheduling.interviews.brevium.com/api/Scheduling/Schedule?token={token}", content);
            Console.WriteLine(await message.Content.ReadAsStringAsync());
        }
    }
}
