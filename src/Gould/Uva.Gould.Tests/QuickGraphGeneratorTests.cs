﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Graphing;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class QuickGraphGeneratorTests
    {
        /* Simple inheritance hierarchy */
        public abstract class Expr { }
        public abstract class Stat { }

        public class Program
        {
            [Child] public SymTable SymbolTable { get; set; }
            [Child] public Sll<Function> Functions { get; set; } 
        }

        public class Function
        {
            [Child] public SymTable SymbolTable { get; set; }
            [Child] public Sll<Stat> Statements { get; set; }
        }

        public class If : Stat
        {
            [Child] public Sll<Stat> Then { get; set; } 
            [Child] public Sll<Stat> Else { get; set; } 
        }

        public class Ret : Stat
        {
            [Child] public Expr Expr { get; set; }
        }

        public class Int : Expr { }
        public class Bool : Expr { }

        public class SymTable { }

        [TestMethod]
        public void TestMethod()
        {
            var g = new GraphGenerator().CreateAstGraph(typeof (Program));
            GraphWriter.WriteToFile("AST.DOT", g);
        }
    }
}
