# Contributing

Thank you for helping improve **Odex.AspNetCore.Clarc.Infrastructure**. This project follows common practices for open-source .NET libraries on GitHub and NuGet.

## Before you start

- Read [README.md](README.md) for scope: **infrastructure helpers** aligned with **Odex.AspNetCore.Clarc.Domain**, not domain rules or application use cases.
- Check [CHANGELOG.md](CHANGELOG.md) and [open issues](https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore/issues) to avoid duplicate work.
- For **security-sensitive** reports, use [SECURITY.md](SECURITY.md) instead of a public issue.

## Development setup

Requirements:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download) (see [global.json](global.json) for the preferred feature band).

Commands:

```bash
git clone https://github.com/o-shabi/odex-clarc-infrastructure-aspnetcore.git
cd odex-clarc-infrastructure-aspnetcore
dotnet restore Odex.AspNetCore.Clarc.Infrastructure.sln
dotnet build Odex.AspNetCore.Clarc.Infrastructure.sln -c Release
dotnet test Odex.AspNetCore.Clarc.Infrastructure.sln -c Release
```

Strict build (matches CI):

```bash
dotnet build Odex.AspNetCore.Clarc.Infrastructure.sln -c Release -p:TreatWarningsAsErrors=true
```

Verify the NuGet layout locally:

```bash
dotnet pack Odex.AspNetCore.Clarc.Infrastructure/Odex.AspNetCore.Clarc.Infrastructure.csproj -c Release -o ./artifacts
```

The output includes **`Odex.AspNetCore.Clarc.Infrastructure.xml`** (API documentation), the `.dll`, and optionally **`.snupkg`**.

## XML documentation (public API)

The library enables **`GenerateDocumentationFile`**. Every **public** type and member must have appropriate **`///` XML documentation** so consumers get IntelliSense and the build does not fail on **CS1591**.

When you add or change public API:

1. Add or update **`/// <summary>`** (and **`<param>`**, **`<typeparam>`**, **`<returns>`**, **`<exception cref="...">`** where they clarify contracts).
2. Verify locally:

```bash
dotnet build Odex.AspNetCore.Clarc.Infrastructure/Odex.AspNetCore.Clarc.Infrastructure.csproj -warnaserror:CS1591
```

## Pull requests

1. Fork the repository and create a focused branch from **`main`**.
2. Keep changes scoped to a single concern when possible.
3. Match existing style (nullable reference types, file layout, naming).
4. Add or update **unit tests** in **Odex.AspNetCore.Clarc.Infrastructure.Tests** for behavior changes.
5. Update **[CHANGELOG.md](CHANGELOG.md)** under **Unreleased** (or the maintainer will before release) for user-visible changes.
6. Open a PR using the template; **CI must pass**.

## API and versioning

This project uses [Semantic Versioning](https://semver.org/). Breaking public API changes belong in a **major** bump (or coordinated **0.x** minor bumps per policy). Prefer additive changes.

## CI/CD (GitHub Actions)

Workflows live under [`.github/workflows/`](.github/workflows/).

### CI — `ci.yml`

**Triggers:** push and pull request to **`main`**, plus **manual** `workflow_dispatch`.

**Steps (summary):** `dotnet restore` → **`dotnet build`** (`Release`, **`TreatWarningsAsErrors=true`**, **`ContinuousIntegrationBuild=true`**) → **`dotnet test`** → **`dotnet pack`** → upload **`*.nupkg`** / **`*.snupkg`** as artifacts.

**Concurrency:** in-progress runs for the same PR or branch are cancelled to save minutes.

### Release — `release.yml`

**Triggers:**

| Trigger | How |
|--------|-----|
| **Git tag** | Push **`v1.2.3`** (leading `v`, then SemVer), e.g. `git tag v0.2.0 && git push origin v0.2.0` |
| **Manual** | **Actions → Release → Run workflow** and enter **`0.2.0`** (no `v` prefix) |

The workflow sets **`Version`** and **`PackageVersion`**, then build → test → pack → **`dotnet nuget push`** to **https://api.nuget.org/v3/index.json** with **`--skip-duplicate`**.

**Repository secret (one-time)**

1. Create a NuGet.org **API key** with permission to **push** this package ID.
2. GitHub repo: **Settings → Secrets and variables → Actions → New repository secret**
   - Name: **`NUGET_API_KEY`**
   - Value: your NuGet API key

Forks will not have this secret; the push step fails fast if it is missing.

## Code of conduct

Participation is governed by the [Code of Conduct](CODE_OF_CONDUCT.md).

## Licensing

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
