namespace webapi.DTOs
{
    public class Property
    {
        // public string Assembly { get; set; }

        // public string FullName { get; set; }

        public string DisplayName { get; set; }

        public List<Property> Properties { get; set; }

        public string Type { get; set; }

        public bool IsEnum { get; set; }

        public bool IsCollection { get; set; }

        public string[]? EnumeratedProperties { get; set; }
    }
}
