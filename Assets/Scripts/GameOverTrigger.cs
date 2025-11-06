using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool destroyPlayerOnContact = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Game Over! Triggered by: {gameObject.name}");

            // Optional: Destroy player for visual effect
            if (destroyPlayerOnContact)
            {
                Destroy(collision.gameObject);
            }

            // Show game over screen
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGameOverScreen();
            }
        }
    }

    // Visual helper in Scene view
    void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red with transparency
            Gizmos.DrawCube(transform.position, collider.bounds.size);
        }
    }
}