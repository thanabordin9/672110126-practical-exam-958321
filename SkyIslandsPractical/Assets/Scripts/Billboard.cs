using UnityEngine;
namespace SirikanyaKawilawan
{
    public sealed class Billboard : MonoBehaviour
    {
        void LateUpdate() { if (Camera.main != null) transform.rotation = Camera.main.transform.rotation; }
    }
}
