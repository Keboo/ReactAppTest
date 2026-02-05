# UI Test Environment Variables Quick Reference

## Available Configuration Options

| Variable | Description | Default | Examples |
|----------|-------------|---------|----------|
| `TEST_BASE_URL` | Base URL of the application to test | `https://ReactApp.dev.localhost:7147` | `https://localhost:7239`<br>`https://staging.example.com` |
| `HEADLESS` | Run browser in headless mode (no visible window) | `true` | `false` or `0` for headed mode |
| `SLOW_MO` | Delay in milliseconds between actions | `0` | `500`, `1000` |
| `BROWSER` | Browser to use for tests | `chromium` | `chromium`, `firefox`, `webkit` |

## Common Scenarios

### Local Development (watch tests execute)
```powershell
$env:HEADLESS = "false"
$env:SLOW_MO = "500"
dotnet test
```

### Test Against Staging Environment
```powershell
$env:TEST_BASE_URL = "https://staging.example.com"
dotnet test
```

### Debug Specific Test
```powershell
$env:HEADLESS = "false"
$env:SLOW_MO = "1000"
dotnet test --filter "FullyQualifiedName~CompleteQAWorkflow"
```

### CI/CD (headless, fast)
```powershell
# Use defaults - no environment variables needed
dotnet test
```

### Test with Firefox
```powershell
$env:BROWSER = "firefox"
$env:HEADLESS = "false"
dotnet test
```

## Clearing Environment Variables

After testing, you may want to clear the variables:

```powershell
Remove-Item Env:\HEADLESS
Remove-Item Env:\SLOW_MO
Remove-Item Env:\TEST_BASE_URL
```

Or simply close and reopen your terminal.
