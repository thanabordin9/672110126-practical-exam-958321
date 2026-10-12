using UnityEngine;
using UnityEngine.InputSystem;

namespace SirikanyaKawilawan
{
    public sealed class PlayerInteraction : MonoBehaviour
    {
        public float range = 2.25f;
        public RunController run;
        public ItemEffect Focused { get; private set; }
        readonly Collider[] hits = new Collider[64];
        void Update()
        {
            if (Focused != null) Focused.SetFocus(false);
            Focused = null;
            if (run == null || !run.CanMove) return;
            int count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up, range, hits, ~0, QueryTriggerInteraction.Collide);
            float best = range * range;
            for (int i = 0; i < count; i++)
            {
                var item = hits[i].GetComponentInParent<ItemEffect>();
                if (item == null || !item.interactionOnly || !item.gameObject.activeInHierarchy) continue;
                var delta = item.transform.position - (transform.position + Vector3.up);
                float distance = delta.sqrMagnitude;
                if (distance >= best || Vector3.Dot(transform.forward, delta.normalized) < -0.25f) continue;
                if (Physics.Linecast(transform.position + Vector3.up, item.transform.position, out var blocker, ~0, QueryTriggerInteraction.Ignore)
                    && blocker.transform != transform && !blocker.transform.IsChildOf(transform)) continue;
                best = distance; Focused = item;
            }
            if (Focused != null)
            {
                Focused.SetFocus(true);
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) Focused.TryCollect(gameObject, true);
            }
        }
        void OnDisable() { if (Focused != null) Focused.SetFocus(false); }
    }
}
