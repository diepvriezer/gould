using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Uva.CivicCompiler.Ast;
using Uva.Gould;
using Uva.Gould.Phases;
using Uva.Gould.Support;

namespace Uva.CivicCompiler.Phases.ScanParse
{
    [Phase("Read file and construct AST")]
    public class ParseInputPhase : CivicBaseVisitor<Node>, IPhase
    {
        public ParseInputPhase(string inputPath)
        {
            if (inputPath == null) throw new ArgumentNullException("inputPath");

            _inputPath = inputPath;
        }

        // Lookups for types and operators.
        private static readonly Dictionary<int, BinOp> BinOpLookup = new Dictionary<int, BinOp>()
        {
            { CivicParser.MUL, BinOp.Multiply },
            { CivicParser.DIV, BinOp.Divide },
            { CivicParser.MOD, BinOp.Mod },
            { CivicParser.ADD, BinOp.Add },
            { CivicParser.SUB, BinOp.Subtract }
        };

        private CommonTokenStream _cts;
        private string[] _lines;

        private readonly string _inputPath;

        public Node Transform(Node root)
        {
            // Read input file, dump source, and setup Antlr.
            if (!File.Exists(_inputPath))
            {
                CTI.Critical("File not found! {0}", _inputPath);
                return root;
            }

            CTI.Debug("Reading file {0}", _inputPath);
            _lines = File.ReadAllLines(_inputPath);

            CTI.Debug("Dumping source");
            WriteLines(_lines);

            CTI.Debug("Creating lexer and parser");
            var source = string.Join(Environment.NewLine, _lines);
            var rawInput = new AntlrInputStream(source);
            var lexer = new CivicLexer(rawInput);
            _cts = new CommonTokenStream(lexer);
            var parser = new CivicParser(_cts);

            // Attach custom error logger, this one prints which rule was on the stack.
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new RuleStackErrorListener());

            CTI.Debug("Creating parse tree");
            IParseTree tree = parser.prog();

            CTI.Debug("Creating AST");
            return Visit(tree);
        }


        public override Node VisitProg(CivicParser.ProgContext context)
        {
            var prog = new Program()
            {
                SourceInfo = SourceFromContext(context)
            };

            foreach (var statement in context.stat())
            {
                var result = Visit(statement) as Statement;
                if (result != null)
                {
                    if (prog.Statements == null)
                        prog.Statements = new LinkedNode<Statement>();

                    prog.Statements.Append(result);
                }
            }

            return prog;
        }

        #region Statements

        public override Node VisitAssign(CivicParser.AssignContext context)
        {
            return new Assignment
            {
                SourceInfo = SourceFromContext(context),
                Id = context.ID().GetText(),
                Expression = Visit(context.expr()) as Expression,
            };
        }
        
        #endregion

        #region Expressions

        public override Node VisitParens(CivicParser.ParensContext context)
        {
            return Visit(context.expr()) as Expression;
        }
        
        public override Node VisitBinop(CivicParser.BinopContext context)
        {
            return new BinaryOperation
            {
                SourceInfo = SourceFromContext(context),
                Operator = ParseBinOp(context),
                Left = Visit(context.expr(0)) as Expression,
                Right = Visit(context.expr(1)) as Expression
            };
        }

        public override Node VisitVar(CivicParser.VarContext context)
        {
            return new Variable
            {
                SourceInfo = SourceFromContext(context),
                Id = context.ID().GetText()
            };
        }

        public override Node VisitConst(CivicParser.ConstContext context)
        {
            return Visit(context.constant()) as Expression;
        }

        #endregion

        #region Constants

        public override Node VisitIntConst(CivicParser.IntConstContext context)
        {
            var intConst = new IntConst() { SourceInfo = SourceFromContext(context) };

            var text = context.GetText();
            int i;
            if (!int.TryParse(text, out i))
                CTI.CriticalWithSource("Value '" + text + "' is not a valid integer.", intConst.SourceInfo);

            intConst.Value = i;
            return intConst;
        }

        public override Node VisitFloatConst(CivicParser.FloatConstContext context)
        {
            var floatConst = new FloatConst() { SourceInfo = SourceFromContext(context) };

            var text = context.GetText();
            float f;
            if (!float.TryParse(text, out f))
                CTI.CriticalWithSource("Value '" + text + "' is not a valid float.", floatConst.SourceInfo);

            floatConst.Value = f;
            return floatConst;
        }

        public override Node VisitBoolConst(CivicParser.BoolConstContext context)
        {
            var boolConst = new BoolConst() { SourceInfo = SourceFromContext(context) };

            var text = context.GetText();
            bool b;
            if (!bool.TryParse(text, out b))
                CTI.CriticalWithSource("Value '" + text + "' is not a valid bool.", boolConst.SourceInfo);

            boolConst.Value = b;
            return boolConst;
        }

        #endregion

        
        private SourceInfo SourceFromContext(ParserRuleContext context)
        {
            var firstToken = _cts.Get(context.SourceInterval.a);

            int l = firstToken.Line;
            return new SourceInfo()
            {
                LineNumber = l,
                Column = firstToken.Column, // note: tab counts as 1 column.
                Text = firstToken.Text,
                TargetLine = _lines[l - 1]
            };
        }

        private void WriteLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
                CTI.Debug(i.ToString().PadLeft(4) + ": " + lines[i]);
        }

        private BinOp ParseBinOp(CivicParser.BinopContext context)
        {
            BinOp value;
            BinOpLookup.TryGetValue(context.op.Type, out value);
            if (value == BinOp.Unknown)
                CTI.ErrorWithSource("Unknown binop {0}!", SourceFromContext(context), context.GetText());

            return value;
        }
    }
}
