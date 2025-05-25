using ButtonBehaviourDemo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public abstract class ButtonBehaviourOnOff : BaseButtonBehaviour
    {
        protected void SetParameterState(bool value)
        {
            // Logic to set the parameter value
            _isOn = value;
            _eventBus.Publish(new ParameterUpdateEvent { _parameterName = _parameterToControl, _parameterValue = Convert.ToInt32(_isOn)});
            Console.WriteLine($"[ButtonBehaviourOnOff] Parameter '{_parameterToControl}' set to: {_isOn}");
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
