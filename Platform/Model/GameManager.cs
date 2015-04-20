using System;
using System.IO;
using PlatformInterface;
using PlatformInterface.EventsGameRelated;

namespace Platform.Model
{
    public class GameManager : IGameManager
    {
        private Boolean _MGameEnded;
        private Boolean _MIsWin;

        public event EventHandler<EventArgs>  GameStartedEvent;
        public event EventHandler<GameEndedEventArgs> GameEndedEvent;
        public event EventHandler<GameLoadedEventArgs> GameLoadedEvent;
        
        public void  StartGame()
        {
            _MGameEnded = false;
            _MIsWin = false;
            GameStartedEvent(this, EventArgs.Empty);
        }

        public void EndGame()
        {
            if(_MGameEnded)
            {
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = true, IsWin = _MIsWin });
            }
            else
            {
                GameEndedEvent(this, new GameEndedEventArgs { IsEnded = false, IsWin = false });
            }

        }

        public void SaveGame(String fileName)
        {
            if(string.IsNullOrEmpty(fileName))
                throw new Exception("Filename is empty.");

            var data = DataManager.CurrentGame.SaveGame();
            File.WriteAllBytes(fileName, data);
        }

        public void LoadGame(String fileName)
        {
            var data = File.ReadAllBytes(fileName);
            DataManager.CurrentGame.LoadGame(data);
            GameLoadedEvent(this, new GameLoadedEventArgs{ GameBytes = data });
        }
    }
}
