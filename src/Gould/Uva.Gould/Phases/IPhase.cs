namespace Uva.Gould.Phases
{
    /// <summary>
    /// Denotes a class as a phase, i.e. something which transforms a node graph.
    /// </summary>
    public interface IPhase
    {
        Node Transform(Node node);
    }
}
