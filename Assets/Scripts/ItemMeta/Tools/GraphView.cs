using System;
using System.Collections.Generic;
using System.Linq;
using GramGames.CraftingSystem.DataContainers;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class GraphView : UnityEditor.Experimental.UIElements.GraphView.GraphView
{
    
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public readonly Vector2 defaultNodePosition = new Vector2(10, 30);
    public GraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    private Port GeneratePort(GraphNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateGraphNode(nodeName));
    }

    public GraphNode CreateGraphNode(string nodeName)
    {
        var graphNode = new GraphNode
        {
            title = nodeName,
            NodeText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(graphNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        graphNode.inputContainer.Add(inputPort);
        
        var button = new Button(() =>
        {
            AddPort(graphNode);
        });
        button.text = "New Ingredient";
        graphNode.titleContainer.Add(button);
        
        var textField = new TextField();
        textField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            graphNode.NodeText = evt.newValue;
            graphNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(graphNode.title);
        graphNode.mainContainer.Add(textField);

        var objectField = new ObjectField {objectType = typeof(Sprite)};
        graphNode.mainContainer.Add(objectField);

        var dropDown = new EnumField(CraftableItemTypes.NONE)
        {
            tooltip = "Select Item Type"
        };
        graphNode.mainContainer.Add(dropDown);
        
        var flavorTextField = new TextField
        {
            tooltip = "Enter flavor text of the item.(Max length 128)",
            multiline = true,
            maxLength = 128
        };
        graphNode.mainContainer.Add(flavorTextField);
        
        graphNode.RefreshExpandedState();
        graphNode.RefreshPorts();
        
        graphNode.SetPosition(new Rect(defaultNodePosition, defaultNodeSize));

        return graphNode;
    }
    
    public void CreateAnyNode(string nodeName)
    {
        AddElement(CreateAnyGraphNode(nodeName));
    }
    
    public GraphNode CreateAnyGraphNode(string nodeName)
    {
        var graphNode = new GraphNode
        {
            title = nodeName,
            NodeText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        graphNode.titleContainer.style.backgroundColor = new Color(0.5f, 0, 0.5f, 1);
        
        var inputPort = GeneratePort(graphNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        graphNode.inputContainer.Add(inputPort);
        
        var button = new Button(() =>
        {
            AddException(graphNode);
        });
        button.text = "New Exception";
        graphNode.titleContainer.Add(button);
        var buttonDelete = new Button(() =>
        {
            RemoveException(graphNode);
        });
        buttonDelete.text = "Delete Last Exception";
        graphNode.titleContainer.Add(buttonDelete);

        var textField = new TextField();
        textField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            graphNode.NodeText = evt.newValue;
            graphNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(graphNode.title);
        graphNode.mainContainer.Add(textField);
        
        var objectField = new ObjectField {objectType = typeof(Sprite)};
        graphNode.mainContainer.Add(objectField);
        
        var dropDown = new EnumField(CraftableItemTypes.ANY)
        {
            tooltip = "Select Item Type"
        };
        graphNode.mainContainer.Add(dropDown);
        
        var dropDownException = new EnumField(CraftableItemTypes.NONE)
        {
            tooltip = "Select Item Type That Will be Counted"
        };
        graphNode.mainContainer.Add(dropDownException);

        var flavorTextField = new TextField
        {
            tooltip = "Enter flavor text of the item.(Max length 128)",
            multiline = true,
            maxLength = 128
        };
        graphNode.mainContainer.Add(flavorTextField);
        
        graphNode.RefreshExpandedState();
        graphNode.RefreshPorts();
        
        graphNode.SetPosition(new Rect(defaultNodePosition, defaultNodeSize));

        return graphNode;
    }

    public void AddPort(GraphNode graphNode)
    {
        var generatedPort = GeneratePort(graphNode, Direction.Output);

        //var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        //generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = graphNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Ingredient {outputPortCount}";

        var choicePortName = $"Ingredient {outputPortCount + 1}";

        var textField = new TextField
        {
            name = "CountTextField",
            value = "1",
            tooltip = "Enter Amount"
        };
        var countLabel = new Label("Count: ");
        var countLabelDynamicPart = new Label("1");
        countLabelDynamicPart.name = "CountLabel";
        textField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            var isNumeric = int.TryParse(evt.newValue, out int n);
            if (isNumeric && n != 0)
            {
                countLabelDynamicPart.text = evt.newValue.TrimStart(new Char[] { '0' } );
                textField.value = evt.newValue.TrimStart(new Char[] { '0' } );
            }
            else if (string.IsNullOrEmpty(evt.newValue))
            {
                evt.Dispose();
                textField.value = "0";
                countLabelDynamicPart.text = "0";
            }
            else
            {
                evt.Dispose();
                textField.value = countLabelDynamicPart.text;
            }
            
        });
        generatedPort.contentContainer.Add(countLabelDynamicPart);
        generatedPort.contentContainer.Add(countLabel);
        
        var incrementButton = new Button(() => { textField.value = (Int32.Parse(countLabelDynamicPart.text) + 1).ToString();})
        {
            text = "+"
        }; 
        generatedPort.contentContainer.Add(incrementButton);
        
        generatedPort.contentContainer.Add(textField);
        
        var decrementButton = new Button(() => { textField.value = (Int32.Parse(countLabelDynamicPart.text) - 1).ToString(); })
        {
            text = "-"
        }; 
        generatedPort.contentContainer.Add(decrementButton);
        var deleteButton = new Button(() => RemovePort(graphNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        graphNode.outputContainer.Add(generatedPort);
        graphNode.RefreshExpandedState();
        graphNode.RefreshPorts();
    }

    private void RemovePort(GraphNode graphNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        graphNode.outputContainer.Remove(generatedPort);
        graphNode.RefreshPorts();
        graphNode.RefreshExpandedState();
    }
    
    public void AddException(GraphNode graphNode, NodeContainer value = null)
    {
        var generatedPort = GeneratePort(graphNode, Direction.Output);
        generatedPort.SetEnabled(false);

        var objectField = new ObjectField {objectType = typeof(NodeContainer)};
        
        if (value != null)
        {
            objectField.value = value;
        }
        generatedPort.contentContainer.Add(objectField);
        
        generatedPort.portName = "";
        graphNode.outputContainer.Add(generatedPort);
        graphNode.RefreshExpandedState();
        graphNode.RefreshPorts();
    }
    
    private void RemoveException(GraphNode graphNode)
    {
        if (graphNode.outputContainer.Children().Any())
        {
            graphNode.outputContainer.RemoveAt(graphNode.outputContainer.Children().Count() - 1);
            graphNode.RefreshPorts();
            graphNode.RefreshExpandedState();
        }
        
        
    }
}
