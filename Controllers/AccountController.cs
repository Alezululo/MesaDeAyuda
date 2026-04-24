using System.Security.Claims;
using MesaDeAyuda.Data;
using MesaDeAyuda.Models.Entities;
using MesaDeAyuda.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MesaDeAyuda.Controllers
{
    public class AccountController : Controller
    {
        private readonly MesaDeAyudaContext _context;

        public AccountController(MesaDeAyudaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == model.Usuario && u.Activo);

            if (user == null)
            {
                ViewBag.LoginError = "Usuario o contraseña inválidos.";
                return View(model);
            }

            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Contrasena);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.LoginError = "Usuario o contraseña inválidos.";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim("Username", user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Rol.Nombre),
                new Claim("Photo", user.FotoUrl ?? "/img/users/default-user.png")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}