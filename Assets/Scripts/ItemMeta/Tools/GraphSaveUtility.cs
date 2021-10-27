using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GramGames.CraftingSystem.DataContainers;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;
using Object = System.Object;

namespace GramGames.CraftingSystem.Editor
{
    public class GraphSaveUtility
    {
        
        private GraphView _targetGraphView;
        private NodeContainer _containerCache;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<GraphNode> Nodes => _targetGraphView.nodes.ToList().Cast<GraphNode>().ToList();

        public static GraphSaveUtility GetInstance(GraphView targetGraphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targetGraphView
            };
        }

        public void SaveGraph()
        {
            if(!Edges.Any()) return;

            var freeInputs = 0;
            foreach (var node in Nodes)
            {
                var input = node.inputContainer.Children().Cast<Port>().ToList();
                foreach (var port in input)
                {
                    if (!port.connected)
                    {
                        freeInputs++;
                        if (freeInputs > 1)
                        {
                            EditorUtility.DisplayDialog("Can't Save", "You must have only 1 main node (a node without input connection). Please rearrange your graph.", "OK");
                            return;
                        }
                    } 
                }

            }

            NodeContainer nodeContainer;
            
            foreach (var mainNode in Nodes)
            {
                var edges = new List<Edge>();
                var nodes = new HashSet<Node>();
                var traverseNodes = new HashSet<Node>();
                
                nodes.Add(mainNode);
                traverseNodes.Add(mainNode);

                while (traverseNodes.Count > 0)
                {
                    var traversePorts = traverseNodes.First().outputContainer.Children().Cast<Port>().ToList();
                    traverseNodes.Remove(traverseNodes.First());
                    foreach (var port in traversePorts)
                    {
                        foreach (var edge in port.connections)
                        {
                            edges.Add(edge);
                            if (!nodes.Contains(edge.input.node))
                            {
                                traverseNodes.Add(edge.input.node);
                            }
                            nodes.Add(edge.input.node);
                        }
                    }
                }
                
                if(!File.Exists("Assets/Resources/CraftingObjects/" + mainNode.NodeText + ".asset"))
                    nodeContainer = ScriptableObject.CreateInstance<NodeContainer>();
                else
                {
                    nodeContainer = Resources.Load<NodeContainer>("CraftingObjects/" + mainNode.NodeText);
                    nodeContainer.MainNodeData = null;
                    nodeContainer.NodeData.Clear();
                    nodeContainer.NodeLinks.Clear();
                }
                
                

                if ((CraftableItemTypes) (mainNode.mainContainer.Children().ToList()[4] as EnumField).value == CraftableItemTypes.ANY)
                {
                    var portList = mainNode.outputContainer.Children().Cast<Port>().ToList();
                    var exceptions = new List<NodeContainer>();

                    foreach (var port in portList)
                    {
                        exceptions.Add((port.Children().ToList()[2] as ObjectField).value as NodeContainer);
                    }
                    
                    nodeContainer.MainNodeData = new NodeData
                    {
                        NodeGUID = mainNode.GUID,
                        NodeText = mainNode.NodeText,
                        Position = mainNode.GetPosition().position,
                        Sprite = (Sprite) (mainNode.mainContainer.Children().ToList()[3] as ObjectField).value,
                        Type = (CraftableItemTypes) (mainNode.mainContainer.Children().ToList()[4] as EnumField).value,
                        Exceptions = exceptions,
                        DesiredType = (CraftableItemTypes) (mainNode.mainContainer.Children().ToList()[5] as EnumField).value,
                        FlavorText = (mainNode.mainContainer.Children().ToList()[6] as TextField).value,
                    };
                }
                else
                {
                    nodeContainer.MainNodeData = new NodeData
                    {
                        NodeGUID = mainNode.GUID,
                        NodeText = mainNode.NodeText,
                        Position = mainNode.GetPosition().position,
                        Sprite = (Sprite) (mainNode.mainContainer.Children().ToList()[3] as ObjectField).value,
                        Type = (CraftableItemTypes) (mainNode.mainContainer.Children().ToList()[4] as EnumField).value,
                        Exceptions = null,
                        DesiredType = CraftableItemTypes.NONE,
                        FlavorText = (mainNode.mainContainer.Children().ToList()[5] as TextField).value,
                    };
                }

                int linkingProcessCount = 0;
                string outputNodeName = "";
                
                for (var i = 0; i < edges.Count; i++)
                {
                    var outputNode = edges[i].output.node as GraphNode;
                    var inputNode = edges[i].input.node as GraphNode;

                    if (outputNodeName != outputNode.NodeText)
                    {
                        linkingProcessCount = 0;
                        outputNodeName = outputNode.NodeText;
                    }
                    
                    var portList = outputNode.outputContainer.Children().Cast<Port>().ToList();

                    List<VisualElement> portElements = null;
                    Label countLabel = null;
                    
                    if (portList.Count > 0)
                    {
                        portElements = portList[linkingProcessCount].Children().ToList();
                        countLabel = portElements.Single(a => a.name == "CountLabel") as Label;
                    }
                    
                    nodeContainer.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGUID = outputNode.GUID,
                        PortName = edges[i].output.portName,
                        TargetNodeGUID = inputNode.GUID,
                        Count = (countLabel == null ? "0" : countLabel.text)
                    });

                    linkingProcessCount++;
                }
                
