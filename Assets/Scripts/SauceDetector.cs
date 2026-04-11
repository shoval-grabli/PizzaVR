using UnityEngine;

public class SauceDetector : MonoBehaviour
{
    // הצבע שהבצק יהפוך אליו כשהרוטב נוגע בו
    public Color sauceColor = Color.red;

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // אם האובייקט שנגע בנו הוא הרוטב
        if (collision.gameObject.CompareTag("Sauce"))
        {
            objectRenderer.material.color = sauceColor;
        }
    }
}