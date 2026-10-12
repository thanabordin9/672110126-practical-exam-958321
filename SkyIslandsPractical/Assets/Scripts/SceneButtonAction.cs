using UnityEngine;

namespace SirikanyaKawilawan
{
    public sealed class SceneButtonAction : MonoBehaviour
    {
        public enum Command { LoadScene, StartStage, OpenOptions, CloseOptions, Retry, Quit }
        public Command command;
        public string targetScene = "MainMenu";
        public int stage = 1;
        public void Invoke()
        {
            switch(command)
            {
                case Command.LoadScene: SceneNavigation.Load(targetScene);break;
                case Command.StartStage: SceneNavigation.StartStage(stage);break;
                case Command.OpenOptions: SceneNavigation.OpenOptions();break;
                case Command.CloseOptions: SceneNavigation.CloseOptions();break;
                case Command.Retry: SceneNavigation.Retry();break;
                case Command.Quit: SceneNavigation.Quit();break;
            }
        }
    }
}
