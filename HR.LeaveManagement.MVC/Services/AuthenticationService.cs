﻿using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HR.LeaveManagement.MVC.Services
{
    public class AuthenticationService : BaseHttpService, Contracts.IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAcessor;
        private JwtSecurityTokenHandler _tokenHandler;

        public AuthenticationService(IClient client, ILocalStorageService localStorage, IHttpContextAccessor httpContextAcessor, JwtSecurityTokenHandler tokenHandler) 
            : base(client, localStorage)
        {
            this._httpContextAcessor = httpContextAcessor;
            this._tokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            try
            {
                AuthRequest authRequest = new AuthRequest() { Email = email, Password = password };
                var authenticationResponse = await _client.LoginAsync(authRequest);

                if (authenticationResponse.Token != string.Empty)
                {
                    //Get Claims from token and Build auth user object
                    var tokenContent = _tokenHandler.ReadJwtToken(authenticationResponse.Token);
                    var claims = ParseClaims(tokenContent);
                    var user = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    var login = _httpContextAcessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                    _localStorage.SetStorageValue("token", authenticationResponse.Token);

                    return true;
                }
                return false;
            }
            catch 
            {
                return false;
            }
        }       

        public async Task<bool> Register(string firstName, string lastName, string userName, string email, string password)
        {
            RegistrationRequest registrationRequest = new() { FirstName = firstName, LastName = lastName, Email = email, UserName = userName, Password = password };
            var response = await _client.RegisterAsync(registrationRequest);

            if (!string.IsNullOrEmpty(response.UserId))
                return true;
            
            return false;
        }
        public async Task Logout()
        {
            _localStorage.ClearStorage(new List<string> { "token" });
            await _httpContextAcessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
