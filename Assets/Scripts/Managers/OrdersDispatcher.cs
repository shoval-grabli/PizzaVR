using System.Collections.Generic;
using UnityEngine;

public class OrdersDispatcher : MonoBehaviour
{
    public static OrdersDispatcher instance;
    [Header("Prefabs")]
    [SerializeField] PizzaOrder orderPF;
    [SerializeField] OrderUI orderUIPF;
    [SerializeField] Pizza pizzaPF;

    [Space]
    [SerializeField] Transform OrdersUIGroup;

    [Header("Settings")]
    [SerializeField] bool ImmediateDispatchAfterPack;

    List<PizzaOrder> orders = new List<PizzaOrder>();

    private void Start()
    {
        instance = this;
    }
    public void SubmitOrder(PizzaOrder order)
    {
        if(!orders.Exists(o => o == order))
            orders.Add(order);
    }
    public void PizzaPacked(PizzaOrder order)
    {
        orders.Remove(order);
        Destroy(order.pizza.gameObject);
        Destroy(order.orderUI.gameObject);
        Destroy(order.gameObject);
        if (ImmediateDispatchAfterPack)
            DispatchPizza();
    }

    void DispatchPizza()
    {
        PizzaOrder order = Instantiate(orderPF);
        OrderUI orderUI = Instantiate(orderUIPF, OrdersUIGroup);
        Vector3 pizzaPos = PizzaPlacements.instance.SubmitOrder();
        Pizza pizza = Instantiate(pizzaPF, pizzaPos, Quaternion.identity);
        order.pizza = pizza;
        order.orderUI = orderUI;
        orders.Add(order);
    }
}
