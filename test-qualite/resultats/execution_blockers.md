# Execution Blockers

Date: 2026-03-14

## Blocker 1: OLTP database unavailable locally

Observed during backend startup:
- "Cannot connect to OLTP database. Ensure WideWorldImporters is restored."

Impact:
- Integration tests requiring transactional data access cannot be validated end-to-end in this environment.

## Blocker 2: Frontend unit-test target unavailable

Observed command:
- npx ng test --no-watch --browsers ChromeHeadless

Observed error:
- Unknown arguments: watch, browsers

Impact:
- Existing frontend unit test files cannot be executed through configured CLI target in current project state.

## Workaround executed

- Backend unit tests executed via xUnit and passed.
- System Selenium test executed and passed.
- Non-functional security/performance checks executed and passed.
