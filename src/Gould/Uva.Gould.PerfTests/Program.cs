using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.PerfTests
{
    public class Asdf : Visitor
    {
        public Int Visit(Bool b)
        {
            return new Int();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const int iters = 250;
            const int trials = 3;

            var stopwatch = new Stopwatch();

            var candidates = new List<IVisitor>()
            {
                new ReflectionLookup(),
//                new DynCastNoGenerics2(),
//                new DynInvoke(),
//                new DynInvokeNoGenerics(),
//                new DynCast(),
                new DynCastNoGenerics(),
//                new DynInvokeOld()
            }.ToDictionary(c => c, c => new List<long>(trials));
            
            //Thread.Sleep(250);
            
            for (int i = 0; i < trials; i++)
            {
                foreach (var candidate in candidates)
                {
                    Console.Write("Testing " + candidate.Key.GetType().Name + "... ");
                    stopwatch.Start();

                    for (int j = 0; j < iters; j++)
                    {
                        Node tree = Trees.ForFixedForAndStatementBlock();
//                                                var n = tree.Children(allInTree: true).Count();
                        tree = candidate.Key.Visit(tree);
                        tree = candidate.Key.Visit(tree);
                        tree = candidate.Key.Visit(tree);
                        tree = candidate.Key.Visit(tree);

//                        var n = tree.Children(allInTree: true).Count();
                    }

                    stopwatch.Stop();
                    candidate.Value.Add(stopwatch.ElapsedMilliseconds);
                    stopwatch.Reset();
                    Console.WriteLine("OK!");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Results: ");
            foreach (var candidate in candidates)
            {
                Console.Write(candidate.Key.GetType().Name.PadRight(35));
                long sum = 0;
                foreach (var result in candidate.Value)
                {
                    Console.Write(result + "ms ");
                    sum += result;
                }
                Console.WriteLine();

                Console.WriteLine("  avg: {0:0}ms", ((double)sum) / trials);
                Console.WriteLine();
            }

//            Console.ReadLine();
        }

        // Tests:
        // 1) Expand statements into a new fixedforforstat block, unless parent is For or type is block.
        // 2) Convert integers to binop with int and bool
        // 3) Repeat twice (external).
        internal class DynInvoke : LambdaVisitorDynInvoke, IVisitor
        {
            public DynInvoke()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class DynCast : LambdaVisitorDynCast, IVisitor
        {
            public DynCast()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class DynInvokeNoGenerics : LambdaVisitorDynInvokeNoGen, IVisitor
        {
            public DynInvokeNoGenerics()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class DynCastNoGenerics : LambdaVisitorDynCastNoGen, IVisitor
        {
            public DynCastNoGenerics()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class DynCastNoGenerics2 : LambdaVisitorDynCastNoGen, IVisitor
        {
            public DynCastNoGenerics2()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class DynInvokeOld : OldVisitor, IVisitor
        {
            public DynInvokeOld()
            {
                View<For>(VisitChildren);
                ReplaceIf<Statement>(s => !(s is Block), s => Trees.ForFixedForAndStatementBlock());
                Replace<Int, Expression>(i => Trees.IntegerBooleanBinOp());
            }
        }

        internal class ReflectionLookup : Visitor, IVisitor
        {
            public For Visit(For node)
            {
                VisitChildren(node);
                return node;
            }

            public Block Visit(Block node)
            {
                VisitChildren(node);
                return node;
            }

            public Statement Visit(Statement node)
            {
                return Trees.ForFixedForAndStatementBlock();
            }

            public Expression Visit(Int node)
            {
                return Trees.IntegerBooleanBinOp();
            }
        }

        internal interface IVisitor
        {
            T Visit<T>(T node) where T : Node;
            void VisitChildren(Node node);
        }
    }
}
