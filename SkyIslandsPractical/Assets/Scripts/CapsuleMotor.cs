using UnityEngine;
using UnityEngine.InputSystem;

namespace SirikanyaKawilawan
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CapsuleMotor : MonoBehaviour
    {
        public RunController run;
        public Transform cameraTransform;
        public float speed = 8.5f, sprintSpeed = 11f;
        CharacterController controller;
        float vertical;
        void Awake() => controller = GetComponent<CharacterController>();
        void Update()
        {
            var keys = Keyboard.current;
            if (run == null || !run.CanMove || keys == null) return;
            Vector2 input = new Vector2((keys.dKey.isPressed ? 1 : 0) - (keys.aKey.isPressed ? 1 : 0), (keys.wKey.isPressed ? 1 : 0) - (keys.sKey.isPressed ? 1 : 0));
            input = Vector2.ClampMagnitude(input, 1);
            float yaw = cameraTransform == null ? 0 : cameraTransform.eulerAngles.y;
            Vector3 direction = Quaternion.Euler(0, yaw, 0) * new Vector3(input.x, 0, input.y);
            if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 14);
            if (controller.isGrounded && vertical < 0) vertical = -2;
            if (controller.isGrounded && keys.spaceKey.wasPressedThisFrame) vertical = 8;
            vertical += -22 * Time.deltaTime;
            controller.Move((direction * (keys.leftShiftKey.isPressed ? sprintSpeed : speed) + Vector3.up * vertical) * Time.deltaTime);
        }
    }
}
