using System.Collections.Generic;

namespace ServiceStack.PartialResponse.ServiceModel.UnitTests
{
    public class FakePerson
    {
        public FakeName Name { get; set; }
        public List<FakeName> ChildrenNames { get; set; }
    }
}