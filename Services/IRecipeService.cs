using MyWheelApp.Models;

namespace MyWheelApp.Services
{
    public interface IRecipeService
    {
        Task<Recipe?> GenerateRecipeAsync(string recipeName);
    }
}