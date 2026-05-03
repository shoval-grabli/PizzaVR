
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PizzaTopping
{
    public PizzaIngredient toppingName;
}

public enum PizzaIngredient { Sauce, Cheese, Olives, Onion, Mushroom, Pepper}
public static class PizzaIngredientGroups
{
    public static readonly PizzaIngredient[][] pizzaIngredientsGroups = { Sauces, Toppings };
    public static readonly PizzaIngredient[] Sauces =
    {
        PizzaIngredient.Sauce,
        PizzaIngredient.Cheese
    };

    public static readonly PizzaIngredient[] Toppings =
    {
        PizzaIngredient.Olives,
        PizzaIngredient.Onion,
        PizzaIngredient.Mushroom,
        PizzaIngredient.Pepper
    }; 
}