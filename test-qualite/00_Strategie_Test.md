# 00 - Strategie de Test

## 1. Contexte

Le SUT est l'application BI composee:
- d'un backend ASP.NET Core 8 (API REST, JWT, Firebase)
- d'un frontend Angular 17 (dashboard, CRUD, guard/interceptor)

## 2. Objectifs

- Valider les exigences fonctionnelles principales (authentification, consultation, CRUD, dashboard).
- Detecter les regressions sur les parcours critiques.
- Evaluer des attributs non fonctionnels de base (performance et securite d'acces).

## 3. Niveaux de test

1. Unitaires
- Services backend (ex: `AuthService`, `CustomerService`, `AnalyticsService`).
- Composants/services frontend (tests Angular existants + complements).

2. Integration
- API avec base de donnees (OLTP/DW), verification statuts HTTP et contrats JSON.
- Verification interceptor + guard avec token JWT.

3. Systeme
- Scenarios utilisateurs bout en bout via Selenium + pytest (navigateur reel).

## 4. Types de test

- Fonctionnels:
  - Basés exigences
  - Confirmation
  - Regression
  - Systeme

- Non fonctionnels:
  - Performance simple (temps de reponse API)
  - Securite basique (controle d'acces 401/403)

## 5. Technique de conception

- Boite noire:
  - Classes d'equivalence (inputs valides/non valides)
  - Valeurs limites (pagination, topN, champs obligatoires)
- Boite blanche:
  - Branches critiques (token invalide, role insuffisant, utilisateur inactif)

## 6. Environnements

- Local dev:
  - Front: `http://localhost:4200`
  - API: `http://localhost:5000/api`
- Production cible:
  - Front/API via `https://bi.tunibyte.com`

## 7. Entrees / sorties

Entrees:
- Exigences applicatives
- Routes API et ecrans UI
- Jeux de donnees de test

Sorties:
- Cas de test documentes
- Scripts automatises
- Rapports d'execution
- Tableau de tracabilite
- Rapport final avec defauts et couverture

## 8. Critères d'acceptation

- 100% des cas critiques executes.
- 0 defaut critique ouvert sur auth/autorisation.
- Au moins un test systeme automatise (Selenium/pytest).
- Tracabilite complete exigences -> cas -> resultat.
