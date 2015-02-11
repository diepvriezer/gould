using System;
using System.Collections.Generic;
using System.Linq;

namespace Uva.Gould.Graphs
{
    /// <summary>
    /// Bundle of graph construction routines.
    /// </summary>
    public class GraphBuilder
    {
        private int _nodeId;
        private Dictionary<Type, List<Type>> _inheritances;


        /// <summary>
        /// Creates a graph detailing the inheritance for all subtypes of Node.
        /// </summary>
        public Graph CreateInheritanceGraph()
        {
            _nodeId = 1;

            // Ensure we have the inheritance lookup.
            FillInheritanceLookup();

            // Graph and visited node tracker.
            var g = new Graph("AST inheritance graph");
            var lookup = new Dictionary<Type, GraphNode>();

            // Add all nodes in the hierarchy, should all be keys.
            foreach (var nodeType in _inheritances.Keys)
            {
                var node = nodeType == typeof(Node)
                    ? new GraphNode("root")
                    : new GraphNode(NextId());
                node.Name = nodeType.GetCleanName();

                lookup.Add(nodeType, node);
                g.Nodes.Add(node);
            }

            // Draw edges between node and all of its subtypes.
            foreach (var kv in _inheritances)
            {
                var parentNode = lookup[kv.Key];
                foreach (var child in kv.Value)
                {
                    var childNode = lookup[child];
                    g.Edges.Add(new GraphEdge(childNode, parentNode));
                }
            }

            return g;
        }

        /// <summary>
        /// Creates a representation of the AST using reflection.
        /// </summary>
        public Graph CreateAstGraph<TRoot>()
            where TRoot : Node
        {
            _nodeId = 1;

            // Ensure we have the inheritance lookup.
            FillInheritanceLookup();

            // Graph and visited node tracker.
            var g = new Graph("AST definition");
            var lookup = new Dictionary<Type, GraphNode>();
            
            // Add root node.
            var nodeType = typeof (TRoot);
            var node = new GraphNode("root", nodeType.GetCleanName());
            g.Nodes.Add(node);
            lookup.Add(nodeType, node);

            // In-depth traversal using stack.
            var s = new Stack<Tuple<Type, GraphNode>>();
            s.Push(new Tuple<Type, GraphNode>(nodeType, node));
            while (s.Any())
            {
                var tp = s.Pop();
                node = tp.Item2;
                nodeType = tp.Item1;

                // First process the child properties.
                foreach (var childProp in nodeType.GetChildNodeProperties())
                {
                    var propName = childProp.Name;
                    var childType = childProp.PropertyType;

                    // Add backedge to self if the same as this one.
                    if (childType == nodeType)
                    {
                        g.Edges.Add(new GraphEdge(node, node, propName));
                        continue;
                    }

                    // Add if not already present.
                    GraphNode childNode;
                    if (lookup.ContainsKey(childType))
                    {
                        childNode = lookup[childType];
                    }
                    else
                    {
                        childNode = new GraphNode(NextId(), childType.GetCleanName());
                        g.Nodes.Add(childNode);
                        lookup.Add(childType, childNode);

                        s.Push(new Tuple<Type, GraphNode>(childType, childNode));
                    }

                    // Add edge.
                    g.Edges.Add(new GraphEdge(childNode, node, propName));
                }

                // If this type has sub-classes, graph it.
                if (!_inheritances.ContainsKey(nodeType))
                    continue;

                var subTypes = _inheritances[nodeType];
                foreach (var subType in subTypes)
                {
                    // Add if not already present.
                    GraphNode subNode;
                    if (lookup.ContainsKey(subType))
                    {
                        subNode = lookup[subType];
                    }
                    else
                    {
                        subNode = new GraphNode(NextId(), subType.GetCleanName());
                        g.Nodes.Add(subNode);
                        lookup.Add(subType, subNode);

                        s.Push(new Tuple<Type, GraphNode>(subType, subNode));
                    }

                    g.Edges.Add(new GraphEdge(subNode, node, style: 1));
                }
            }

            return g;
        }

        /// <summary>
        /// Creates a graph based on a given tree.
        /// </summary>
        public Graph CreateObjectGraph(Node node)
        {
            _nodeId = 1;

            // Graph.
            var g = new Graph("AST object graph");

            // Add root node.
            var gNode = new GraphNode("root", node.GetType().GetCleanName());
            g.Nodes.Add(gNode);

            // In-depth traversal using stack.
            var s = new Stack<Tuple<Node, GraphNode>>();
            s.Push(new Tuple<Node, GraphNode>(node, gNode));
            while (s.Any())
            {
                var tp = s.Pop();
                node = tp.Item1;
                gNode = tp.Item2;

                // Process all child properties.
                foreach (var childProp in node.ChildProperties)
                {
                    // Null objects don't have titles and are styled differently
                    var child = (Node) childProp.GetValue(node);
                    if (child == null)
                    {
                        var nullChild = new GraphNode(NextId(), "null", style: 3);
                        g.Nodes.Add(nullChild);
                        g.Edges.Add(new GraphEdge(nullChild, gNode, childProp.Name, style: 3));
                        continue;
                    }

                    var gChild = new GraphNode(NextId(), child.GetType().GetCleanName());
                    g.Nodes.Add(gChild);
                    g.Edges.Add(new GraphEdge(gChild, gNode, childProp.Name));
                    s.Push(new Tuple<Node, GraphNode>(child, gChild));
                }
            }

            return g;
        }
        

        private IEnumerable<Type> GetSubclassesInAppDomain(Type nodeType)
        {
            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                types.AddRange(assembly.DefinedTypes.Where(t => t.IsClass && t.IsSubclassOf(nodeType)).Select(t => t.AsType()));
            }

            return types;
        }
        private void FillInheritanceLookup()
        {
            if (_inheritances != null)
                return;

            var nodeType = typeof(Node);
            _inheritances = new Dictionary<Type, List<Type>>() { { nodeType, new List<Type>() } };

            var subClasses = GetSubclassesInAppDomain(nodeType).ToList();
            subClasses.ForEach(sub =>
            {
                var parent = sub.BaseType;
                if (parent == null)
                    throw new InvalidOperationException("This is clearly illegal. ILLEGAL!");

                if (!_inheritances.ContainsKey(parent))
                    _inheritances.Add(parent, new List<Type>());
                if (!_inheritances.ContainsKey(sub))
                    _inheritances.Add(sub, new List<Type>());

                if (!_inheritances[parent].Contains(sub))
                    _inheritances[parent].Add(sub);
            });
        }
        private string NextId()
        {
            return "N" + (++_nodeId);
        }
    }
}
