using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace VetCare.shared.persistence.EFC.extensions;

public static class NamingConventionsExtension
{
    public static void UseSnakeCaseNamingConventions(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName() ?? entity.DisplayName()));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}
