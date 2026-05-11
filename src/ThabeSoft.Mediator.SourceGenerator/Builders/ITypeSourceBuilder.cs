using Microsoft.CodeAnalysis;
using ThabeSoft.Mediator.SourceGenerator.Models;

namespace ThabeSoft.Mediator.SourceGenerator.Builders;

public interface ITypeSourceBuilder
{
    void Build(SourceProductionContext context, IReadOnlyCollection<TypeRegistration> infos);
}
