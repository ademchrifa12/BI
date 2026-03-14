# Dossier Test et Qualite Logiciel - Projet BI

Ce dossier contient un kit complet de livrables pour le module Test et Qualite Logiciel.

## Structure

- `00_Strategie_Test.md` : plan et strategie de test
- `01_Tests_Statiques.md` : revues + analyse statique
- `02_Cas_De_Test.csv` : catalogue des cas de test
- `03_Traceabilite.csv` : exigences vers cas vers resultats
- `04_Rapport_Execution.md` : rapport d'execution
- `05_Rapport_Final.md` : bilan final (couverture, defauts)
- `automation/pytest-selenium/` : script d'automatisation systeme (Selenium + pytest)
- `automation/pytest-selenium/tests/test_add_customer.py` : E2E ajout client + verification + cleanup
- `automation/nonfunctional_api_checks.ps1` : checks non fonctionnels (securite/performance)
- `../WideWorldImportersBI.Tests/` : suite de tests unitaires backend (xUnit + Moq)
- `preuves/` : captures, logs, exports outils
- `resultats/` : rapports d'execution (json, html, csv)

## Perimetre couvre

- Backend .NET 8 API (`WideWorldImportersBI`)
- Frontend Angular 17 (`wwi-bi-frontend`)

## Rappel barème cible

- Plan et strategie: 5%
- Tests statiques: 5%
- Cas fonctionnels + non fonctionnels: 25%
- Automatisation: 40%
- Tracabilite + rapport final: 10%
- Presentation orale: 10%
- Bonus: 5%

## Utilisation IA (obligatoire)

Documentez dans chaque rapport:
- Outil IA utilise
- Prompt utilise
- Contenu genere par IA
- Ajustements humains realises
