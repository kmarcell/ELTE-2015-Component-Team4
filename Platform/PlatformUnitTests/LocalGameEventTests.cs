using System;
using System.Collections.Generic;
using System.IO;
using GTInterfacesLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platform.Model;

namespace PlatformUnitTests
{
    public partial class PlatformUnitTests
    {
        [TestMethod]
        public void StartLocalGameTest()
        {
            NetworkManager = new NetworkManager();
            GameManager = new GameManager(NetworkManager);

            var receivedEvents = new List<string>();
            ((GTPlatformManagerInterface)GameManager).SendGameStateChangedEvent += (sender, args) => receivedEvents.Add(args.ToString());
            GameManager.GameStartedEvent += (sender, args) => receivedEvents.Add(args.ToString());

            var testGame1 = GetFirstGame();
            GameManager.SetCurrentGame(testGame1);
            GameManager.StartLocalGame(null);

            const int eventNumberAtStart = 2;
            Assert.AreEqual(eventNumberAtStart, receivedEvents.Count, "Instead of {0} event were raised, there was: {1}", eventNumberAtStart, receivedEvents.Count);
        }

        [TestMethod]
        public void EndLocalGameTest()
        {
            NetworkManager = new NetworkManager();
            GameManager = new GameManager(NetworkManager);

            var receivedEvents = new List<string>();
            ((GTPlatformManagerInterface)GameManager).SendGameStateChangedEvent += (sender, args) => receivedEvents.Add(args.ToString());
            GameManager.GameStartedEvent += (sender, args) => receivedEvents.Add(args.ToString());
            
            var testGame1 = GetFirstGame();
            GameManager.SetCurrentGame(testGame1);
            GameManager.StartLocalGame(null);
            
            receivedEvents.Clear();
            GameManager.GameEndedEvent += (sender, args) => receivedEvents.Add(args.ToString());
            GameManager.EndLocalGame();

            const int eventNumberAtEnd = 2;
            Assert.AreEqual(eventNumberAtEnd, receivedEvents.Count, "Instead of {0} event were raised, there was: {1}", eventNumberAtEnd, receivedEvents.Count);
        }

        [TestMethod]
        public void SaveAndLoadLocalGameTest()
        {
            NetworkManager = new NetworkManager();
            GameManager = new GameManager(NetworkManager);

            var testGame1 = GetFirstGame();
            GameManager.SetCurrentGame(testGame1);

            const string fileName = "file1";

            try
            {
                var dataToSaveBeforeSave = testGame1.SaveGame();
                GameManager.SaveLocalGame(fileName);

                Assert.IsTrue(File.Exists(fileName), "File does not exists after save.");
                GameManager.LoadLocalGame(fileName);

                var loadedDataAfterLoad = testGame1.SaveGame();

                Assert.AreEqual(dataToSaveBeforeSave.Length, loadedDataAfterLoad.Length, "After save and load the data length is not equal.");
                for (var i = 0; i < dataToSaveBeforeSave.Length; i++)
                {
                    Assert.AreEqual(dataToSaveBeforeSave[i], loadedDataAfterLoad[i], "After save and load the data at position: {0} is not equal.", i);
                }
                
            }
            finally
            {
                if(File.Exists(fileName))
                    File.Delete(fileName);
            }
        }
    }
}
