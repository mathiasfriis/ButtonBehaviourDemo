using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    internal interface IButtonBehaviour
    {
        public void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt);
    }
}
