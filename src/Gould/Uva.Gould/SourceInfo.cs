namespace Uva.Gould
{
    /// <summary>
    /// Holds source code data associated with a node.
    /// </summary>
    public class SourceInfo
    {
        public int LineNumber { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }
        public string TargetLine { get; set; }
    }
}
