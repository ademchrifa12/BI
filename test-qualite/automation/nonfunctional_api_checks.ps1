$ErrorActionPreference = 'Stop'

$baseUrl = "http://localhost:5000/api"
$outCsv = "d:\mahdi-adem-v2\BI\test-qualite\resultats\nonfunctional_api_checks.csv"
$outMd = "d:\mahdi-adem-v2\BI\test-qualite\resultats\nonfunctional_api_checks.md"

$rows = @()

# Security basic check: endpoint requiring auth should reject request without token.
$securityStatus = ""
try {
    $resp = Invoke-WebRequest -Uri "$baseUrl/users" -Method Get -UseBasicParsing
    $securityStatus = "UnexpectedStatus:$($resp.StatusCode)"
} catch {
    if ($_.Exception.Response) {
        $securityStatus = "HTTP:$([int]$_.Exception.Response.StatusCode)"
    } else {
        $securityStatus = "Error:$($_.Exception.Message)"
    }
}

$rows += [PSCustomObject]@{
    CheckId = "NF-SEC-01"
    CheckType = "Security"
    Endpoint = "/users"
    Method = "GET"
    Expected = "HTTP 401 or 403 without token"
    Actual = $securityStatus
    Passed = ($securityStatus -eq "HTTP:401" -or $securityStatus -eq "HTTP:403")
}

# Performance simple check: response time over 20 requests to auth-protected endpoint without token.
$durations = @()
for ($i = 1; $i -le 20; $i++) {
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        Invoke-WebRequest -Uri "$baseUrl/auth/me" -Method Get -UseBasicParsing | Out-Null
    } catch {
        # 401 is expected here without token; time measurement still valid.
    }
    $sw.Stop()
    $durations += [double]$sw.ElapsedMilliseconds
}

$sorted = $durations | Sort-Object
$avg = [Math]::Round((($durations | Measure-Object -Average).Average), 2)
$p95Index = [Math]::Ceiling($sorted.Count * 0.95) - 1
if ($p95Index -lt 0) { $p95Index = 0 }
$p95 = [Math]::Round($sorted[$p95Index], 2)

$perfPassed = ($avg -lt 800 -and $p95 -lt 1500)

$rows += [PSCustomObject]@{
    CheckId = "NF-PERF-01"
    CheckType = "Performance"
    Endpoint = "/auth/me"
    Method = "GET"
    Expected = "Average < 800ms and P95 < 1500ms"
    Actual = "Average=${avg}ms; P95=${p95}ms"
    Passed = $perfPassed
}

$rows | Export-Csv -Path $outCsv -NoTypeInformation -Encoding UTF8

$md = @()
$md += "# Non Functional API Checks"
$md += ""
$md += "- Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$md += "- Base URL: $baseUrl"
$md += ""
$md += "| CheckId | Type | Endpoint | Expected | Actual | Passed |"
$md += "|---------|------|----------|----------|--------|--------|"
foreach ($r in $rows) {
    $md += "| $($r.CheckId) | $($r.CheckType) | $($r.Endpoint) | $($r.Expected) | $($r.Actual) | $($r.Passed) |"
}
$md += ""
$md += "Detailed CSV: nonfunctional_api_checks.csv"

$md | Set-Content -Path $outMd -Encoding UTF8

Write-Output "Non-functional checks completed."
Write-Output "CSV: $outCsv"
Write-Output "MD : $outMd"
