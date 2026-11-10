using UnityEngine;
using UnityEngine.InputSystem;

// Attach this script to Player.
// Requires an Animator and a Rigidbody2D on the same GameObject.
// Set the Rigidbody2D's Gravity Scale to 0 and freeze its Z rotation.
public class PlayerMovement : MonoBehaviour
{
    // Movement speed in world units per second.
    // SerializeField keeps this field private while allowing
    // its value to be edited and saved through the Inspector.
    [SerializeField] private float moveSpeed = 3f;

    // References to the components used for animation and physics.
    private Animator animator;
    private Rigidbody2D rb;

    // Combines the arrow keys into one movement action.
    private InputAction moveAction;

    // Unity calls Awake before OnEnable and Start.
    // Initialize the components and action before they are used.
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Create a Value action that returns directional input.
        moveAction = new InputAction(
            name: "Move",
            type: InputActionType.Value
        );

        // Combine four buttons into a Vector2:
        // X represents horizontal input.
        // Y represents vertical input.
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    // Enable input whenever this component becomes active.
    void OnEnable()
    {
        moveAction.Enable();
    }

    // Stop reading input and stop the player when this
    // component or its GameObject is disabled.
    void OnDisable()
    {
        moveAction.Disable();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("IsMoving", false);
    }

    // Release the action's resources when this object is destroyed.
    void OnDestroy()
    {
        moveAction.Dispose();
    }

    // Unity calls Update once per frame.
    void Update()
    {
        // Read the combined input from the Move action.
        // With no keys pressed, the result is Vector2.zero.
        // Opposite keys on the same axis cancel each other.
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convert the input into one of four cardinal directions.
        // Horizontal input takes priority over vertical input,
        // preventing diagonal movement.
        Vector2 direction = Vector2.zero;

        if (input.x < 0f)
            direction = Vector2.left;
        else if (input.x > 0f)
            direction = Vector2.right;
        else if (input.y > 0f)
            direction = Vector2.up;
        else if (input.y < 0f)
            direction = Vector2.down;

        // Set velocity so physics handles movement and collisions.
        // Each chosen direction has a length of 1, keeping the
        // movement speed consistent in all four directions.
        // Velocity is already measured in units per second,
        // so Time.deltaTime is not needed here.
        rb.linearVelocity = direction * moveSpeed;

        // Tell the Animator whether movement input is active.
        animator.SetBool("IsMoving", direction != Vector2.zero);

        // Update the facing direction:
        // 0 = Down, 1 = Up, 2 = Left, 3 = Right.
        // When input stops, keep the last facing direction
        // so the correct idle animation remains active.
        if (direction == Vector2.down)
            animator.SetInteger("Direction", 0);
        else if (direction == Vector2.up)
            animator.SetInteger("Direction", 1);
        else if (direction == Vector2.left)
            animator.SetInteger("Direction", 2);
        else if (direction == Vector2.right)
            animator.SetInteger("Direction", 3);
    }
}