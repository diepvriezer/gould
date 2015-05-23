using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Uva.Gould
{
    /// <summary>
    /// Visitor for Node graphs, relies on reflection.
    /// </summary>
    public abstract class OldVisitor
    {
        protected OldVisitor()
        {
            _handles = new List<Handler>();
            _genericVisitPrototype = GetGenericVisitMethod();
        }

        private readonly List<Handler> _handles;
        private readonly MethodInfo _genericVisitPrototype;
        
        /// <summary>
        /// Invokes the handler registered for the node within specified context,
        /// or if not found traverses the children.
        /// </summary>
        /// <typeparam name="TContext">Type in which the node must be seen</typeparam>
        /// <param name="node">Node to visit</param>
        /// <returns>Result of the handler, or the original node if none were found.</returns>
        [DebuggerHidden]
        public TContext Visit<TContext>(TContext node)
            where TContext : Node
        {
            if (node == null)
                return null;

            // Try to invoke handler from list.
            var handler = FindHandler(node, typeof(TContext));
            if (handler != null)
            {
                try
                {
                    var result = handler.Action.DynamicInvoke(node);
                    return handler.ActionReturns 
                        ? (TContext) result 
                        : node;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            // If no handlers are found, try the child properties.
            VisitChildren(node);
            return node;
        }

        /// <summary>
        /// Tries to invoke handlers on the children of this node.
        /// </summary>
        /// <param name="node">Node to visit</param>
        [DebuggerHidden]
        public void VisitChildren(Node node)
        {
            foreach (var prop in node.ChildProperties)
            {
                // Ensure we have read/write access.
                if (!prop.CanRead)
                    throw new InvalidOperationException("Unable to read property " + prop.Name);
                if (!prop.CanWrite)
                    throw new InvalidOperationException("Unable to write to property " + prop.Name);

                // Retrieve child node from property.
                var childNode = prop.GetValue(node);
                if (childNode == null)
                    continue;

                // Invoke this visit method using the property type as context.
                try
                {
                    childNode = _genericVisitPrototype
                        .MakeGenericMethod(prop.PropertyType)
                        .Invoke(this, new object[] {childNode});
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }

                // Set property with result of invocation.
                prop.SetValue(node, childNode);
            }
        }


        #region Handler configuration methods

        /// <summary>
        /// Adds a 'view-only' handler which doesn't require a return value.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <param name="action">Action delegate, takes a T and doesn't return.</param>
        protected void View<T>(Action<T> action)
            where T : Node
        {
            ViewIf<T>(null, action);
        }
        /// <summary>
        /// Adds a 'view-only' handler which doesn't require a return value when the predicate is met.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <typeparam name="TContext">Node context, or freedom, for which the handler is valid</typeparam>
        /// <param name="action">Action delegate, takes a T and doesn't return.</param>
        protected void View<T, TContext>(Action<T> action)
            where T : TContext
            where TContext : Node
        {
            ViewIf<T, TContext>(null, action);
        }
        /// <summary>
        /// Adds a 'view-only' handler which doesn't require a return value.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <param name="predicate">Predicate on T which must be true before the handler is invoked.</param>
        /// <param name="action">Action delegate, takes a T and doesn't return.</param>
        protected void ViewIf<T>(Func<T, bool> predicate, Action<T> action)
            where T : Node
        {
            _handles.Add(new Handler
            {
                Type = typeof(T),
                ContextType = null,
                Predicate = predicate,
                Action = action,
                ActionReturns = false
            });
        }

        /// <summary>
        /// Adds a 'view-only' handler which doesn't require a return value when the predicate is met.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <typeparam name="TContext">Node context, or freedom, for which the handler is valid</typeparam>
        /// <param name="predicate">Predicate on T which must be true before the handler is invoked.</param>
        /// <param name="action">Action delegate, takes a T and doesn't return.</param>
        protected void ViewIf<T, TContext>(Func<T, bool> predicate, Action<T> action)
            where T : TContext
            where TContext : Node
        {
            _handles.Add(new Handler
            {
                Type = typeof(T),
                ContextType = typeof(TContext),
                Predicate = predicate,
                Action = action,
                ActionReturns = false
            });
        }


        /// <summary>
        /// Adds a handler which replaces a node.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <param name="func">Replacement delegate, takes a T and returns a T.</param>
        protected void Replace<T>(Func<T, T> func)
            where T : Node
        {
            ReplaceIf<T, T>(null, func);
        }
        
        /// <summary>
        /// Adds a handler which replaces a node.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <typeparam name="TContext">Node context, or freedom, for which the handler is valid</typeparam>
        /// <param name="func">Replacement delegate, takes a T and returns TContext or T.</param>
        protected void Replace<T, TContext>(Func<T, TContext> func)
            where T : TContext
            where TContext : Node
        {
            ReplaceIf<T, TContext>(null, func);
        }
        
        /// <summary>
        /// Adds a handler which replaces a node if the predicate is met.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <param name="predicate">Predicate on T which must be true before the handler is invoked.</param>
        /// <param name="func">Replacement delegate, takes a T and returns a T.</param>
        protected void ReplaceIf<T>(Func<T, bool> predicate, Func<T, T> func)
            where T : Node
        {
            ReplaceIf<T, T>(predicate, func);
        }
        
        /// <summary>
        /// Adds a handler which replaces a node if the predicate is met.
        /// </summary>
        /// <typeparam name="T">Concrete type of the node</typeparam>
        /// <typeparam name="TContext">Node context, or freedom, for which the handler is valid</typeparam>
        /// <param name="predicate">Predicate on T which must be true before the handler is invoked.</param>
        /// <param name="func">Replacement delegate, takes a T and returns TContext or T.</param>
        protected void ReplaceIf<T, TContext>(Func<T, bool> predicate, Func<T, TContext> func)
            where T : TContext
            where TContext : Node
        {
            _handles.Add(new Handler
            {
                Type = typeof(T),
                ContextType = typeof(TContext),
                Predicate = predicate,
                Action = func,
                ActionReturns = true
            });
        }

        /// <summary>
        /// Clears all registered handlers.
        /// </summary>
        protected void RemoveAllHandlers()
        {
            _handles.Clear();
        }

        #endregion


        private Handler FindHandler(Node node, Type contextType)
        {
            var nodeType = node.GetType();
            foreach (var handle in _handles)
            {
                if (!handle.Type.IsAssignableFrom(nodeType))
                    continue;

                if (handle.ContextType != null && !contextType.IsAssignableFrom(handle.ContextType))
                    continue;

                if (handle.Predicate != null)
                {
                    var result = (bool) handle.Predicate.DynamicInvoke(node);
                    if (!result)
                        continue;
                }

                return handle;
            }

            return null;
        }
        private MethodInfo GetGenericVisitMethod()
        {
            return GetType()
                .GetMethods()
                .SingleOrDefault(m => m.Name == "Visit"
                                      && m.IsGenericMethod
                                      && m.GetParameters().Length == 1);
        }

        internal class Handler
        {
            public Type Type { get; set; }
            public Type ContextType { get; set; }
            public Delegate Predicate { get; set; }
            public Delegate Action { get; set; }

            public bool ActionReturns { get; set; }
        }
    }
}
