﻿using Core.Entities;
using Core.Interfaces;
using Core.Utilities.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Core.Utilities
{
    public static class JWTManager
    {
        public static string GenerateToken(User user)
        {
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ArduinoESP8266App"));
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetToken(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer", "").TrimStart(' ');
            return token;
        }

        public static int GetUserId(HttpContext context,IUnitOfWork uow)
        {
            int userId = default;
            var token = GetToken(context);
            if (String.IsNullOrEmpty(token))
                return userId;
            IDataResult<User> user = uow.User.CheckToken(token);
            if (user.Success)
                userId = user.Data.Id;
            return userId;
        }

        public static User GetUser(int userId,IUnitOfWork uow)
        {
            User user = default;
            IDataResult<User> userExists = uow.User.FindById(userId);
            if (userExists.Success)
                user = userExists.Data;
            return user;
        }
    }
}
