# Running UI Tests on Linux/Ubuntu

## The Problem (SOLVED)

The Aspire `DistributedApplicationTestingBuilder` had limitations with JavaScript apps on Linux. The frontend couldn't communicate with the backend because Aspire on Linux uses a different environment variable format (`REACTAPP_BACKEND_HTTP`) than on Windows (`services__ReactApp-backend__http__0`).

## The Solution

The Vite configuration has been updated to recognize both environment variable formats, allowing tests to work on both Windows and Linux.

## Running Tests on Linux

### Using AppHost (Recommended)

Start the AppHost, wait for all resources to be healthy, then run tests with the frontend URL:

**Terminal 1: Start the AppHost**

```bash
cd /path/to/ReactAppTest
dotnet run --project ReactApp.AppHost
```

Wait for all resources to show as "Running" in the Aspire dashboard.

**Terminal 2: Run the Tests**

```bash
cd /path/to/ReactAppTest

# Get the frontend URL from the Aspire dashboard (usually a dynamic port)
# Example: http://localhost:42949
export FRONTEND_URL=http://localhost:<port>

cd ReactApp.UITests
dotnet test
```

Or use the provided script:

```bash
export FRONTEND_URL=http://localhost:<port>
./run-ui-tests-linux.sh
```

### Using Integrated Testing (May Still Have Issues)

You can try running tests without the external AppHost:

```bash
cd ReactApp.UITests
dotnet test
```

This uses `DistributedApplicationTestingBuilder` to start all resources automatically. However, on some Linux systems, only the database resource may start, causing test failures. If this happens, use the AppHost method above.

## Quick Reference

### One-Line Test Command

Once the AppHost is running in another terminal:

```bash
FRONTEND_URL=http://localhost:<port> dotnet test --project ReactApp.UITests
```

### Finding the Frontend Port

Check the Aspire dashboard or run:

```bash
lsof -Pan -i -c node 2>/dev/null | grep LISTEN | grep vite
```

## What Was Fixed

1. **Vite Configuration**: Updated `ReactApp.Web/vite.config.ts` to check for `REACTAPP_BACKEND_HTTP` environment variable in addition to the Windows format
2. **UI Test Base**: Added support for `FRONTEND_URL` environment variable to use an external AppHost instance
3. **Documentation**: Clarified Linux-specific requirements and provided clear instructions

## Troubleshooting

### Tests timeout during navigation

- Verify all resources are healthy in the Aspire dashboard
- Check the frontend URL is correct and accessible: `curl http://localhost:<port>`
- Ensure the backend is running: `curl http://localhost:<backend-port>/health`

### Frontend can't connect to backend

- Check Vite logs for proxy errors
- Verify `REACTAPP_BACKEND_HTTP` environment variable is set for the Vite process
- Restart the AppHost if backend URL changed

### Port conflicts

```bash
# Check what's using ports
sudo lsof -i :<port>

# Kill process if needed (use specific PID from lsof output)
kill <PID>
```
