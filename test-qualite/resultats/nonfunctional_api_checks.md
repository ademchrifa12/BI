# Non Functional API Checks

- Date: 2026-03-15 02:12:20
- Base URL: https://bi.tunibyte.com/api

| CheckId | Type | Endpoint | Expected | Actual | Passed |
|---------|------|----------|----------|--------|--------|
| NF-SEC-01 | Security | /users | HTTP 401 or 403 without token | HTTP:401 | True |
| NF-PERF-01 | Performance | /auth/me | Average < 800ms and P95 < 1500ms | Average=66.7ms; P95=81ms | True |

Detailed CSV: nonfunctional_api_checks.csv
