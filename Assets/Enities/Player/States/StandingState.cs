public class StandingState : BaseState
{
    public StandingState(Player Context) : base(Context)
    {
        _PartMachine = _PartMachine.With(new MovingPart(Context, 3f));
    }

    public override StateMachine<Player>.State UpdateState()
    {
        if (!_Context.Controller.isGrounded)
        {
            return _Context.StateMachine.Get<FallingState>();
        }

        if (_Context.Running && _Context.Moving.y >= 0)
        {
            return _Context.StateMachine.Get<RunningState>();
        }

        return this;
    }
}