# WideWorldImportersBI.Tests

Suite de tests unitaires backend (niveau Unitaire) basee sur xUnit + Moq.

## Contenu

- `Controllers/AuthControllerTests.cs`
- `Controllers/CustomersControllerTests.cs`
- `Controllers/UsersControllerTests.cs`

## Execution

Depuis le dossier `BI`:

```powershell
dotnet test .\WideWorldImportersBI.Tests\WideWorldImportersBI.Tests.csproj
```

Avec export TRX:

```powershell
dotnet test .\WideWorldImportersBI.Tests\WideWorldImportersBI.Tests.csproj --logger "trx;LogFileName=dotnet_unit_tests.trx" --results-directory .\test-qualite\resultats
```

## Resultat actuel

- 12 tests executes
- 12 passes
- 0 echecs

Preuve disponible dans `BI/test-qualite/resultats/dotnet_unit_tests.trx`.
