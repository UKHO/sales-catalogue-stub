using System;

namespace MyNamespace
{
    public class Products
    {
        public Guid ID { get; set; }

        public string Identifier { get; set; }

        public Guid ProductTypeID { get; set; }

        public Guid? ProductSourceFeedID { get; set; }

        public DateTime? LastUpdated { get; set; }

    }
}