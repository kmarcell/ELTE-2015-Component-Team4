using ConnectionInterface;

namespace PlatformInterface
{
    public interface IDataManager
    {
        IGame CurrentGame { get; }

        void RegisterGame(IGame game);
    }
}
