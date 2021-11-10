using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Common.Models.EntityLuis
{
    public class EntityLuisModel
    {
        public List<DateTimeEntity> datetime { get; set; }
    }

    public class DateTimeEntity
    {
        public List<string> timex { get; set; }

        public string type { get; set; }
    }
}
