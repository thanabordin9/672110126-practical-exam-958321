using UnityEngine;
using StarterAssets;

namespace SirikanyaKawilawan
{
    [DefaultExecutionOrder(-100)]
    public sealed class StarterRunGate : MonoBehaviour
    {
        public RunController run;
        public ThirdPersonController motor;
        void Update() { if (motor != null && run != null) motor.enabled = run.CanMove; }
    }
}
