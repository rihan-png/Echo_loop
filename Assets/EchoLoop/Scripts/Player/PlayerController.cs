using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Fix wall sticking
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
            // Ground check (fallback to self position - offset if groundCheck is null)
            Vector2 checkPos = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position + Vector2.down * 0.55f;
            isGrounded = Physics2D.OverlapCircle(checkPos, 0.25f, groundLayer);

            // Handle Direct Keyboard fallback (A/D, Left/Right arrows, Space)
            float h = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;

                if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && isGrounded)
                {
                    PerformJump();
                }
            }

            // Handle Mobile Touch Controls
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
                {
                    Vector2 screenPos = touch.screenPosition;
                    float normX = screenPos.x / (float)Screen.width;

                    // Left 40% of screen = Move Left
                    if (normX < 0.4f)
                    {
                        h = -1f;
                    }
                    // Middle 40% to 75% of screen = Move Right
                    else if (normX >= 0.4f && normX < 0.75f)
                    {
                        h = 1f;
                    }
                    // Right 25% of screen = Jump
                    else if (normX >= 0.75f)
                    {
                        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began && isGrounded)
                        {
                            PerformJump();
                        }
                    }
                }
            }

            if (h != 0f)
            {
                moveInput = new Vector2(h, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            }
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (isGrounded)
            {
                PerformJump();
            }
        }

        private void PerformJump()
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (EchoLoop.Core.SoundManager.Instance != null)
                {
                    EchoLoop.Core.SoundManager.Instance.PlayJump();
                }
            }
        }
    }
}
