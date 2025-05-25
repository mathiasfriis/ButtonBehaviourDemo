using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public abstract class BaseButtonBehaviour
    {
        protected EventBus _eventBus;

        protected BaseButtonBehaviour(EventBus bus)
        {
            _eventBus = bus;
        }
        public abstract void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt);
    }
}
