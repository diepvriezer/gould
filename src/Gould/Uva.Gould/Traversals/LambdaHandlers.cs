using System;
using System.Reflection;

namespace Uva.Gould.Traversals
{
    /// <summary>
    /// Interface for all handlers.
    /// </summary>
    public interface ILambdaHandler
    {
        bool Valid(object node, Type contextType);
        object Execute(object node);
    }

    /// <summary>
    /// Handler which wraps an action.
    /// </summary>
    public class ActionHandler<T> : ILambdaHandler
    {
        public ActionHandler(Action<T> action)
        {
            Action = action;
        }

        public Action<T> Action { get; private set; }

        public bool Valid(object node, Type contextType)
        {
            return node is T && contextType.IsAssignableFrom(typeof(T));
        }

        public object Execute(object node)
        {
            Action.Invoke((T) node);
            return node;
        }
    }

    /// <summary>
    /// Handler which wraps a delegate.
    /// </summary>
    public class DelegateHandler<T, TResult> : ILambdaHandler
    {
        public DelegateHandler(Func<T, TResult> func)
        {
            Function = func;
        }
        
        public Func<T, TResult> Function { get; private set; }

        public bool Valid(object node, Type contextType)
        {
            return node is T && contextType.IsAssignableFrom(typeof(TResult));
        }

        public object Execute(object node)
        {
            return Function.Invoke((T)node);
        }
    }

    /// <summary>
    /// Handler which wraps a method, likely created through the TravMethodAttribute.
    /// </summary>
    public class MethodHandler : ILambdaHandler
    {
        public MethodHandler(MethodInfo method, object owner, Type argType)
        {
            Method = method;
            Owner = owner;
            isVoidMethod = method.ReturnType == typeof (void);
            this.argType = argType;
        }

        public MethodInfo Method { get; private set; }
        public object Owner { get; private set; }

        private bool isVoidMethod;
        private Type argType;

        public bool Valid(object node, Type contextType)
        {
            return isVoidMethod
                ? argType.IsInstanceOfType(node) && contextType.IsAssignableFrom(argType)
                : argType.IsInstanceOfType(node) && contextType.IsAssignableFrom(Method.ReturnType);
        }

        public object Execute(object node)
        {
            var result = Method.Invoke(Owner, new object[] {node});
            return isVoidMethod ? node : result;
        }
    }

    /// <summary>
    /// Handler which wraps another handler (decorator pattern) and embeds a predicate into the validation check.
    /// </summary>
    public class PredicateHandler<T> : ILambdaHandler
    {
        public PredicateHandler(Predicate<T> predicate, ILambdaHandler innerHandler)
        {
            Predicate = predicate;
            InnerHandler = innerHandler;
        }

        public Predicate<T> Predicate { get; private set; }
        public ILambdaHandler InnerHandler { get; private set; }

        public bool Valid(object node, Type contextType)
        {
            return node is T && Predicate.Invoke((T) node) && InnerHandler.Valid(node, contextType);
        }

        public object Execute(object node)
        {
            return InnerHandler.Execute(node);
        }
    }
}