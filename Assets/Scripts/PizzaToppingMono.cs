using UnityEngine;

public class PizzaToppingMono : MonoBehaviour
{
    public PizzaTopping topping;
    //remove and use ReturnToOriginAuto
    Vector3 startingPos;
    Quaternion startingRot;

    private void Awake()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
    }
    public void ReturnToOrigin()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = startingPos;
        transform.rotation = startingRot;
        if (rb != null) rb.isKinematic = false;
    }

}
