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
        String Name { get; }

        String Description { get; }

        Difficulty Difficulty { get; }
    }
}
