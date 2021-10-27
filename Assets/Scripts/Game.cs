using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
	public MergableItem DraggableObjectPrefab;
	public GridHandler MainGrid;

	private List<string> ActiveRecipes = new List<string>();

	private void Awake()
	{
		Screen.fullScreen =
			false; // https://issuetracker.unity3d.com/issues/game-is-not-built-in-windowed-mode-when-changing-the-build-settings-from-exclusive-fullscreen

		// load all item definitions
		ItemUtils.InitializeMap();
	}

	private void Start()
	{
		ReloadLevel(1);
	}

	public void ReloadLevel(int difficulty = 1)
	{
		// clear the board
		var fullCells = MainGrid.GetFullCells.ToArray();
		for (int i = fullCells.Length - 1; i >= 0; i--)
			MainGrid.ClearCell(fullCells[i]);

		// choose new recipes
		ActiveRecipes.Clear();
		difficulty = Mathf.Max(difficulty, 1);
		for (int i = 0; i < difficulty; i++)
		{
			// a 'recipe' has more than 1 ingredient, else it is just a raw ingredient.
			var recipe = ItemUtils.RecipeMap.FirstOrDefault(kvp => kvp.Value.Count > 1).Key;
			ActiveRecipes.Add(recipe);
		}

		// populate the board
		var emptyCells = MainGrid.GetEmptyCells.ToArray();
		foreach (var cell in emptyCells)
		{
			var chosenRecipe = ActiveRecipes[0];
			var ingredients = ItemUtils.RecipeMap[chosenRecipe].ToArray();
			var ingredient = ingredients[0];
			var item = ItemUtils.ItemsMap[ingredient.NodeGUID];
			cell.SpawnItem(item);
		}
	}
}
