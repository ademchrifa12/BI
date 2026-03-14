# Static Checks Evidence

Date: 2026-03-14

1. Backend static check
- Command: dotnet build -warnaserror
- Result: PASS

2. Frontend static check
- Command: npm run build
- Result: PASS with warnings
- Observed warnings: CSS budget warnings on multiple components

3. Frontend unit test command attempt
- Command: npx ng test --no-watch --browsers ChromeHeadless
- Result: BLOCKED
- Error: Unknown arguments: watch, browsers
- Interpretation: test target not configured/active in current angular configuration.
