using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
	#region fields

	//temp until refactor generation code
	[SerializeField] List<Transform> _rows = new List<Transform>();

	//caching
	[SerializeField] protected List<GridCell> _emptyCells;
	[SerializeField] protected List<GridCell> _fullCells;
	public List<GridCell> GetFullCells => _fullCells;
	public List<GridCell> GetEmptyCells => _emptyCells;

	#endregion

	#region initialization

	public void HackishInitialization()
	{
		ClearExistingCells();

		for (int i = 0; i < _rows.Count; i++)
		{
			//each row
			List<GridCell> currentRow = _rows[i].GetComponentsInChildren<GridCell>().ToList();
			List<GridCell> upperRow = i > 0 ? _rows[i - 1].GetComponentsInChildren<GridCell>().ToList() : null;
			List<GridCell> lowerRow = i + 1 < _rows.Count ? _rows[i + 1].GetComponentsInChildren<GridCell>().ToList() : null;

			for (int j = 0; j < currentRow.Count; j++)
			{
				//each cell
				currentRow[j].SetNeighbor(upperRow?[j], MoveDirection.Up);
				currentRow[j].SetNeighbor(lowerRow?[j], MoveDirection.Down);

				var leftN = j > 0 ? currentRow[j - 1] : null;
				currentRow[j].SetNeighbor(leftN, MoveDirection.Left);

				var rightN = j < currentRow.Count - 1 ? currentRow?[j + 1] : null;
				currentRow[j].SetNeighbor(rightN, MoveDirection.Right);
				currentRow[j].SetHandler(this);
				
				//cache the cell as empty
				_emptyCells.Add(currentRow[j]);
			}

		}
	}

	private void ClearExistingCells()
	{
		_emptyCells = new List<GridCell>();
		_fullCells = new List<GridCell>();
	}

	private void Awake()
	{
		foreach (var cell in _emptyCells)
		{
			cell.SetHandler(this);
		}
	}

	#endregion

	#region helpers

	public void AddMergeableItemToEmpty(MergableItem item)
	{
		var cell = _emptyCells.FirstOrDefault();
		if (cell != null)
		{
			item.AssignToCell(cell);
		}
	}

	public void ClearCell(GridCell cell)
	{
		if (_fullCells.Contains(cell))
		{
			_fullCells.Remove(cell);
			cell.ClearItem();
		}
		
		if (!_emptyCells.Contains(cell))
			_emptyCells.Add(cell);
	}


	public void SetCellState(GridCell cell, bool empty)
	{
		if (cell == null) return;
		if (empty)
		{
			if (_fullCells.Contains(cell))
			{
				_fullCells.Remove(cell);
			}

			if (_emptyCells.Contains(cell) == false)
			{
				_emptyCells.Add(cell);
			}
		}
		else
		{
			if (_emptyCells.Contains(cell))
			{
				_emptyCells.Remove(cell);
			}

			if (_fullCells.Contains(cell) == false)
			{
				_fullCells.Add(cell);
			}
		}
	}

	#endregion
}
