using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ToppingBox : MonoBehaviour
{
    [SerializeField] PizzaIngredient ingredient;
    public PizzaIngredient Ingredient {  get { return ingredient; } }
    public Dictionary<PizzaToppingMono, Transform> toppingsPositions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToppingBoxesManager.instance.Register(this);
    }

}

class ToppingBoxesManager
{
    public static ToppingBoxesManager instance;
    Dictionary<PizzaIngredient, ToppingBox> toppingsBoxes;

    public void Register(ToppingBox box)
    {
        toppingsBoxes.Add(box.Ingredient, box);
    }
}
