using System;
using System.IO;

namespace Platform.Model
{
    public class GameManager
    {

        public void SaveGame(String fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                //TODO: get current gamestate
                writer.WriteLine();
                writer.Close();
            }
        }

        public void LoadGame(String fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                reader.ReadToEnd();
                // TODO: send current gamestate
                reader.Close();
            }
        }
    }
}
