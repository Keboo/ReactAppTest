#!/bin/bash
# Script to run UI tests on Linux
# This script expects the AppHost to be running separately
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

# Check if FRONTEND_URL is set
if [ -z "$FRONTEND_URL" ]; then
    echo "ERROR: FRONTEND_URL environment variable is not set"
    echo ""
    echo "Please start the AppHost in a separate terminal:"
    echo "  dotnet run --project ReactApp.AppHost"
    echo ""
    echo "Then note the frontend URL from the Aspire dashboard and run:"
    echo "  export FRONTEND_URL=<frontend-url>"
    echo "  ./run-ui-tests-linux.sh"
    echo ""
    echo "Example:"
    echo "  export FRONTEND_URL=https://localhost:5173"
    echo "  ./run-ui-tests-linux.sh"
    exit 1
fi

echo "================================"
echo "Running UI tests"
echo "Frontend URL: $FRONTEND_URL"
echo "================================"

# Verify frontend is accessible
echo "Verifying frontend is accessible..."
if ! curl -k -s -f "$FRONTEND_URL" > /dev/null 2>&1; then
    echo "WARNING: Could not connect to $FRONTEND_URL"
    echo "Make sure the AppHost is running and all resources are healthy"
    read -p "Continue anyway? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Run the tests
cd ReactApp.UITests
dotnet test

echo ""
echo "================================"
echo "Tests completed!"
echo "================================"

