using GTInterfacesLibrary;
using GTInterfacesLibrary.MessageTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Platform.Model;

namespace PlatformUnitTests
{
    public partial class PlatformUnitTests
    {

        [TestMethod]
        public void MaintainCurrentGameTest()
        {
            NetworkManager = new NetworkManager();
            GameManager = new GameManager(NetworkManager);

            Assert.IsTrue(GameManager.CurrentGame == null, "The current game is not null, adfer creating new instance of Gamemanager.");

            const int firstGameId = 1;
            const string firstGameName = "1Game";
            const string firstGameDescription = "1GameDescription";
            var testGame1 = new TestGame(firstGameId, firstGameName, firstGameDescription);
            GameManager.SetCurrentGame(testGame1);
            Assert.IsTrue(GameManager.CurrentGame != null, "Set current game failed.");
            Assert.IsTrue(GameManager.CurrentGame.Id == firstGameId, "Set current game faild due to ID mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Name == firstGameName, "Set current game failed due to Name mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Description == firstGameDescription, "Set current game failed due ti Description mismatch.");


            const int secondGameId = 2;
            const string secondGameName = "2Game";
            const string secondGameDescription = "2GameDescription";
            var testGame2 = new TestGame(secondGameId, secondGameName, secondGameDescription);
            GameManager.SetCurrentGame(testGame2);
            Assert.IsTrue(GameManager.CurrentGame != null, "Set current game failed.");
            Assert.IsTrue(GameManager.CurrentGame.Id == secondGameId, "Set current game faild due to ID mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Name == secondGameName, "Set current game failed due to Name mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Description == secondGameDescription, "Set current game failed due ti Description mismatch.");



        }
    }
}
