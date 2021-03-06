﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.PerfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            const int iters = 500;
            const int trials = 3;

            var stopwatch = new Stopwatch();

            var candidates = new List<IVisitor>()
            {
                new ReflectionLookup(),
                new DynCastNoGenerics(),
            }.ToDictionary(c => c, c => new List<long>(trials));
            
            Thread.Sleep(250);
            
            for (int i = 0; i < trials; i++)
            {
                foreach (var candidate in candidates)
                {
                    Console.Write("Testing " + candidate.Key.GetType().Name + "... ");
                    stopwatch.Start();

                    for (int j = 0; j < iters; j++)
                    {
                        Node tree = Trees.ForFixedForAndStatementBlock();
                        tree = candidate.Key.Visit(tree);
                        candidate.Key.Visit(tree);
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

            Console.ReadKey();
        }

        // Tests:
        // 1) Expand statements into a new fixedforforstat block, unless parent is For or type is block.
        // 2) Convert integers to binop with int and bool
        // 3) Repeat twice (external).
        internal class DynCastNoGenerics : LambdaVisitor, IVisitor
        {
            public DynCastNoGenerics()
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
