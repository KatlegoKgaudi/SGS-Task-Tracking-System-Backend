using Microsoft.AspNetCore.Mvc;
using SGS.TaskTracker.Interfaces;

namespace SGS.TaskTracker.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        //todo - define authorization actions
        public IActionResult Index()
        {
            return View();
        }
    }
}
