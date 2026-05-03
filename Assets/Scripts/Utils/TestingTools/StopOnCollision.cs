using Oculus.Interaction;
using UnityEngine;

public class StopOnCollision : MonoBehaviour
{
    [SerializeField] string tag;
    private void OnCollisionEnter(Collision collision)
    {
        DebugStop(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        DebugStop(other.gameObject);
    }

    void DebugStop(GameObject GO)
    {
        if (tag == string.Empty || tag == tag)
            Debug.Break();
    }
}
