using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace SirikanyaKawilawan.Tests
{
    public class EditableCanvasTests
    {
        [TestCase("MainMenu")][TestCase("StageSelection")][TestCase("Options")][TestCase("Credits")]
        [TestCase("Stage1")][TestCase("Stage2")][TestCase("Win")][TestCase("GameOver")]
        public void SceneContainsEditableCanvasAndSavedButtonActions(string name)
        {
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity");
            var canvases=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Canvas>(true)).ToArray();
            Assert.That(canvases.Length,Is.EqualTo(1),"UI must exist before Play and must not be duplicated.");
            var buttons=canvases[0].GetComponentsInChildren<Button>(true);Assert.That(buttons.Length,Is.GreaterThan(0));
            foreach(var button in buttons)Assert.That(button.onClick.GetPersistentEventCount(),Is.GreaterThan(0),button.name+" has no editable On Click action.");
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/"+name+"Canvas.prefab"),Is.Not.Null);
        }
    }
}
