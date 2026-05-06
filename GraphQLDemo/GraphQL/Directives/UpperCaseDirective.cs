using HotChocolate.Types;
using HotChocolate.Resolvers;

namespace GraphQLDemo.GraphQL.Directives;

/// <summary>
/// Custom directive - demonstrates GraphQL Directives
/// Directives are used to modify the execution behavior of fields
/// Usage: @uppercase on string fields
/// </summary>
public class UpperCaseDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
    {
        descriptor
            .Name("uppercase")
            .Description("Converts the string result to uppercase")
            .Location(DirectiveLocation.FieldDefinition);
    }
}

public class UpperCaseDirective
{
    public static string Apply(IResolverContext context, string value)
    {
        return value.ToUpper();
    }
}
