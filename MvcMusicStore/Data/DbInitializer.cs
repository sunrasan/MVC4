using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;

namespace MvcMusicStore.Data
{
    /// <summary>
    /// Replaces the legacy SampleData (DropCreateDatabaseIfModelChanges) and AppConfig
    /// admin-user creation. Called from Program.cs after EF Core migrations run.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            // Ensure the Administrator role exists
            const string adminRole = "Administrator";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Create the default admin user
            string adminUsername = configuration["AdminSettings:DefaultAdminUsername"] ?? "Administrator";
            string adminPassword = configuration["AdminSettings:DefaultAdminPassword"] ?? "Admin@123456";

            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = $"{adminUsername}@musicstore.local",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
            else
            {
                var existingAdmin = await userManager.FindByNameAsync(adminUsername);
                if (existingAdmin != null && !await userManager.IsInRoleAsync(existingAdmin, adminRole))
                {
                    await userManager.AddToRoleAsync(existingAdmin, adminRole);
                }
            }

            // Seed genres, artists, and albums if the database is empty
            if (await context.Genres.AnyAsync())
                return;

            const string imgUrl = "/images/placeholder.png";

            var genres = new List<Genre>
            {
                new Genre { Name = "Pop" },
                new Genre { Name = "Rock" },
                new Genre { Name = "Jazz" },
                new Genre { Name = "Metal" },
                new Genre { Name = "Electronic" },
                new Genre { Name = "Blues" },
                new Genre { Name = "Latin" },
                new Genre { Name = "Rap" },
                new Genre { Name = "Classical" },
                new Genre { Name = "Alternative" },
                new Genre { Name = "Country" },
                new Genre { Name = "R&B" },
                new Genre { Name = "Indie" },
                new Genre { Name = "Punk" },
                new Genre { Name = "World" }
            };
            context.Genres.AddRange(genres);
            await context.SaveChangesAsync();

            var artists = new List<Artist>
            {
                new Artist { Name = "AC/DC" },
                new Artist { Name = "Accept" },
                new Artist { Name = "Aerosmith" },
                new Artist { Name = "Alanis Morissette" },
                new Artist { Name = "Alice in Chains" },
                new Artist { Name = "Audioslave" },
                new Artist { Name = "The Beatles" },
                new Artist { Name = "Black Sabbath" },
                new Artist { Name = "Coldplay" },
                new Artist { Name = "Daft Punk" },
                new Artist { Name = "Deep Purple" },
                new Artist { Name = "Def Leppard" },
                new Artist { Name = "Eminem" },
                new Artist { Name = "Foo Fighters" },
                new Artist { Name = "Guns N' Roses" },
                new Artist { Name = "Iron Maiden" },
                new Artist { Name = "Led Zeppelin" },
                new Artist { Name = "Metallica" },
                new Artist { Name = "Nirvana" },
                new Artist { Name = "Ozzy Osbourne" },
                new Artist { Name = "Pearl Jam" },
                new Artist { Name = "Pink Floyd" },
                new Artist { Name = "Queen" },
                new Artist { Name = "Radiohead" },
                new Artist { Name = "Red Hot Chili Peppers" },
                new Artist { Name = "The Rolling Stones" },
                new Artist { Name = "Rush" },
                new Artist { Name = "Santana" },
                new Artist { Name = "Slayer" },
                new Artist { Name = "Soundgarden" },
                new Artist { Name = "Tool" },
                new Artist { Name = "U2" },
                new Artist { Name = "Van Halen" },
                new Artist { Name = "Various Artists" },
                new Artist { Name = "Weezer" }
            };
            context.Artists.AddRange(artists);
            await context.SaveChangesAsync();

            Genre Rock(string n) => genres.Single(g => g.Name == n);
            Artist Art(string n) => artists.Single(a => a.Name == n);

