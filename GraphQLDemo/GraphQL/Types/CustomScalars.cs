using HotChocolate.Language;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL.Types;

/// <summary>
/// Custom Scalar Type for URLs - demonstrates GraphQL Custom Scalars
/// Scalars are primitive types in GraphQL (String, Int, Float, Boolean, ID)
/// You can create custom scalars for special data types
/// </summary>
public class CustomUrlType : ScalarType<Uri, StringValueNode>
{
    public CustomUrlType() : base("URL")
    {
        Description = "Represents a valid URL string";
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is null)
            return NullValueNode.Default;

        if (resultValue is string s)
            return new StringValueNode(s);

        if (resultValue is Uri uri)
            return new StringValueNode(uri.ToString());

        throw new SerializationException(
            "Cannot serialize the given value as URL.",
            this);
    }

    protected override Uri ParseLiteral(StringValueNode valueSyntax)
    {
        if (Uri.TryCreate(valueSyntax.Value, UriKind.Absolute, out var uri))
            return uri;

        throw new SerializationException(
            "Cannot parse the given value as URL.",
            this);
    }

    protected override StringValueNode ParseValue(Uri runtimeValue)
    {
        return new StringValueNode(runtimeValue.ToString());
    }
}

/// <summary>
/// Custom Scalar Type for Email addresses
/// </summary>
public class EmailType : ScalarType<string, StringValueNode>
{
    public EmailType() : base("Email")
    {
        Description = "Represents a valid email address";
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is null)
            return NullValueNode.Default;

        if (resultValue is string s)
            return new StringValueNode(s);

        throw new SerializationException(
            "Cannot serialize the given value as Email.",
            this);
    }

    protected override string ParseLiteral(StringValueNode valueSyntax)
    {
        if (IsValidEmail(valueSyntax.Value))
            return valueSyntax.Value;

        throw new SerializationException(
            "Cannot parse the given value as Email.",
            this);
    }

    protected override StringValueNode ParseValue(string runtimeValue)
    {
        if (IsValidEmail(runtimeValue))
            return new StringValueNode(runtimeValue);

        throw new SerializationException(
            "Cannot parse the given value as Email.",
            this);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
