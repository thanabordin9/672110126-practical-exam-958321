using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using StarterAssets;

namespace StudentGame.Tests
{
    public class ScenePlayTests
    {
        Keyboard keys;
        Mouse mouse;
        InputSettings.BackgroundBehavior previousBackground;
        InputSettings.EditorInputBehaviorInPlayMode previousEditorInput;
        [SetUp] public void SetupInput()
        {
            previousBackground=InputSystem.settings.backgroundBehavior;
            previousEditorInput=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        static Button Button(string text)=>Object.FindObjectsByType<Button>().First(b=>b.name==text);
        void SetKeys(params Key[] values) { InputSystem.QueueStateEvent(keys,new KeyboardState(values)); }
        IEnumerator Capture(string name)
        {
            if(SystemInfo.graphicsDeviceType==UnityEngine.Rendering.GraphicsDeviceType.Null) yield break;
            var camera=Camera.main;var canvas=Object.FindAnyObjectByType<Canvas>();
            var render=new RenderTexture(1600,900,24);camera.targetTexture=render;
            if(canvas!=null){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=0.5f;}
            yield return null;Canvas.ForceUpdateCanvases();camera.Render();
            var previous=RenderTexture.active;RenderTexture.active=render;
            var texture=new Texture2D(1600,900,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,1600,900),0,0);texture.Apply();
            Directory.CreateDirectory("Verification/Screenshots");File.WriteAllBytes("Verification/Screenshots/"+name+".png",texture.EncodeToPNG());
            RenderTexture.active=previous;camera.targetTexture=null;if(canvas!=null)canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            render.Release();Object.Destroy(render);Object.Destroy(texture);
        }
        [UnityTest] public IEnumerator MenusOptionsResultsAndRetryConnectCorrectly()
        {
            SceneNavigation.Load("MainMenu");yield return null;yield return null;
            Assert.That(Button("Start Adventure"),Is.Not.Null);yield return Capture("MainMenu");
            Button("Options").onClick.Invoke();yield return null;yield return null;
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Options"));
            var sliders=Object.FindObjectsByType<Slider>();Assert.That(sliders.Length,Is.EqualTo(2));
            float oldMusic=AudioService.Instance.MusicVolume,oldEffects=AudioService.Instance.EffectsVolume;
            sliders.First(s=>s.name=="Background Music Slider").value=0;sliders.First(s=>s.name=="Sound Effects Slider").value=0.65f;
            Assert.That(AudioService.Instance.MusicVolume,Is.EqualTo(0));Assert.That(AudioService.Instance.EffectsVolume,Is.EqualTo(0.65f).Within(0.01));
            yield return Capture("Options");Button("OK / Save and Return").onClick.Invoke();yield return null;yield return null;
            Assert.That(AudioService.Instance.MusicVolume,Is.EqualTo(0));AudioService.Instance.SetMusicVolume(oldMusic);AudioService.Instance.SetEffectsVolume(oldEffects);
            Button("Credits").onClick.Invoke();yield return null;yield return null;Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Credits"));Button("Back").onClick.Invoke();yield return null;yield return null;
            Button("Start Adventure").onClick.Invoke();yield return null;yield return null;yield return Capture("StageSelection");
            Button("02 / Humanoid Explorer").onClick.Invoke();yield return null;yield return null;
            var run=Object.FindAnyObjectByType<RunController>();Assert.That(run.stage,Is.EqualTo(2));yield return Capture("Stage2");
            run.EndFall();yield return new WaitForSeconds(3.8f);Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("GameOver"));yield return Capture("GameOver");
            Button("Try Again").onClick.Invoke();yield return null;yield return null;
            run=Object.FindAnyObjectByType<RunController>();Assert.That(run.stage,Is.EqualTo(2));Assert.That(run.ActiveState.Score,Is.EqualTo(0));Assert.That(run.ActiveState.Remaining,Is.EqualTo(30f));
            Assert.That(Object.FindObjectsByType<ItemEffect>().Count(i=>i.kind==ItemKind.ScoreBonus),Is.GreaterThanOrEqualTo(24));
            SceneNavigation.Finish(RunOutcome.Won,10500);yield return null;yield return null;Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Win"));yield return Capture("Win");
            Button("Back to Main Menu").onClick.Invoke();yield return null;yield return null;
            Assert.That(Object.FindObjectsByType<AudioService>().Length,Is.EqualTo(1));
        }
        [UnityTest] public IEnumerator CapsuleWalksAndCollectsAlongTheSkyRoute()
        {
            SceneNavigation.StartStage(1);yield return null;yield return new WaitForSeconds(3.3f);
            var run=Object.FindAnyObjectByType<RunController>();var start=run.player.position;
            Assert.That(run.CanMove,Is.True,"Start delay remaining: "+run.StartDelay);
            keys=InputSystem.AddDevice<Keyboard>();SetKeys(Key.W);yield return new WaitForSeconds(0.8f);SetKeys();
            Assert.That(run.player.position.z,Is.GreaterThan(start.z+3),"W held: "+keys.wKey.isPressed+" / delay "+run.StartDelay);Assert.That(run.ActiveState.Score,Is.GreaterThan(0));
            Assert.That(run.player.position.y,Is.GreaterThan(-0.2f));yield return Capture("Stage1");
            SetKeys(Key.W);yield return new WaitForSeconds(7.2f);SetKeys();
            Assert.That(run.ActiveState.Score,Is.GreaterThan(10000));Assert.That(run.ActiveState.Outcome,Is.EqualTo(RunOutcome.Playing));
            run.ActiveState.Tick(100);yield return new WaitForSeconds(0.7f);Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Win"));
        }
        [UnityTest] public IEnumerator HumanoidMovesAndECollectsWithoutCollisionPickup()
        {
            SceneNavigation.StartStage(2);yield return null;yield return new WaitForSeconds(3.3f);
            var run=Object.FindAnyObjectByType<RunController>();var start=run.player.position;
            Assert.That(run.CanMove,Is.True,"Start delay remaining: "+run.StartDelay);
            keys=InputSystem.AddDevice<Keyboard>();mouse=InputSystem.AddDevice<Mouse>();
            var input=run.player.GetComponent<PlayerInput>();input.SwitchCurrentControlScheme("KeyboardMouse",keys,mouse);
            SetKeys(Key.W);yield return new WaitForSeconds(0.6f);SetKeys();
            Assert.That(run.player.position.z,Is.GreaterThan(start.z+1),"Move input: "+run.player.GetComponent<StarterAssetsInputs>().move+" / W held "+keys.wKey.isPressed);Assert.That(run.ActiveState.Score,Is.EqualTo(0));
            Assert.That(run.player.GetComponent<ThirdPersonController>().Grounded,Is.True);
            var item=Object.FindObjectsByType<ItemEffect>().First(i=>i.kind==ItemKind.ScoreBonus);
            var controller=run.player.GetComponent<CharacterController>();controller.enabled=false;run.player.position=item.transform.position+new Vector3(0,-0.9f,-0.9f);run.player.rotation=Quaternion.identity;controller.enabled=true;Physics.SyncTransforms();
            yield return null;yield return null;
            var interaction=run.player.GetComponent<PlayerInteraction>();Assert.That(interaction.Focused,Is.Not.Null);Assert.That(run.ActiveState.Score,Is.EqualTo(0));
            SetKeys(Key.E);yield return null;SetKeys();yield return null;
            Assert.That(run.ActiveState.Score,Is.EqualTo(500));yield return Capture("Stage2Interaction");
        }
        [UnityTearDown] public IEnumerator Cleanup()
        {
            if(keys!=null){InputSystem.RemoveDevice(keys);keys=null;}if(mouse!=null){InputSystem.RemoveDevice(mouse);mouse=null;}
            InputSystem.settings.backgroundBehavior=previousBackground;
            InputSystem.settings.editorInputBehaviorInPlayMode=previousEditorInput;
            SceneNavigation.Load("MainMenu");yield return null;yield return null;
            foreach(var audio in Object.FindObjectsByType<AudioService>())Object.Destroy(audio.gameObject);
            yield return null;
        }
    }
}