                foreach (var node in nodes)
                {
                    var graphNode = node as GraphNode;
                    var childrenList = node.mainContainer.Children().ToList();
                    var objectField = childrenList[3] as ObjectField;
                    var enumField = childrenList[4] as EnumField;
                    
                    if ((CraftableItemTypes) enumField.value == CraftableItemTypes.ANY)
                    {
                        var portList = graphNode.outputContainer.Children().Cast<Port>().ToList();
                        var exceptions = new List<NodeContainer>();

                        foreach (var port in portList)
                        {
                            exceptions.Add((port.Children().ToList()[2] as ObjectField).value as NodeContainer);
                        }
                    
                        nodeContainer.NodeData.Add( new NodeData
                        {
                            NodeGUID = graphNode.GUID,
                            NodeText = graphNode.NodeText,
                            Position = graphNode.GetPosition().position,
                            Sprite = (Sprite) (graphNode.mainContainer.Children().ToList()[3] as ObjectField).value,
                            Type = (CraftableItemTypes) (graphNode.mainContainer.Children().ToList()[4] as EnumField).value,
                            Exceptions = exceptions,
                            DesiredType = (CraftableItemTypes) (graphNode.mainContainer.Children().ToList()[5] as EnumField).value,
                            FlavorText = (graphNode.mainContainer.Children().ToList()[6] as TextField).value,
                        });
                    }
                    else
                    {
                        nodeContainer.NodeData.Add(new NodeData
                        {
                            NodeGUID = graphNode.GUID,
                            NodeText = graphNode.NodeText,
                            Position = graphNode.GetPosition().position,
                            Sprite = (Sprite)objectField.value,
                            Type = (CraftableItemTypes)enumField.value,
                            Exceptions = null,
                            DesiredType = CraftableItemTypes.NONE,
                            FlavorText = (graphNode.mainContainer.Children().ToList()[5] as TextField).value,
                        });
                    }
                    
                    
                }

                if (!File.Exists("Assets/Resources/CraftingObjects/" + mainNode.NodeText + ".asset"))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources/CraftingObjects"))
                        AssetDatabase.CreateFolder("Assets/Resources", "CraftingObjects");
            
