using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class GameState
    {
        public enum SceneType
        {
            Menu,
            Gameplay,
            Gameover,
            Victory
        }

        protected MainGame mainGame;
        public Scene CurrentScene {  get; set; }

        public GameState(MainGame pGame)
        {
            mainGame = pGame;
        }

        public void ChangeScene(SceneType pSceneType)
        {
            if (CurrentScene != null)
            {
                CurrentScene.UnLoad();
                CurrentScene = null;
            }

            switch (pSceneType)
            {
                case SceneType.Menu:
                    CurrentScene = new SceneMenu(mainGame);
                    break;
                case SceneType.Gameplay:
                    CurrentScene = new SceneGameplay(mainGame);
                    break;
                case SceneType.Gameover:
                    CurrentScene = new SceneGameover(mainGame);
                    break;
                case SceneType.Victory:
                    CurrentScene = new SceneVictory(mainGame);
                    break;
                default:
                    break;
            }
            CurrentScene.Load();
        }
    }
}
