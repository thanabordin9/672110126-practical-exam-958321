using System;
using System.Linq;
using NUnit.Framework;

namespace StudentGame.Tests
{
    public class RunStateTests
    {
        Type stateType;
        object state;
        [SetUp] public void Setup()
        {
            stateType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("StudentGame.RunState")).FirstOrDefault(t => t != null);
            Assert.That(stateType, Is.Not.Null, "The shared run model has not been implemented.");
            state = Activator.CreateInstance(stateType);
        }
        object Get(string name) => stateType.GetProperty(name).GetValue(state);
        void Call(string name, params object[] args) => stateType.GetMethod(name).Invoke(state, args);
        void Apply(string kind) => Call("Apply", Enum.Parse(stateType.Assembly.GetType("StudentGame.ItemKind"), kind));
        [Test] public void StartsWithThirtySecondsAndZeroScore() { Assert.That(Get("Remaining"), Is.EqualTo(30f)); Assert.That(Get("Score"), Is.EqualTo(0)); }
        [TestCase("TimeBonus",33f,0)] [TestCase("TimePenalty",25f,0)]
        [TestCase("ScoreBonus",30f,500)] [TestCase("ScorePenalty",30f,-500)]
        public void ItemUsesSpecifiedEffect(string item,float time,int score) { Apply(item); Assert.That(Get("Remaining"), Is.EqualTo(time)); Assert.That(Get("Score"), Is.EqualTo(score)); }
        [TestCase(20,"Lost")] [TestCase(21,"Won")]
        public void TimeoutUsesStrictScoreThreshold(int items,string outcome) { for(int i=0;i<items;i++) Apply("ScoreBonus"); Assert.That(Get("Outcome").ToString(),Is.EqualTo("Playing")); Call("Tick",30f); Assert.That(Get("Outcome").ToString(),Is.EqualTo(outcome)); }
        [Test] public void FallingLosesRegardlessOfScore() { for(int i=0;i<21;i++) Apply("ScoreBonus"); Call("Fall"); Assert.That(Get("Outcome").ToString(),Is.EqualTo("Lost")); }
        [Test] public void TimePenaltyCanEndRunAndLaterItemsAreIgnored() { Call("Tick",28f); Apply("TimePenalty"); Assert.That(Get("Remaining"),Is.EqualTo(0f)); Apply("ScoreBonus"); Assert.That(Get("Score"),Is.EqualTo(0)); Assert.That(Get("Outcome").ToString(),Is.EqualTo("Lost")); }
        [Test] public void ResetRestoresAllRunValues() { Apply("ScorePenalty"); Call("Fall"); Call("Reset"); Assert.That(Get("Score"),Is.EqualTo(0)); Assert.That(Get("Remaining"),Is.EqualTo(30f)); Assert.That(Get("Outcome").ToString(),Is.EqualTo("Playing")); }
        [Test] public void InvalidDeltaDoesNotCorruptRun() { Call("Tick",-3f); Call("Tick",float.NaN); Assert.That(Get("Remaining"),Is.EqualTo(30f)); }
    }
}
