namespace VertigoCase.Core
{
    /// <summary>
    /// Every legal phase of a game run.
    /// </summary>
    public enum GameState
    {
        Idle,
        Spinning,
        BombHit,
        CashedOut
    }
}
