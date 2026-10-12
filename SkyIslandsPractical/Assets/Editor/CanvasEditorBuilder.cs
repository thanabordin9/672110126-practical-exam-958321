using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

namespace SirikanyaKawilawan.Editor
{
    public static class CanvasEditorBuilder
    {
        [MenuItem("Sky Islands/Create Editable Canvas UI")]
        public static void CreateEditableUi()
        {
            Directory.CreateDirectory("Assets/Prefabs/UI");Directory.CreateDirectory("Assets/Resources");AssetDatabase.Refresh();
            AssetDatabase.DeleteAsset("Assets/Resources/StudentFont.fontsettings");
            const string fontPath="Assets/Resources/StudentFont.ttf";
            var font=AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if(font==null)throw new System.InvalidOperationException("Missing packaged StudentFont.ttf and its OFL license.");
            foreach(var name in new[]{"MainMenu","StageSelection","Options","Credits","Stage1","Stage2","Win","GameOver"})
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity");
                var screen=Object.FindAnyObjectByType<SceneScreen>();var hud=Object.FindAnyObjectByType<GameHud>();RectTransform canvas;
                if(screen!=null){if(screen.uiCanvas==null)screen.CreateCanvas();screen.BindCanvas();canvas=screen.uiCanvas;}
                else {if(hud.uiCanvas==null)hud.CreateCanvas();canvas=hud.uiCanvas;}
                foreach(var text in canvas.GetComponentsInChildren<Text>(true))
                    if(text.font==null || !AssetDatabase.Contains(text.font))text.font=font;
                foreach(var button in canvas.GetComponentsInChildren<Button>(true))
                {
                    if(button.onClick.GetPersistentEventCount()>0)continue;
                    var action=button.GetComponent<SceneButtonAction>();
                    if(action!=null)UnityEventTools.AddPersistentListener(button.onClick,action.Invoke);
                    else UnityEventTools.AddPersistentListener(button.onClick,button.GetComponentInParent<StudentForm>().Save);
                    button.onClick.RemoveAllListeners();
                }
                foreach(var slider in canvas.GetComponentsInChildren<Slider>(true))
                {
                    if(slider.onValueChanged.GetPersistentEventCount()>0)continue;
                    UnityEventTools.AddPersistentListener(slider.onValueChanged,slider.GetComponent<VolumeControlBinding>().Apply);slider.onValueChanged.RemoveAllListeners();
                }
                PrefabUtility.SaveAsPrefabAssetAndConnect(canvas.gameObject,"Assets/Prefabs/UI/"+name+"Canvas.prefab",InteractionMode.AutomatedAction);
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("All eight scenes now contain editable Canvas prefab instances with saved On Click actions.");
        }
    }
}
