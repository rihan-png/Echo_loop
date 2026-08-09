using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoLoop.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public Transform groundCheck;
        public LayerMask groundLayer;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private bool isGrounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Fix the "wall sticking" bug by removing friction from the collider
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                PhysicsMaterial2D frictionless = new PhysicsMaterial2D("Frictionless");
                frictionless.friction = 0f;
                frictionless.bounciness = 0f;
                col.sharedMaterial = frictionless;
            }
        }

        private void Update()
        {
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            }
        }

        private void FixedUpdate()
        {
            if (rb != null)
            {
                // In Unity 6, linearVelocity is the updated property
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            }
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private void OnJump(InputValue value)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (EchoLoop.Core.SoundManager.Instance != null)
                    EchoLoop.Core.SoundManager.Instance.PlayJump();
            }
        }
    }
}
