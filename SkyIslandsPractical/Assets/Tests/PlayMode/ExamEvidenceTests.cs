using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SirikanyaKawilawan.Tests
{
    public sealed class ExamAudioCapture : MonoBehaviour
    {
        public static FileStream Stream;
        public static StreamWriter Index;
        public static long Origin;
        public static int Channels = 2;
        public static readonly object Gate = new object();
        void OnAudioFilterRead(float[] data, int channels)
        {
            lock (Gate)
            {
                if (Stream == null) return;
                Channels = channels;
                var bytes = new byte[data.Length * 4];
                Index?.WriteLine(((System.Diagnostics.Stopwatch.GetTimestamp()-Origin)/(double)System.Diagnostics.Stopwatch.Frequency).ToString("F6",System.Globalization.CultureInfo.InvariantCulture)+"\t"+Stream.Position+"\t"+data.Length+"\t"+channels);
                Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length); Stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
    public sealed class ExamEvidenceTests
    {
        Keyboard keyboard; Mouse mouse;
        InputSettings.BackgroundBehavior background;
        InputSettings.EditorInputBehaviorInPlayMode editorInput;
        RenderTexture render; Texture2D pixels;
        StreamWriter index, chapters;
        int frame; double started; string folder;
        float music, effects;
        void Keys(params Key[] keys) => InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
        static Button Button(string name) => Object.FindObjectsByType<Button>().First(b => b.name == name);
        void Chapter(string text) { chapters.WriteLine((Time.realtimeSinceStartupAsDouble-started).ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\t"+text);chapters.Flush(); }
        IEnumerator Watch(float seconds)
        {
            double end=Time.realtimeSinceStartupAsDouble+seconds;
            while(Time.realtimeSinceStartupAsDouble<end)
            {
                yield return null;
                var camera=Camera.main;
                if(camera==null) continue;
                if(camera.GetComponent<ExamAudioCapture>()==null)camera.gameObject.AddComponent<ExamAudioCapture>();
                var canvases=Object.FindObjectsByType<Canvas>();
                foreach(var c in canvases){c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=camera;c.planeDistance=0.5f;}
                camera.targetTexture=render;Canvas.ForceUpdateCanvases();camera.Render();
                var previous=RenderTexture.active;RenderTexture.active=render;
                pixels.ReadPixels(new Rect(0,0,1280,720),0,0);pixels.Apply();
                string name="frame-"+(frame++).ToString("D6")+".jpg";File.WriteAllBytes(Path.Combine(folder,name),pixels.EncodeToJPG(88));
                index.WriteLine((Time.realtimeSinceStartupAsDouble-started).ToString("F6",System.Globalization.CultureInfo.InvariantCulture)+"\t"+name);index.Flush();
                camera.targetTexture=null;RenderTexture.active=previous;
                foreach(var c in canvases)if(c!=null)c.renderMode=RenderMode.ScreenSpaceOverlay;
                yield return new WaitForSecondsRealtime(0.075f);
            }
        }
        IEnumerator Tap(Key key) {Keys(key);yield return Watch(0.12f);Keys();yield return Watch(0.15f);}
        [UnityTest, Explicit("Records an actual engine run for exam evidence; invoke by test filter only.")]
        public IEnumerator RecordExamEvidence()
        {
            background=InputSystem.settings.backgroundBehavior;editorInput=InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            keyboard=InputSystem.AddDevice<Keyboard>();mouse=InputSystem.AddDevice<Mouse>();
            folder=Path.GetFullPath("../Submission/RecordedFrames");Directory.CreateDirectory(folder);
            render=new RenderTexture(1280,720,24);pixels=new Texture2D(1280,720,TextureFormat.RGB24,false);
            index=new StreamWriter(Path.Combine(folder,"frames.tsv"));chapters=new StreamWriter(Path.Combine(folder,"chapters.tsv"));
            ExamAudioCapture.Stream=new FileStream(Path.Combine(folder,"audio.f32"),FileMode.Create);
            ExamAudioCapture.Index=new StreamWriter(Path.Combine(folder,"audio.tsv"));ExamAudioCapture.Origin=System.Diagnostics.Stopwatch.GetTimestamp();
            started=Time.realtimeSinceStartupAsDouble;
            SceneNavigation.Load("MainMenu");yield return null;yield return null;
            music=AudioService.Instance.MusicVolume;effects=AudioService.Instance.EffectsVolume;
            AudioService.Instance.SetMusicVolume(0.35f);AudioService.Instance.SetEffectsVolume(0.85f);
            Chapter("Q1 Q10 MainMenu and student identity");yield return Watch(2);
            Button("Options").onClick.Invoke();yield return null;yield return null;Chapter("Q1 Q9 additive Options and volume control");yield return Watch(2);
            Object.FindObjectsByType<Slider>().First(s=>s.name=="Background Music Slider").value=0;yield return Watch(2);
            Object.FindObjectsByType<Slider>().First(s=>s.name=="Background Music Slider").value=0.5f;yield return Watch(2);
            Object.FindObjectsByType<Slider>().First(s=>s.name=="Sound Effects Slider").value=0.85f;
            Button("OK / Save and Return").onClick.Invoke();yield return null;yield return null;
            Button("Credits").onClick.Invoke();yield return null;yield return null;Chapter("Q1 Credits identity");yield return Watch(2);
            Button("Back").onClick.Invoke();yield return null;yield return null;
            Button("Start Adventure").onClick.Invoke();yield return null;yield return null;Chapter("Q1 StageSelection");yield return Watch(2);
            Button("01 / Capsule Sky Run").onClick.Invoke();yield return null;yield return null;Chapter("Q2 Q3 Stage1 capsule and sky route");yield return Watch(3.4f);
            var run=Object.FindAnyObjectByType<RunController>();
            Keys(Key.W);yield return Watch(7.7f);Keys();
            Assert.That(run.ActiveState.Score,Is.GreaterThan(10000));Assert.That(run.ActiveState.Outcome,Is.EqualTo(RunOutcome.Playing));
            Chapter("Q4 Q5 Q11 collected score and time bonuses; wait for natural timeout");
            while(SceneManager.GetActiveScene().name=="Stage1")yield return Watch(0.4f);
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("Win"));Chapter("Q1 Q5 Win after natural timeout");yield return Watch(3);
            Button("Back to Main Menu").onClick.Invoke();yield return null;yield return null;
            SceneNavigation.StartStage(1);yield return null;yield return null;yield return Watch(3.4f);
            Chapter("Q4 Q12 Stage1 four item effects and obstacle collision");
            Keys(Key.W);yield return Watch(1.8f);Keys();yield return Watch(0.2f);
            Keys(Key.D);yield return Watch(0.23f);Keys();yield return Watch(0.4f);
            Keys(Key.A);yield return Watch(0.47f);Keys();yield return Watch(0.4f);
            Keys(Key.S);yield return Watch(0.12f);Keys();yield return Watch(0.3f);
            Keys(Key.W);yield return Watch(0.32f);Keys();yield return Watch(0.4f);
            Keys(Key.A);yield return Watch(1.2f);Keys();yield return Watch(2.5f);
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("GameOver"));Chapter("Q1 Q5 falling causes GameOver");yield return Watch(3);
            Button("Try Again").onClick.Invoke();yield return null;yield return null;Chapter("Q1 Q5 Retry resets timer and score");yield return Watch(3.5f);
            Button("Back to Stage Selection").onClick.Invoke();yield return null;yield return null;yield return Watch(1);
            Button("02 / Humanoid Explorer").onClick.Invoke();yield return null;yield return null;
            run=Object.FindAnyObjectByType<RunController>();var input=run.player.GetComponent<PlayerInput>();input.SwitchCurrentControlScheme("KeyboardMouse",keyboard,mouse);
            Chapter("Q6 Q7 Stage2 humanoid arena");yield return Watch(3.5f);
            Keys(Key.W);yield return Watch(0.6f);Keys();yield return Watch(0.5f);
            Assert.That(run.ActiveState.Score,Is.EqualTo(0));Chapter("Q8 collision alone does not collect; E interaction");
            yield return Tap(Key.E);yield return Watch(1);
            Assert.That(run.ActiveState.Score,Is.GreaterThan(0));
            Keys(Key.D);yield return Watch(0.5f);Keys();yield return Tap(Key.E);
            Keys(Key.W);yield return Watch(0.6f);Keys();yield return Watch(0.5f);yield return Tap(Key.E);yield return Tap(Key.Space);
            Chapter("Q6 Q8 Q11 Q12 movement interaction jump and obstacles");yield return Watch(2);
            while(SceneManager.GetActiveScene().name=="Stage2")yield return Watch(0.4f);
            Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("GameOver"));Chapter("Q5 Stage2 natural timeout below target");yield return Watch(3);
            Button("Back to Main Menu").onClick.Invoke();yield return null;yield return null;Chapter("Q1 return MainMenu");yield return Watch(2);
        }
        [TearDown] public void Cleanup()
        {
            if(keyboard!=null)InputSystem.RemoveDevice(keyboard);if(mouse!=null)InputSystem.RemoveDevice(mouse);
            InputSystem.settings.backgroundBehavior=background;InputSystem.settings.editorInputBehaviorInPlayMode=editorInput;
            if(AudioService.Instance!=null){AudioService.Instance.SetMusicVolume(music);AudioService.Instance.SetEffectsVolume(effects);}
            lock(ExamAudioCapture.Gate){ExamAudioCapture.Stream?.Dispose();ExamAudioCapture.Stream=null;ExamAudioCapture.Index?.Dispose();ExamAudioCapture.Index=null;}
            index?.Dispose();chapters?.Dispose();
            if(folder!=null)File.WriteAllText(Path.Combine(folder,"audio-format.txt"),AudioSettings.outputSampleRate+" "+ExamAudioCapture.Channels);
            if(render!=null){render.Release();Object.Destroy(render);}if(pixels!=null)Object.Destroy(pixels);
        }
    }
}
