using RecipeExtractApp.Domain.Recipes;

namespace RecipeExtractApp.Domain.Tags;

public class Tag
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }

    public List<Recipe> Recipes { get; } = [];
}