using GramGames.CraftingSystem.DataContainers;
using GramGames.CraftingSystem.Editor;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Graph : EditorWindow
{
	private GraphView _graphView;
	private string _fileName = "New File";
	private int _currentPickerWindowForPickItem;
	private int _currentPickerWindowForAddItem;

	[MenuItem("Graph/Crafting System Graph")]
	public static void OpenGraphWindow()
	{
		var window = GetWindow<Graph>();
		window.titleContent = new GUIContent("Crafting System Graph");
	}

	private void OnGUI()
	{
		if (Event.current.commandName == "ObjectSelectorClosed" &&
		    EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindowForPickItem)
		{
			var scriptableObject = EditorGUIUtility.GetObjectPickerObject();
			if (scriptableObject == null)
				return;
			_currentPickerWindowForPickItem = -1;
			GraphSaveUtility.GetInstance(_graphView).LoadGraph(scriptableObject.name);
		}
		else if (Event.current.commandName == "ObjectSelectorClosed" &&
		         EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindowForAddItem)
		{
			var scriptableObject = EditorGUIUtility.GetObjectPickerObject();
			if (scriptableObject == null)
				return;
			_currentPickerWindowForAddItem = -1;
			GraphSaveUtility.GetInstance(_graphView).AddGraph(scriptableObject.name);
		}
	}

	private void OnEnable()
	{
		ConstructGraphView();
		GenerateToolbar();
		GenerateMinimap();
	}

	private void OnDisable()
	{
		this.GetRootVisualContainer().Remove(_graphView);
	}

	private void ConstructGraphView()
	{
		_graphView = new GraphView
		{
			name = "Crafting System Graph"
		};

		_graphView.StretchToParentSize();
		this.GetRootVisualContainer().Add(_graphView);
	}

	private void GenerateToolbar()
	{
		var toolbar = new Toolbar();
		toolbar.style.height = 20;

		toolbar.Add(new Button(SaveOperation) {text = "Save Data"});
		toolbar.Add(new Button(SaveAll) {text = "Save All"});

		var nodeCreateButton = new Button(() => { _graphView.CreateNode("New Node"); });
		nodeCreateButton.text = "Create Node";
		toolbar.Add(nodeCreateButton);

		var anyNodeCreateButton = new Button(() => { _graphView.CreateAnyNode("New ANY Node"); });
		anyNodeCreateButton.text = "Create ANY Node";
		toolbar.Add(anyNodeCreateButton);

		var objectPickerButton = new Button(() =>
		{
			_currentPickerWindowForPickItem = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
			EditorGUIUtility.ShowObjectPicker<NodeContainer>(null, false, "", _currentPickerWindowForPickItem);
		});

		objectPickerButton.text = "Pick Item";
		toolbar.Add(objectPickerButton);

		var objectAddButton = new Button(() =>
		{
			_currentPickerWindowForAddItem = EditorGUIUtility.GetControlID(FocusType.Passive) + 200;
			EditorGUIUtility.ShowObjectPicker<NodeContainer>(null, false, "", _currentPickerWindowForAddItem);
		});

		objectAddButton.text = "Add Item";
		toolbar.Add(objectAddButton);

		var clearGraphButton = new Button(() => { GraphSaveUtility.GetInstance(_graphView).ClearWholeGraph(); });

		clearGraphButton.text = "Clear Graph";
		toolbar.Add(clearGraphButton);

		this.GetRootVisualContainer().Add(toolbar);
	}

	private void GenerateMinimap()
	{
		var miniMap = new MiniMap
		{
			anchored = true
		};
		miniMap.SetPosition(new Rect(10, 30, 300, 150));

		_graphView.Add(miniMap);
	}

	private void SaveOperation()
	{
		if (string.IsNullOrEmpty(_fileName))
		{
			EditorUtility.DisplayDialog("Invalid file!", "Please select a valid file.", "OK");
			return;
		}

		GraphSaveUtility.GetInstance(_graphView).SaveGraph();
	}

	private void SaveAll()
	{
		var nodes = Resources.LoadAll<NodeContainer>("CraftingObjects");
		foreach (var so in nodes)
		{
			EditorUtility.SetDirty(so);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

}
