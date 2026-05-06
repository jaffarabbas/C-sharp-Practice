using GraphQLDemo.GraphQL.Mutations;
using HotChocolate;

namespace GraphQLDemo.GraphQL.Types;

/// <summary>
/// Input validators - demonstrates validation in GraphQL
/// These validators run before the mutation executes
/// </summary>
public static class InputValidators
{
    /// <summary>
    /// Validates book input
    /// </summary>
    public static void ValidateAddBookInput(AddBookInput input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Title))
            errors.Add("Title is required");

        if (input.Title.Length > 200)
            errors.Add("Title must be 200 characters or less");

        if (input.Pages <= 0)
            errors.Add("Pages must be greater than 0");

        if (input.Price < 0)
            errors.Add("Price cannot be negative");

        if (string.IsNullOrWhiteSpace(input.Description))
            errors.Add("Description is required");

        if (errors.Any())
            throw new GraphQLException(string.Join("; ", errors));
    }

    /// <summary>
    /// Validates review input
    /// </summary>
    public static void ValidateAddReviewInput(AddReviewInput input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.ReviewerName))
            errors.Add("Reviewer name is required");

        if (input.Rating < 1 || input.Rating > 5)
            errors.Add("Rating must be between 1 and 5");

        if (string.IsNullOrWhiteSpace(input.Comment))
            errors.Add("Comment is required");

        if (input.Comment.Length < 10)
            errors.Add("Comment must be at least 10 characters");

        if (errors.Any())
            throw new GraphQLException(string.Join("; ", errors));
    }

    /// <summary>
    /// Validates author input
    /// </summary>
    public static void ValidateAddAuthorInput(AddAuthorInput input)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Name))
            errors.Add("Name is required");

        if (input.Name.Length > 100)
            errors.Add("Name must be 100 characters or less");

        if (string.IsNullOrWhiteSpace(input.Email))
            errors.Add("Email is required");

        if (!IsValidEmail(input.Email))
            errors.Add("Invalid email format");

        if (input.BirthDate > DateTime.Now.AddYears(-18))
            errors.Add("Author must be at least 18 years old");

        if (string.IsNullOrWhiteSpace(input.Country))
            errors.Add("Country is required");

        if (errors.Any())
            throw new GraphQLException(string.Join("; ", errors));
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
