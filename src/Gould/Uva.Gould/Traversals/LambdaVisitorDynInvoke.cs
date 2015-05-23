using System;
using System.Collections.Generic;

namespace Uva.Gould.Traversals
{
    public abstract class LambdaVisitorDynInvoke
    {
        private List<Handler> _handlers = new List<Handler>();

        public T Visit<T>(T node)
            where T : Node
        {
            return Visit(node, typeof(T));
        }

        private T Visit<T>(T node, Type maxUpcast)
            where T : Node
        {
            if (node == null)
                return null;

            // If a handler exist for this class with convertible return type, invoke.
            var nodeType = node.GetType();
            foreach (var handle in _handlers)
            {
                // Test for matching type (or subtype).
                if (!handle.Type.IsAssignableFrom(nodeType))
                    continue;

                // Test for fitness of transformation result.
                if (!maxUpcast.IsAssignableFrom(handle.MaxUpcast))
                    continue;

                // Test for predicate.
                if (handle.Predicate != null)
                {
                    bool result = (bool) handle.Predicate.DynamicInvoke(node);
                    if (!result)
                        continue;
                }

                // Check if this is an action or function.
                if (handle.Function != null)
                {
                    return (T) handle.Function.DynamicInvoke(node);
                }
                else
                {
                    handle.Action.DynamicInvoke(node);
                    return node;
                }
            }

            // If no handlers are found or if all fail, traverse children.
            VisitChildren(node);

            return node;
        }

        public void VisitChildren(Node node)
        {
            if (node == null)
                return;

            // Traverse child nodes using attributes and replace with result from visitor.
            foreach (var prop in node.ChildProperties)
            {
                var value = (Node)prop.GetValue(node);
                if (value != null)
                {
                    value = Visit(value, prop.PropertyType);
                    prop.SetValue(node, value);
                }
            }
        }

        #region Visitor configuration methods

        protected void Replace<T>(Func<T, T> function)
            where T : Node
        {
            _handlers.Add(new Handler(typeof (T), typeof (T)) {Function = function});
        }
        protected void ReplaceIf<T>(Func<T, bool> predicate, Func<T, T> function)
            where T : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(T), predicate) { Function = function });
        }
        protected void Replace<T, TContext>(Func<T, TContext> function)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(TContext)) { Function = function });
        }
        protected void ReplaceIf<T, TContext>(Func<T, bool> predicate, Func<T, TContext> function)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(TContext), predicate) { Function = function });
        }

        protected void View<T>(Action<T> action)
            where T : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(T)) { Action = action });
        }
        protected void ViewIf<T>(Func<T, bool> predicate, Action<T> action)
            where T : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(T), predicate) { Action = action });
        }
        protected void View<T, TContext>(Action<T> action)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(TContext)) { Action = action });
        }
        protected void ViewIf<T, TContext>(Func<T, bool> predicate, Action<T> action)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new Handler(typeof(T), typeof(TContext), predicate) { Action = action });
        }

        protected void RemoveAllHandlers()
        {
            _handlers = new List<Handler>();
        }

        #endregion

        internal class Handler
        {
            public Handler(Type type, Type maxUpcast, Delegate predicate = null)
            {
                this.Type = type;
                this.MaxUpcast = maxUpcast;
                this.Predicate = predicate;
            }

            public Type Type { get; set; }
            public Type MaxUpcast { get; set; }

            public Delegate Function { get; set; }
            public Delegate Action { get; set; }
            public Delegate Predicate { get; set; }
        }
    }
}
