using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApptScheduler
{
    internal class Appointment
    {
        public int doctorId {  get; private set; }
        public int personId { get; private set; }
        public DateTime appointmentTime { get; private set; }
        public bool isNewPatientAppointment { get; private set; }

        [JsonConstructor]
        public Appointment(int doctorId, int personId, DateTime appointmentTime, bool isNewPatientAppointment)
        {
            this.doctorId = doctorId;
            this.personId = personId;
            this.appointmentTime = appointmentTime;
            this.isNewPatientAppointment = isNewPatientAppointment;
        }
    }
}
