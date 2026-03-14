# 05 - Rapport Final

## 1. Synthese

Ce rapport presente les resultats globaux des activites de test statique et dynamique sur l'application BI (backend .NET + frontend Angular).

## 2. Couverture

- Couverture fonctionnelle: 10/13 cas definis, dont 5 executes PASS sur parcours critiques.
- Couverture non fonctionnelle: Security basic + Performance simple executes PASS.
- Couverture par niveau (unitaire/integration/systeme):
	- Unitaire: couvert (12 tests backend xUnit executes, 100% pass)
	- Integration: couvert partiellement (checks API securite/perf executes)
	- Systeme: couvert (scenario Selenium login incorrect execute PASS)

## 3. Resultats majeurs

- Nombre total de defauts: 2 (statiques)
- Defauts critiques: 0
- Defauts fermes: 0
- Defauts ouverts: 2 (budgets CSS, target test frontend)

## 4. Qualite logicielle observee

Points forts:
- Architecture claire (controllers/services/repositories)
- Separation OLTP / DW
- Gestion auth + roles

Points a ameliorer:
- Durcir gestion des secrets/config
- Uniformiser certains contrats API frontend/backend
- Completer automatisation de regression

## 5. Recommandations

1. Integrer la suite de test dans CI/CD.
2. Ajouter tests unitaires backend avec mocks.
3. Ajouter tests de securite automatises (401/403, injections simples).
4. Stabiliser jeux de donnees de test.

## 6. Declaration IA (obligatoire)

- Outil IA utilise:
- Contributions IA:
- Validation humaine:
- Modifications manuelles:
