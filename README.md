# TMDB Explorer 🎬

Application **Blazor Server** moderne pour explorer et gérer vos films préférés via l'API TMDB (The Movie Database). Design sombre inspiré de l'esthétique terminal avec accents teal/indigo.

## Fonctionnalités

- 🔥 **Films populaires** — Carrousel interactif avec navigation automatique
- 🔍 **Recherche avancée** — Recherche avec filtres Chips (favoris/vus/adultes), tri par date
- ⭐ **Favoris** — Marquer des films avec mini-posters et badges rating
- 👁️ **Visionnés** — Historique de visionnage consultable
- 🎥 **Page détaillée** — Layout carte : poster sticky, stats, genres, reviews, bande-annonce

## Stack

| Composant | Technologie |
|-----------|------------|
| Framework | .NET 11.0 (preview) |
| UI | Blazor Server + MudBlazor 9.5 |
| API TMDB | TMDbLib 3.0 |
| Stockage | JSON local (cache mémoire) |
| Style | Dark theme · Inter font · Glass cards |

## Démarrage

```bash
dotnet run
```
→ `https://localhost:5001`

> ⚠️ Clé API TMDB de démo incluse. Remplacez-la dans `Program.cs`.

## Structure

```
TMDB_blazor/
├── Components/        # Composants Blazor
│   ├── FavorisList    # Liste favoris avec recherche
│   ├── GenericList    # Table générique paginée
│   ├── MovieDetail    # Carte détail film
│   ├── Populars       # Carrousel films populaires
│   ├── SearchBarItem  # Input recherche moderne
│   └── ViewedList     # Historique visionnage
├── Data/
│   ├── TMDBTheme.cs   # Thème sombre MudBlazor
│   ├── Endpoints.cs   # URLs images + chemins JSON
│   └── UserMovie.cs   # Modèle film étendu
├── Pages/             # Pages application
├── Services/          # JsonFileRepository (cache async) + TmdbExtension
└── wwwroot/data/      # favorite.json / viewed.json / local.json
```

## Performance

- **Cache mémoire** — JSON lu une fois, invalidé par timestamp
- **Async complet** — Zéro `.Result`, tout `async/await`
- **OnInitializedAsync** — Films populaires chargés une seule fois
- **0 dépendance Bootstrap**
