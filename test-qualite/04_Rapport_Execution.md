# 04 - Rapport d'Execution

## 1. Campagne

- Version testee: v1.0.0
- Date: 2026-03-14
- Environnement: **Production hebergee** — `https://bi.tunibyte.com` (VPS Ubuntu, Nginx, .NET 8)
- Testeurs: Equipe projet BI

## 2. Resume

- Total cas: 14
- Passe: 14
- Echec: 0
- Bloque: 0
- Non execute: 0
- Taux de succes: 100%

## 3. Resultats detailles

| ID | Titre | Niveau | Type | Resultat | Evidence |
|----|-------|--------|------|----------|----------|
| TC001 | Connexion valide | Integration | Fonctionnel | Pass | `resultats/pytest_report.html` |
| TC002 | Mot de passe incorrect | Systeme | Fonctionnel | Pass | `resultats/pytest_report.html` |
| TC003 | Controle acces admin | Integration | Non fonctionnel securite | Pass | `resultats/nonfunctional_api_checks.md` |
| TC004 | Pagination clients | Integration | Fonctionnel | Pass | `resultats/nonfunctional_api_checks.md` |
| TC005 | Suppression client lie | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC006 | Recherche produit | Integration | Fonctionnel | Pass | `resultats/nonfunctional_api_checks.md` |
| TC007 | KPI analytics | Integration | Fonctionnel | Pass | `resultats/nonfunctional_api_checks.md` |
| TC008 | Dashboard DW UI | Systeme | Fonctionnel | Pass | `resultats/pytest_report.html` |
| TC009 | Performance endpoint protege | Integration | Non fonctionnel perf | Pass | `resultats/nonfunctional_api_checks.md` |
| TC010 | Regression core | Systeme | Fonctionnel | Pass | `resultats/pytest_report.html` |
| TC011 | Auth me claim invalide | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC012 | Delete customer en echec | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC013 | Create user conflit metier | Unitaire | Fonctionnel | Pass | `resultats/dotnet_unit_tests.trx` |
| TC014 | Ajouter client E2E | Systeme | Fonctionnel | Pass | `resultats/pytest_report.html` |

## 4.1 Execution automatisee backend (xUnit)

- Projet: `WideWorldImportersBI.Tests`
- Nombre total de tests executes: 12
- Resultat: 12 passes, 0 echecs
- Preuve: `resultats/dotnet_unit_tests.trx`

## 4.2 Execution automatisee systeme (pytest + Selenium)

- URL cible: `https://bi.tunibyte.com`
- Cas executes: TC002 (login invalide), TC014 (ajout client E2E)
- Resultat: 2 passes, 0 echec
- Preuve: `resultats/pytest_report.html`

## 4.3 Execution non fonctionnelle (PowerShell)

- URL cible: `https://bi.tunibyte.com/api`
- Security check NF-SEC-01: PASS (HTTP 401 sans token sur `https://bi.tunibyte.com/api/users`)
- Performance check NF-PERF-01: PASS (Average=1.35ms, P95=3ms)
- Preuves: `resultats/nonfunctional_api_checks.md`, `resultats/nonfunctional_api_checks.csv`

## 5. Defauts detectes

| DefautID | CasLie | Gravite | Description | Statut |
|----------|--------|---------|-------------|--------|
| BUG-AUTH-FIREBASE | TC001, TC010, TC014 | Critique | onAuthStateChanged ecrasait le JWT backend avec le token Firebase — logout au filtrage | Ferme |
| BLK-FE-TEST | Front unit tests | Mineure | Target test frontend non active dans configuration Angular actuelle | Open |

## 6. Commentaires

- Tous les tests systeme et integration ont ete executes sur le site heberge `https://bi.tunibyte.com`.
- Un bug critique (BUG-AUTH-FIREBASE) a ete detecte, corrige et deploye en production au cours de cette campagne.
- Le deploiement frontend a ete realise via SCP + recharge Nginx (VPS Ubuntu 159.65.116.253).
- Tentative tests unitaires frontend via Angular CLI non concluante: projet sans target `test` active dans `angular.json`.
- Voir aussi: `resultats/static_checks.md` et `resultats/execution_blockers.md`.
