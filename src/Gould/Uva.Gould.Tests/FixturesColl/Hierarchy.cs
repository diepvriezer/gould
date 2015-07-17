using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uva.Gould.Tests.FixturesColl
{
    public class SomeNode
    {
        [Child] public object Prop { get; set; }
        [Child] public List<object> Collection { get; set; } 
    }
}
