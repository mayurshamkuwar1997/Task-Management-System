using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskManagementSystem.Helper;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            LogInSessionHelper.Logout();
            return View("Login");
        }

        public IActionResult RegisterUser(RegisterViewModel registerViewModel)
        {
            //Get existing all user information in list
            string existingUserJsonStr = FileHelper.GetFile("Database/UserDetails.json");
            List<RegisterViewModel> allUsers = new List<RegisterViewModel>();

            if (!string.IsNullOrWhiteSpace(existingUserJsonStr))
            {
                allUsers = JsonConvert.DeserializeObject<List<RegisterViewModel>>(existingUserJsonStr) ?? new List<RegisterViewModel>();
            }

            //Check new user email exist already in database
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (registerViewModel.Email == allUsers[i].Email)
                {
                    return RedirectToAction("Login", new { errorMessage = "User already exists" });
                }
            }

            //If not exist Register new user
            //Generate unique id for new user
            Guid newUserId = Guid.NewGuid();
            registerViewModel.Id = newUserId;

            allUsers.Add(registerViewModel);
            string newUserJsonStr = JsonConvert.SerializeObject(allUsers, Formatting.Indented);  // Serialize / Deserialize

            //Save json file 
            FileHelper.SaveFile(newUserJsonStr, "Database/UserDetails.json");
            return RedirectToAction("Login");
        }

        public IActionResult Login(string errorMessage)
        {
            ViewData["Error"] = errorMessage;
            return View();
        }

        public IActionResult LoginUser(LoginViewModel loginViewModel)
        {
            //email and password requred
            if (string.IsNullOrWhiteSpace(loginViewModel.Username) || string.IsNullOrWhiteSpace(loginViewModel.Password))
            {
                return RedirectToAction("Login", new { errorMessage = "Username and password is mandatory." });
            }

            //if username and password exists then login successful
            string existingUserJsonStr = FileHelper.GetFile("Database/UserDetails.json");
            List<RegisterViewModel> allUsers = new List<RegisterViewModel>();

            if (!string.IsNullOrWhiteSpace(existingUserJsonStr))
            {
                allUsers = JsonConvert.DeserializeObject<List<RegisterViewModel>>(existingUserJsonStr) ?? new List<RegisterViewModel>();
            }

            //Check new user email/username and password exist already in database then it is valid user else invalid user
            bool isValidUser = false;
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (loginViewModel.Username == allUsers[i].Email && loginViewModel.Password == allUsers[i].Password)
                {
                    isValidUser = true;
                    LogInSessionHelper.SetLoggedInUser(allUsers[i].Id, allUsers[i].Email);
                    break;
                }
            }

            if (isValidUser)
                return RedirectToAction("Index", "Task");
            else
                return RedirectToAction("Login", new { errorMessage = "Username or Password is invalid." });
        }
    }
}
