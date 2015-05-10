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

            var testGame1 = GetFirstGame();
            GameManager.SetCurrentGame(testGame1);
            Assert.IsTrue(GameManager.CurrentGame != null, "Set current game failed.");
            Assert.IsTrue(GameManager.CurrentGame.Id == testGame1.Id, "Set current game faild due to ID mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Name == testGame1.Name, "Set current game failed due to Name mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Description == testGame1.Description, "Set current game failed due ti Description mismatch.");

            var testGame2 = GetSecondGame();
            GameManager.SetCurrentGame(testGame2);
            Assert.IsTrue(GameManager.CurrentGame != null, "Set current game failed.");
            Assert.IsTrue(GameManager.CurrentGame.Id == testGame2.Id, "Set current game faild due to ID mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Name == testGame2.Name, "Set current game failed due to Name mismatch.");
            Assert.IsTrue(GameManager.CurrentGame.Description == testGame2.Description, "Set current game failed due ti Description mismatch.");
        }
    }
}
