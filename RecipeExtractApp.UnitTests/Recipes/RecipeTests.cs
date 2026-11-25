using RecipeExtractApp.Domain.Recipes;

namespace RecipeExtractApp.UnitTests.Recipes;

public class RecipeTests
{
    [Fact]
    public void AddIngredient_AssignPositionsInOrder()
    {
        var recipe = new Recipe { Title = "Pancakes" };

        recipe.AddIngredient("2 cups flour", "flour", 2m, "cup");
        recipe.AddIngredient("1 egg", "egg", 1m);

        Assert.Equal(new[] { 1, 2 }, recipe.Ingredients.Select(i => i.Position));
    }

    [Fact]
    public void AddIngredient_WithNegativeQuantity_Throws()
    {
        var recipe = new Recipe { Title = "Pancakes" };

        Assert.Throws<ArgumentOutOfRangeException>(() => recipe.AddIngredient("-1 cup milk", "milk", -1m, "cup"));
    }

    [Fact]
    public void AddIngredient_WithoutQuantity_IsAllowed()
    {
        var recipe = new Recipe { Title = "Pancakes" };

        var salt = recipe.AddIngredient("salt to taste", "salt");

        Assert.Null(salt.Quantity);
    }

    [Fact]
    public void AddStep_WithBlankText_Throws()
    {
        var recipe = new Recipe { Title = "Pancakes" };

        Assert.Throws<ArgumentException>(() => recipe.AddStep(" "));
    }
}
