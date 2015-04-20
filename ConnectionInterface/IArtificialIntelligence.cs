using System;

namespace ConnectionInterface
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public interface IArtificialIntelligence
    {
        Int32 GetHashCode();

        Difficulty GetDifficulty();
    }
}
