using UnityEngine;

public class PizzaTrigger : MonoBehaviour
{
    [SerializeField] Pizza pizza;
    private void Start()
    {
        if(pizza == null)
        {
            Debug.Log("pizza is null");
            Debug.Break();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        pizza.InspectTopping(other.gameObject);
    }
}
