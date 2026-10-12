using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentGame
{
    public static class SceneNavigation
    {
        public static int LastStage { get; private set; } = 1;
        public static int FinalScore { get; private set; }
        public static void Load(string scene)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            SceneManager.LoadScene(scene);
        }
        public static void StartStage(int stage) { LastStage = stage == 2 ? 2 : 1; Load("Stage" + LastStage); }
        public static void Retry() => StartStage(LastStage);
        public static void Finish(RunOutcome outcome, int score) { FinalScore = score; Load(outcome == RunOutcome.Won ? "Win" : "GameOver"); }
        public static void Quit() { Application.Quit(); }
    }
}
