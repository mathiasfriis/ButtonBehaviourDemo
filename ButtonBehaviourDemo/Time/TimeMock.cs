using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Time
{
    // Class that implements the ITime interface for mocking purposes in tests.
    public class TimeMock : ITime
    {
        DateTime _time;
        public DateTime GetTime()
        {
            return _time;
        }

        public void SetTime(DateTime time)
        {
            _time = time;
        }
    }
}
