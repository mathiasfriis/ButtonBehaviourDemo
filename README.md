# ButtonBehaviourDemo

This is a demo project implementing a system which can monitor a number of buttons, react to presses, releases, press and hold and multi-clicks, and implement some sort of domain-specific logic based on said events.

It is implemented as a number of microservices.

ButtonService: Posts the raw state of a button (i.e. is it pushed or released)  
ButtonInterpretationService: Reacts to raw button state changes, and interprets these into button events (Press, Release, PressAndHold, MultiClick)  
ButtonBehaviourService: Reacts to interpreted button events and relays these to a configured ButtonBehaviour.  
ButtonBehaviour is a class which handles interpreted button events and implements domain-specific logic.

The following diagram gives a visual representation of the flow of messages.
```mermaid
sequenceDiagram
    ButtonService->>+ButtonInterpretationService: RawButtonState
    ButtonInterpretationService->>ButtonInterpretationService : Interpret RawButtonState
    ButtonInterpretationService->>+ButtonBehaviourService: InterpretedButtonEvent
    ButtonBehaviourService->>+ButtonBehaviour : handle InterpreteButtonEvent

    ButtonBehaviour->> Rest of system: Behaviour specific messages
```


## Previous design
FootswitchService
 - Readout of specific HW
 - Calibration of HW
 - Registration of multiClick and press and hold
 - Domain/product specific logic regarding footswitch state machines (e.g. entering/exiting Tap Tempo Session)
 - Tempo detection

Product specific service
 - Handling footswitch states, triggering events in rest of system
   - E.g. turning effects on/off
   - Updating tempo for effect based on received tempo.
