﻿using Conference.Auth;
using Conference.Helpers;
using Conference.Models.ViewModels;
using Conferency.Data;
using Conferency.Data.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Conferancy.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;

        private readonly AuthService _authService;

        public AuthController(IUserRepository repository)
        {
            _repository = repository;
            _authService = new AuthService();
        }

        public async Task<ActionResult> IsEmailUnique(string email)
        {
            var existedUser = await _repository.GetUserByEmailAsync(email);

            if (existedUser != null)
                return Json("Email is already in use", JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Registration()
        {
            var regions = await _repository.GetRegionsAsync();

            var viewModel = new RegistrationViewModel { Regions = regions };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Password = PasswordHelper.HashPassword(model.Password);

                var user = AutoMapper.Mapper.Map<User>(model);
                _repository.AddUser(user);
                await _repository.SaveChangesAsync();

                SetAuthCoockie(user);

                return RedirectToAction("", ""); //Home Index
            }

            var regions = await _repository.GetRegionsAsync();
            var viewModel = new RegistrationViewModel { Regions = regions };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Login(LoginViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginRequestModel request)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = PasswordHelper.HashPassword(request.Password);

                var user = await _repository.GetUserAsync(request.Email, hashedPassword);

                if (user == null)
                    return View(new LoginViewModel { ErrorMessage = "Invalid email or password" });
                else
                {
                    SetAuthCoockie(user);

                    return RedirectToAction("", ""); //Home Index
                }
            }
            else
                return View("Error");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var jwt = HttpContext.Response.Cookies.Get("Jwt");

            jwt.Expires = DateTime.Now;

            return RedirectToAction("", ""); //Home Index
        }


        private void SetAuthCoockie(User user)
        {
            var token = _authService.GenerateJwt(user);
            var jwtCookie = new System.Web.HttpCookie("Jwt", token)
            {
                Expires = DateTime.UtcNow.AddDays(3)
            };

            HttpContext.Response.Cookies.Add(jwtCookie);
        }
    }
}