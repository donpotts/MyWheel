using System.Net.Http.Json;
using System.Text.Json;
using MyWheelApp.Models;

namespace MyWheelApp.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly HttpClient _httpClient;

        public RecipeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Recipe?> GenerateRecipeAsync(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
                return null;

            try
            {
                var request = new { Name = recipeName };
                var response = await _httpClient.PostAsJsonAsync("api/ai/recipe", request);
                var bodyText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"AI API returned {(int)response.StatusCode} {response.ReasonPhrase}: {bodyText}");
                }

                if (string.IsNullOrWhiteSpace(bodyText)) return null;

                // Attempt to parse the response JSON into a JsonDocument for robust handling
                JsonDocument doc;
                try
                {
                    doc = JsonDocument.Parse(bodyText);
                }
                catch (JsonException)
                {
                    // Not JSON, try to parse as direct Recipe string (unlikely)
                    throw new FormatException("API response was not valid JSON.");
                }

                using (doc)
                {
                    var root = doc.RootElement;

                    // Case A: server returned the recipe object directly
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        // Quick heuristic: if object contains recipe-like properties, deserialize that object
                        if (root.TryGetProperty("name", out _) || root.TryGetProperty("ingredients", out _) || root.TryGetProperty("instructions", out _))
                        {
                            var recipe = JsonSerializer.Deserialize<Recipe>(root.GetRawText(), new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (recipe != null) return recipe;
                        }

                        // Case B: server returned { "reply": "<json>" }
                        if (root.TryGetProperty("reply", out var replyElement))
                        {
                            var replyText = replyElement.GetString() ?? string.Empty;

                            var cleanJson = CleanFence(replyText);

                            try
                            {
                                var recipe = JsonSerializer.Deserialize<Recipe>(cleanJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                                if (recipe != null) return recipe;
                            }
                            catch (JsonException)
                            {
                                throw new FormatException("AI reply was not valid recipe JSON.");
                            }
                        }

                        // Case C: server returned an envelope like { "data": { ... } } or different wrapper - try to find a nested object with recipe-like keys
                        foreach (var property in root.EnumerateObject())
                        {
                            if (property.Value.ValueKind == JsonValueKind.Object)
                            {
                                var candidate = property.Value;
                                if (candidate.TryGetProperty("name", out _) || candidate.TryGetProperty("ingredients", out _))
                                {
                                    var recipe = JsonSerializer.Deserialize<Recipe>(candidate.GetRawText(), new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });

                                    if (recipe != null) return recipe;
                                }
                            }
                        }

                        // Case D: server returned a different shape (e.g., LLM choices). Try to extract a 'content' string if present
                        if (root.TryGetProperty("choices", out var choicesElement) && choicesElement.ValueKind == JsonValueKind.Array && choicesElement.GetArrayLength() > 0)
                        {
                            var first = choicesElement[0];
                            if (first.TryGetProperty("message", out var messageEl) && messageEl.TryGetProperty("content", out var contentEl))
                            {
                                var content = contentEl.GetString() ?? string.Empty;
                                var clean = CleanFence(content);
                                try
                                {
                                    var recipe = JsonSerializer.Deserialize<Recipe>(clean, new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    });

                                    if (recipe != null) return recipe;
                                }
                                catch (JsonException)
                                {
                                    throw new FormatException("AI 'choices' content was not valid recipe JSON.");
                                }
                            }
                        }

                        // If we reach here, we couldn't find a recipe within the returned JSON
                        throw new FormatException("API returned JSON but did not contain a recipe or 'reply' field.");
                    }
                    else
                    {
                        // Root is not an object (array or string) - treat as invalid for our purposes
                        throw new FormatException("API response JSON is not an object.");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static string CleanFence(string text)
        {
            var cleanJson = (text ?? string.Empty).Trim();
            if (cleanJson.StartsWith("```"))
            {
                var idx = cleanJson.IndexOf('\n');
                if (idx >= 0) cleanJson = cleanJson.Substring(idx + 1);
                if (cleanJson.EndsWith("```")) cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
                cleanJson = cleanJson.Trim();
            }

            return cleanJson;
        }
    }
}