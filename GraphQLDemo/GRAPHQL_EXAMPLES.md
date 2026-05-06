# GraphQL Examples and Learning Guide

This document provides comprehensive examples of all GraphQL concepts implemented in this project.

## Table of Contents
1. [Getting Started](#getting-started)
2. [Queries](#queries)
3. [Mutations](#mutations)
4. [Subscriptions](#subscriptions)
5. [Advanced Features](#advanced-features)

---

## Getting Started

After running the application, navigate to `http://localhost:5000/graphql` to access the Banana Cake Pop GraphQL IDE.

### Basic Concepts

GraphQL uses a schema to define:
- **Queries**: Read operations (like GET in REST)
- **Mutations**: Write operations (like POST, PUT, DELETE in REST)
- **Subscriptions**: Real-time updates via WebSocket

---

## Queries

### 1. Simple Query
```graphql
query {
  hello
}
```

### 2. Query with Arguments
```graphql
query {
  greet(name: "John")
}
```

### 3. Get All Books
```graphql
query {
  books {
    id
    title
    description
    price
    pages
    publishedDate
    genre
  }
}
```

### 4. Nested Query - Books with Authors
```graphql
query {
  books {
    id
    title
    price
    author {
      id
      name
      email
      country
    }
  }
}
```

### 5. Query with Filtering
```graphql
query {
  books(where: { genre: { eq: FANTASY } }) {
    title
    price
    genre
  }
}
```

### 6. Query with Multiple Filters
```graphql
query {
  books(
    where: {
      and: [
        { price: { lt: 20 } }
        { pages: { gt: 200 } }
      ]
    }
  ) {
    title
    price
    pages
  }
}
```

### 7. Query with Sorting
```graphql
query {
  books(order: { title: ASC }) {
    title
    price
  }
}
```

### 8. Query with Sorting (Multiple Fields)
```graphql
query {
  books(
    order: [
      { price: DESC }
      { title: ASC }
    ]
  ) {
    title
    price
  }
}
```

### 9. Get Single Book by ID
```graphql
query {
  book(id: 1) {
    id
    title
    description
    author {
      name
      email
    }
    reviews {
      reviewerName
      rating
      comment
    }
  }
}
```

### 10. Get All Authors with Their Books
```graphql
query {
  authors {
    id
    name
    country
    books {
      title
      genre
      price
    }
  }
}
```

### 11. Search Books
```graphql
query {
  searchBooks(searchTerm: "Harry") {
    title
    description
    author {
      name
    }
  }
}
```

### 12. Get Books by Genre
```graphql
query {
  booksByGenre(genre: SCIFI) {
    title
    author {
      name
    }
  }
}
```

### 13. Get Book Statistics
```graphql
query {
  bookStats {
    totalBooks
    averagePrice
    totalPages
    mostExpensiveBook
  }
}
```

### 14. Get Reviews with Filtering
```graphql
query {
  reviews(where: { rating: { gte: 4 } }) {
    reviewerName
    rating
    comment
    createdAt
    book {
      title
    }
  }
}
```

### 15. Field Selection (Projection)
Only request the fields you need - GraphQL will only query for those fields:
```graphql
query {
  books {
    title
    price
  }
}
```

### 16. Using Fragments
Reusable pieces of query logic:
```graphql
query {
  book(id: 1) {
    ...BookDetails
  }
}

fragment BookDetails on Book {
  id
  title
  description
  price
  author {
    name
  }
}
```

### 17. Named Queries with Variables
```graphql
query GetBookById($bookId: Int!) {
  book(id: $bookId) {
    title
    price
    author {
      name
    }
  }
}

# Variables (in separate panel):
{
  "bookId": 1
}
```

### 18. Multiple Queries in One Request
```graphql
query {
  allBooks: books {
    title
  }

  fantasyBooks: books(where: { genre: { eq: FANTASY } }) {
    title
  }

  stats: bookStats {
    totalBooks
    averagePrice
  }
}
```

---

## Mutations

### 1. Add a New Book
```graphql
mutation {
  addBook(input: {
    title: "The Hobbit"
    description: "A fantasy adventure"
    pages: 310
    price: 24.99
    genre: FANTASY
    authorId: 1
  }) {
    id
    title
    price
  }
}
```

### 2. Update a Book
```graphql
mutation {
  updateBook(
    id: 1
    input: {
      title: "Harry Potter and the Sorcerer's Stone"
      price: 22.99
    }
  ) {
    id
    title
    price
  }
}
```

### 3. Delete a Book
```graphql
mutation {
  deleteBook(id: 1)
}
```

### 4. Add a New Author
```graphql
mutation {
  addAuthor(input: {
    name: "J.R.R. Tolkien"
    email: "tolkien@example.com"
    birthDate: "1892-01-03"
    country: "United Kingdom"
  }) {
    author {
      id
      name
      email
    }
    success
    message
  }
}
```

### 5. Add a Review
```graphql
mutation {
  addReview(input: {
    bookId: 1
    reviewerName: "Jane Doe"
    rating: 5
    comment: "Absolutely wonderful book! Couldn't put it down."
  }) {
    id
    reviewerName
    rating
    comment
    createdAt
  }
}
```

### 6. Delete Multiple Reviews (Batch Operation)
```graphql
mutation {
  deleteReviews(ids: [1, 2, 3])
}
```

### 7. Named Mutation with Variables
```graphql
mutation AddNewBook($input: AddBookInput!) {
  addBook(input: $input) {
    id
    title
    price
    author {
      name
    }
  }
}

# Variables:
{
  "input": {
    "title": "1984",
    "description": "Dystopian classic",
    "pages": 328,
    "price": 15.99,
    "genre": "FICTION",
    "authorId": 2
  }
}
```

### 8. Multiple Mutations in One Request
```graphql
mutation {
  book1: addBook(input: {
    title: "Book 1"
    description: "Description 1"
    pages: 200
    price: 19.99
    genre: FICTION
    authorId: 1
  }) {
    id
    title
  }

  book2: addBook(input: {
    title: "Book 2"
    description: "Description 2"
    pages: 250
    price: 24.99
    genre: MYSTERY
    authorId: 2
  }) {
    id
    title
  }
}
```

---

## Subscriptions

Subscriptions use WebSocket for real-time updates. You need a WebSocket-enabled client.

### 1. Subscribe to New Books
```graphql
subscription {
  onBookAdded {
    id
    title
    price
    author {
      name
    }
  }
}
```

### 2. Subscribe to New Reviews
```graphql
subscription {
  onReviewAdded {
    id
    rating
    comment
    book {
      title
    }
  }
}
```

### 3. Subscribe to Reviews for Specific Book
```graphql
subscription {
  onReviewAddedForBook(bookId: 1) {
    id
    rating
    comment
    reviewerName
  }
}
```

### 4. Subscribe to Random Numbers (Demo)
```graphql
subscription {
  randomNumber
}
```

### 5. Subscribe to Current Time
```graphql
subscription {
  currentTime
}
```

---

## Advanced Features

### 1. Using Directives

#### @skip - Conditionally skip a field
```graphql
query GetBook($skipAuthor: Boolean!) {
  book(id: 1) {
    title
    price
    author @skip(if: $skipAuthor) {
      name
    }
  }
}

# Variables:
{
  "skipAuthor": false
}
```

#### @include - Conditionally include a field
```graphql
query GetBook($includeReviews: Boolean!) {
  book(id: 1) {
    title
    price
    reviews @include(if: $includeReviews) {
      rating
      comment
    }
  }
}

# Variables:
{
  "includeReviews": true
}
```

### 2. Inline Fragments (for Interfaces)
```graphql
query {
  books {
    id
    title
    ... on IReviewable {
      reviews {
        rating
      }
    }
  }
}
```

### 3. Using Aliases
```graphql
query {
  cheapBooks: books(where: { price: { lt: 15 } }) {
    title
    price
  }

  expensiveBooks: books(where: { price: { gt: 20 } }) {
    title
    price
  }
}
```

### 4. Pagination Examples

#### First N items
```graphql
query {
  books(first: 5) {
    title
    price
  }
}
```

#### Skip and Take
```graphql
query {
  books(skip: 2, first: 3) {
    title
  }
}
```

### 5. Complex Filtering

#### OR conditions
```graphql
query {
  books(
    where: {
      or: [
        { genre: { eq: FANTASY } }
        { genre: { eq: SCIFI } }
      ]
    }
  ) {
    title
    genre
  }
}
```

#### String operations
```graphql
query {
  books(
    where: {
      title: { contains: "Harry" }
    }
  ) {
    title
  }
}
```

#### Range queries
```graphql
query {
  books(
    where: {
      publishedDate: {
        gte: "1990-01-01"
        lte: "2000-12-31"
      }
    }
  ) {
    title
    publishedDate
  }
}
```

### 6. Introspection Queries

#### Get Schema Types
```graphql
query {
  __schema {
    types {
      name
      kind
    }
  }
}
```

#### Get Query Type Fields
```graphql
query {
  __type(name: "Query") {
    name
    fields {
      name
      type {
        name
        kind
      }
    }
  }
}
```

#### Get Book Type Details
```graphql
query {
  __type(name: "Book") {
    name
    fields {
      name
      type {
        name
        kind
      }
    }
  }
}
```

---

## GraphQL Concepts Summary

### 1. **Schema Definition Language (SDL)**
- Defines the structure of your API
- Types, fields, arguments, etc.

### 2. **Queries** - Read Operations
- Fetch data from the server
- Supports filtering, sorting, pagination
- Allows field selection (only get what you need)

### 3. **Mutations** - Write Operations
- Create, update, delete data
- Can return the modified data
- Supports batch operations

### 4. **Subscriptions** - Real-time Updates
- WebSocket-based
- Push updates when data changes
- Event-driven

### 5. **Types**
- **Object Types**: Complex types with fields (Book, Author)
- **Scalar Types**: Primitive types (String, Int, Float, Boolean, ID)
- **Enum Types**: Fixed set of values (BookGenre)
- **Input Types**: For mutation arguments
- **Interface Types**: Abstract types that others implement
- **Union Types**: Can be one of several types

### 6. **DataLoaders**
- Solve N+1 query problem
- Batch and cache database requests
- Improve performance significantly

### 7. **Directives**
- Modify execution behavior
- Built-in: @skip, @include
- Custom: @uppercase

### 8. **Error Handling**
- GraphQL returns errors in a structured format
- Custom validation errors
- Business logic errors

---

## Best Practices

1. **Request Only What You Need**: Use field selection to minimize data transfer
2. **Use Variables**: Don't hardcode values in queries
3. **Use Fragments**: Reuse common field selections
4. **Handle Errors**: Always check for errors in the response
5. **Use DataLoaders**: For related data to avoid N+1 problems
6. **Validate Inputs**: Always validate mutation inputs
7. **Use Pagination**: Don't fetch all data at once
8. **Use Subscriptions**: For real-time features instead of polling

---

## Testing Your Queries

1. Start the application: `dotnet run`
2. Open browser: `http://localhost:5000/graphql`
3. Use Banana Cake Pop IDE to:
   - Explore the schema
   - Write and test queries
   - View documentation
   - Test subscriptions

---

## Additional Resources

- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [GraphQL Official Site](https://graphql.org)
- [GraphQL Best Practices](https://graphql.org/learn/best-practices/)