                    AssetDatabase.CreateAsset(nodeContainer, $"Assets/Resources/CraftingObjects/" + mainNode.NodeText +".asset");
                    AssetDatabase.SaveAssets();
                }
                
                
                var existingCraftingObjects = Resources.LoadAll("CraftingObjects", typeof(ScriptableObject)).Cast<NodeContainer>().ToList();
                foreach (var craftingObject in existingCraftingObjects)
                {
                    if (craftingObject.NodeData.Select(node => node.NodeGUID).ToList().Contains(mainNode.GUID))
                    {
                        for (int i = 0; i < craftingObject.NodeLinks.Count; i++)
                        {
                            for (int j = 0; j < nodeContainer.NodeLinks.Count; j++)
                            {
                                if (craftingObject.NodeLinks[i].BaseNodeGUID == nodeContainer.NodeLinks[j].BaseNodeGUID && craftingObject.NodeLinks[i].TargetNodeGUID == nodeContainer.NodeLinks[j].TargetNodeGUID)
                                {
                                    craftingObject.NodeLinks[i] = nodeContainer.NodeLinks[j];
                                }

                                
                            }
                        }
                        
                        for (int i = 0; i < craftingObject.NodeData.Count; i++)
                        {
                            for (int j = 0; j < nodeContainer.NodeData.Count; j++)
                            {
                                if (craftingObject.NodeData[i].NodeGUID == nodeContainer.NodeData[j].NodeGUID)
                                {
                                    var oldPos = craftingObject.NodeData[i].Position;
                                    craftingObject.NodeData[i] = nodeContainer.NodeData[j];
                                    craftingObject.NodeData[i].Exceptions = nodeContainer.NodeData[j].Exceptions;
                                    craftingObject.NodeData[i].Position = oldPos;
                                }

                                
                            }
                        }

                        var linksToDelete = craftingObject.NodeLinks.Where(linkData => linkData.BaseNodeGUID == mainNode.GUID).Except(nodeContainer.NodeLinks.Where(linkData => linkData.BaseNodeGUID == mainNode.GUID)).ToList();

                        foreach (var linkData in linksToDelete)
                        {
                            craftingObject.NodeLinks.Remove(linkData);
                        }
                        
                        var linksToAdd = nodeContainer.NodeLinks.Where(linkData => linkData.BaseNodeGUID == mainNode.GUID).Except(craftingObject.NodeLinks.Where(linkData => linkData.BaseNodeGUID == mainNode.GUID)).ToList();

                        foreach (var linkData in linksToAdd)
                        {
                            craftingObject.NodeLinks.Add(linkData);
                        }
                        
                        var nodesToAdd = nodeContainer.NodeData.Except(craftingObject.NodeData).ToList();
                        foreach (var nodeData in nodesToAdd)
                        {
                            craftingObject.NodeData.Add(nodeData);
                        }

                        var nodesToDelete = craftingObject.NodeData.Select(nodeData => nodeData.NodeGUID).Except(craftingObject.NodeLinks.Select(linkData => linkData.TargetNodeGUID).ToList()).ToList();
                        foreach (var nodeData in nodesToDelete)
                        {
                            if(craftingObject.NodeLinks.Count == 0 && craftingObject.NodeData.Count == 1)
                                break;
                            
                            if (!craftingObject.NodeLinks.Select(linkData => linkData.BaseNodeGUID).ToList()
                                .Contains(nodeData))
                            {
                                craftingObject.NodeData.Remove(
                                    craftingObject.NodeData.Single(x => x.NodeGUID == nodeData));
                            }
                        }
                        
                    }
                }
                EditorUtility.SetDirty(nodeContainer);
            }
            
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ClearWholeGraph()
        {
            ClearGraph();
        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<NodeContainer>("CraftingObjects/" + fileName);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target graph file does not exits!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        public void AddGraph(string fileName)
        {
            if (Nodes.Select(node => node.title).ToList().Contains(fileName))
            {
                EditorUtility.DisplayDialog("Already Exists", "Item already exists in the graph!", "OK");
                return;
            }
                
            
            _containerCache = Resources.Load<NodeContainer>("CraftingObjects/" + fileName);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target graph file does not exits!", "OK");
                return;
            }
            
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
                for (var j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
                    
                    targetNode.SetPosition(new Rect(
                        _containerCache.NodeData.First(x => x.NodeGUID == targetNodeGuid).Position,
                        _targetGraphView.defaultNodeSize));

                    Nodes[i].outputContainer[j].Q<Label>("CountLabel").text = connections[j].Count;
                    Nodes[i].outputContainer[j].Q<TextField>("CountTextField").value = connections[j].Count;
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _targetGraphView.Add(tempEdge);
            
        }

        private void CreateNodes()
        {
            foreach (var nodeData in _containerCache.NodeData)
            {
                if (nodeData.Type == CraftableItemTypes.ANY)
                {
                    var tempNode = _targetGraphView.CreateAnyGraphNode(nodeData.NodeText);
                    tempNode.GUID = nodeData.NodeGUID;
                    
                    if(Nodes.Select(node => node.NodeText).ToList().Contains(tempNode.NodeText))
                        continue;
                    
                    ((ObjectField) tempNode.mainContainer.Children().ToList()[3]).value = nodeData.Sprite;
                    ((EnumField) tempNode.mainContainer.Children().ToList()[4]).value = nodeData.Type;
                    ((EnumField) tempNode.mainContainer.Children().ToList()[5]).value = nodeData.DesiredType;
                    ((TextField) tempNode.mainContainer.Children().ToList()[6]).value = nodeData.FlavorText;

                    _targetGraphView.AddElement(tempNode);
                    
                    var nodePorts = nodeData.Exceptions;
                    nodePorts.ForEach(x => _targetGraphView.AddException(tempNode, x));
                }
                else
                {
                    var tempNode = _targetGraphView.CreateGraphNode(nodeData.NodeText);
                    tempNode.GUID = nodeData.NodeGUID;
                
                    if(Nodes.Select(node => node.NodeText).ToList().Contains(tempNode.NodeText))
                        continue;

                    ((ObjectField) tempNode.mainContainer.Children().ToList()[3]).value = nodeData.Sprite;
                    ((EnumField) tempNode.mainContainer.Children().ToList()[4]).value = nodeData.Type;
                    ((TextField) tempNode.mainContainer.Children().ToList()[5]).value = nodeData.FlavorText;

                    _targetGraphView.AddElement(tempNode);

                    var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
                    nodePorts.ForEach(x => _targetGraphView.AddPort(tempNode));
                }
            }
        }

        private void ClearGraph()
        {
            //Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGUID;

            foreach (var node in Nodes)
            {
                //if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
                
                _targetGraphView.RemoveElement(node);
            }
        }
    }
}