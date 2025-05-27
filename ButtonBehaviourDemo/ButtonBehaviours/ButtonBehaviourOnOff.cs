using ButtonBehaviourDemo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    // Implements methods to get and set a parameter value based on button state.
    // Specifics of when to get and set must be implemented in derived classes.
    public abstract class ButtonBehaviourOnOff : BaseButtonBehaviour
    {
        protected void SetParameterState(bool value)
        {
            _isOn = value;
            _eventBus.Publish(new ParameterUpdateEvent { _parameterName = _parameterToControl, _parameterValue = Convert.ToInt32(_isOn)});
        }

        protected bool GetParameterState()
        {
            return _isOn;
        }

        private string _parameterToControl;
        private bool _isOn = false;

        public ButtonBehaviourOnOff(EventBus eventbus, string parameterToControl) : base(eventbus)
        {
            _parameterToControl = parameterToControl;
        }
    }
}
