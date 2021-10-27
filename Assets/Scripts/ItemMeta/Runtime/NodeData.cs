using System;
using System.Collections.Generic;
using UnityEngine;

namespace GramGames.CraftingSystem.DataContainers
{
	[Serializable]
	public class NodeData
	{
		public string NodeGUID;
		public string NodeText;
		public Vector2 Position;
		public Sprite Sprite;
		public CraftableItemTypes Type;
		public List<NodeContainer> Exceptions = new List<NodeContainer>();
		public CraftableItemTypes DesiredType;
		public string FlavorText;
	}
}