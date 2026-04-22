using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PizzaPlacements : MonoBehaviour
{
    public static PizzaPlacements instance;
    [SerializeField] List<Transform> placements;
    int occupiedPlacements = 0;
    private void Start()
    {
        if (instance == null)
        instance = this;
        else
            Destroy(instance);
    }
    public Vector3 SubmitOrder()
    {
        if (occupiedPlacements == placements.Count)
        {
            Debug.Log("There aren't enough placements in the table");
            Debug.Break();
        }
        occupiedPlacements++;
        return placements[occupiedPlacements - 1].position;
    }
    public void OrderCompleted()
    {
        occupiedPlacements--;
    }
    public bool IsThereAPlacement()
    {
        return occupiedPlacements < placements.Count - 1;
    }
}
