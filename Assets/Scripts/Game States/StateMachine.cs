using System;
using System.Collections.Generic;
using UnityEngine;

// my brain is fried, i cant think anymore, this is the best solution i can think of

/// <summary>
/// Reusable state machine for global, menu, and level-specific state graphs.
/// A state must be registered before it can be entered, and transitions must
/// be explicitly allowed by the owner of the machine.
/// </summary>
public sealed class StateMachine<TState>
{
    private readonly Dictionary<TState, IGameState> states = new Dictionary<TState, IGameState>();
    private readonly Dictionary<TState, HashSet<TState>> transitions = new Dictionary<TState, HashSet<TState>>();

    public event Action<TState, TState> StateChanged;
    private IGameState currentState;
    private TState currentStateType;

    public bool HasCurrentState { get; private set; }
    public TState CurrentState
    {
        get
        {
            if (!HasCurrentState)
            {
                throw new InvalidOperationException("The state machine has not been started.");
            }

            return currentStateType;
        }
    }

    #region Unity stuff
    public void Start(TState initialState)
    {
        EnsureRegistered(initialState);

        if (HasCurrentState)
        {
            throw new InvalidOperationException("The state machine has already been started.");
        }

        currentStateType = initialState;
        currentState = states[initialState];
        HasCurrentState = true;
        Debug.Log($"[StateMachine<{typeof(TState).Name}>] Entered {initialState} state.");
        currentState.EnterState();
    }

    public void Update()
    {
        currentState?.UpdateState();
    }
    #endregion

    public void RegisterState(TState stateType, IGameState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (states.ContainsKey(stateType))
        {
            throw new InvalidOperationException($"A state is already registered for '{stateType}'.");
        }

        states.Add(stateType, state);
        transitions[stateType] = new HashSet<TState>();
    }

    public void AllowTransition(TState from, TState to)
    {
        EnsureRegistered(from);
        EnsureRegistered(to);
        transitions[from].Add(to);
    }

    public bool CanChangeTo(TState targetState)
    {
        return HasCurrentState
            && transitions.ContainsKey(currentStateType)
            && transitions[currentStateType].Contains(targetState);
    }

    public bool TryChangeState(TState targetState)
    {
        EnsureRegistered(targetState);

        if (!HasCurrentState)
        {
            Debug.LogWarning($"[StateMachine] Ignoring transition to '{targetState}' because the machine has not started.");
            return false;
        }

        if (EqualityComparer<TState>.Default.Equals(currentStateType, targetState))
        {
            Debug.Log($"[StateMachine<{typeof(TState).Name}>] Already in {targetState} state.");
            return true;
        }

        if (!CanChangeTo(targetState))
        {
            Debug.LogWarning($"[StateMachine] Transition '{currentStateType}' -> '{targetState}' is not allowed.");
            return false;
        }

        TState previousStateType = currentStateType;
        currentState.ExitState();
        currentStateType = targetState;
        currentState = states[targetState];
        Debug.Log($"[StateMachine<{typeof(TState).Name}>] Transitioned {previousStateType} -> {targetState}.");
        currentState.EnterState();
        StateChanged?.Invoke(previousStateType, targetState);
        return true;
    }

    private void EnsureRegistered(TState stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            throw new KeyNotFoundException($"No state is registered for '{stateType}'.");
        }
    }
}
