using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<Context> : MonoBehaviour where Context : StateMachine<Context>
{

    public abstract class State
    {
        protected Context Context { get; }

        internal State(Context Context)
        {
            this.Context = Context;
        }

        internal virtual void Enter()
        {

        }

        internal virtual void Exit()
        {

        }

        internal virtual void Update()
        {

        }

        internal virtual void FixedUpdate()
        {

        }

        internal void TransitTo<S>() where S : State
        {
            this.Context.TransitTo<S>();
        }

        internal void Transit()
        {
            this.Context.TransitTo(this);
        }

        internal S Get<S>() where S : State
        {
            return this.Context.Get<S>();
        }
    }


    private State _CurrentState;

    private readonly Dictionary<Type, State> _States = new();

    public StateMachine()
    {
    }

    protected StateMachine<Context> With(State state)
    {
        _States.Add(state.GetType(), state);
        return this;
    }

    protected StateMachine<Context> WithInitial(State state)
    {
        _CurrentState = state;
        _States.Add(state.GetType(), state);
        return this;
    }

    private S Get<S>() where S : State
    {
        return (S)_States[typeof(S)];
    }

    public bool IsIn<S>()
    {
        return _CurrentState.GetType() == typeof(S);
    }

    private void Update()
    {
        _CurrentState.Update();
    }

    private void FixedUpdate()
    {
        _CurrentState.FixedUpdate();
    }


    private void TransitTo<S>() where S : State
    {

        var state = Get<S>();

        if (state == _CurrentState)
        {
            return;
        }

        Debug.Log(state);

        _CurrentState.Exit();
        state.Enter();
        _CurrentState = state;

    }

    private void TransitTo(State state)
    {
        if (state == _CurrentState)
        {
            return;
        }

        Debug.Log(state);

        _CurrentState.Exit();
        state.Enter();
        _CurrentState = state;

    }
}