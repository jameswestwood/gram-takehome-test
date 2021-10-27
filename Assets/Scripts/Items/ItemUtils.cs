using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GramGames.CraftingSystem.DataContainers;
using UnityEngine;

public static class ItemUtils
{
	public static Dictionary<string, NodeContainer> ItemsMap = new Dictionary<string, NodeContainer>();
	public static Dictionary<string, HashSet<NodeData>> RecipeMap = new Dictionary<string, HashSet<NodeData>>();

	public static void InitializeMap()
	{
		var nodes = Resources.LoadAll<NodeContainer>("CraftingObjects");
		foreach (var node in nodes)
		{
			ItemsMap.Add(node.MainNodeData.NodeGUID, node);
			
			if (node.IsRawMaterial())
				continue;

			var ingredients = node.GetRecipe();
			
			if (RecipeMap.ContainsKey(node.MainNodeData.NodeGUID) == false)
			{
				var dt = new HashSet<NodeData>(ingredients.Keys);
				RecipeMap.Add(node.MainNodeData.NodeGUID, dt);
			}
			else
			{
				Debug.LogError($"Tried to add recipe for '{node.MainNodeData.Sprite.name}' more than once!");
			}
				
			// foreach (var recipe in node.GetRecipe())
			// {
			// 	string recipId = recipe.Key.NodeGUID;
			// 	if (RecipeMap.ContainsKey(recipId) == false)
			// 	{
			// 		var dt = new HashSet<NodeData>();
			// 		dt.Add(recipe.Key);
			// 		RecipeMap.Add(recipId, dt);
			// 	}
			// 	else
			// 	{
			// 		RecipeMap[recipId].Add(recipe.Key);
			// 	}
			// }
			//
		}

		Debug.Log($"recipes: {RecipeMap.Count}");
	}

	public static NodeContainer FindBestRecipe(GridCell cell, NodeContainer droppedItem)
	{
		string droppedNodeId = droppedItem.MainNodeData.NodeGUID;

		if (RecipeMap.ContainsKey(droppedNodeId))
		{
			var possibilities = RecipeMap[droppedNodeId];
			Debug.Log($"possibles recipes: {possibilities.Count}");
		}

		return null;
	}

	//return one recipe that have all its ingredient in a chain of cells

	public static NodeContainer FindBestRecipe(NodeContainer[] items)
	{
		int quantityFound = 0;

		foreach (var entries in RecipeMap)
		{
			if (items.Length != entries.Value.Count)
				continue;
			
			bool hasAllIngredient = true;
			foreach (var ingredient in entries.Value)
			{
				NodeContainer ingr = ItemsMap[ingredient.NodeGUID];
				if (items.Contains(ingr) == false)
				{
					hasAllIngredient = false;
					break;
				}
			}
			
			if (hasAllIngredient)
				return ItemsMap[entries.Key];
		}

		return null;
	}
}
