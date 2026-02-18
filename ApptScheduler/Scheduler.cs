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
            //code shouldn't get to this point, unfortunately looks like some bug is still hitting it
            return new Appointment(0, 0, new DateTime(), false);
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
