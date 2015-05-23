using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class LambdaVisitorTests : LambdaVisitorDynInvoke
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
            RemoveAllHandlers();
            Replace<Int, Expression>(i =>
            {
                replacements++;
                return new Bool();
            });

            Visit(tree);

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
            RemoveAllHandlers();
            Replace<Int, Expression>(i =>
            {
                replacements++;
                return new Bool();
            });

            Visit(tree);

            Assert.IsTrue(replacements == 0);

            // Complex tree example, fixed tree does not replace ints.
            var complexTree = Trees.ForFixedForAndStatementBlock();
            Visit(complexTree);

            Assert.IsTrue(replacements == 3);
        }
    }
}
