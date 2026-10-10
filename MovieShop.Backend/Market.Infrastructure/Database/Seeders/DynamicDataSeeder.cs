using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Market.Domain.Entities.Catalog;
using Market.Domain.Entities.Identity;

namespace Market.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        await context.Database.EnsureCreatedAsync();

        await SeedUsersAsync(context);
        var categories = await SeedCategoriesAsync(context);
        var directors = await SeedDirectorsAsync(context);
        var actors = await SeedActorsAsync(context);
        var movies = await SeedMoviesAsync(context, directors);
        await SeedMovieCategoriesAsync(context, movies, categories);
        await SeedMovieActorsAsync(context, movies, actors);
    }

    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        var hasher = new PasswordHasher<MarketUserEntity>();

        var baselineUsers = new[]
        {
            new MarketUserEntity
            {
                Username = "admin",
                Email = "admin@market.local",
                PasswordHash = hasher.HashPassword(null!, "Admin123!"),
                Firstname = "System",
                Lastname = "Admin",
                IsAdmin = true,
                IsEnabled = true,
                IsEmployee = false,
                IsEmailConfirmed = true,
                IsTwoFactorEnabled = false
            },
            new MarketUserEntity
            {
                Username = "user",
                Email = "user@market.local",
                PasswordHash = hasher.HashPassword(null!, "User123!"),
                Firstname = "Demo",
                Lastname = "User",
                IsEnabled = true,
                IsEmployee = true,
                IsAdmin = false,
                IsManager = false,
                IsEmailConfirmed = true,
                IsTwoFactorEnabled = false
            }
        };

        foreach (var baselineUser in baselineUsers)
        {
            var normalizedEmail = baselineUser.Email.Trim().ToLowerInvariant();
            var existing = await context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail);

            if (existing is null)
            {
                baselineUser.Email = normalizedEmail;
                context.Users.Add(baselineUser);
                continue;
            }

            existing.Firstname = string.IsNullOrWhiteSpace(existing.Firstname)
                ? baselineUser.Firstname
                : existing.Firstname;
            existing.Lastname = string.IsNullOrWhiteSpace(existing.Lastname)
                ? baselineUser.Lastname
                : existing.Lastname;
            existing.Username = baselineUser.Username;
            existing.IsEnabled = true;
            existing.IsEmailConfirmed = true;
            existing.IsTwoFactorEnabled = false;
            existing.TwoFactorCodeHash = null;
            existing.TwoFactorCodeExpiresAtUtc = null;
            existing.TwoFactorFailedAttempts = 0;
            existing.EmailConfirmationTokenHash = null;
            existing.EmailConfirmationTokenExpiresAtUtc = null;
            existing.EmailConfirmedAtUtc ??= DateTime.UtcNow;

            if (normalizedEmail == "admin@market.local")
            {
                existing.IsAdmin = true;
                existing.IsManager = false;
                existing.IsEmployee = false;
            }
            else if (normalizedEmail == "user@market.local")
            {
                existing.IsAdmin = false;
                existing.IsManager = false;
                existing.IsEmployee = true;
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, CategoryEntity>> SeedCategoriesAsync(DatabaseContext context)
    {
        var categoryNames = new[]
        {
            "Action",
            "Crime",
            "Comedy"
        };

        foreach (var categoryName in categoryNames)
        {
            var exists = await context.Categories
                .AnyAsync(x => x.CategoryName.ToLower() == categoryName.ToLower());

            if (!exists)
            {
                context.Categories.Add(new CategoryEntity
                {
                    CategoryName = categoryName
                });
            }
        }

        await context.SaveChangesAsync();

        var categoryKeys = categoryNames
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        var seededCategories = await context.Categories
            .AsNoTracking()
            .ToListAsync();

        return seededCategories
            .Where(x => categoryKeys.Contains(x.CategoryName.ToLower()))
            .GroupBy(x => x.CategoryName.ToLower())
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First());
    }

    private static async Task<Dictionary<string, DirectorEntity>> SeedDirectorsAsync(DatabaseContext context)
    {
        var directorSeeds = new[]
        {
            new DirectorEntity { FirstName = "Peter", LastName = "Jackson", BirthDate = new DateTime(1961, 10, 31) },
            new DirectorEntity { FirstName = "Martin", LastName = "Scorsese", BirthDate = new DateTime(1942, 11, 17) },
            new DirectorEntity { FirstName = "Yorgos", LastName = "Lanthimos", BirthDate = new DateTime(1973, 9, 23) }
        };

        foreach (var seed in directorSeeds)
        {
            var exists = await context.Directors.AnyAsync(x =>
                x.FirstName.ToLower() == seed.FirstName.ToLower() &&
                x.LastName.ToLower() == seed.LastName.ToLower() &&
                x.BirthDate == seed.BirthDate.Date);

            if (!exists)
            {
                context.Directors.Add(seed);
            }
        }

        await context.SaveChangesAsync();

        var directorKeys = directorSeeds
            .Select(CreateDirectorKey)
            .ToHashSet();

        var seededDirectors = await context.Directors
            .AsNoTracking()
            .ToListAsync();

        return seededDirectors
            .Where(x => directorKeys.Contains(CreateDirectorKey(x)))
            .GroupBy(CreateDirectorKey)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First());
    }

    private static async Task<Dictionary<string, ActorEntity>> SeedActorsAsync(DatabaseContext context)
    {
        var actorSeeds = new[]
        {
            new ActorEntity
            {
                FirstName = "Emma",
                LastName = "Stone",
                BirthDate = new DateTime(1988, 11, 6),
                CountryId = 2,
                ImdbLink = "nm1297015",
                Biography = "American actress."
            },
            new ActorEntity
            {
                FirstName = "Willem",
                LastName = "Dafoe",
                BirthDate = new DateTime(1955, 7, 22),
                CountryId = 2,
                ImdbLink = "nm0000353",
                Biography = "American actor."
            },
            new ActorEntity
            {
                FirstName = "Mark",
                LastName = "Ruffalo",
                BirthDate = new DateTime(1967, 11, 22),
                CountryId = 2,
                ImdbLink = "nm0749263",
                Biography = "American actor."
            },
            new ActorEntity
            {
                FirstName = "Ramy",
                LastName = "Youssef",
                BirthDate = new DateTime(1991, 3, 26),
                CountryId = 2,
                ImdbLink = "nm5358492",
                Biography = "American actor and writer."
            },
            new ActorEntity
            {
                FirstName = "Christopher",
                LastName = "Abbott",
                BirthDate = new DateTime(1986, 2, 1),
                CountryId = 2,
                ImdbLink = "nm2277940",
                Biography = "American actor."
            },
            new ActorEntity
            {
                FirstName = "Jerrod",
                LastName = "Carmichael",
                BirthDate = new DateTime(1987, 4, 22),
                CountryId = 2,
                ImdbLink = "nm3325846",
                Biography = "American comedian and actor."
            },
            new ActorEntity
            {
                FirstName = "Margaret",
                LastName = "Qualley",
                BirthDate = new DateTime(1994, 10, 23),
                CountryId = 2,
                ImdbLink = "nm4960279",
                Biography = "American actress."
            }
        };

        foreach (var seed in actorSeeds)
        {
            var exists = await context.Actors.AnyAsync(x =>
                x.FirstName.ToLower() == seed.FirstName.ToLower() &&
                x.LastName.ToLower() == seed.LastName.ToLower() &&
                x.BirthDate == seed.BirthDate.Date);

            if (!exists)
            {
                context.Actors.Add(seed);
            }
        }

        await context.SaveChangesAsync();

        var actorKeys = actorSeeds
            .Select(CreateActorKey)
            .ToHashSet();

        var seededActors = await context.Actors
            .AsNoTracking()
            .ToListAsync();

        return seededActors
            .Where(x => actorKeys.Contains(CreateActorKey(x)))
            .GroupBy(CreateActorKey)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First());
    }

    private static async Task<Dictionary<string, MovieEntity>> SeedMoviesAsync(
        DatabaseContext context,
        Dictionary<string, DirectorEntity> directors)
    {
        var directorPeter = GetRequired(directors, CreateDirectorKey("Peter", "Jackson", new DateTime(1961, 10, 31)));
        var directorMartin = GetRequired(directors, CreateDirectorKey("Martin", "Scorsese", new DateTime(1942, 11, 17)));
        var directorYorgos = GetRequired(directors, CreateDirectorKey("Yorgos", "Lanthimos", new DateTime(1973, 9, 23)));

        var movieSeeds = new[]
        {
            new MovieEntity
            {
                Title = "The Lord of the Rings: The Fellowship of the Ring",
                DirectorId = directorPeter.Id,
                Duration = 178,
                ReleaseDate = new DateTime(2001, 12, 19),
                Price = 5m,
                CountryId = 2,
                TrailerLink = "https://www.youtube.com/watch?v=V75dMMIW2B4",
                StoryLine = "A young hobbit begins a perilous journey to destroy a powerful ring."
            },
            new MovieEntity
            {
                Title = "The Departed",
                DirectorId = directorMartin.Id,
                Duration = 151,
                ReleaseDate = new DateTime(2006, 10, 6),
                Price = 10m,
                CountryId = 2,
                TrailerLink = "https://www.youtube.com/watch?v=iojhqm0JTW4",
                StoryLine = "An undercover cop and a criminal mole race to reveal each other's identity."
            },
            new MovieEntity
            {
                Title = "Poor Things",
                DirectorId = directorYorgos.Id,
                Duration = 141,
                ReleaseDate = new DateTime(2023, 12, 8),
                Price = 12m,
                CountryId = 2,
                TrailerLink = "https://www.youtube.com/watch?v=RlbR5N6veqw",
                StoryLine = "A woman's unusual journey of self-discovery unfolds in a surreal world."
            }
        };

        foreach (var seed in movieSeeds)
        {
            var exists = await context.Movies.AnyAsync(x =>
                x.Title.ToLower() == seed.Title.ToLower() &&
                x.ReleaseDate == seed.ReleaseDate.Date);

            if (!exists)
            {
                context.Movies.Add(seed);
            }
        }

        await context.SaveChangesAsync();

        var movieKeys = movieSeeds
            .Select(CreateMovieKey)
            .ToHashSet();

        var seededMovies = await context.Movies
            .AsNoTracking()
            .ToListAsync();

        return seededMovies
            .Where(x => movieKeys.Contains(CreateMovieKey(x)))
            .GroupBy(CreateMovieKey)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First());
    }

    private static async Task SeedMovieCategoriesAsync(
        DatabaseContext context,
        Dictionary<string, MovieEntity> movies,
        Dictionary<string, CategoryEntity> categories)
    {
        var links = new[]
        {
            (Movie: CreateMovieKey("The Lord of the Rings: The Fellowship of the Ring", new DateTime(2001, 12, 19)), Category: "action"),
            (Movie: CreateMovieKey("The Lord of the Rings: The Fellowship of the Ring", new DateTime(2001, 12, 19)), Category: "crime"),
            (Movie: CreateMovieKey("The Departed", new DateTime(2006, 10, 6)), Category: "crime"),
            (Movie: CreateMovieKey("Poor Things", new DateTime(2023, 12, 8)), Category: "comedy")
        };

        foreach (var (movieKey, categoryKey) in links)
        {
            var movie = GetRequired(movies, movieKey);
            var category = GetRequired(categories, categoryKey);

            var exists = await context.MovieCategories
                .AnyAsync(x => x.MovieId == movie.Id && x.CategoryId == category.Id);

            if (!exists)
            {
                context.MovieCategories.Add(new MovieCategoryEntity
                {
                    MovieId = movie.Id,
                    CategoryId = category.Id
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedMovieActorsAsync(
        DatabaseContext context,
        Dictionary<string, MovieEntity> movies,
        Dictionary<string, ActorEntity> actors)
    {
        var poorThingsKey = CreateMovieKey("Poor Things", new DateTime(2023, 12, 8));
        var poorThings = GetRequired(movies, poorThingsKey);

        var links = new[]
        {
            (Actor: CreateActorKey("Emma", "Stone", new DateTime(1988, 11, 6)), Character: "Bella Baxter"),
            (Actor: CreateActorKey("Willem", "Dafoe", new DateTime(1955, 7, 22)), Character: "Dr Godwin Baxter"),
            (Actor: CreateActorKey("Mark", "Ruffalo", new DateTime(1967, 11, 22)), Character: "Duncan Wedderburn"),
            (Actor: CreateActorKey("Ramy", "Youssef", new DateTime(1991, 3, 26)), Character: "Max McCandles"),
            (Actor: CreateActorKey("Christopher", "Abbott", new DateTime(1986, 2, 1)), Character: "Alfie Blessington"),
            (Actor: CreateActorKey("Jerrod", "Carmichael", new DateTime(1987, 4, 22)), Character: "Harry Astley"),
            (Actor: CreateActorKey("Margaret", "Qualley", new DateTime(1994, 10, 23)), Character: "Felicity")
        };

        foreach (var (actorKey, character) in links)
        {
            var actor = GetRequired(actors, actorKey);

            var exists = await context.MovieActors
                .AnyAsync(x => x.MovieId == poorThings.Id && x.ActorId == actor.Id);

            if (!exists)
            {
                context.MovieActors.Add(new MovieActorEntity
                {
                    MovieId = poorThings.Id,
                    ActorId = actor.Id,
                    CharacterName = character
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static string CreateDirectorKey(DirectorEntity director)
        => CreateDirectorKey(director.FirstName, director.LastName, director.BirthDate);

    private static string CreateDirectorKey(string firstName, string lastName, DateTime birthDate)
        => $"{firstName.Trim().ToLowerInvariant()}|{lastName.Trim().ToLowerInvariant()}|{birthDate:yyyy-MM-dd}";

    private static string CreateActorKey(ActorEntity actor)
        => CreateActorKey(actor.FirstName, actor.LastName, actor.BirthDate);

    private static string CreateActorKey(string firstName, string lastName, DateTime birthDate)
        => $"{firstName.Trim().ToLowerInvariant()}|{lastName.Trim().ToLowerInvariant()}|{birthDate:yyyy-MM-dd}";

    private static string CreateMovieKey(MovieEntity movie)
        => CreateMovieKey(movie.Title, movie.ReleaseDate);

    private static string CreateMovieKey(string title, DateTime releaseDate)
        => $"{title.Trim().ToLowerInvariant()}|{releaseDate:yyyy-MM-dd}";

    private static TValue GetRequired<TValue>(IReadOnlyDictionary<string, TValue> map, string key)
        where TValue : class
    {
        if (map.TryGetValue(key, out var value))
            return value;

        throw new InvalidOperationException($"Seed dependency not found for key '{key}'.");
    }
}
