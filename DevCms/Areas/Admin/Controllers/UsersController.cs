using System;
using System.Linq;
using System.Threading.Tasks;
using DevCms.Db;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly DevCmsDb _db;

        public UsersController(DevCmsDb context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    CreateUser(model.Email, model.Password);
                    return View();
                }

                ModelState.AddModelError("Email", "Пользователь с таким Email уже существует.");
                return View(model);
            }
            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id < 1)
                return NotFound();

            var user = _db.Users
                .FirstOrDefault(m => m.Id == id);
            if (user == null)
                return NotFound();

            var model = new EditUserModel
            {
                Id = user.Id,
                Email = user.Email,
                Password = PasswordHelper.SixAsterix,
                ConfirmPassword = PasswordHelper.SixAsterix
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users
                    .FirstOrDefault(u => u.Id == model.Id);
                if (user == null)
                    return NotFound();
                if (_db.Users.Any(u => u.Email == model.Email && u.Id != model.Id))
                {
                    ModelState.AddModelError("Email", "Пользователь с таким Email уже существует.");
                    return View(model);
                }
                user.Email = model.Email;
                if(model.Password != PasswordHelper.SixAsterix)
                    PasswordHelper.SetPasswordHashed(user, model.Password);
                _db.SaveChanges();

                return View();

            }

            return View(model);
        }

        private void CreateUser(string email, string password)
        {
            var user = new User
            {
                Email = email,
                RegistrationStatus = UserExt.ApprovedStatus,
                RegistrationDateTime = DateTime.UtcNow,
                EmailStatus = UserExt.PendingStatus,
                SecurityQuestion = string.Empty, //todo:
                SecurityAnswer = string.Empty //todo:
            };

            PasswordHelper.SetPasswordHashed(user, password);
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}