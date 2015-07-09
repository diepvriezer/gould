namespace Uva.Gould.Graphing
{
    public interface IEdge<TVertex>
    {
        TVertex Source { get; }
        TVertex Target { get; }
    }
}
