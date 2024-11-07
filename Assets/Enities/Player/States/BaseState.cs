public class BaseState : StateMachine<Player>.State
{
    protected PartMachine<Player> _PartMachine = new();

    public BaseState(Player Context) : base(Context)
    {
        _PartMachine = _PartMachine.With(new LookingPart(Context));
    }

    public override void Update()
    {
        _PartMachine.Update();
    }

}