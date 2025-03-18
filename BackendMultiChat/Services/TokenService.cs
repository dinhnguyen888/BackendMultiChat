using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackendMultiChat.Data;
using BackendMultiChat.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BackendMultiChat.Dtos;
using Microsoft.EntityFrameworkCore;
using BackendMultiChat.Interfaces;

namespace BackendMultiChat.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public TokenService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        // Generate Access Token
        public string GenerateAccessToken(AccountGetDto account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("fullName", account.FullName),
                new Claim("email", account.Email),
                new Claim("id", account.AccountId.ToString()),
                new Claim("role", account.Role.ToString()),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate Refresh Token
        public async Task<string> GenerateRefreshToken(Guid accountId)
        {

            // Check refesh token is exist in database. If exist, remove it
            var tokenExist = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.AccountId == accountId);
            if (tokenExist != null)
            {
                _context.RefreshTokens.Remove(tokenExist);
                await _context.SaveChangesAsync();
            }

            // Generate random number for refresh token
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiryTime = DateTime.UtcNow.AddDays(7),
                AccountId = accountId,

            };

            //save refresh token to database
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken.Token;
        }

        // validate token
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, //don't validate lifetime here
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        // Validate Refresh Token
        public async Task<bool> ValidateRefreshToken(Guid accountId, string refreshToken)
        {

            //check refresh token is exist in database
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.AccountId == accountId && rt.Token == refreshToken);

            //check if refresh token is expired or not exist
            if (storedToken == null || storedToken.ExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

    }
}
