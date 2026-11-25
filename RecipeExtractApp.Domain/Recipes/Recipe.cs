using RecipeExtractApp.Domain.Tags;

namespace RecipeExtractApp.Domain.Recipes;

public class Recipe
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public required string Title { get; set; }
    public string? SourceUrl { get; set; }
    public string? SourceSite { get; set; }
    public string? ImageUrl { get; set; }
    public string? Servings { get; set; }
    public int? PrepMinutes { get; set; }
    public int? CookMinutes { get; set; }
    public string? Notes { get; set; }
    public ExtractionMethod ExtractionMethod { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<Ingredient> Ingredients { get; } = [];
    public List<Step> Steps { get; } = [];
    public List<Tag> Tags { get; } = [];

    public Ingredient AddIngredient(
        string originalText,
        string name,
        decimal? quantity = null,
        string? unit = null,
        string? note = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalText);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        var ingredient = new Ingredient
        {
            Position = Ingredients.Count + 1,
            OriginalText = originalText,
            Name = name,
            Quantity = quantity,
            Unit = unit,
            Note = note,
        };
        Ingredients.Add(ingredient);
        return ingredient;
    }

    public Step AddStep(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var step = new Step { Position = Steps.Count + 1, Text = text };
        Steps.Add(step);
        return step;
    }
}