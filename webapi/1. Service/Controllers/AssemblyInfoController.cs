namespace webapi.Controllers;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using webapi.Managers;
using webapi.Managers.Contracts;

[ApiController]
[Route("[controller]")]
public class AssemblyInfoController : ControllerBase
{
    public AssemblyInfoController(IClassManager classManager)
    {
        ClassManager = classManager;
    }

    private IClassManager ClassManager { get; set; }

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

        return Ok(ClassManager.GetClasses(serviceName, productName));
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

        var properties = ClassManager.GetProperties(type);

        return Ok(properties);
    }
}
