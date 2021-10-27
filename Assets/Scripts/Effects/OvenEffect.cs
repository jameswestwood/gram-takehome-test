using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenEffect : BaseEffect
{
    public override void ApplyEffect()
    {
        foreach (var cell in _grid.GetFullCells)
        {
            //cell.GetItem
        }
    }
}
