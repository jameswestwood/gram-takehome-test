#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;

[CustomEditor(typeof(ItemTypesContainer))]
public class ItemTypesContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (ItemTypesContainer)target;
 
        if(GUILayout.Button("Generate Enum File", GUILayout.Height(40)))
        {
            script.GenerateEnum();
        }
         
    }
}
#endif