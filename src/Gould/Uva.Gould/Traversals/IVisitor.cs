namespace Uva.Gould.Traversals
{
    /// <summary>
    /// Common visitor interface.
    /// </summary>
    public interface IVisitor
    {
        T Visit<T>(T node);
        void VisitChildren<T>(T node);
    }
}
