using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiveRabbitsDemo
{
    public class RabbitController : MonoBehaviour
    {
        [Header("Plane Boundaries (XZ Plane)")]
        [SerializeField] private Vector2 minBounds = new Vector2(-1f, -1f);
        [SerializeField] private Vector2 maxBounds = new Vector2(1f, 1f);
        [SerializeField] private float groundY = 0f; // Fixed height to lock movement to a plane

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float idleDurationMin = 1.5f;
        [SerializeField] private float idleDurationMax = 2f;

        [Header("Animator Param Values")]
        [SerializeField] private int idleAnimIndex = 0;
        [SerializeField] private int moveAnimIndex = 1;

        private Animator m_animator;
        private Vector3 m_targetPosition;
        private bool m_isMoving = false;

        void Start()
        {
            m_animator = GetComponent<Animator>();

            // Ensure rabbit starts strictly at plane Y height
            Vector3 startPos = transform.position;
            startPos.y = groundY;
            transform.position = startPos;

            StartCoroutine(WanderRoutine());
        }

        void Update()
        {
            if (m_isMoving)
            {
                MoveAndRotate();
            }

            // Lock position Y to keep strictly within plane (prevents root motion drift)
            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;
        }

        private IEnumerator WanderRoutine()
        {
            while (true)
            {
                // 1. Pause in Idle
                SetAnimation(idleAnimIndex);
                m_isMoving = false;
                float idleTime = Random.Range(idleDurationMin, idleDurationMax);
                yield return new WaitForSeconds(idleTime);

                // 2. Pick a new random point within plane bounds
                float randomX = Random.Range(minBounds.x, maxBounds.x);
                float randomZ = Random.Range(minBounds.y, maxBounds.y);
                m_targetPosition = new Vector3(randomX, groundY, randomZ);

                // 3. Switch to move/hop animation
                SetAnimation(moveAnimIndex);
                m_isMoving = true;

                // 4. Move until reaching destination
                while (Vector3.Distance(new Vector3(transform.position.x, groundY, transform.position.z), m_targetPosition) > 0.3f)
                {
                    yield return null;
                }

                m_isMoving = false;
            }
        }

        private void MoveAndRotate()
        {
            Vector3 direction = (m_targetPosition - transform.position);
            direction.y = 0f; // Stay horizontal

            if (direction.sqrMagnitude > 0.001f)
            {
                // Smoothly rotate towards movement target direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Translate forward along horizontal plane
                transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, moveSpeed * Time.deltaTime);
            }
        }

        private void SetAnimation(int animIndex)
        {
            if (m_animator != null)
            {
                m_animator.SetInteger("AnimIndex", animIndex);
                m_animator.SetTrigger("Next");
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize plane boundaries in Scene View
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, groundY, (minBounds.y + maxBounds.y) / 2f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
            Gizmos.DrawWireCube(center, size);
        }
    }
}