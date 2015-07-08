using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class LambdaVisitorTests
    {
        [TestMethod]
        public void VisitsChildNodes()
        {
            IncNode.StartId = 0;
            
            // Binop with two ints both set to 0.
            var tree = Trees.IntegerBinOp();
            Assert.IsTrue(tree.Left.Id == 1 && tree.Right.Id == 2);

            int replacements = 0;

            // Register integer <> bool swap handler.
            var visitor = new LambdaVisitor();
            visitor.Add((Int i) => { replacements++; return new Bool(); });
            tree = visitor.Visit(tree);

            Assert.IsNotNull(tree.Left);
            Assert.IsNotNull(tree.Right);
            Assert.IsInstanceOfType(tree.Left, typeof(Bool));
            Assert.IsInstanceOfType(tree.Right, typeof(Bool));
            Assert.IsTrue(tree.Left.Id == 3 && tree.Right.Id == 4);
            Assert.IsTrue(replacements == 2);
        }

        [TestMethod]
        public void DoesNotReplaceIfNotApplicable()
        {
            // Fixed for with ints loaded and empty statement.
            var tree = Trees.ForFixed();

            int replacements = 0;

            // Register integer <> bool swap handler.
            var visitor = new LambdaVisitor();
            visitor.Add((Int i) => { replacements++; return new Bool(); });
            visitor.Visit(tree);

            Assert.IsTrue(replacements == 0);

            // Complex tree example, fixed tree does not replace ints.
            var complexTree = Trees.ForFixedForAndStatementBlock();
            complexTree = visitor.Visit(complexTree);

            Assert.IsTrue(replacements == 3);
        }
        

        public class StatementIntExprFunctions
        {
            // In this trav we alternatingly replace statements with For and ForFixed subtrees,
            // replace any integer with a bool and count the number of times the expression handler
            // hits.
            private bool nextIsFor = true;

            public int ExprHit = 0;

            [TravMethod]
            Statement Transform(Statement statement)
            {
                Statement ret;
                if (nextIsFor)
                {
                    var fo = Trees.For();
                    fo.Do = null;
                    ret = fo;
                }
                else
                {
                    var ff = Trees.ForFixed();
                    ff.Do = null;
                    ret = ff;
                }

                nextIsFor = !nextIsFor;
                return ret;
            }

            [TravMethod]
            Bool Transform(Int node)
            {
                return new Bool();
            }

            [TravMethod]
            void View(Expression expr)
            {
                ExprHit++;
            }
        }
        [TestMethod]
        public void AcceptsMethods()
        {
            // Functions can also be passed in from other classes, i.e. you don't
            // need to inherit from LambdaVisitor.
            var visitor = new LambdaVisitor();
            var funs = new StatementIntExprFunctions();

            // Other handlers can be specified inline.
            visitor.Add((For f) => visitor.VisitChildren(f));
            visitor.Add((ForFixed f) => visitor.VisitChildren(f));
            visitor.AddTravMethods(funs);

            // Block with 3 statements.
            var tree = Trees.BlockWithStatements();

            tree = visitor.Visit(tree); // will replace all statements with for's.

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.Children().OfType<For>().Count() == 2);
            Assert.IsTrue(tree.Children().OfType<ForFixed>().Count() == 1);

            // Running the visitor again should replace all ints in For with bools, and
            // count the ones in the ForFixed.
            tree = visitor.Visit(tree);
            
            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.AllChildren().OfType<Int>().Count() == 3);
            Assert.IsTrue(tree.AllChildren().OfType<Bool>().Count() == 6);
        }


        // Sample object hierarchy without common ancestor.
        class ObjA
        {
            [Child] public object Prop1 { get; set; }
            [Child] public ObjB Prop2 { get; set; }
        }

        class ObjB { }
        class ObjC { }
        [TestMethod]
        public void IndependentFromRoot()
        {
            var visitor = new LambdaVisitor();
            visitor.Add((ObjB b) => new ObjC());
            
            // Prop2 is bound to ObjB so wont receive ObjC.
            var a = new ObjA { Prop1 = new ObjB(), Prop2 = new ObjB() };
            visitor.Visit(a);

            Assert.IsTrue(a.Children().OfType<ObjB>().Count() == 1);
            Assert.IsTrue(a.Children().OfType<ObjC>().Count() == 1);
        }
        [TestMethod]
        public void AllowsHighLevelInspection()
        {
            int count = 0;
            var visitor = new LambdaVisitor();

            // Function valid on all objects.
            visitor.Add((object o) => { count++; visitor.VisitChildren(o); });

            var a = new ObjA { Prop1 = new ObjB(), Prop2 = new ObjB() };
            visitor.Visit(a);

            Assert.IsTrue(count == 3);
        }
    }
}
