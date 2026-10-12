using UnityEngine;

namespace SirikanyaKawilawan
{
    public sealed class ItemEffect : MonoBehaviour
    {
        public ItemKind kind;
        public bool interactionOnly;
        public RunController run;
        public AudioClip pickupClip;
        public Transform visual;
        public string Label => kind == ItemKind.ScoreBonus ? "+500" : kind == ItemKind.ScorePenalty ? "-500" : kind == ItemKind.TimeBonus ? "+3s" : "-5s";
        bool consumed;
        Vector3 baseScale, basePosition;
        bool focused;
        void Start() { if (run == null) run = FindAnyObjectByType<RunController>(); if (visual != null) { baseScale = visual.localScale; basePosition = visual.localPosition; } }
        void Update()
        {
            if (visual == null) return;
            visual.Rotate(0, 65 * Time.deltaTime, 0, Space.Self);
            visual.localPosition = basePosition + Vector3.up * (Mathf.Sin(Time.time * 2.4f + transform.position.z) * 0.10f);
            visual.localScale = baseScale * (focused ? 1.22f : 1f);
        }
        public void SetFocus(bool value) => focused = value;
        void OnTriggerEnter(Collider other) => TryCollect(other.gameObject, false);
        public bool TryCollect(GameObject collector, bool explicitInteraction = false)
        {
            if (consumed || collector == null || !collector.CompareTag("Player") || (interactionOnly && !explicitInteraction)) return false;
            if (run == null) run = FindAnyObjectByType<RunController>();
            if (run == null || run.ActiveState.Outcome != RunOutcome.Playing) return false;
            consumed = true; run.ActiveState.Apply(kind); run.NotifyItem(kind);
            AudioService.Instance?.PlayEffect(pickupClip);
            gameObject.SetActive(false); return true;
        }
    }
}
