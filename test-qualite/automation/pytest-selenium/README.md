# Automatisation Systeme - pytest + Selenium (POM)

## Cas automatise

- TC002: connexion avec mot de passe incorrect

## Prerequis

- Python 3.10+
- Google Chrome
- ChromeDriver compatible (via PATH)
- Application frontend en execution (`http://localhost:4200` par defaut)

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
$env:APP_BASE_URL="http://localhost:4200"
pytest
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
