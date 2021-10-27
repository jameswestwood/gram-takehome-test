using System;

namespace GramGames.CraftingSystem.DataContainers
{
	[Serializable]
	public class NodeLinkData
	{
		public string BaseNodeGUID;
		public string PortName;
		public string TargetNodeGUID;
		public string Count;
	}
}