            var albums = new List<Album>
            {
                new Album { Title = "For Those About To Rock We Salute You", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("AC/DC"), AlbumArtUrl = imgUrl },
                new Album { Title = "Balls to the Wall", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Accept"), AlbumArtUrl = imgUrl },
                new Album { Title = "Restless and Wild", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Accept"), AlbumArtUrl = imgUrl },
                new Album { Title = "Big Ones", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Aerosmith"), AlbumArtUrl = imgUrl },
                new Album { Title = "Jagged Little Pill", Genre = Rock("Alternative"), Price = 8.99M, Artist = Art("Alanis Morissette"), AlbumArtUrl = imgUrl },
                new Album { Title = "Facelift", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Alice in Chains"), AlbumArtUrl = imgUrl },
                new Album { Title = "Dirt", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Alice in Chains"), AlbumArtUrl = imgUrl },
                new Album { Title = "Audioslave", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Audioslave"), AlbumArtUrl = imgUrl },
                new Album { Title = "Abbey Road", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("The Beatles"), AlbumArtUrl = imgUrl },
                new Album { Title = "Revolver", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("The Beatles"), AlbumArtUrl = imgUrl },
                new Album { Title = "Black Sabbath", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Black Sabbath"), AlbumArtUrl = imgUrl },
                new Album { Title = "X&Y", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Coldplay"), AlbumArtUrl = imgUrl },
                new Album { Title = "A Rush of Blood to the Head", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Coldplay"), AlbumArtUrl = imgUrl },
                new Album { Title = "Homework", Genre = Rock("Electronic"), Price = 8.99M, Artist = Art("Daft Punk"), AlbumArtUrl = imgUrl },
                new Album { Title = "Alive 2007", Genre = Rock("Electronic"), Price = 8.99M, Artist = Art("Daft Punk"), AlbumArtUrl = imgUrl },
                new Album { Title = "Machine Head", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Deep Purple"), AlbumArtUrl = imgUrl },
                new Album { Title = "Deep Purple In Rock", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Deep Purple"), AlbumArtUrl = imgUrl },
                new Album { Title = "Hysteria", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Def Leppard"), AlbumArtUrl = imgUrl },
                new Album { Title = "Recovery [Explicit]", Genre = Rock("Rap"), Price = 8.99M, Artist = Art("Eminem"), AlbumArtUrl = imgUrl },
                new Album { Title = "The Colour And The Shape", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Foo Fighters"), AlbumArtUrl = imgUrl },
                new Album { Title = "In Your Honor [Disc 1]", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Foo Fighters"), AlbumArtUrl = imgUrl },
                new Album { Title = "Appetite for Destruction", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Guns N' Roses"), AlbumArtUrl = imgUrl },
                new Album { Title = "Use Your Illusion I", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Guns N' Roses"), AlbumArtUrl = imgUrl },
                new Album { Title = "The Number of The Beast", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Iron Maiden"), AlbumArtUrl = imgUrl },
                new Album { Title = "Powerslave", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Iron Maiden"), AlbumArtUrl = imgUrl },
                new Album { Title = "Led Zeppelin IV", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Led Zeppelin"), AlbumArtUrl = imgUrl },
                new Album { Title = "Physical Graffiti [Disc 1]", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Led Zeppelin"), AlbumArtUrl = imgUrl },
                new Album { Title = "Black Album", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Metallica"), AlbumArtUrl = imgUrl },
                new Album { Title = "Master of Puppets", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Metallica"), AlbumArtUrl = imgUrl },
                new Album { Title = "Kill 'Em All", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Metallica"), AlbumArtUrl = imgUrl },
                new Album { Title = "Nevermind", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Nirvana"), AlbumArtUrl = imgUrl },
                new Album { Title = "Blizzard of Ozz", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Ozzy Osbourne"), AlbumArtUrl = imgUrl },
                new Album { Title = "Ten", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Pearl Jam"), AlbumArtUrl = imgUrl },
                new Album { Title = "Vs.", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Pearl Jam"), AlbumArtUrl = imgUrl },
                new Album { Title = "The Dark Side Of The Moon", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Pink Floyd"), AlbumArtUrl = imgUrl },
                new Album { Title = "The Wall", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Pink Floyd"), AlbumArtUrl = imgUrl },
                new Album { Title = "Wish You Were Here", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Pink Floyd"), AlbumArtUrl = imgUrl },
                new Album { Title = "Greatest Hits I", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Queen"), AlbumArtUrl = imgUrl },
                new Album { Title = "Night At The Opera", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Queen"), AlbumArtUrl = imgUrl },
                new Album { Title = "OK Computer", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Radiohead"), AlbumArtUrl = imgUrl },
                new Album { Title = "In Rainbows", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Radiohead"), AlbumArtUrl = imgUrl },
                new Album { Title = "Californication", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Red Hot Chili Peppers"), AlbumArtUrl = imgUrl },
                new Album { Title = "By The Way", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Red Hot Chili Peppers"), AlbumArtUrl = imgUrl },
                new Album { Title = "Hot Rocks, 1964-1971 (Disc 1)", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("The Rolling Stones"), AlbumArtUrl = imgUrl },
                new Album { Title = "Moving Pictures", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Rush"), AlbumArtUrl = imgUrl },
                new Album { Title = "Supernatural", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Santana"), AlbumArtUrl = imgUrl },
                new Album { Title = "Reign In Blood", Genre = Rock("Metal"), Price = 8.99M, Artist = Art("Slayer"), AlbumArtUrl = imgUrl },
                new Album { Title = "Bad Motorfinger", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Soundgarden"), AlbumArtUrl = imgUrl },
                new Album { Title = "10,000 Days", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Tool"), AlbumArtUrl = imgUrl },
                new Album { Title = "Undertow", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Tool"), AlbumArtUrl = imgUrl },
                new Album { Title = "The Joshua Tree", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("U2"), AlbumArtUrl = imgUrl },
                new Album { Title = "Achtung Baby", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("U2"), AlbumArtUrl = imgUrl },
                new Album { Title = "Van Halen", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Van Halen"), AlbumArtUrl = imgUrl },
                new Album { Title = "Blue", Genre = Rock("Rock"), Price = 8.99M, Artist = Art("Weezer"), AlbumArtUrl = imgUrl }
            };
            context.Albums.AddRange(albums);
            await context.SaveChangesAsync();
        }
    }
}
