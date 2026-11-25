namespace RecipeExtractApp.Domain.Recipes;

public class Ingredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public int Position { get; set; }

    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public required string Name { get; set; }
    public string? Note { get; set; }
    public required string OriginalText { get; set; }
}
