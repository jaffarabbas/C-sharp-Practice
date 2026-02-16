# Database Setup

## SQL Server Setup

1. Make sure SQL Server is running on localhost
2. Update the password in `appsettings.json` to match your SQL Server SA password
3. Run the setup script:

```bash
sqlcmd -S localhost -U sa -P YourStrong@Password -i Database/setup.sql
```

Or use SQL Server Management Studio (SSMS) to run the `setup.sql` file.

## Test the API

Use the correct endpoint with the leave plugin:

```bash
POST http://localhost:5077/api/chat/with-leave
Content-Type: application/json

{
  "message": "what is my leave count?"
}
```

The regular `/api/chat` endpoint does NOT have access to the leave plugin - only `/api/chat/with-leave` does!
