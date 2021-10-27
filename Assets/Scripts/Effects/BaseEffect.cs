using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    [SerializeField] protected GridHandler _grid;
    
    public virtual void ApplyEffect()
    {
    }
}
