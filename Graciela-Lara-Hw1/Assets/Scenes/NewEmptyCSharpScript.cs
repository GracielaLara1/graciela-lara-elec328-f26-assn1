using UnityEngine;

public class ContinuousMove : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public string animationStateName = "Walk"; // Replace with your exact clip/state name

    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Play the walk animation if an Animator exists
        if (animator != null && !string.IsNullOrEmpty(animationStateName))
        {
            animator.Play(animationStateName);
        }
    }

    void Update()
    {
        // Move the deer forward continuously
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}