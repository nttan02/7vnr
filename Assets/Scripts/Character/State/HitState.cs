using UnityEngine;

public class HitState : State
{
    private float hitDuration = 0.4f; // thời gian stun
    private float timer;

    public HitState(Character character) : base(character) { }

    public override void Enter()
    {
        timer = hitDuration;

        _character.rb.velocity = Vector2.zero;

        _character.SetAnimation("Hit", false);

        Debug.Log("Enter HitState");
    }

    public override void Update(float horizontal, float vertical)
    {
        timer -= Time.fixedDeltaTime;

        if (timer <= 0)
        {
            _character.SetState(new IdleState(_character));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit HitState");
    }
}
