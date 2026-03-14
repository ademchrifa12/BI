# 01 - Tests Statiques

## 1. Revue de code (obligatoire)

### 1.1 Scope revu
- Backend: controllers, services, configuration JWT/CORS
- Frontend: routes, guards, interceptor, services API

### 1.2 Methode
- Revue croisee par pair programming + checklist
- Checklist orientee: securite, validation input, gestion erreurs, coherence API

### 1.3 Exemple de checklist
- Secrets hardcodes en config ?
- Validation des parametres limites (page/pageSize/topN) ?
- Verification cryptographique des tokens ?
- Coherence parametres frontend/backend ?
- Gestion 401/403/500 harmonisee ?

## 2. Analyse statique outillee

### 2.1 Backend (.NET)
Commandes proposees:

```powershell
cd BI/WideWorldImportersBI
dotnet restore
dotnet build -warnaserror
```

### 2.2 Frontend (Angular)
Commandes proposees:

```powershell
cd BI/wwi-bi-frontend
npm install
npm run test -- --watch=false
```

## 3. Observations initiales a verifier

1. Parametres query potentiellement incoherents (`top` vs `topN`) dans certains appels analytics.
2. Presence de cles/identifiants sensibles dans fichiers de configuration.
3. Fallback d'auth Firebase sans verification cryptographique complete dans certains contextes.

## 3.1 Resultats executes (2026-03-14)

1. Backend static check
- Commande: `dotnet build -warnaserror`
- Resultat: PASS (0 warning, 0 erreur)

2. Frontend static check
- Commande: `npm run build`
- Resultat: PASS avec warnings de budgets CSS depasses sur plusieurs composants
- Impact: non bloquant execution, a traiter pour qualite UI/performance

3. Frontend unit test target
- Tentative: `npx ng test --no-watch --browsers ChromeHeadless`
- Resultat: BLOQUE (arguments inconnus `watch`, `browsers`), indiquant une configuration `test` non active dans le projet actuel

## 4. Preuves a joindre

- Captures console build/test
- Export warnings/erreurs
- Captures ou diff des corrections appliquees

Deposer les preuves dans `preuves/`.

## 5. Corrections effectuees

| ID | Probleme detecte | Gravite | Fichier | Correction | Statut |
|----|------------------|---------|---------|------------|--------|
| STA-01 | A completer | A completer | A completer | A completer | Open |
| STA-02 | A completer | A completer | A completer | A completer | Open |
| STA-03 | Budget CSS depasse sur plusieurs composants Angular | Mineure | wwi-bi-frontend/src/app/features/*/*.css | Optimiser styles / ajuster budgets | Open |
| STA-04 | Configuration tests unitaires frontend non active | Majeure | wwi-bi-frontend/angular.json | Ajouter/activer target test Karma/Jest | Open |
