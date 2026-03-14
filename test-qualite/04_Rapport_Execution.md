# 04 - Rapport d'Execution

## 1. Campagne

- Version testee: v1.0.0
- Date: 2026-03-14
- Environnement: Windows + .NET 8.0.11
- Testeurs: Equipe projet BI

## 2. Resume

- Total cas: 13
- Passe: 6
- Echec: 0
- Bloque: 0
- Non execute: 7
- Taux de succes: 46.15%

## 3. Resultats detailles

| ID | Titre | Niveau | Type | Resultat | Evidence |
|----|-------|--------|------|----------|----------|
| TC001 | Connexion valide | Integration | Fonctionnel | Not Executed | |
| TC002 | Mot de passe incorrect | Systeme | Fonctionnel | Pass | `resultats/pytest_report.html` |
| TC003 | Controle acces admin | Integration | Non fonctionnel securite | Pass | `resultats/nonfunctional_api_checks.md` |
| TC004 | Pagination clients | Integration | Fonctionnel | Not Executed | |
| TC005 | Suppression client lie | Unitaire | Fonctionnel | Not Executed | |
| TC006 | Recherche produit | Integration | Fonctionnel | Not Executed | |
| TC007 | KPI analytics | Integration | Fonctionnel | Not Executed | |
| TC008 | Dashboard DW UI | Systeme | Fonctionnel | Not Executed | |
| TC009 | Performance endpoint protege | Integration | Non fonctionnel perf | Pass | `resultats/nonfunctional_api_checks.md` |
| TC010 | Regression core | Systeme | Fonctionnel | Not Executed | |
| TC011 | Auth me claim invalide | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC012 | Delete customer en echec | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC013 | Create user conflit metier | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |

## 4.1 Execution automatisee backend (xUnit)

- Projet: `WideWorldImportersBI.Tests`
- Nombre total de tests executes: 12
- Resultat: 12 passes, 0 echecs
- Preuve: `resultats/dotnet_unit_tests.trx`

## 4.2 Execution automatisee systeme (pytest + Selenium)

- Cas execute: TC002
- Resultat: 1 passe, 0 echec
- Preuve: `resultats/pytest_report.html`

## 4.3 Execution non fonctionnelle (PowerShell)

- Security check NF-SEC-01: PASS (HTTP 401 sans token sur `/api/users`)
- Performance check NF-PERF-01: PASS (Average=1.35ms, P95=3ms)
- Preuves: `resultats/nonfunctional_api_checks.md`, `resultats/nonfunctional_api_checks.csv`

## 5. Defauts detectes

| DefautID | CasLie | Gravite | Description | Statut |
|----------|--------|---------|-------------|--------|
| BLK-ENV-DB | TC001, TC004, TC006, TC007, TC010 | Majeure | Base OLTP indisponible localement pour execution integration complete | Open |
| BLK-ENV-DW | TC008 | Majeure | Data Warehouse indisponible pour scenario dashboard complet | Open |
| BLK-FE-TEST | Front unit tests | Majeure | Target test frontend non active dans configuration Angular actuelle | Open |

## 6. Commentaires

- Ajouter captures ecran dans `preuves/`
- Ajouter logs d'execution automatiques dans `resultats/`
- Tentative tests unitaires frontend via Angular CLI non concluante: projet sans target `test` active dans `angular.json` (arguments `watch`/`browsers` rejetes).
- Voir aussi: `resultats/static_checks.md` et `resultats/execution_blockers.md`.
