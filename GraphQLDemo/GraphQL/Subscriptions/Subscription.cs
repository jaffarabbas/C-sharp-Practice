using System.Runtime.CompilerServices;
using GraphQLDemo.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQLDemo.GraphQL.Mutations;

/// <summary>
/// Subscription type - demonstrates GraphQL Subscriptions for real-time updates
/// Subscriptions use WebSocket to push updates to clients when data changes
/// </summary>
public class Subscription
{
    // CONCEPT 1: Simple subscription
    /// <summary>
    /// Subscribe to new books being added
    /// Example:
    /// subscription {
    ///   onBookAdded {
    ///     id
    ///     title
    ///     author { name }
    ///   }
    /// }
    /// </summary>
    [Subscribe]
    [Topic(nameof(OnBookAdded))]
    public Book OnBookAdded([EventMessage] Book book) => book;

    // CONCEPT 2: Subscription with filtering
    /// <summary>
    /// Subscribe to new reviews being added
    /// Example:
    /// subscription {
    ///   onReviewAdded {
    ///     id
    ///     rating
    ///     comment
    ///     book { title }
    ///   }
    /// }
    /// </summary>
    [Subscribe]
    [Topic(nameof(OnReviewAdded))]
    public Review OnReviewAdded([EventMessage] Review review) => review;

    // CONCEPT 3: Subscription with arguments
    /// <summary>
    /// Subscribe to reviews for a specific book
    /// Example:
    /// subscription {
    ///   onReviewAddedForBook(bookId: 1) {
    ///     id
    ///     rating
    ///     comment
    ///   }
    /// }
    /// </summary>
    [Subscribe]
    public async ValueTask<ISourceStream<Review>> OnReviewAddedForBookAsync(
        int bookId,
        [Service] ITopicEventReceiver eventReceiver)
    {
        return await eventReceiver.SubscribeAsync<Review>(
            $"OnReviewAddedForBook_{bookId}");
    }

    // CONCEPT 4: Time-based subscription (simulated real-time data)
    /// <summary>
    /// Subscribe to a stream of random numbers (demonstrates streaming data)
    /// Example:
    /// subscription {
    ///   randomNumber
    /// }
    /// </summary>
    [Subscribe]
    public async IAsyncEnumerable<int> RandomNumber(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var random = new Random();
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
            yield return random.Next(1, 100);
        }
    }

    // CONCEPT 5: Clock subscription
    /// <summary>
    /// Subscribe to current time updates every second
    /// Example:
    /// subscription {
    ///   currentTime
    /// }
    /// </summary>
    [Subscribe]
    public async IAsyncEnumerable<DateTime> CurrentTime(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return DateTime.UtcNow;
            await Task.Delay(1000, cancellationToken);
        }
    }
}
