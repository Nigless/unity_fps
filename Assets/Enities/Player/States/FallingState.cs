public class FallingState : BaseState
{
    public FallingState(Player Context) : base(Context)
    {
        _PartMachine = _PartMachine.With(new FallingPart(Context, 0.5f));
    }

    public override StateMachine<Player>.State UpdateState()
    {

        if (_Context.Controller.isGrounded)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }

        return this;
    }
}