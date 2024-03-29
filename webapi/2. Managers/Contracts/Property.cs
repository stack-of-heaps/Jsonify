﻿namespace webapi.Managers.Contracts
{
    public class Property
    {
        public string Assembly { get; set; }

        public int Depth { get; set; }

        public string DisplayName { get; set; }

        public List<Property> Properties { get; set; }

        public PropertyTypes PropertyType { get; set; }

        public string[]? EnumeratedProperties { get; set; }
    }
}
