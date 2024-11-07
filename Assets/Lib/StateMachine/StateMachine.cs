using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{

    public abstract class State
    {

        protected T _Context { get; }

        public State(T Context)
        {
            _Context = Context;
        }

        public virtual void Enter()
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void Update()
        {

        }

        public virtual State UpdateState()
        {
            return this;
        }
    }


    private State _CurrentState;

    private readonly Dictionary<Type, State> _States = new();


    public StateMachine(State state)
    {
        _CurrentState = state;
        _States.Add(state.GetType(), state);
    }

    protected void SetState(State state)
    {
        _CurrentState.Exit();
        state.Enter();
        _CurrentState = state;
    }

    public StateMachine<T> With(State state)
    {
        _States.Add(state.GetType(), state);
        return this;
    }

    public State Get<S>()
    {
        return _States[typeof(S)];
    }

    public void Update()
    {
        UpdateState();
        _CurrentState.Update();
    }

    public void ChangeState<S>()
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

    private void UpdateState()
    {
        var state = _CurrentState.UpdateState();

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