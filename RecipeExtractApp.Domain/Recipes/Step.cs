namespace RecipeExtractApp.Domain.Recipes;

public class Step
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public int Position { get; set; }
    public required string Text { get; set; }
}