# Presentation - Test Quality Report (Hosted Environment)

## 1. Project Context

Application tested:
- Backend: ASP.NET Core 8 API
- Frontend: Angular 17
- Authentication: Firebase + backend JWT
- Hosted URL: https://bi.tunibyte.com
- Hosted API URL: https://bi.tunibyte.com/api

Objective:
- Validate that the production-hosted version is stable and fully functional.
- Execute automated functional and non-functional tests on hosted URLs only.
- Confirm no remaining localhost references in the test-quality scope.

---

## 2. What Was Tested

### 2.1 Unit Tests (Backend)
- Framework: xUnit
- Scope: Controllers/services critical branches
- Result: 12 passed, 0 failed
- Evidence: test-qualite/resultats/dotnet_unit_tests.trx

### 2.2 System/UI Tests (Hosted Website)
- Framework: pytest + Selenium
- Target: https://bi.tunibyte.com
- Executed tests:
  - test_login_invalid_password.py
  - test_add_customer.py
- Result: 2 passed, 0 failed
- Evidence: test-qualite/resultats/pytest_report.html

### 2.3 Non-Functional API Checks (Hosted API)
- Script: nonfunctional_api_checks.ps1
- Target: https://bi.tunibyte.com/api
- Checks:
  - NF-SEC-01: Unauthorized access blocked (401/403 expected)
  - NF-PERF-01: Response time threshold validation
- Latest measured values:
  - Security: HTTP 401 on /users without token (PASS)
  - Performance: Average 66.7 ms, P95 81 ms (PASS)
- Evidence:
  - test-qualite/resultats/nonfunctional_api_checks.md
  - test-qualite/resultats/nonfunctional_api_checks.csv

---

## 3. Key Bug Found and Fixed

Bug ID:
- BUG-AUTH-FIREBASE

Impact:
- User was logged out during filtering/navigation because frontend token handling overwrote backend JWT with Firebase token.

Fix applied:
- Removed problematic onAuthStateChanged token overwrite flow in frontend auth service.
- Deployed corrected frontend to VPS and reloaded Nginx.

Validation after fix:
- Hosted UI tests pass end-to-end.
- Add customer flow is stable: create -> verify -> delete -> verify absence.

---

## 4. Hosted Deployment Status

Server:
- VPS: 159.65.116.253
- Domain: bi.tunibyte.com
- Nginx root: /var/www/bi

Frontend deployment:
- Current bundle served: main-JXOUCFQO.js
- Hosted site now serves updated build (not old cached bundle).

---

## 5. How To Run All Tests Now (Hosted)

Prerequisites:
- Python virtual environment available at d:/mahdi-adem-v2/.venv
- Chrome installed for Selenium
- PowerShell execution allowed for test scripts
- Credentials for hosted admin account

### 5.1 Open PowerShell and activate environment

```powershell
cd d:\mahdi-adem-v2
.\.venv\Scripts\Activate.ps1
```

### 5.2 Run Selenium tests on hosted URL (headless)

```powershell
cd d:\mahdi-adem-v2\BI\test-qualite\automation\pytest-selenium
$env:APP_BASE_URL = "https://bi.tunibyte.com"
$env:TEST_USER_EMAIL = "admin@wideworldimporters.com"
$env:TEST_USER_PASSWORD = "Admin@123"
$env:HEADLESS = "true"
d:/mahdi-adem-v2/.venv/Scripts/python.exe -m pytest -v --tb=short
```

### 5.3 Run Selenium with visible browser UI

```powershell
cd d:\mahdi-adem-v2\BI\test-qualite\automation\pytest-selenium
$env:APP_BASE_URL = "https://bi.tunibyte.com"
$env:TEST_USER_EMAIL = "admin@wideworldimporters.com"
$env:TEST_USER_PASSWORD = "Admin@123"
$env:HEADLESS = "false"
d:/mahdi-adem-v2/.venv/Scripts/python.exe -m pytest -v --tb=short
```

### 5.4 Run non-functional hosted API checks

```powershell
cd d:\mahdi-adem-v2\BI\test-qualite\automation
./nonfunctional_api_checks.ps1 -ApiBase "https://bi.tunibyte.com/api"
```

### 5.5 Optional: Run backend unit tests

```powershell
cd d:\mahdi-adem-v2\BI
dotnet test
```

---

## 6. Artifacts To Show In Presentation

Primary reports:
- test-qualite/04_Rapport_Execution.md
- test-qualite/05_Rapport_Final.md

Generated evidence:
- test-qualite/resultats/pytest_report.html
- test-qualite/resultats/nonfunctional_api_checks.md
- test-qualite/resultats/nonfunctional_api_checks.csv
- test-qualite/resultats/dotnet_unit_tests.trx

---

## 7. Final Conclusion

- All hosted critical tests are green.
- Functional, non-functional, and key regression scenarios were validated on production-hosted URLs.
- Localhost references in test-quality materials were removed.
- The current hosted release is verified for demonstration and presentation.

Prepared on: 2026-03-15
Prepared for: BI Test Quality Presentation
