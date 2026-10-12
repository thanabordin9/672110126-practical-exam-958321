using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SirikanyaKawilawan
{
    public static class SceneNavigation
    {
        public static int LastStage { get; private set; } = 1;
        public static int FinalScore { get; private set; }
        static bool openingOptions, closingOptions;
        static readonly List<CanvasGroup> menuGroups = new List<CanvasGroup>();
        static GameObject menuSelection;
        static void RestoreMenu()
        {
            foreach (var group in menuGroups) if (group != null) { group.interactable = true; group.blocksRaycasts = true; }
            menuGroups.Clear();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(menuSelection);
            menuSelection = null;
        }
        public static void Load(string scene)
        {
            SceneManager.sceneLoaded -= OptionsLoaded; openingOptions = closingOptions = false;
            RestoreMenu();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
            SceneManager.LoadScene(scene);
        }
        public static void StartStage(int stage) { LastStage = stage == 2 ? 2 : 1; Load("Stage" + LastStage); }
        public static void Retry() => StartStage(LastStage);
        public static void RememberStage(int stage) => LastStage = stage == 2 ? 2 : 1;
        public static void OpenOptions()
        {
            if (openingOptions || closingOptions || SceneManager.GetSceneByName("Options").isLoaded) return;
            menuSelection = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            foreach (var canvas in Object.FindObjectsByType<Canvas>())
            {
                if (canvas.gameObject.scene.name != "MainMenu") continue;
                var group = canvas.GetComponent<CanvasGroup>();
                if (group == null) group = canvas.gameObject.AddComponent<CanvasGroup>();
                group.interactable = false; group.blocksRaycasts = false; menuGroups.Add(group);
            }
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            openingOptions = true; SceneManager.sceneLoaded += OptionsLoaded;
            SceneManager.LoadScene("Options", LoadSceneMode.Additive);
        }
        static void OptionsLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Options") return;
            SceneManager.sceneLoaded -= OptionsLoaded; openingOptions = false;
            SceneManager.SetActiveScene(scene);
            foreach (var root in scene.GetRootGameObjects())
                if (root.GetComponent<SceneScreen>() == null) root.SetActive(false);
        }
        public static void CloseOptions()
        {
            var menu = SceneManager.GetSceneByName("MainMenu");
            if (!menu.isLoaded) { Load("MainMenu"); return; }
            if (closingOptions) return;
            SceneManager.SetActiveScene(menu);
            var options = SceneManager.GetSceneByName("Options");
            if (!options.isLoaded) return;
            closingOptions = true;
            var operation = SceneManager.UnloadSceneAsync(options);
            if (operation == null) { closingOptions = false; RestoreMenu(); }
            else operation.completed += _ => { closingOptions = false; RestoreMenu(); };
        }
        public static void Finish(RunOutcome outcome, int score) { FinalScore = score; Load(outcome == RunOutcome.Won ? "Win" : "GameOver"); }
        public static void Quit() { Application.Quit(); }
    }
}
