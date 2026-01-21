using UnityEngine;

public class DieState : State
{
    public DieState(Character character) : base(character) { }

    public override void Enter()
    {
        _character.rb.velocity = Vector2.zero;
        _character.rb.simulated = false; // tắt physics

        _character.SetAnimation("Die", false);

        Debug.Log("Enter DieState");
    }

    public override void Update(float horizontal, float vertical)
    {
    }

    public override void Exit()
    {
    }
}
