using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;
using WhiteLagoon.Application.Common.SD;
namespace WhiteLagoon.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login(string? ReturnUrl = null)
        {
            //check if user is already logged in, if so redirect to home page
            // if(readiractUrl == null) rediractUrl = Url.Content("~/");
            ReturnUrl ??= Url.Content("~/");

            LoginVM loginVM = new LoginVM()
            {
                RedirectUrl = ReturnUrl
            };
            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if(ModelState.IsValid)
            {
                //lockoutOnFailure check if the user fails to login multiple times
                var Result = await _signInManager.
                    PasswordSignInAsync(loginVM.Email, loginVM.Password,loginVM.RememberMe , lockoutOnFailure:false);
                if(Result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginVM.Email);
                    if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                    {
                        return RedirectToAction(nameof(Index), "Dashboard");
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(loginVM.RedirectUrl))
                        {
                            return LocalRedirect(loginVM.RedirectUrl);
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index), "Home");
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(loginVM);
        }
        public IActionResult Register(string? ReturnUrl = null)
        {
            if(!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                _roleManager.CreateAsync(new IdentityRole("Customer")).Wait();
            }
            RegisterVM registerVM = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }),
                RedirectUrl = ReturnUrl
            };
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            ModelState.Remove("Role");
            if(ModelState.IsValid)
            {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                NormalizedEmail = registerVM.Email.ToUpper(),
                EmailConfirmed = true,
                Name = registerVM.Name,
                PhoneNumber = registerVM.PhoneNumber,
                CreatedAt = DateTime.Now
            };
            var Result = await _userManager.CreateAsync(user, registerVM.Password);
            if(Result.Succeeded)
            {
                if(!string.IsNullOrEmpty(registerVM.Role))
                {
                    await _userManager.AddToRoleAsync(user, registerVM.Role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                if(!string.IsNullOrEmpty(registerVM.RedirectUrl))
                {
                    return LocalRedirect(registerVM.RedirectUrl);
                }
                else
                {
                    return RedirectToAction(nameof(Index), "Home");
                }
            }
            else
            {
                foreach(var error in Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
            registerVM = new RegisterVM()
            {
                RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
            };
            return View(registerVM);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
