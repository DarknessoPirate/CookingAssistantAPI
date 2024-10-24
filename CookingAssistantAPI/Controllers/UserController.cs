﻿using Microsoft.AspNetCore.Mvc;
using CookingAssistantAPI.Database.Models;
using CookingAssistantAPI.DTO.Users;
using CookingAssistantAPI.Repositories;
using CookingAssistantAPI.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using CookingAssistantAPI.DTO.Recipes;
using CookingAssistantAPI.Tools;
using CookingAssistantAPI.DTO;


namespace CookingAssistantAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UserRegisterDTO user)
        {
            if (await _service.RegisterUser(user))
            {
                return Created();
            }
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult> LoginUser([FromBody] UserLoginDTO user)
        {
            string token = await _service.GenerateToken(user);
            var response = new LogInResponseDTO { Token = token };
            return Ok(response);
        }

        [HttpPost("favourite/{recipeId}")]
        [Authorize]
        public async Task<ActionResult> AddRecipeToFavourites([FromRoute] int recipeId)
        {
            if (await _service.AddRecipeToFavourites(recipeId))
            {
                return Created();
            }
            return BadRequest();
        }

        [HttpGet("favourite-recipes")]
        [Authorize]
        public async Task<ActionResult<List<RecipeSimpleGetDTO>>> GetFavouriteRecipes([FromQuery] RecipeQuery query)
        {
            var favouriteRecipes = await _service.GetFavouriteRecipesAsync(query);
            return Ok(favouriteRecipes);
        }

        [HttpPost("image")]
        [Authorize]
        public async Task<ActionResult> AddProfilePicture([FromForm] UploadFileDTO profilePicture)
        {
            if (await _service.UploadProfilePicture(profilePicture))
            {
                return Created();
            }
            return BadRequest();
        }
        // public async DeleteAccount
        // public async ChangeRole


    }
}
