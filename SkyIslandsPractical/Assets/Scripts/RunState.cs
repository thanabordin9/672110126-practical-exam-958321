using System;

namespace SirikanyaKawilawan
{
    public enum ItemKind { TimeBonus, TimePenalty, ScoreBonus, ScorePenalty }
    public enum RunOutcome { Playing, Won, Lost }
    public sealed class RunState
    {
        public int Score { get; private set; }
        public float Remaining { get; private set; } = 30f;
        public RunOutcome Outcome { get; private set; } = RunOutcome.Playing;
        public void Tick(float seconds)
        {
            if (Outcome != RunOutcome.Playing || seconds < 0 || float.IsNaN(seconds) || float.IsInfinity(seconds)) return;
            Remaining = Math.Max(0, Remaining - seconds);
            ResolveTimeout();
        }
        public void Apply(ItemKind kind)
        {
            if (Outcome != RunOutcome.Playing) return;
            switch (kind)
            {
                case ItemKind.TimeBonus: Remaining += 3; break;
                case ItemKind.TimePenalty: Remaining = Math.Max(0, Remaining - 5); break;
                case ItemKind.ScoreBonus: Score += 500; break;
                case ItemKind.ScorePenalty: Score -= 500; break;
            }
            ResolveTimeout();
        }
        void ResolveTimeout() { if (Remaining <= 0) Outcome = Score > 10000 ? RunOutcome.Won : RunOutcome.Lost; }
        public void Fall() { if (Outcome == RunOutcome.Playing) Outcome = RunOutcome.Lost; }
        public void Reset() { Score = 0; Remaining = 30; Outcome = RunOutcome.Playing; }
    }
}
