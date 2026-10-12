using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using StarterAssets;

namespace SirikanyaKawilawan.Tests
{
    public class SceneConfigurationTests
    {
        [TestCase("MainMenu")] [TestCase("StageSelection")] [TestCase("Options")] [TestCase("Credits")]
        [TestCase("Stage1")] [TestCase("Stage2")] [TestCase("Win")] [TestCase("GameOver")]
        public void SceneIsRegisteredAndContainsCameraAndMusic(string name)
        {
            var path="Assets/Scenes/"+name+".unity";
            Assert.That(EditorBuildSettings.scenes.Any(s=>s.enabled && s.path==path),Is.True,"Missing build scene "+name);
            EditorSceneManager.OpenScene(path);
            Assert.That(Object.FindAnyObjectByType<Camera>(),Is.Not.Null);
            Assert.That(Object.FindObjectsByType<MeshFilter>().All(f=>f.sharedMesh!=null),Is.True,"Every saved scene mesh must survive reopening.");
            var service=Object.FindObjectsByType<MonoBehaviour>().FirstOrDefault(c=>c.GetType().Name=="AudioService");
            Assert.That(service,Is.Not.Null);
            Assert.That(service.GetType().GetField("musicClip").GetValue(service),Is.Not.Null);
        }
        [TestCase("Stage1")] [TestCase("Stage2")]
        public void StageUsesProBuilderAndEnoughPositiveScorePrefabs(string name)
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/"+name+".unity"),Is.Not.Null);
            EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity");
            Assert.That(Object.FindObjectsByType<ProBuilderMesh>().Length,Is.GreaterThan(0));
            var items=Object.FindObjectsByType<MonoBehaviour>().Where(c=>c.GetType().Name=="ItemEffect").ToArray();
            Assert.That(items.Count(c=>c.GetType().GetField("kind").GetValue(c).ToString()=="ScoreBonus"),Is.GreaterThanOrEqualTo(24));
            Assert.That(items.All(PrefabUtility.IsPartOfPrefabInstance),Is.True);
            if(name=="Stage2")
            {
                var floor=GameObject.Find("Arena 12 x 12 metres").GetComponent<Renderer>().bounds;
                Assert.That(floor.size.x*floor.size.z,Is.GreaterThanOrEqualTo(50));
                Assert.That(items.All(c=>(bool)c.GetType().GetField("interactionOnly").GetValue(c)),Is.True);
            }
        }
        [Test] public void HumanoidStageUsesStarterAssetsAndValidAvatar()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Stage2.unity"),Is.Not.Null);
            EditorSceneManager.OpenScene("Assets/Scenes/Stage2.unity");
            var motor=Object.FindAnyObjectByType<ThirdPersonController>();
            Assert.That(motor,Is.Not.Null);
            var animator=motor.GetComponent<Animator>();
            Assert.That(animator,Is.Not.Null);Assert.That(animator.avatar,Is.Not.Null);Assert.That(animator.avatar.isValid && animator.avatar.isHuman,Is.True);
            Assert.That(animator.runtimeAnimatorController,Is.Not.Null);
            Assert.That(animator.runtimeAnimatorController.animationClips.Length,Is.GreaterThanOrEqualTo(3));
        }
        [Test] public void HumanoidUsesProvidedCharacterTextures()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Stage2.unity");
            var materials=Object.FindAnyObjectByType<ThirdPersonController>().GetComponentsInChildren<SkinnedMeshRenderer>().SelectMany(r=>r.sharedMaterials).ToArray();
            Assert.That(materials.Any(m=>m!=null && m.mainTexture!=null),Is.True,string.Join(" / ",materials.Select(m=>m==null?"null":m.name+" texture="+(m.mainTexture==null?"NONE":m.mainTexture.name))));
        }
        [TearDown] public void Clear() { EditorSceneManager.NewScene(NewSceneSetup.EmptyScene); }
    }
}
