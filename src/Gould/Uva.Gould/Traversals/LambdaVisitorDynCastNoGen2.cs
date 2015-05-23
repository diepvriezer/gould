using System;
using System.Collections.Generic;

namespace Uva.Gould.Traversals
{
    public abstract class LambdaVisitorDynCastNoGen2
    {
        private List<Handler> _handlers = new List<Handler>();
 
        public T Visit<T>(T node)
            where T : Node
        {
            return (T) Visit(node, typeof (T));
        }

        private Node Visit(Node node, Type maxUpcast)
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

                // For performance reasons, dynamic cast to ConcreteHandler which avoids DynamicInvoke.
                dynamic dynHandle = handle;

                // Test for predicate.
                if (dynHandle.Predicate != null)
                {
                    bool result = dynHandle.Predicate.Invoke(node);
                    if (!result)
                        continue;
                }

                // Check if this is an action or function.
                if (dynHandle.Function != null)
                {
                    return dynHandle.Function.Invoke(node);
                }
                else
                {
                    dynHandle.Action.Invoke(node);
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
                var value = (Node) prop.GetValue(node);
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
            _handlers.Add(new ConcreteHandler<T, T>(function));
        }
        protected void ReplaceIf<T>(Func<T, bool> predicate, Func<T, T> function)
            where T : Node
        {
            _handlers.Add(new ConcreteHandler<T, T>(function, predicate));
        }
        protected void Replace<T, TContext>(Func<T, TContext> function)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new ConcreteHandler<T, TContext>(function));
        }
        protected void ReplaceIf<T, TContext>(Func<T, bool> predicate, Func<T, TContext> function)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new ConcreteHandler<T, TContext>(function, predicate));
        }

        protected void View<T>(Action<T> action)
            where T : Node
        {
            _handlers.Add(new ConcreteHandler<T, T>(action));
        }
        protected void ViewIf<T>(Func<T, bool> predicate, Action<T> action)
            where T : Node
        {
            _handlers.Add(new ConcreteHandler<T, T>(action, predicate));
        }
        protected void View<T, TContext>(Action<T> action)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new ConcreteHandler<T, TContext>(action));
        }
        protected void ViewIf<T, TContext>(Func<T, bool> predicate, Action<T> action)
            where T : TContext
            where TContext : Node
        {
            _handlers.Add(new ConcreteHandler<T, TContext>(action, predicate));
        }

        protected void RemoveAllHandlers()
        {
            _handlers = new List<Handler>();
        }

        #endregion
        
        internal abstract class Handler
        {
            public abstract Type Type { get; }
            public abstract Type MaxUpcast { get; }
        }
        internal class ConcreteHandler<T, TContext> : Handler
            where T : TContext
            where TContext : Node
        {
            public ConcreteHandler(Func<T, TContext> func, Func<T, bool> predicate = null)
            {
                this.Function = func;
                this.Predicate = predicate;
            }
            public ConcreteHandler(Action<T> action, Func<T, bool> predicate = null)
            {
                this.Action = action;
                this.Predicate = predicate;
            }

            // Types can be deduced from generic parameters.
            public override Type Type
            {
                get { return typeof (T); }
            }
            public override Type MaxUpcast
            {
                get { return typeof (TContext); }
            }

            public Func<T, TContext> Function { get; private set; }
            public Action<T> Action { get; private set; }
            public Func<T, bool> Predicate { get; private set; }
        }
    }
}
