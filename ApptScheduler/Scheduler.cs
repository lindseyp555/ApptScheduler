using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApptScheduler
{

    internal class Scheduler
    {
        Dictionary<int, HashSet<DateTime>> apptByDoctor = new();
        Dictionary<int, HashSet<DateTime>> apptByPatient = new();

        public Scheduler() {
            apptByDoctor.Add(1, new HashSet<DateTime>());
            apptByDoctor.Add(2, new HashSet<DateTime>());
            apptByDoctor.Add(3, new HashSet<DateTime>());
        }

        public void LoadAppointments(Appointment[] appts)
        {
            foreach (Appointment appt in appts)
            {
                apptByDoctor[appt.doctorId].Add(appt.appointmentTime);
                if (!apptByPatient.ContainsKey(appt.personId))
                {
                    apptByPatient.Add(appt.personId, new HashSet<DateTime>());
                }
                apptByPatient[appt.personId].Add(appt.appointmentTime);
            }
        }


    //    Appointments may only be scheduled on the hour.
    //    Appointments can be scheduled as early as 8 am UTC and as late as 4 pm UTC. 
    //    Appointments may only be scheduled on weekdays during the months of November and December 2021. 
    //    Appointments can be scheduled on holidays. 
    //    For a given doctor, you may only have one appointment scheduled per hour (though different doctors may have appointments at the same time). 
    //    For a given patient, each appointment must be separated by at least one week.For example, if Bob Smith has an appointment on 11/17 you may schedule another appointment on or before 11/10 or on or after 11/24. 
    //    Appointments for new patients may only be scheduled for 3 pm and 4 pm.

        public Appointment? Schedule(AppointmentRequest request)
        {
            foreach (int id in request.preferredDocs)
            {
                foreach (var preferredDate in request.preferredDays)
                {
                    if (!verifyDay(preferredDate))
                    {
                        continue;
                    }
                    int earliestTime = 8;
                    if (request.isNew)
                    {
                        earliestTime = 15;
                    }
                    for (int i = earliestTime; i < 17; i++)
                    {
                        DateTime currentTime = new DateTime(preferredDate.Year, preferredDate.Month, preferredDate.Day, i, 0, 0, DateTimeKind.Utc);
                        if (!apptByDoctor[id].Contains(currentTime) && verifyPatientAppt(currentTime, request.personId)) {
                            apptByDoctor[id].Add(currentTime);
                            if (!apptByPatient.ContainsKey(request.personId))
                            {
                                apptByPatient.Add(request.personId, new HashSet<DateTime>());
                            }
                            apptByPatient[request.personId].Add(currentTime);
                            return new Appointment(id, request.personId, currentTime, request.isNew);
                        }
                    }
                }
            }
            return null;
        }

        private bool verifyDay(DateTime dt)
        {
            if (dt < new DateTime(2021, 11, 1) || dt > new DateTime(2021, 12, 31))
            {
                return false;
            }
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            return true;
        }

        private bool verifyPatientAppt(DateTime preferredDate, int patientId)
        {
            if (!apptByPatient.ContainsKey(patientId))
            {
                return true;
            }
            foreach (DateTime appt in apptByPatient[patientId]) {
                if (Math.Abs((preferredDate - appt).TotalDays) < 7)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
