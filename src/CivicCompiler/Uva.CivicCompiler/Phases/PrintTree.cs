using System;
using Uva.CivicCompiler.Ast;
using Uva.Gould;
using Uva.Gould.Phases;

namespace Uva.CivicCompiler.Phases
{
    [Phase("Print AST")]
    class PrintTree : Visitor, IPhase
    {
        public PrintTree()
        {
            View<Assignment>(PrintAssign);
            View<BinaryOperation>(PrintBinary);
            View<Variable>(PrintVar);

            View<FloatConst>(f => Console.Write(f.Value));
            View<BoolConst>(b => Console.Write(b.Value.ToString().ToLower()));
            View<IntConst>(i => Console.Write(i.Value));
        }
        
        public Node Transform(Node root)
        {
            Visit(root);
            return root;
        }
        
        public void PrintAssign(Assignment assignment)
        {
            Console.Write("{0} = ", assignment.Id);
            Visit(assignment.Expression);
            Console.WriteLine(";");
        }

        public void PrintBinary(BinaryOperation op)
        {
            Console.Write("(");
            Visit(op.Left);
            Console.Write(" {0} ", op.Operator.ToCivic());
            Visit(op.Right);
            Console.Write(")");
        }

        public void PrintVar(Variable var)
        {
            Console.Write(var.Id);
        }
    }
}
