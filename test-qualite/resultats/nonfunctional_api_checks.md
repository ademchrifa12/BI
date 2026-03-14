# Non Functional API Checks

- Date: 2026-03-14 00:58:02
- Base URL: http://localhost:5000/api

| CheckId | Type | Endpoint | Expected | Actual | Passed |
|---------|------|----------|----------|--------|--------|
| NF-SEC-01 | Security | /users | HTTP 401 or 403 without token | HTTP:401 | True |
| NF-PERF-01 | Performance | /auth/me | Average < 800ms and P95 < 1500ms | Average=0.2ms; P95=1ms | True |

Detailed CSV: nonfunctional_api_checks.csv
