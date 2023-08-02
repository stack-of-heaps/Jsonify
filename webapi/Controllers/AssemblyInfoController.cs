namespace webapi.Controllers;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using webapi.DTOs;

[ApiController]
[Route("[controller]")]
public class AssemblyInfoController : ControllerBase
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

    [EnableCors]
    [HttpGet("products", Name = "Products")]
    public string[] GetProducts()
    {
        return Enum.GetNames<ProductNames>();
    }

    [EnableCors]
    [HttpGet("services", Name = "Services")]
    public string[] GetServices()
    {
        return Enum.GetNames<ServiceNames>();
    }

    [EnableCors]
    [HttpGet("classes")]
    public IActionResult GetClasses(ServiceNames? serviceName, ProductNames? productName)
    {
        if (serviceName == null && productName == null)
        {
            return BadRequest(
                $"You must provide at least one queryParameter. " +
                $"Both {nameof(ServiceNames)} and {nameof(ProductNames)} cannot be null.");
        }

        return Ok(GetGetClassesResponse(serviceName, productName));
    }

    [EnableCors]
    [HttpGet("classes/{fullClassName}/properties")]
    public IActionResult GetClassProperties(string fullClassName, string @namespace)
    {
        if (fullClassName == null)
        {
            return BadRequest($"{nameof(fullClassName)} cannot be null.");
        }

        if (@namespace == null)
        {
            return BadRequest("namespace cannot be null.");
        }

        var assembly = Assembly.Load(@namespace);

        if (assembly == null)
        {
            return BadRequest("Invalid namespace.");
        }

        var type = assembly.GetType(fullClassName);

        if (type == null)
        {
            return BadRequest("Invalid type.");
        }

        return Ok(GetProperties(type));
    }

    private Property GetProperties(Type type, string name = null, int depth = 0)
    {
        if (name == null)
        {
            name = type.Name;
        }

        var nullablePropertyType = Nullable.GetUnderlyingType(type);
        var propertyType = string.Empty;
        var isNullable = false;

        if (nullablePropertyType != null)
        {
            type = nullablePropertyType;
            propertyType = nullablePropertyType.Name;
            isNullable = true;
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
                Nullable = isNullable,
                // FullName = type.FullName,
                PropertyType = PropertyTypes.Enum,
                Type = propertyType,
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
                Nullable = isNullable,
                PropertyType = PropertyTypes.List,
                Type = propertyType,
                Properties = item.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name, depth + 1)).ToList()
            };
        }

        var property = new Property
        {
            Assembly = type.Module.Name,
            Depth = depth,
            DisplayName = name,
            Nullable = isNullable,
            // FullName = type.FullName,
            PropertyType = GetPropertyType(propertyType),
            Type = propertyType
        };


        // WARNING!
        // If this hits a class which uses recursion--e.g., V3 BaseQuestion, it will never end and throw stackoverflow exception.
        if (type.Namespace != "System")
        {
            property.Properties = type.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name, depth + 1)).ToList();
        };

        return property;
    }

    private GetClassesResponse GetGetClassesResponse(ServiceNames? serviceName, ProductNames? productName)
    {
        var assemblies = new List<Assembly>();

        if (!serviceName.HasValue)
        {
            foreach (var service in Enum.GetValues<ServiceNames>())
            {
                assemblies.Add(Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(service)));
            };
        }
        else
        {
            assemblies.Add(Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(serviceName.Value)));
        }

        return new GetClassesResponse { Classes = GetClassInfo(assemblies, serviceName, productName).ToList() };
    }

    private IEnumerable<ClassInfo> GetClassInfo(List<Assembly> assemblies, ServiceNames? serviceName, ProductNames? productName)
    {
        var productSearchFilter = GetProductSearchFilters(productName);

        foreach (var assembly in assemblies)
        {
            var assemblyFilter = GetAssemblyFilter(serviceName);

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
                    Version = GetVersion(fullName)
                };
            }
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

    private string GetVersion(string fullClassName)
    {
        if (fullClassName.Contains("V2") || fullClassName.Contains("Api"))
        {
            return "2";
        }

        if (fullClassName.Contains("V3"))
        {
            return fullClassName.Contains("Quote")
                ? "2"
                : "3";
        }

        return "1";
    }

    private List<string> GetAssemblyFilter(ServiceNames? serviceName)
    {
        if (!serviceName.HasValue)
        {
            return new List<string>();
        }

        var assemblyFilters = new List<string> { ServiceToNamespaceLookup[serviceName.Value] };
        switch (serviceName)
        {
            case ServiceNames.Api:
            case ServiceNames.Occupation:
                break;

            case ServiceNames.Forms:
                assemblyFilters.Add("Assurity.Forms.V2.Populate.Contracts");
                break;

            case ServiceNames.NBFrameworkRestAPI:
                assemblyFilters.Add("Assurity.NBFrameworkRestApi.V2.Contracts");
                break;

            case ServiceNames.Questions:
                assemblyFilters.Add("Assurity.Questions.V2.Contracts");
                break;

            case ServiceNames.Quote:
                assemblyFilters.Add("Assurity.Quote.V3.Contracts");
                break;

            case ServiceNames.Underwriting:
                assemblyFilters.Add("Assurity.Underwriting.V2.Contracts");
                break;
        }

        return assemblyFilters;
    }

    private PropertyTypes GetPropertyType(string property)
    {
        if (property.Contains("Int"))
        {
            return PropertyTypes.Integer;
        }

        switch (property)
        {
            case "String":
                return PropertyTypes.String;

            case "Decimal":
                return PropertyTypes.Decimal;

            case "Boolean":
                return PropertyTypes.Boolean;

            default:
                return PropertyTypes.Object;
        }
    }
}
