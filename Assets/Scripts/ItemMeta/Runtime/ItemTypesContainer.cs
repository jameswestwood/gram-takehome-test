using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[Serializable]
[CreateAssetMenu(fileName = "ItemTypeContainer", menuName = "CraftingSystem/EnumContainer")]
public class ItemTypesContainer : ScriptableObject
{
	public List<String> ItemTypes = new List<string> {"NONE", "ANY"};

	public void GenerateEnum()
	{
		string enumName = "CraftableItemTypes";

		if (!AssetDatabase.IsValidFolder("Assets/Scripts/Enums"))
			AssetDatabase.CreateFolder("Assets/Scripts", "Enums");

		string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs";

		using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
		{
			streamWriter.WriteLine("public enum " + enumName);
			streamWriter.WriteLine("{");
			for (int i = 0; i < ItemTypes.Count; i++)
			{
				streamWriter.WriteLine("\t" + ItemTypes[i] + ",");
			}
			streamWriter.WriteLine("}");
		}
		AssetDatabase.Refresh();
	}
}
