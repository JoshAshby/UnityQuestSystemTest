using System;
using UnityEngine;

public class aaDefaultFact : ScriptableObject
{
    [SerializeField]
    string Key;

    [SerializeField]
    object Value;

    [SerializeField]
    Type Type;
}