using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudentGame
{
    public sealed class SceneScreen : MonoBehaviour
    {
        void Start()
        {
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            string scene = SceneManager.GetActiveScene().name;
            var canvas = UiKit.Canvas();
            UiKit.Text(canvas, "Brand", "SKY / ISLANDS", 92, 32, 450, 46, 26, Color.white, true);
            UiKit.Text(canvas, "Scene Label", scene.ToUpperInvariant(), 1240, 32, 280, 46, 18, UiKit.Accent, true);
            var panel = UiKit.Panel(canvas, "Menu Panel", 90, 130, 650, 685, UiKit.Ink);
            UiKit.Panel(panel.transform, "Accent", 0, 0, 6, 685, UiKit.Accent);
            switch (scene)
            {
                case "MainMenu":
                    UiKit.Text(panel.transform, "Eyebrow", "A 3D TIME ATTACK ADVENTURE", 42, 28, 550, 40, 18, UiKit.Accent, true);
                    UiKit.Text(panel.transform, "Title", "SKY\nISLANDS", 38, 82, 570, 205, 82, Color.white, true);
                    UiKit.Text(panel.transform, "Description", "Collect. Explore. Beat the clock.\nTwo challenges above the clouds.", 42, 290, 555, 82, 25, UiKit.Muted);
                    UiKit.Button(panel.transform, "Start Adventure", 42, 405, 565, () => SceneNavigation.Load("StageSelection"), true);
                    UiKit.Button(panel.transform, "Options", 42, 480, 274, () => SceneNavigation.Load("Options"));
                    UiKit.Button(panel.transform, "Credits", 331, 480, 276, () => SceneNavigation.Load("Credits"));
                    UiKit.Button(panel.transform, "Exit", 42, 555, 565, SceneNavigation.Quit);
                    break;
                case "StageSelection":
                    Heading(panel.transform, "CHOOSE\nYOUR STAGE", "Two ways to chase a high score.");
                    UiKit.Button(panel.transform, "01 / Capsule Sky Run", 42, 300, 565, () => SceneNavigation.StartStage(1), true);
                    UiKit.Text(panel.transform, "Stage1 Detail", "Collect on contact. Cross narrow sky bridges.", 42, 373, 565, 62, 21, UiKit.Muted);
                    UiKit.Button(panel.transform, "02 / Humanoid Explorer", 42, 453, 565, () => SceneNavigation.StartStage(2), true);
                    UiKit.Text(panel.transform, "Stage2 Detail", "Get close. Press E. Explore the floating arena.", 42, 526, 565, 62, 21, UiKit.Muted);
                    Back(panel.transform); break;
                case "Options":
                    Heading(panel.transform, "MAKE IT\nYOUR OWN", "Independent music and sound effect controls.");
                    UiKit.Slider(panel.transform, "Background Music", 42, 310, AudioService.Instance.MusicVolume, AudioService.Instance.SetMusicVolume);
                    UiKit.Slider(panel.transform, "Sound Effects", 42, 460, AudioService.Instance.EffectsVolume, AudioService.Instance.SetEffectsVolume);
                    UiKit.Button(panel.transform, "OK / Save and Return", 42, 600, 565, () => SceneNavigation.Load("MainMenu"), true); break;
                case "Credits":
                    Heading(panel.transform, "BUILT FOR\nEXPLORATION", "Game Development 3 / Practical Project");
                    UiKit.Text(panel.transform, "Credits", "Character, dances and audio:\nProvided practical examination resources\n\nHumanoid movement and locomotion:\nUnity Technologies / Starter Assets\n\nLevel geometry: Unity ProBuilder", 42, 300, 565, 285, 23, UiKit.Muted);
                    Back(panel.transform); break;
                case "Win": case "GameOver":
                    bool won = scene == "Win";
                    Heading(panel.transform, won ? "ABOVE\nAND BEYOND" : "ONE MORE\nADVENTURE?", won ? "WIN / Challenge completed at timeout." : "GAME OVER / The sky awaits another try.");
                    UiKit.Text(panel.transform, "Result Label", "FINAL SCORE / STAGE " + SceneNavigation.LastStage, 42, 305, 560, 40, 20, UiKit.Accent, true);
                    UiKit.Text(panel.transform, "Final Score", SceneNavigation.FinalScore.ToString("N0"), 42, 355, 560, 95, 74, Color.white, true);
                    UiKit.Text(panel.transform, "Result Rule", "Target: more than 10,000 points before time expires.\nFalling from an island always ends the run.", 42, 465, 565, 64, 21, UiKit.Muted);
                    if (!won) UiKit.Button(panel.transform, "Try Again", 42, 540, 565, SceneNavigation.Retry, true);
                    UiKit.Button(panel.transform, "Back to Main Menu", 42, won ? 575 : 612, 565, () => SceneNavigation.Load("MainMenu"), won); break;
            }
            var info = UiKit.Panel(canvas, "Adventure Rules", 1020, 610, 490, 205, UiKit.Ink);
            UiKit.Text(info.transform, "Clock Rule", "30 SECONDS. ONE CHALLENGE.", 25, 18, 450, 42, 23, UiKit.Accent, true);
            UiKit.Text(info.transform, "Rules", "Earn more than 10,000 points.\nGold items buy you extra time.\nWatch your step above the clouds.", 25, 68, 450, 112, 23, Color.white);
            UiKit.Panel(canvas, "Footer Background", 90, 836, 1420, 44, UiKit.Ink);
            UiKit.Text(canvas, "Footer", "WASD MOVE  /  SHIFT SPRINT  /  SPACE JUMP  /  E INTERACT IN STAGE 2", 108, 840, 1360, 35, 16, UiKit.Muted);
        }
        static void Heading(Transform panel, string title, string description)
        {
            UiKit.Text(panel, "Title", title, 42, 42, 565, 178, 60, Color.white, true);
            UiKit.Text(panel, "Description", description, 42, 230, 565, 58, 22, UiKit.Muted);
        }
        static void Back(Transform panel) => UiKit.Button(panel, "Back", 42, 600, 565, () => SceneNavigation.Load("MainMenu"));
    }
}
