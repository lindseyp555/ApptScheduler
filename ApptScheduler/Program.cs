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

    //I debated for a while on whether or not I should combine my Client and Scheduler classes into one entity. I decided I wanted to respect the separation of concerns as it is nice for testing purposes to keep the scheduler's logic isolated,
    //but I acknowledge the drawback that running the specified procedure with two separate objects is a little complicated. It would be nice to have this procedure as one method belonging to an object.
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