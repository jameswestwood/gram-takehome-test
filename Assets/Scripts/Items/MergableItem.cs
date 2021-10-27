using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GramGames.CraftingSystem.DataContainers;
using UnityEngine;
using Random = UnityEngine.Random;

public class MergableItem : DraggableObject
{
	public SpriteRenderer spriteRenderer;
	
    private SpriteRenderer _sprite;
    private GridCell _parentCell;
    private RayCastHandler<GridCell> _rayCast;

    private NodeContainer _itemData;	// this is our item definition
    public NodeContainer ItemData => _itemData;
    
    private void Awake()
    {
	    _rayCast = new RayCastHandler<GridCell>(transform);
    }

    /// <summary>
    /// Should be called immediately after instantiation!
    /// </summary>
    public void Configure(NodeContainer item, GridCell current)
    {
	    _itemData = item;
	    
	    if (_itemData != null)
	        spriteRenderer.sprite = _itemData.MainNodeData.Sprite;
        else
	        spriteRenderer.sprite = null;
	    
	    AssignToCell(current);
    }
    
    public void AssignToCell(GridCell current)
    {
        _parentCell = current;
        transform.SetParent(current.transform);
        transform.position = current.transform.position;
        current.SetItemAssigned(this);
    }

    protected override void DoBeginDrag()
    {
    }

    public LayerMask mask;
    protected override void DoEndDrag()
    {
        if (_rayCast.RayCastDown(mask))
        {
            //we hit a slot
            var targets = _rayCast.GetTargets();
            foreach (var slot in targets)
            {
                if (slot.HasItem() == false)
                {
                    Debug.Log("end");
                    AssignToCell(slot);
                    return;
                }
            }
            // TODO: else what do we do?
        }
        else
        {
            //return to previous slot
            if (_parentCell != null)
            {
	            Debug.Log("off grid");
                AssignToCell(_parentCell);
            }
        }
    }

    public GridCell GetCurrentCell()
    {
        return _parentCell;
    }
}
