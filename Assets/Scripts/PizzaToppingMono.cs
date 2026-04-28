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

}
