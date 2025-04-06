using System.Collections.Generic;
using UnityEngine;

public class DiceConfigurationTree
{
    private Dictionary<ConfigurationNode, ConfigurationNode> parents = new(); // Key -> Child, Value -> Parent
    private Dictionary<ConfigurationNode, Dictionary<(int, int), ConfigurationNode>> children = new(); // Key -> Parent, Value -> (int faceIndex, int eyeIndex) -> Child
    private Dictionary<DiceConfiguration, ConfigurationNode> lookup = new(); // Key -> Configuration, Value -> Node

    public void AddRoot(DiceConfiguration configuration)
    {
        if (lookup.Count != 0) throw new System.Exception("should not add root when tree already contains elements");

        var node = new ConfigurationNode()
        {
            configuration = configuration
        };

        parents[node] = null;
        lookup[configuration] = node;
    }

    public void Add(DiceConfiguration config, DiceConfiguration parent, int faceIndex, int eyeIndex)
    {
        if (parent == null) throw new System.Exception("Null parent");

        var parentNode = lookup[parent];
        var childNode = new ConfigurationNode()
        {
            configuration = config
        };
        parents[childNode] = parentNode;
        lookup[config] = childNode;
        if (!children.ContainsKey(parentNode))
            children[parentNode] = new();
        children[parentNode][(faceIndex, eyeIndex)] = childNode;
    }

    public DiceConfiguration GetParentConfiguration(DiceConfiguration child)
    {
        if (lookup.TryGetValue(child, out var childNode))
        {
            if (parents.TryGetValue(childNode, out var parentNode))
            {
                if (parentNode == null)
                    return null;
                return parentNode.configuration;
            }
        }
        return null;
    }

    public DiceConfiguration GetChildConfiguration(DiceConfiguration parent, int faceIndex, int eyeIndex)
    {
        if (lookup.TryGetValue(parent, out var parentNode))
        {
            if (children.TryGetValue(parentNode, out var childrenNodes))
            {
                if (childrenNodes.TryGetValue((faceIndex, eyeIndex), out var child))
                {
                    return child.configuration;
                }
            }
        }
        return null;
    }


    private class ConfigurationNode
    {
        public DiceConfiguration configuration;
    }
}


