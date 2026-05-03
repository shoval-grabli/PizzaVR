using UnityEngine;

public class ToppingTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform originPoint;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // בדיקה: האם זו פיצה?
        bool isPizza = collision.gameObject.CompareTag("Pizza") || 
                       collision.gameObject.name.Contains("Pizza");

        if (!isPizza)
        {
            // ❌ פגע במשהו אחר → טלפורט חזרה!
            TeleportBack();
        }
        // אם זו פיצה → PizzaLayerReveal על הפיצה יטפל בזה
    }

    private void TeleportBack()
    {
        if (originPoint != null)
        {
            transform.position = originPoint.position;
            transform.rotation = originPoint.rotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}