﻿using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HR.LeaveManagement.MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAuthenticationService _authService;
        public UsersController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                returnUrl ??= Url.Content("~/");
                var isLoggedIn = await _authService.Authenticate(login.Email, login.Password);
                if (isLoggedIn)
                    return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError("", "Log In Attempt Failed. Please try again.");
            return View(login);

        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registration)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var returnUrl = Url.Content("~/");
                    var isCreated = await _authService.Register(registration);
                    if (isCreated)
                        return LocalRedirect(returnUrl);
                }
            }
            catch (ApiException ex)
            {
                
                if (ex.StatusCode == StatusCodes.Status422UnprocessableEntity)
                {
                    var deserialized_response = JsonConvert.DeserializeObject<List<string>>(ex.Response, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    
                    if (deserialized_response != null)
                    {
                        foreach (var error in deserialized_response)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
                if (ex.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    var deserialized_response = JsonConvert.DeserializeObject<ErrorDetails>(ex.Response, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    ModelState.AddModelError("", "Registration Attempt Failed. Please try again.");
                }
            }            
            return View(registration);
        }
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            await _authService.Logout();
            return LocalRedirect(returnUrl);
        }
    }
}
