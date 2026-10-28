using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private readonly StateMachine<GlobalStateType> stateMachine = new StateMachine<GlobalStateType>();

    public GlobalStateType CurrentState => stateMachine.CurrentState;

    private void OnEnable() => GameEvents.OnGlobalStateChanged += SwitchState;
    private void OnDisable() => GameEvents.OnGlobalStateChanged -= SwitchState;

    private void Awake()
    {
        stateMachine.RegisterState(GlobalStateType.Menu, new MENU());
        stateMachine.RegisterState(GlobalStateType.Active, new ACTIVE());
        stateMachine.RegisterState(GlobalStateType.Hub, new HUB());
        stateMachine.RegisterState(GlobalStateType.Dialogue, new DIALOGUE());
        stateMachine.RegisterState(GlobalStateType.Outcome, new OUTCOME());

        foreach (GlobalStateType from in Enum.GetValues(typeof(GlobalStateType)))
        {
            foreach (GlobalStateType to in Enum.GetValues(typeof(GlobalStateType)))
            {
                stateMachine.AllowTransition(from, to);
            }
        }
    }

    private void Start()
    {
        stateMachine.Start(GlobalStateType.Menu);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void SwitchState(GlobalStateType targetState)
    {
        stateMachine.TryChangeState(targetState);
    }
}
