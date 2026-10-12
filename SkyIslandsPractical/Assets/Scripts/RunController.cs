using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SirikanyaKawilawan
{
    public sealed class RunController : MonoBehaviour
    {
        public Transform player;
        public int stage = 1;
        public AudioClip winClip, loseClip;
        public RunState ActiveState { get; private set; } = new RunState();
        public event Action<string> ItemCollected;
        public float StartDelay { get; private set; } = 3f;
        bool ending;
        void Start() => SceneNavigation.RememberStage(stage);
        void Update()
        {
            if (player == null) return;
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) { SceneNavigation.Load("MainMenu"); return; }
            if (player.position.y < -8) EndFall();
            if (StartDelay > 0) { StartDelay = Mathf.Max(0, StartDelay - Time.deltaTime); return; }
            ActiveState.Tick(Time.deltaTime);
            if (!ending && ActiveState.Outcome != RunOutcome.Playing)
            {
                ending = true;
                AudioService.Instance?.PlayEffect(ActiveState.Outcome == RunOutcome.Won ? winClip : loseClip);
                Invoke(nameof(OpenResult), 0.45f);
            }
        }
        public bool CanMove => StartDelay <= 0 && ActiveState.Outcome == RunOutcome.Playing;
        public void EndFall() => ActiveState.Fall();
        public void NotifyItem(ItemKind kind)
        {
            string text = kind == ItemKind.ScoreBonus ? "+500 POINTS" : kind == ItemKind.ScorePenalty ? "-500 POINTS" : kind == ItemKind.TimeBonus ? "+3 SECONDS" : "-5 SECONDS";
            ItemCollected?.Invoke(text);
        }
        void OpenResult() => SceneNavigation.Finish(ActiveState.Outcome, ActiveState.Score);
    }
}
