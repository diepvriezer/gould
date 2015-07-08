using System;
using System.Collections.Generic;

namespace Uva.Gould.Traversals
{
    /// <summary>
    /// Visitor which removes the need for overloaded public functions by maintaining a list
    /// of handlers, which can be added in a variety of ways. See HandlerList.
    /// </summary>
    public class LambdaVisitor : PropertyVisitor
    {
        public LambdaVisitor(bool addTravMethods = true)
        {
            Handlers = new List<ILambdaHandler>();

            if (addTravMethods)
                AddTravMethods(this);
        }

        public List<ILambdaHandler> Handlers { get; set; }

        protected override object Visit(object node, Type propertyType)
        {
            if (node == null)
                return null;

            foreach (var handler in Handlers)
            {
                if (handler.Valid(node, propertyType))
                    return handler.Execute(node);
            }

            VisitChildren(node);

            return node;
        }


        #region Handler configuration shortcuts
        public void AddTravMethods(object visitor = null)
        {
            // It is possible to add methods from other visitors, if omitted the current class is used.
            if (visitor == null)
                visitor = this;

            foreach (var method in visitor.GetType().GetOrderedTravMethods())
            {
                var parms = method.GetParameters();
                if (parms.Length != 1)
                    throw new InvalidOperationException("TravMethod attribute requires a method with a single argument, which is the targetted node.");

                Handlers.Add(new MethodHandler(method, visitor, parms[0].ParameterType));
            }
        }

        public void Add<T>(Action<T> action)
        {
            Handlers.Add(new ActionHandler<T>(action));
        }
        public void Add<T>(Predicate<T> predicate, Action<T> action)
        {
            Handlers.Add(new PredicateHandler<T>(predicate, new ActionHandler<T>(action)));
        }

        public void Add<T, TResult>(Func<T, TResult> func)
        {
            Handlers.Add(new DelegateHandler<T, TResult>(func));
        }
        public void Add<T, TResult>(Predicate<T> predicate, Func<T, TResult> func)
        {
            Handlers.Add(new PredicateHandler<T>(predicate, new DelegateHandler<T, TResult>(func)));
        }

        public void Clear()
        {
            Handlers.Clear();
        }
        #endregion
    }
}
