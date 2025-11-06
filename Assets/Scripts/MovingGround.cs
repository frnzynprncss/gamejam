using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (IsOutOfScreen())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOutOfScreen()
    {
        float groundRightEdge = transform.position.x + (GetComponent<Renderer>().bounds.size.x / 2);

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(groundRightEdge, transform.position.y, transform.position.z));

        return screenPoint.x < -0.1f;
    }
}