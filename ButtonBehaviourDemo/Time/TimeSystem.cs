using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Time
{
    internal class TimeSystem : ITime
    {
        public DateTime GetTime()
        {
            return DateTime.Now; // This could be replaced with a mockable interface for testing purposes.
        }
    }
}
