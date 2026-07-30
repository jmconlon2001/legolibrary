# LEGO Set Tracker & Analytics

A self-hosted Blazor Server app for tracking a personal LEGO collection: import sets from
[Rebrickable](https://rebrickable.com), track build status, browse the full brick/minifig
inventory, view collection analytics, cache instruction manuals, and manage storage locations.

## Tech stack

- **.NET 10** / Blazor Server (interactive server render mode)
- **MudBlazor** for UI components (data grids, charts, dialogs)
- **EF Core + SQLite** for storage
- **Polly** for resilient HTTP calls to the Rebrickable API
- Plain C# events (`INotificationService`) for live progress updates — Blazor Server already
  runs each connected page over its own persistent SignalR circuit, so no second hub is needed
- Docker + GitHub Actions → GHCR for deployment

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A free [Rebrickable API key](https://rebrickable.com/api/) (Account → Settings → API)
- Docker, if you want to run it containerized

## Local development

```bash
cd src/LegoTracker
dotnet user-secrets set "Rebrickable:ApiKey" "<your-key>"
dotnet tool restore                 # installs the pinned dotnet-ef version (see ../dotnet-tools.json)
dotnet tool run dotnet-ef database update
dotnet run
```

The app applies any pending EF Core migrations automatically at startup, so `dotnet ef database
update` above is optional for a first run — it's useful mainly for inspecting the schema ahead of
time. The SQLite file is created at `src/LegoTracker/data/legotracker.db` and box art / instruction
PDFs are cached under `src/LegoTracker/media/`.

Run the tests with:

```bash
dotnet test
```

## Configuration

All settings can be set in `appsettings.json`, `appsettings.Development.json`, .NET user-secrets
(local dev), or environment variables (`__` as the section separator, e.g. `Rebrickable__ApiKey`).

| Key | Purpose | Default |
|---|---|---|
| `Rebrickable:ApiKey` | Your Rebrickable API key | *(empty — required)* |
| `Rebrickable:BaseUrl` | Rebrickable API base URL | `https://rebrickable.com/api/v3/lego/` |
| `ConnectionStrings:Default` | SQLite connection string | `Data Source=data/legotracker.db` |
| `Media:RootPath` | Directory for cached box art / instruction PDFs | `media` |

## Docker

```bash
REBRICKABLE_API_KEY=<your-key> docker compose up -d --build
```

This builds the multi-stage `Dockerfile`, starts the app on `http://localhost:8080`, and mounts
`./data` and `./media` on the host so your collection and cached files survive container
rebuilds/restarts.

## Features

- **`/sets`** — browse your collection in a filterable/sortable grid; add a set by number
  (`75192` or `75192-1`), which fetches its metadata, full parts inventory, and minifigs from
  Rebrickable and downloads its box art locally.
- **`/sets/{id}`** — set detail: editable build status, storage location, MSRP/estimated value,
  full parts and minifig inventory, and the missing/damaged parts tracker.
- **`/import`** — bulk import: paste or upload a list of set numbers, pick an initial build
  status for the whole batch, and watch live progress (a background job queue processes sets
  one at a time so one bad set number doesn't stop the batch).
- **`/analytics`** — total parts, sets, unique-vs-duplicate part/color combinations, estimated
  collection value, a build-status breakdown, a color-distribution chart (rendered in each
  part's *real* LEGO color), and the most common part categories.
- **`/sets/{id}/instructions`** — paste a PDF URL you've found elsewhere (see limitation below);
  it downloads in the background and is served from local storage, never hotlinked, with live
  status updates as the download completes.
- **`/storage-locations`** — a Room → Shelf → Bin style hierarchy you can assign sets to.
- **Missing/damaged part tracking** with CSV and Rebrickable-wanted-list CSV export, per set.

## Known limitations

- **No automated instruction-PDF discovery.** Rebrickable's public API has no endpoint for
  instruction manual links (verified directly against their API during development) — you add a
  manual URL yourself (e.g. from LEGO's customer service site), and the app downloads and caches
  it. The wanted-list export's exact column-acceptance rules ("Part, Color, Quantity" headers,
  order-independent) come from Rebrickable's own help documentation but weren't fully verified
  against every edge case, since those help pages block automated fetches — re-check against a
  real upload if Rebrickable rejects it.
- **No live market pricing.** Rebrickable doesn't provide MSRP or resale-value data either — MSRP
  and Estimated Value are both plain manual-entry fields per set, not fetched or computed.

## Roadmap

Rough build order this project followed, useful as a map of the codebase:
1. Project scaffold + MudBlazor
2. EF Core entities/migrations
3. Rebrickable API client (DTOs verified against live responses)
4. Single-set import
5. Background job queue + bulk import with live progress
6. Analytics dashboard
7. Instruction manual download + viewer
8. Missing parts / storage locations / exports / valuation
9. Docker + CI

## License

No license file included yet — add one (MIT is a common default for a personal project like this)
if you plan to share the repository publicly.
