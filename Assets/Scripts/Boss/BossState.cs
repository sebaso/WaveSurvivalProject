// BossState.cs
// Defines all possible states for the boss state machine.

public enum BossStateID
{
    Idle,
    Chase,
    MeleeAttack,
    SlamAttack,
    ThrowBoulder,
    GiveUp,
    Dead
}
