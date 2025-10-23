using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator; // Animator của player
    public float moveSpeed = 5f;

    public GameObject alternatePrefab; // Prefab nhân vật biến hình
    private bool isTransformed = false;

    void Update()
    {
        HandleMovement();
        HandleAttack();
        HandleTransform();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        bool isMoving = movement.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("attack");
        }
    }

    void HandleTransform()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTransformed)
        {
            Transform currentPosition = transform;
            GameObject newPlayer = Instantiate(alternatePrefab, currentPosition.position, currentPosition.rotation);

            // Xóa nhân vật hiện tại khỏi Hierarchy
            Destroy(gameObject);

            isTransformed = true;
        }
    }
}