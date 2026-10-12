using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

namespace SirikanyaKawilawan
{
    public sealed class SceneScreen : MonoBehaviour
    {
        public RectTransform uiCanvas;
        void Start()
        {
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            if(uiCanvas==null)CreateCanvas();
            BindCanvas();
        }
        public void CreateCanvas()
        {
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            string scene = gameObject.scene.name;
            var canvas = UiKit.Canvas();
            uiCanvas=canvas;
            if (scene == "Options")
            {
                canvas.GetComponent<Canvas>().sortingOrder = 100;
                UiKit.Panel(canvas, "Options Overlay", 0, 0, 1600, 900, new Color(0.015f, 0.04f, 0.06f, 0.85f));
            }
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
                    UiKit.Button(panel.transform, "Options", 42, 480, 274, SceneNavigation.OpenOptions);
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
                    UiKit.Slider(panel.transform, "Background Music", 42, 310, AudioService.Instance != null ? AudioService.Instance.MusicVolume : 0.35f, v => AudioService.Instance?.SetMusicVolume(v));
                    UiKit.Slider(panel.transform, "Sound Effects", 42, 460, AudioService.Instance != null ? AudioService.Instance.EffectsVolume : 0.85f, v => AudioService.Instance?.SetEffectsVolume(v));
                    var okay = UiKit.Button(panel.transform, "OK / Save and Return", 42, 600, 565, SceneNavigation.CloseOptions, true);
                    if (UnityEngine.EventSystems.EventSystem.current != null) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(okay.gameObject);
                    break;
                case "Credits":
                    Heading(panel.transform, "BUILT FOR\nEXPLORATION", "Game Development 3 / Practical Project");
                    StudentDetailsPanel.IdentityText(panel.transform, "Student Credits", "Name: " + (StudentProfile.DisplayName.Length == 0 ? "Not entered" : StudentProfile.DisplayName) + "\nStudent Code: " + (StudentProfile.StudentCode.Length == 0 ? "Not entered" : StudentProfile.StudentCode) + "\nEnglish Name: " + StudentProfile.EnglishName + "   Section: " + StudentProfile.Section, 42, 300, 565, 110, 21);
                    UiKit.Text(panel.transform, "Credits", "Character and audio: Exam resources\nHumanoid movement: Unity / Starter Assets\nLevel geometry: Unity ProBuilder", 42, 425, 565, 145, 22, UiKit.Muted);
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
            var identity = StudentDetailsPanel.IdentityText(canvas, "Student Identity", StudentProfile.Summary, 108, 840, 1360, 35, 16);
            if (scene == "MainMenu") StudentDetailsPanel.Build(canvas, () => identity.text = StudentProfile.Summary);
        }
        public void BindCanvas()
        {
            string scene=gameObject.scene.name;
            var texts=uiCanvas.GetComponentsInChildren<Text>(true);
            var identity=texts.First(t=>t.name=="Student Identity");identity.text=StudentProfile.Summary;
            if(scene=="Credits")texts.First(t=>t.name=="Student Credits").text="Name: "+StudentProfile.DisplayName+"\nStudent Code: "+StudentProfile.StudentCode+"\nEnglish Name: "+StudentProfile.EnglishName+"   Section: "+StudentProfile.Section;
            if(scene=="Win"||scene=="GameOver")
            {
                texts.First(t=>t.name=="Final Score").text=SceneNavigation.FinalScore.ToString("N0");
                texts.First(t=>t.name=="Result Label").text="FINAL SCORE / STAGE "+SceneNavigation.LastStage;
            }
            foreach(var button in uiCanvas.GetComponentsInChildren<Button>(true))
            {
                if(button.name=="Save Student Details")continue;
                var action=button.GetComponent<SceneButtonAction>();
                if(action==null)
                {
                    action=button.gameObject.AddComponent<SceneButtonAction>();
                    switch(button.name)
                    {
                        case "Start Adventure":action.targetScene="StageSelection";break;
                        case "Credits":action.targetScene="Credits";break;
                        case "Back":case "Back to Main Menu":action.targetScene="MainMenu";break;
                        case "Options":action.command=SceneButtonAction.Command.OpenOptions;break;
                        case "OK / Save and Return":action.command=SceneButtonAction.Command.CloseOptions;break;
                        case "Try Again":action.command=SceneButtonAction.Command.Retry;break;
                        case "Exit":action.command=SceneButtonAction.Command.Quit;break;
                        case "01 / Capsule Sky Run":action.command=SceneButtonAction.Command.StartStage;action.stage=1;break;
                        case "02 / Humanoid Explorer":action.command=SceneButtonAction.Command.StartStage;action.stage=2;break;
                    }
                }
                button.onClick.RemoveAllListeners();if(button.onClick.GetPersistentEventCount()==0)button.onClick.AddListener(action.Invoke);
            }
            if(scene=="MainMenu")
            {
                var panel=uiCanvas.GetComponentsInChildren<RectTransform>(true).First(t=>t.name=="Student Details");
                var form=panel.GetComponent<StudentForm>();
                if(form==null)
                {
                    form=panel.gameObject.AddComponent<StudentForm>();var fields=panel.GetComponentsInChildren<InputField>();
                    form.nameInput=fields.First(f=>f.name=="Student Name Input");form.englishInput=fields.First(f=>f.name=="English Name Input");
                    form.codeInput=fields.First(f=>f.name=="Student Code Input");form.sectionInput=fields.First(f=>f.name=="Student Section Input");
                    form.status=texts.First(t=>t.name=="Student Save Status");form.identity=identity;
                }
                var save=panel.GetComponentsInChildren<Button>().First();save.onClick.RemoveAllListeners();if(save.onClick.GetPersistentEventCount()==0)save.onClick.AddListener(form.Save);
                form.Refresh();
            }
            if(scene=="Options")
            {
                foreach(var slider in uiCanvas.GetComponentsInChildren<Slider>())
                {
                    var binding=slider.GetComponent<VolumeControlBinding>();
                    if(binding==null){binding=slider.gameObject.AddComponent<VolumeControlBinding>();binding.slider=slider;binding.music=slider.name=="Background Music Slider";binding.percentage=texts.First(t=>t.name==(binding.music?"Background Music Value":"Sound Effects Value"));}
                    slider.onValueChanged.RemoveAllListeners();if(slider.onValueChanged.GetPersistentEventCount()==0)slider.onValueChanged.AddListener(binding.Apply);
                }
                if(UnityEngine.EventSystems.EventSystem.current!=null)UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(uiCanvas.GetComponentsInChildren<Button>().First(b=>b.name=="OK / Save and Return").gameObject);
            }
        }
        static void Heading(Transform panel, string title, string description)
        {
            UiKit.Text(panel, "Title", title, 42, 42, 565, 178, 60, Color.white, true);
            UiKit.Text(panel, "Description", description, 42, 230, 565, 58, 22, UiKit.Muted);
        }
        static void Back(Transform panel) => UiKit.Button(panel, "Back", 42, 600, 565, () => SceneNavigation.Load("MainMenu"));
    }
}
