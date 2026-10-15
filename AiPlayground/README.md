# AiPlayground

## Run with Docker Desktop

1. Start Docker Desktop using Linux containers.
2. Start LM Studio's server on port 1234 with the model configured in `appsettings.json` available.
3. Stop any locally running copy of this API using port 5088.
4. From this directory, run:

   ```powershell
   docker compose up --build -d
   ```

Send a GET request in Postman:

```text
http://localhost:5088/chat?message=Hello
```

OpenAPI JSON is available at `http://localhost:5088/openapi/v1.json`.

```powershell
docker compose logs -f api
docker compose down
```

Compose uses HTTP locally and disables HTTPS redirection because the container has no development HTTPS certificate. Running the project normally retains HTTPS redirection. This Compose setup is for local development.

Inside the container, `localhost` refers to the container itself. Compose overrides `AISettings:OpenAIEndpoint` with `http://host.docker.internal:1234/v1` to reach LM Studio on your computer. If LM Studio refuses the connection, check its network binding (enable serving on the local network if needed) and host firewall access.

To use another model, add `AISettings__OpenAIModel` under the Compose service's `environment` section.

The Dockerfile uses this directory as its build context and publishes the API with the .NET 10 SDK into a non-root ASP.NET Core runtime image.
