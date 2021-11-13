using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// Source: https://github.com/domaindrivendev/Swashbuckle.AspNetCore#add-security-definitions-and-requirements
public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        context.ApiDescription.TryGetMethodInfo(out var methodInfo);

        if (methodInfo == null)
        {
            return;
        }

        var hasAuthorizeAttribute = false;
        if (methodInfo.MemberType == MemberTypes.Method)
        {
            // NOTE: Check the controller itself has Authorize attribute
            hasAuthorizeAttribute = methodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false;

            if (hasAuthorizeAttribute)
            {
                // NOTE: Controller has Authorize attribute, so check the endpoint itself.
                // Take into account the allow anonymous attribute
                hasAuthorizeAttribute = !methodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();
            }
            else
            {
                hasAuthorizeAttribute = methodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            }
        }

        if (!hasAuthorizeAttribute)
        {
            return;
        }

        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }

        if (!operation.Responses.ContainsKey("403"))
        {
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }

        // NOTE: This adds the "Padlock" icon to the endpoint in swagger,
        //       we can also pass through the names of the policies in the string[]
        //       which will indicate which permission you require.
        var oAuthScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
        };

        operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [oAuthScheme] = System.Array.Empty<string>(),
                },
            };
    }
}
