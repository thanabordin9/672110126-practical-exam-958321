using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StudentGame.Tests
{
    public class GameplayTests
    {
        static Type TypeOf(string name)
        {
            var t=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("StudentGame."+name)).FirstOrDefault(x=>x!=null);
            Assert.That(t,Is.Not.Null,name+" gameplay component is missing"); return t;
        }
        static object Prop(object o,string n)=>o.GetType().GetProperty(n).GetValue(o);
        static void Field(object o,string n,object v)=>o.GetType().GetField(n).SetValue(o,v);
        static object Call(object o,string n,params object[] a)=>o.GetType().GetMethod(n).Invoke(o,a);
        [UnityTest] public IEnumerator CollisionItemsAreSingleUseAndStoppedAfterGameOver()
        {
            var run=new GameObject("test run").AddComponent(TypeOf("RunController"));
            var player=new GameObject("Player");player.tag="Player";
            var item=new GameObject("item").AddComponent(TypeOf("ItemEffect"));
            Field(item,"kind",Enum.Parse(TypeOf("ItemKind"),"ScoreBonus"));
            Field(item,"run",run);
            Assert.That(Call(item,"TryCollect",player,false),Is.EqualTo(true));
            Assert.That(Call(item,"TryCollect",player,false),Is.EqualTo(false));
            Assert.That(Prop(Prop(run,"ActiveState"),"Score"),Is.EqualTo(500));
            Call(Prop(run,"ActiveState"),"Fall");
            var second=new GameObject("item2").AddComponent(TypeOf("ItemEffect")); Field(second,"run",run);
            Assert.That(Call(second,"TryCollect",player,false),Is.EqualTo(false));
            UnityEngine.Object.Destroy(run.gameObject);UnityEngine.Object.Destroy(player);UnityEngine.Object.Destroy(item.gameObject);UnityEngine.Object.Destroy(second.gameObject);
            yield return null;
        }
        [UnityTest] public IEnumerator InteractionRequiresExplicitActionAndOnlyOneConsumption()
        {
            var run=new GameObject("test run").AddComponent(TypeOf("RunController"));
            var player=new GameObject("Player");player.tag="Player";
            var item=new GameObject("interact item").AddComponent(TypeOf("ItemEffect"));
            Field(item,"kind",Enum.Parse(TypeOf("ItemKind"),"ScoreBonus")); Field(item,"run",run);Field(item,"interactionOnly",true);
            Assert.That(Call(item,"TryCollect",player,false),Is.EqualTo(false));
            Assert.That(Prop(Prop(run,"ActiveState"),"Score"),Is.EqualTo(0));
            Assert.That(Call(item,"TryCollect",player,true),Is.EqualTo(true));
            Assert.That(Call(item,"TryCollect",player,true),Is.EqualTo(false));
            Assert.That(Prop(Prop(run,"ActiveState"),"Score"),Is.EqualTo(500));
            UnityEngine.Object.Destroy(run.gameObject);UnityEngine.Object.Destroy(player);UnityEngine.Object.Destroy(item.gameObject);yield return null;
        }
        [UnityTest] public IEnumerator MusicMutePersistsAndDuplicateServiceDoesNotSurvive()
        {
            var key="SkyIslands.MusicVolume";var sfxKey="SkyIslands.EffectsVolume";
            bool had=PlayerPrefs.HasKey(key),hadSfx=PlayerPrefs.HasKey(sfxKey);float old=PlayerPrefs.GetFloat(key),oldSfx=PlayerPrefs.GetFloat(sfxKey);
            var t=TypeOf("AudioService");var a=new GameObject("audio").AddComponent(t);
            Call(a,"SetMusicVolume",0f);Call(a,"SetEffectsVolume",0.65f);
            UnityEngine.Object.Destroy(a.gameObject);yield return null;
            var b=new GameObject("audio reloaded").AddComponent(t);var c=new GameObject("audio duplicate").AddComponent(t);yield return null;
            Assert.That(Prop(b,"MusicVolume"),Is.EqualTo(0f));Assert.That(Prop(b,"EffectsVolume"),Is.EqualTo(0.65f).Within(0.001f));
            Assert.That(UnityEngine.Object.FindObjectsByType(t,FindObjectsSortMode.None).Length,Is.EqualTo(1));
            UnityEngine.Object.Destroy(b.gameObject);yield return null;
            if(had)PlayerPrefs.SetFloat(key,old);else PlayerPrefs.DeleteKey(key);
            if(hadSfx)PlayerPrefs.SetFloat(sfxKey,oldSfx);else PlayerPrefs.DeleteKey(sfxKey);
        }
    }
}
