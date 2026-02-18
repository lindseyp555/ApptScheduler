using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApptScheduler
{
    internal class AppointmentRequest
    {
        public int requestId { get; private set; }
        public int personId { get; private set; }
        public DateTime[] preferredDays { get; private set; }
        public int[] preferredDocs {  get; private set; }
        public bool isNew { get; private set; }

        [JsonConstructor]
        public AppointmentRequest(int requestId, int personId, DateTime[] preferredDays, int[] preferredDocs, bool isNew)
        {
            this.requestId = requestId;
            this.personId = personId;
            this.preferredDays = preferredDays;
            this.preferredDocs = preferredDocs;
            this.isNew = isNew;
        }
    }
}
