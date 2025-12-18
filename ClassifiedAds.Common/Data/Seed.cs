using ClassifiedAds.Common.DTOs;
using ClassifiedAds.Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ClassifiedAds.Common.Data
{
    public static class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            // Try to read embedded resource first
            var assembly = typeof(Seed).Assembly; // the Common assembly
            var resourceName = assembly.GetManifestResourceNames()
                                       .FirstOrDefault(n => n.EndsWith("UserSeedData.json", StringComparison.OrdinalIgnoreCase));

            string json;
            if (!string.IsNullOrEmpty(resourceName))
            {
                using var stream = assembly.GetManifestResourceStream(resourceName)
                              ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                using var reader = new StreamReader(stream);
                json = await reader.ReadToEndAsync();
            }
            else
            {
                // Fallback: attempt to read from output directory (use AppContext.BaseDirectory)
                var fallbackPath = Path.Combine(AppContext.BaseDirectory, "Data", "UserSeedData.json");
                if (!File.Exists(fallbackPath))
                    throw new FileNotFoundException($"Seed file not found at '{fallbackPath}' and embedded resource not found.");

                json = await File.ReadAllTextAsync(fallbackPath);
            }

            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(json);
            if (members == null || members.Count == 0)
            {
                Console.WriteLine("No members in seed data");
                return;
            }

            foreach (var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    UserName = member.Email,
                    DisplayName = member.DisplayName,
                    ImageUrl = member.ImageUrl,
                    Member = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created
                    }
                };

                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id,
                    IsApproved = true
                });

                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors.First().Description);
                }
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}
