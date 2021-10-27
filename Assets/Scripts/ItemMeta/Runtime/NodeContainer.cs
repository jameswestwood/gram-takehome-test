using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GramGames.CraftingSystem.DataContainers
{
	[Serializable]
	public class NodeContainer : ScriptableObject
	{
		public NodeData MainNodeData;
		public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
		public List<NodeData> NodeData = new List<NodeData>();

		public Dictionary<NodeData, int> GetRecipe()
		{
			Dictionary<NodeData, int> ingredients = new Dictionary<NodeData, int>();

			foreach (var linkData in NodeLinks)
			{
				if (linkData.BaseNodeGUID == MainNodeData.NodeGUID)
				{
					// TODO: allow more than one of an ingredient from a config?  force all to 1 for now
					//ingredients.Add(NodeData.Single(nodeData => nodeData.NodeGUID == linkData.TargetNodeGUID), Int32.Parse(linkData.Count));
					ingredients.Add(NodeData.Single(nodeData => nodeData.NodeGUID == linkData.TargetNodeGUID), 1);
				}
			}

			return ingredients;
		}

		public bool IsRawMaterial()
		{
			return NodeLinks.Count == 0;
		}

		public bool IsAnyNode()
		{
			return MainNodeData.Type == CraftableItemTypes.ANY;
		}

		public CraftableItemTypes GetDesiredTypeOfAnyNode()
		{
			return MainNodeData.DesiredType;
		}

		public List<NodeContainer> GetExceptionsOfAnyNode()
		{
			return MainNodeData.Exceptions;
		}
	}
}