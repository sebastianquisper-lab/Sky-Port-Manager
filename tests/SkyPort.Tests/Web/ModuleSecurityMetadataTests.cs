using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyPort.Application.Common.Constants;
using SkyPort.Web.Controllers;

namespace SkyPort.Tests.Web;

public sealed class ModuleSecurityMetadataTests
{
    [Fact]
    public void TerminalAndGateWritesRequireAdminAndAntiforgeryOnPosts()
    {
        AssertWriteActions<TerminalesController>(Policies.SoloAdmin, "Create", "Edit", "Delete");
        AssertWriteActions<PuertasController>(Policies.SoloAdmin, "Create", "Edit", "Delete");
        AssertReadActions<TerminalesController>(Policies.GestionOperaciones, "Index", "Details");
        AssertReadActions<PuertasController>(Policies.GestionOperaciones, "Index", "Details");
    }

    [Fact]
    public void BaggageAndPersonnelUseTheirConfirmedPolicies()
    {
        AssertControllerPolicy<EquipajeController>(Policies.TodosLosRoles);
        AssertControllerPolicy<PersonalController>(Policies.GestionOperaciones);
        AssertWriteActions<PersonalController>(Policies.SoloAdmin, "Create", "Edit", "Delete");
    }

    private static void AssertWriteActions<TController>(string policy, params string[] actionNames)
    {
        foreach (var actionName in actionNames)
        {
            var methods = typeof(TController).GetMethods()
                .Where(method => method.Name == actionName ||
                    method.GetCustomAttributes(typeof(ActionNameAttribute), true)
                        .Cast<ActionNameAttribute>().Any(attribute => attribute.Name == actionName))
                .ToArray();
            Assert.NotEmpty(methods);
            foreach (var method in methods)
            {
                Assert.Contains(method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>(),
                    attribute => attribute.Policy == policy);
                if (method.GetCustomAttributes(typeof(HttpPostAttribute), true).Length > 0)
                    Assert.NotEmpty(method.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true));
            }
        }
    }

    private static void AssertReadActions<TController>(string policy, params string[] actionNames)
    {
        foreach (var actionName in actionNames)
        {
            var method = typeof(TController).GetMethods().FirstOrDefault(candidate => candidate.Name == actionName);
            Assert.NotNull(method);
            Assert.Contains(method.GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>(),
                attribute => attribute.Policy == policy);
        }
    }

    private static void AssertControllerPolicy<TController>(string policy)
    {
        Assert.Contains(typeof(TController).GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>(),
            attribute => attribute.Policy == policy);
    }
}
