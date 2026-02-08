# Linux UI Testing Issue

## Problem
UI tests fail on Linux (Ubuntu/WSL) with timeouts during form submission and navigation.

## Root Cause
The Aspire UI tests are not starting the backend API or frontend resources properly:

1. **Only the database resource starts** - The test logs show only `ReactApp-db` and `docs` resources are initialized
2. **Backend and frontend don't start** - The `ReactApp-backend` and `ReactApp-frontend` resources never appear in the logs
3. **Frontend can't connect to backend** - Vite proxy errors show `ECONNREFUSED 127.0.0.1:5001` because the backend isn't running

## Evidence
```
# From test logs - only these resources run:
[ReactApp-db] [OUT] ...
[docs] ...

# No backend or frontend logs found
# Frontend shows connection errors:
fail: ReactApp.AppHost.Resources.ReactApp-frontend[0]
      Error: connect ECONNREFUSED 127.0.0.1:5001
```

## Why This Happens
`Aspire.Hosting.Testing` has limitations with `AddJavaScriptApp` on Linux:
- JavaScript app resources may not start properly in test contexts
- The `.WithNpm(install: true)` might timeout or fail silently  
- Project resources might not initialize correctly without explicit configuration

## Solutions

### Option 1: Start Aspire AppHost Manually (Recommended)
Instead of using `DistributedApplicationTestingBuilder`, run the AppHost separately and point tests to it:

1. **Terminal 1 - Start AppHost**:
   ```bash
   cd ReactAppTest
   dotnet run --project ReactApp.AppHost
   ```
   
2. **Terminal 2 - Run Tests**:
   ```bash
   cd ReactApp.UITests
   # Set environment variable to use running AppHost
   export FRONTEND_URL=https://localhost:5173  # or whatever port Aspire assigns
   dotnet test
   ```

3. **Modify UITestBase.cs** to skip Aspire host management when external URL is provided:
   ```csharp
   protected static Uri FrontendBaseUri
   {
       get
       {
           var externalUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
           if (!string.IsNullOrWhiteSpace(externalUrl))
               return new Uri(externalUrl);
           
           return _aspireAppHost.GetEndpoint(Resources.Frontend);
       }
   }
   
   [Before(TestSession)]
   public static async Task StartAspireHost()
   {
       var externalUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
       if (!string.IsNullOrWhiteSpace(externalUrl))
       {
           // Skip Aspire host creation, using external instance
           return;
       }
       
       // Existing Aspire host startup code...
   }
   ```

### Option 2: Use Docker Compose for Tests
Create a `docker-compose.test.yml` that starts all services, then run tests against it.

### Option 3: Debug the Aspire Hosting Issue
The resources might not be starting due to:
- Missing dependencies (Node.js, npm)
- Permission issues  
- Port binding conflicts
- Timeout issues with npm install

Check the Aspire dashboard when running `dotnet run --project ReactApp.AppHost` to see if all resources start correctly.

## Workarounds Applied (Incomplete)
The following changes were made to improve button click reliability but don't solve the root cause:

1. Used `Force = true` on button clicks to bypass actionability checks
2. Changed from `WaitUntilState.NetworkIdle` to `WaitUntilState.Load`
3. Added explicit waits for button visibility

These help with Playwright timing but can't fix the missing backend API.

## Next Steps
1. Try Option 1 (manual AppHost) as it's the most reliable
2. If that works, the issue is confirmed to be with `DistributedApplicationTestingBuilder` on Linux
3. Consider filing an issue with the Aspire team about JavaScript app support in test contexts

## Test Command
```bash
cd ReactApp.UITests
dotnet test
```

Check screenshots in `bin/Debug/net10.0/TestResults/screenshots/` and logs in `bin/Debug/net10.0/TestResults/logs/` for debugging.
