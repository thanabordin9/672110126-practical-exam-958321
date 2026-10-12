using UnityEngine;

namespace SirikanyaKawilawan
{
    public sealed class PlayerContact : MonoBehaviour
    {
        public AudioClip obstacleClip;
        float nextSound;
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.GetComponent<Obstacle>() == null || Time.time < nextSound) return;
            nextSound = Time.time + 0.7f; AudioService.Instance?.PlayEffect(obstacleClip);
        }
    }
}
