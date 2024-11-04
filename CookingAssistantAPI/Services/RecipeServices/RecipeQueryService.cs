﻿using CookingAssistantAPI.DTO.Recipes;
using CookingAssistantAPI.Exceptions;
using CookingAssistantAPI.Tools;

namespace CookingAssistantAPI.Services.RecipeServices
{
    public class RecipeQueryService : IRecipeQueryService
    {
        public Dictionary<string, Func<RecipeSimpleGetDTO, object>> SortSelector => new Dictionary<string, Func<RecipeSimpleGetDTO, object>>
        {
            { nameof(RecipeSimpleGetDTO.Ratings), r => r.Ratings },
            { nameof(RecipeSimpleGetDTO.TimeInMinutes), r => r.TimeInMinutes },
            { nameof(RecipeSimpleGetDTO.Difficulty), r => r.Difficulty },
            { nameof(RecipeSimpleGetDTO.VoteCount), r => r.VoteCount },
            { nameof(RecipeSimpleGetDTO.CategoryName), r => r.CategoryName },
            { nameof(RecipeSimpleGetDTO.Caloricity), r => r.Caloricity }
        };

        public Dictionary<string, int> DifficultyOrder => new Dictionary<string, int>
        {
            { "easy", 1 },
            { "medium", 2 },
            { "hard", 3 }
        };

        public List<RecipeSimpleGetDTO> SearchRecipes(ref List<RecipeSimpleGetDTO> recipeDtos, string? searchPhrase, string? ingredientsSearch)
        {
            if (searchPhrase != null)
            {
                recipeDtos = recipeDtos.Where(r => r.Name.ToLower().Contains(searchPhrase.ToLower())
                || r.Description.ToLower().Contains(searchPhrase.ToLower())).ToList();

                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found using this search phrase");
                }
            }

            if (ingredientsSearch != null)
            {
                // Ingredients seprated by comma
                var ingredients = ingredientsSearch.Split(',').Select(i => i.ToLower()).ToList();
                recipeDtos = recipeDtos.Where(r => r.IngredientNames.Any(i => ingredients.Contains(i.ToLower()))).ToList();

                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found using given ingredients");
                }
            }

            return recipeDtos;
        }

        public List<RecipeSimpleGetDTO> SortRecipes(ref List<RecipeSimpleGetDTO> recipeDtos, SortBy? sortBy, SortDirection? sortDirection)
        {
            if (sortDirection != null && sortBy != null)
            {

                var selectedSortBy = SortSelector[sortBy.ToString()];

                if (sortDirection == SortDirection.Ascending)
                {
                    if (sortBy == SortBy.Difficulty)
                    {
                        recipeDtos = recipeDtos.OrderBy(r => DifficultyOrder[r.Difficulty.ToLower()]).ToList();
                    }
                    else
                    {
                        recipeDtos = recipeDtos.OrderBy(selectedSortBy).ToList();
                    }

                }
                else
                {
                    if (sortBy == SortBy.Difficulty)
                    {
                        recipeDtos = recipeDtos.OrderByDescending(r => DifficultyOrder[r.Difficulty.ToLower()]).ToList();
                    }
                    else
                    {
                        recipeDtos = recipeDtos.OrderByDescending(selectedSortBy).ToList();
                    }
                }

                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found using given sorting parameters");
                }
            }
            return recipeDtos;

        }

        public List<RecipeSimpleGetDTO> RecipeFilter(ref List<RecipeSimpleGetDTO> recipeDtos, string? categoryName, string? difficulty, string? occasion)
        {
            if (difficulty != null)
            {
                recipeDtos = recipeDtos.Where(r => r.Difficulty.ToLower() == difficulty.ToLower()).ToList();
                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found with given difficulty");
                }
            }

            if (categoryName != null)
            {
                recipeDtos = recipeDtos.Where(r => r.CategoryName.ToLower() == categoryName.ToLower()).ToList();
                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found with given category");
                }
            }

            if (occasion != null)
            {
                recipeDtos = recipeDtos.Where(r => r.Occasion.ToLower() == occasion.ToLower()).ToList();
                if (!recipeDtos.Any())
                {
                    throw new BadRequestException("No recipe found with given occasion");
                }
            }

            return recipeDtos;

        }

    }
}
