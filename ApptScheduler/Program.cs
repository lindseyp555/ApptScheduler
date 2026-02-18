using ApptScheduler;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://scheduling.interviews.brevium.com/")
        };
        await RunSchedule(httpClient);
    }

    static async Task RunSchedule(HttpClient httpClient)
    {
        Client client = new Client(httpClient);
        var scheduler = new Scheduler();
        await client.Start();
        Appointment[] appts = await client.GetInitSchedule();
        scheduler.LoadAppointments(appts);
        AppointmentRequest? request;
        while ((request = await client.GetApptRequest()) != null)
        {
            Appointment ap = scheduler.Schedule(request);
            await client.Schedule(ap);
        }
        await client.Stop();
    }
}