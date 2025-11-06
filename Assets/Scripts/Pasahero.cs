using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pasahero : MonoBehaviour
{
    public LayerMask Jeepmask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;

        if (otherObject.CompareTag("Player") || otherObject.CompareTag("Wheel"))
        {
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin();
            }
            Destroy(gameObject);
        }
    }

}
