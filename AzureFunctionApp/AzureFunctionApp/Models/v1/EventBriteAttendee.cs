using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionApp.Models.v1
{
    public class EventBriteAttendee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string EMail { get; set; }
    }
}
