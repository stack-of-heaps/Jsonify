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

    private Dictionary<ProductNames, string[]> ProductNamesSearchTermsLookup => new Dictionary<ProductNames, string[]>
        {
            { ProductNames.AccidentGoToMarket, new string[1]{ "GoToMarket" } },
            { ProductNames.AccidentFlex, new string[1]{"Flex" } },
            { ProductNames.AccidentalDeath, new string[1]{ "AccidentalDeath" } },
            { ProductNames.CenturyPlusDisabilityIncome, new string[1]{ "CenturyPlusDisabilityIncome" } },
            { ProductNames.CriticalIllness, new string[1]{ "CriticalIllness" } },
            { ProductNames.CriticalIllnessDirect, new string[1]{ "CriticalIllnessDirect" } },
            { ProductNames.IncomeProtection, new string[1]{ "IncomeProtection" } },
            { ProductNames.TermLife, new string[1]{ "TermLife" } },
            { ProductNames.TermLifeLowStrain, new string[1]{ "TermLifeLowStrain" } }
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
            return BadRequest($"You must provide at least one queryParameter. Both {serviceName} and {productName} cannot be null.");
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

        var properties = type.GetProperties();

        return Ok(GetProperties(type));
    }

    private Property GetProperties(Type type, string name = null)
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
                // Assembly = type.Module.Name,
                // FullName = type.FullName,
                Type = propertyType,
                IsEnum = true,
                EnumeratedProperties = enumValues
            };
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var item = type.GetProperties().First(prop => prop.Name == "Item").PropertyType;
            return new Property
            {
                DisplayName = name,
                IsCollection = true,
                Type = propertyType,
                Properties = item.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name)).ToList()
            };
        }

        var property = new Property
        {
            DisplayName = name,
            // Assembly = type.Module.Name,
            // FullName = type.FullName,
            Type = propertyType
        };


        if (type.Namespace != "System")
        {
            property.Properties = type.GetProperties().Select(prop => GetProperties(prop.PropertyType, prop.Name)).ToList();
        };

        return property;
    }

    private GetClassesResponse GetGetClassesResponse(ServiceNames? serviceName, ProductNames? productName)
    {
        var assemblies = new List<Assembly>();

        if (!serviceName.HasValue)
        {
            assemblies = GetAllAssemblies();
        }
        else
        {
            assemblies.Add(Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(serviceName.Value)));
        }

        return new GetClassesResponse { Classes = GetClassInfo(assemblies, productName).ToList() };
    }

    private List<Assembly> GetAllAssemblies()
    {
        var assemblies = new List<Assembly>();

        foreach (var service in Enum.GetValues<ServiceNames>())
        {
            assemblies.Add(Assembly.Load(ServiceToNamespaceLookup.GetValueOrDefault(service)));
        }

        return assemblies;
    }

    private IEnumerable<ClassInfo> GetClassInfo(List<Assembly> assemblies, ProductNames? productName)
    {
        var productSearchFilter = GetProductSearchFilters(productName);

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!productSearchFilter.Any(filter => type.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                yield return new ClassInfo
                {
                    DisplayName = type.Name,
                    FullName = type.FullName,
                    Namespace = assembly.FullName.Split(',')[0],
                    Version = GetVersion(type.FullName)
                };
            }
        }
    }

    private string[] GetProductSearchFilters(ProductNames? productName)
    {
        var productSearchFilter = Array.Empty<string>();

        if (productName.HasValue)
        {
            productSearchFilter = ProductNamesSearchTermsLookup.GetValueOrDefault(productName.Value);
        }

        return productSearchFilter;
    }

    private string GetVersion(string fullClassName)
    {
        if (fullClassName.Contains("Assurity.Api", StringComparison.OrdinalIgnoreCase)
            || fullClassName.Contains("v2", StringComparison.OrdinalIgnoreCase))
        {
            return "2";
        }

        return "1";
    }
}
