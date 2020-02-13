using System;
using System.Linq;
using System.Threading.Tasks;
using DevCms.Db;
using DevCms.Models;
using DevCms.Util;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly DevCmsDb _db;
        private readonly IAuthProvider _authProvider;

        public AccountController(DevCmsDb context, IAuthProvider authProvider)
        {
            _db = context;
            _authProvider = authProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    if (PasswordHelper.PasswordsEqual(user.Password, user.PasswordSalt, model.Password))
                    {
                        if (LoginSuccess(user))
                        {
                            _authProvider.SignIn(HttpContext, model.Email);
                            return RedirectToAction("Index", "Notifications", new { area = "Admin" });
                        }
                    }
                    else
                    {
                        if(IsApprovedAccount(user))
                            LoginAttempt(user);
                    }
                }
                else
                    ModelState.AddModelError("", "Некорректный логин и (или) пароль");
            }
            return View(model);
        }

        private void LoginAttempt(User user)
        {
            // lock user if he makes too many login attempts
            var attemptCount = CommonSettings.FailedLoginAttemptLimit;
            var attemptMinute = CommonSettings.AttemptMinute;
            if (user.FailedLoginAttemptDateTime == null 
                || user.FailedLoginAttemptDateTime < DateTime.UtcNow.AddMinutes(-attemptMinute))
            {
                user.FailedLoginAttemptDateTime = DateTime.UtcNow;
                user.FailedLoginAttemptCounter = 1;
                _db.SaveChanges();
                ModelState.AddModelError("", ChooseMessageForUser(user, attemptCount));
            }
            else
            {
                user.FailedLoginAttemptCounter++;
                if (user.FailedLoginAttemptCounter == attemptCount && user.IsRegistrationApproved())
                {
                    user.RegistrationStatus = UserExt.LockedStatus;
                    //todo: _emailService.SendInvalidLoginAttemptsResetPasswordEmail(user);
                    ModelState.AddModelError("", string.Format(Get5FailedLoginAttemptsMessage(), attemptCount));
                }
                else
                    ModelState.AddModelError("", ChooseMessageForUser(user, attemptCount));
                _db.SaveChanges();
            }
        }

        private string Get5FailedLoginAttemptsMessage()
        {
            var result = "Ваша учетная запись заблокирована после {0} неудачных попыток входа."
                    + " Пожалуйста обратитесь к администрации сайта.";
            return result;
        }

        private string ChooseMessageForUser(User user, int attemptCount)
        {
            int remainingAttempt = attemptCount - user.FailedLoginAttemptCounter;
            if (remainingAttempt > 0)
                return remainingAttempt == 1
                    ? "Неверный пароль. У вас осталась 1 попытка."
                    : string.Format("Неверный пароль. Осталось попыток: {0}.", remainingAttempt);
            return string.Format(Get5FailedLoginAttemptsMessage(), attemptCount);
        }

        private bool LoginSuccess(User user)
        {
            if (!IsApprovedAccount(user))
                return false;
            if (user.FailedLoginAttemptDateTime != null || user.FailedLoginAttemptCounter > 0)
            {
                user.FailedLoginAttemptDateTime = null;
                user.FailedLoginAttemptCounter = 0;
                _db.SaveChanges();
            }

            return true;
        }

        private bool IsApprovedAccount(User user)
        {
            if (!user.IsRegistrationApproved()) //todo:  && user.IsEmailApproved()
            {
                ModelState.AddModelError("", "Ваша учетная запись заблокирована. Пожалуйста обратитесь к администрации сайта.");
                return false;
            }

            //todo: 
            //if (user.IsRegistrationApproved() && !user.IsEmailApproved())
            //{
            //    ModelState.AddModelError("", string.Format("You have not completed the registration process. "
            //                              + "Please click on the link in the Verification Email sent to {0}.",
            //        user.Email));
            //    return false;
            //}

            //todo: 
            //if (!user.IsRegistrationApproved() && !user.IsEmailApproved())
            //{
            //    ModelState.AddModelError("", "This user is locked/disabled. Please contact Customer Service.");
            //    return false;
            //}
            return true;
        }

        //[HttpGet]
        //public ForgotPasswordValidation CheckForgotPasswordStep1(string userNameOrEmail, string zip)
        //{
        //    var user = _db.Users.FirstOrDefault(u => u.Email == userNameOrEmail || u.UserName == userNameOrEmail);
        //    if (user == null)
        //        return CreateForgotPasswordValidationError(_t("Your email address or User ID could not be found. Please verify and try again."));
        //    return CheckZip(user, zip);
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ApiResponse CheckForgotPasswordStep2(string userNameOrEmail, string securityAnswer)
        //{
        //    var result = new ApiResponse();
        //    var errors = CheckSecurityAnswer(userNameOrEmail, securityAnswer, false);
        //    if (errors.Count == 0)
        //    {
        //        try
        //        {
        //            var user = _db.Users.FirstOrDefault(x => x.Email == userNameOrEmail || x.UserName == userNameOrEmail);
        //            if (user == null)
        //            {
        //                _log.ErrorFormat("User {0} was not found.", userNameOrEmail);
        //                result.AddError(string.Format(_t("User {0} was not found."), userNameOrEmail));
        //            }
        //            else
        //            {
        //                _emailService.SendResetPasswordEmail(user);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            _log.Error("Error in CheckForgotPasswordStep2.", e);
        //            result.AddError(_t("Error occurred on the server side, please try again later."));
        //        }
        //    }
        //    else
        //    {
        //        result.AddErrors(errors);
        //    }

        //    return result;
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register_Int_Adm(RegisterUserModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        //        if (user == null)
        //        {
        //            // добавляем пользователя в бд
        //            await CreateUser(model.Email, model.Password);

        //            await Authenticate(model.Email); // аутентификация

        //            return RedirectToAction("Index", "Home");
        //        }

        //        ModelState.AddModelError("", "Некорректные логин и(или) пароль");
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> Logout()
        {
            _authProvider.SignOut(HttpContext);
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
    }
}