namespace webapi.Managers
{
    using System.Reflection;
    using webapi.Managers.Contracts;

    public class ClassManager : IClassManager
    {
        private Dictionary<ServiceNames, string> ServiceToNamespaceLookup => new Dictionary<ServiceNames, string>
        {
            { ServiceNames.Api, "Assurity.Api.Contracts" },
            { ServiceNames.Forms, "Assurity.Forms.Populate.Contracts" },
            { ServiceNames.NBFrameworkRestAPI, "Assurity.NBFrameworkRestAPI.Contracts" },
            { ServiceNames.Occupation, "Assurity.Occupation.Contracts" },
            { ServiceNames.Questions, "Assurity.Questions.Contracts" },
            { ServiceNames.Quote, "Assurity.Quote.Contracts" },
            { ServiceNames.Underwriting, "Assurity.Underwriting.Contracts" }
        };

        private Dictionary<ProductNames, List<string>> ProductNamesSearchTermsLookup => new Dictionary<ProductNames, List<string>>
        {
            { ProductNames.AccidentGoToMarket, new List<string> { "GoToMarket" } },
            { ProductNames.AccidentFlex, new List <string> { "Flex" } },
            { ProductNames.AccidentalDeath, new List<string>{ "AccidentalDeath" } },
            { ProductNames.CenturyPlusDisabilityIncome, new List<string>{ "CenturyPlusDisabilityIncome" } },
            { ProductNames.CriticalIllness, new List<string>{ "CriticalIllness" } },
            { ProductNames.CriticalIllnessDirect, new List < string > { "CriticalIllnessDirect" } },
            { ProductNames.IncomeProtection, new List < string > { "IncomeProtection" } },
            { ProductNames.TermLife, new List < string > { "TermLife" } },
            { ProductNames.TermLifeDeveloperEdition, new List<string>{ "TermLifeLowStrain", "TermLifeDeveloperEdition" } }
        };

        public GetClassesResponse GetClasses(ServiceNames? serviceName, ProductNames? productName)
        {
            var classes = GetClassInfo(serviceName, productName).ToList();

            return new GetClassesResponse { Classes = classes };
        }

        public Property GetProperties(Type type, string name = null, int depth = 0)
        {
            if (name == null)
            {
                name = type.Name;
            }

            var nullablePropertyType = Nullable.GetUnderlyingType(type);
            var propertyType = string.Empty;

            if (nullablePropertyType != null)
            {
                type = nullablePropertyType;
                propertyType = nullablePropertyType.Name;
            }
            else
            {
                propertyType = type.UnderlyingSystemType.Name;
            }

            if (type.IsEnum)
            {
                var enumValues = Enum.GetNames(type);
                return new Property
                {
                    DisplayName = name,
                    Assembly = type.Module.Name,
                    Depth = depth,
                    PropertyType = PropertyTypes.Enum,
                    EnumeratedProperties = enumValues
                };
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var item = type.GetProperties().First(prop => prop.Name == "Item").PropertyType;
                return new Property
                {
                    Assembly = type.Module.Name,
                    DisplayName = name,
                    Depth = depth,
                    PropertyType = PropertyTypes.List,
                    Properties = item.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name, depth + 1)).ToList()
                };
            }

            var property = new Property
            {
                Assembly = type.Module.Name,
                Depth = depth,
                DisplayName = name,
                PropertyType = GetPropertyType(propertyType)
            };

            // WARNING!
            // If this hits a class which uses recursion--e.g., V3 BaseQuestion, it will never end and throw stackoverflow exception.
            if (type.Namespace != "System")
            {
                property.Properties = type.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name, depth + 1)).ToList();
            };

            return property;
        }

        private PropertyTypes GetPropertyType(string property)
        {
            if (property.Contains("Int32"))
            {
                return PropertyTypes.Integer;
            }

            switch (property)
            {
                case "Boolean":
                    return PropertyTypes.Boolean;

                case "DateTime":
                    return PropertyTypes.DateTime;

                case "Decimal":
                    return PropertyTypes.Decimal;

                case "String":
                    return PropertyTypes.String;

                default:
                    return PropertyTypes.Object;
            }
        }

        private IEnumerable<ClassInfo> GetClassInfo(ServiceNames? serviceName, ProductNames? productName)
        {
            var productSearchFilter = GetProductSearchFilters(productName);
            var assemblies = GetAssemblies(serviceName, productName);

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (productSearchFilter.Any() && !productSearchFilter.Any(filter => type.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var fullName = type.FullName;
                    var contractsPosition = fullName.LastIndexOf("Contracts");
                    if (contractsPosition == -1)
                    {
                        continue;
                    }

                    var product = fullName.Substring(contractsPosition).Split('.')[1];
                    var assemblyName = assembly.FullName.Split(',')[0];

                    yield return new ClassInfo
                    {
                        DisplayName = type.Name,
                        Product = product,
                        FullName = fullName,
                        Namespace = assemblyName,
                    };
                }
            }
        }

        private IEnumerable<Assembly> GetAssemblies(ServiceNames? serviceName, ProductNames? productName)
        {
            if (!serviceName.HasValue)
            {
                foreach (var service in Enum.GetValues<ServiceNames>())
                {
                    yield return Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(service));
                };
            }
            else
            {
                yield return Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(serviceName.Value));
            }
        }

        private List<string> GetProductSearchFilters(ProductNames? productName)
        {
            var productSearchFilter = new List<string>();

            if (productName.HasValue)
            {
                productSearchFilter = ProductNamesSearchTermsLookup.GetValueOrDefault(productName.Value);
            }

            return productSearchFilter;
        }
    }
}
