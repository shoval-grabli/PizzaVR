using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyDictionary<TKey,TValue> : ISerializationCallbackReceiver
{
    public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in dictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }
}