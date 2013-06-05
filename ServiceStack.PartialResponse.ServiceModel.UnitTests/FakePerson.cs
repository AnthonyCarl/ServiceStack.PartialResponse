using System.Collections.Generic;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class FakePerson
    {
        public FakeName Name { get; set; }
        public List<FakeName> ChildrenNames { get; set; }
    }
}