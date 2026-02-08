# UI Tests Linux Fix - Summary

## Problem
UI tests were failing on Ubuntu/Linux with navigation timeout errors. While the tests passed on Windows, on Linux they would timeout waiting for navigation after form submission (e.g., registration, login).

## Root Cause
.NET Aspire on Linux uses a different environment variable naming convention for service discovery:
- **Windows**: `services__ReactApp-backend__http__0`
- **Linux**: `REACTAPP_BACKEND_HTTP` (underscores instead of hyphens, no double underscores)

The Vite configuration in `ReactApp.Web/vite.config.ts` only checked for the Windows format, causing the frontend proxy to fall back to `https://localhost:5001`, which didn't match the actual dynamically-assigned backend port in the Aspire AppHost.

This meant that when the frontend tried to make API calls (e.g., during registration), the proxy couldn't find the backend, causing requests to fail and navigation to timeout.

## Solution
Updated `ReactApp.Web/vite.config.ts` to check for both environment variable formats:

```typescript
const backendUrl = process.env['services__ReactApp-backend__http__0'] 
  || process.env['services__ReactApp-backend__https__0'] 
  || process.env.REACTAPP_BACKEND_HTTP          // Added for Linux
  || process.env.REACTAPP_BACKEND_HTTPS         // Added for Linux
  || process.env.services__backend__http__0 
  || process.env.services__backend__https__0 
  || 'https://localhost:5001'
```

Additionally, enhanced `ReactApp.UITests/UITestBase.cs` to support running tests against an external AppHost via the `FRONTEND_URL` environment variable, providing a fallback option for environments where `DistributedApplicationTestingBuilder` has issues.

## Files Changed
1. **ReactApp.Web/vite.config.ts** - Added Linux environment variable formats
2. **ReactApp.UITests/UITestBase.cs** - Added external frontend URL support
3. **ReactApp.UITests/README.md** - Updated with Linux instructions
4. **ReactApp.UITests/RUNNING-ON-LINUX.md** - Created detailed Linux guide
5. **run-ui-tests-linux.sh** - Created helper script for Linux testing

## Testing
After the fix, all 11 UI tests pass on Ubuntu:
```
Test run summary: Passed!
  total: 11
  failed: 0
  succeeded: 11
  skipped: 0
```

## How to Run Tests on Linux

### Standard Method (Recommended)
```bash
cd ReactApp.UITests
dotnet test
```

### Alternative Method (External AppHost)
If the standard method has issues, you can run the AppHost separately:

Terminal 1:
```bash
dotnet run --project ReactApp.AppHost
```

Terminal 2:
```bash
export FRONTEND_URL=http://localhost:<port>  # Get port from Aspire dashboard
cd ReactApp.UITests
dotnet test
```

## Impact
- ✅ Tests now pass on both Windows and Linux
- ✅ No changes required for Windows users
- ✅ Provides flexibility with external AppHost option
- ✅ Properly handles Aspire's platform-specific environment variable naming

## Future Considerations
This fix addresses the immediate issue, but ideally .NET Aspire should standardize the environment variable naming across platforms. This has been documented in case it needs to be reported to the Aspire team.
