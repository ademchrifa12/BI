# Automatisation Systeme - pytest + Selenium (POM)

## Cas automatise

- TC002: connexion avec mot de passe incorrect
- TC014: ajout client + verification + suppression (cleanup)

## Prerequis

- Python 3.10+
- Google Chrome
- ChromeDriver compatible (via PATH)
- Application frontend en execution (`https://bi.tunibyte.com` par defaut)

## Installation

```powershell
cd BI/test-qualite/automation/pytest-selenium
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

## Execution

```powershell
pytest
```

Avec URL custom:

```powershell
$env:APP_BASE_URL="https://bi.tunibyte.com"
pytest
```

Pour le test d'ajout client (compte Admin requis):

```powershell
$env:TEST_USER_EMAIL="admin@wideworldimporters.com"
$env:TEST_USER_PASSWORD="<votre-mot-de-passe>"
pytest tests\test_add_customer.py -s
```

Mode headless:

```powershell
$env:HEADLESS="true"
pytest
```

## Sorties

- Rapport HTML: `BI/test-qualite/resultats/pytest_report.html`

## Important

Les selecteurs CSS dans `pages/login_page.py` peuvent necessiter un ajustement mineur selon le template HTML reel du composant login.
