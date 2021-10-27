
using System;
using System.Collections.Generic;
using System.Linq;
using GramGames.CraftingSystem.DataContainers;
using UnityEngine;
using UnityEngine.UI;

public class MixingGridHandler : GridHandler
{
	[SerializeField] private MergableItem _itemPrefab;
	[SerializeField] private GridHandler _fallbackMixItem;
	[SerializeField] private Button _mixbutton;

	private void Awake()
	{
		_mixbutton.onClick.AddListener(PressMixButton);
	}

	private void OnDestroy()
	{
		_mixbutton.onClick.RemoveListener(PressMixButton);
	}

	public void PressMixButton()
	{
		var mixItems = new Dictionary<GridCell, NodeContainer>();
		foreach (var cell in _fullCells)
		{
			if (cell.Item != null)
				mixItems[cell] = cell.Item.ItemData;
		}

		var result = ItemUtils.FindBestRecipe(mixItems.Values.ToArray());
		if (result != null)
		{
			var mixedItemNames = mixItems.Values.ToArray().Select(s => s.MainNodeData.Sprite.name).ToArray();
			Debug.Log($"mix success: [{string.Join(",", mixedItemNames)}] => {result.MainNodeData.Sprite.name}");
			
			// clear the mixed items
			for (int i = _fullCells.Count - 1; i >= 0; i--)
				ClearCell(_fullCells[i]);

			// create the new item from the recipe
			var instance = Instantiate(_itemPrefab);
			instance.Configure(result, _emptyCells[0]);
			// _fallbackMixItem.AddMergeableItemToEmpty(instance);
		}
		else
		{
			Debug.LogError("mix failed!");
		}
	}
}
