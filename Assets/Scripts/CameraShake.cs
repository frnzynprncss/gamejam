using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Shake Settings")]
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.7f;
    public float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private bool isShaking = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            if (shakeDuration > 0)
            {
                transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                shakeDuration = 0f;
                transform.localPosition = initialPosition;
                isShaking = false;
            }
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        isShaking = true;
    }

    // Call this when player expands springs or hits obstacles
    public void ShakeOnSpringExpand()
    {
        Shake(0.3f, 0.1f);
    }

    // Call this when player hits obstacles
    public void ShakeOnObstacleHit()
    {
        Shake(0.5f, 0.2f);
    }
}