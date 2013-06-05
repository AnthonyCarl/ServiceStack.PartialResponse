﻿using System.Collections.Generic;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class FakeDto
    {
        public FakePerson Person { get; set; }
        public decimal Salary { get; set; }
        public List<FakeLink> Links { get; set; }
    }
}