namespace Uva.CivicCompiler.Ast
{
    public enum BinOp
    {
        Unknown = 0,
        Subtract, Add, Multiply, Divide, Mod
    }

    public static class BinOpExtensions
    {
        public static string ToCivic(this BinOp op)
        {
            switch (op)
            {
                case BinOp.Subtract:
                    return "-";
                case BinOp.Add:
                    return "+";
                case BinOp.Multiply:
                    return "*";
                case BinOp.Divide:
                    return "/";
                case BinOp.Mod:
                    return "%";
                default:
                    return "???";
            }
        }
    }
}
