using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class aaDefaultBlackboard : ScriptableObject
{
    [SerializeField]
    string Hint;

    [SerializeField]
    List<aaDefaultFact> DefaultFacts = new List<aaDefaultFact>();
}