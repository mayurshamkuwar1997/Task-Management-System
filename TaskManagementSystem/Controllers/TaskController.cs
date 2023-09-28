using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskManagementSystem.Helper;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        public List<RegisterViewModel> Users { get; set; } = new List<RegisterViewModel>();
        public TaskController()
        {
            string existingUserJsonStr = FileHelper.GetFile("Database/UserDetails.json");
            if (!string.IsNullOrWhiteSpace(existingUserJsonStr))
            {
                Users = JsonConvert.DeserializeObject<List<RegisterViewModel>>(existingUserJsonStr) ?? new List<RegisterViewModel>();
            }
        }

        public IActionResult Index()
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            List<TaskViewModel> allTasks = GetAllTask();
            return View(allTasks);
        }

        public IActionResult Reset()
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            return View("Index", allTask);
        }

        public IActionResult Search(string searchKey)
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            if (string.IsNullOrWhiteSpace(searchKey))
            {
                return View("Index", allTask);
            }
            var modifiedTasks = new List<TaskViewModel>();
            for (int i = 0; i < allTask.Count; i++)
            {
                if (allTask[i].Title.Contains(searchKey) || allTask[i].Description.Contains(searchKey))
                {
                    modifiedTasks.Add(allTask[i]);
                }
            }
            return View("Index", modifiedTasks);
        }

        public IActionResult Edit(Guid id)
        {
            ViewData["UserList"] = Users;
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            for (int i = 0; i < allTask.Count; i++)
            {
                if (allTask[i].Id == id)
                    allTask[i].IsEditMode = true;
            }
            return View("Index", allTask);
        }

        [HttpPost]
        public IActionResult Update(Guid Id, string Title, string Description, string IsCompleted, DateTime DueDate, Guid AssignedUserId)
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            for (int i = 0; i < allTask.Count; i++)
            {
                if (allTask[i].Id == Id)
                {
                    allTask[i].Title = Title;
                    allTask[i].Description = Description;
                    allTask[i].IsCompleted = Convert.ToBoolean(IsCompleted);
                    allTask[i].DueDate = DueDate;
                    allTask[i].AssignedUserId = AssignedUserId;
                    allTask[i].AssignedUserName = Users.FirstOrDefault(x => x.Id == AssignedUserId).Name;
                    break;
                }
            }
            FileHelper.SaveFile(JsonConvert.SerializeObject(allTask, Formatting.Indented), "Database/TaskDetails.json");
            return View("Index", allTask);
        }

        public IActionResult Create()
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            return View();
        }

        [HttpPost]
        public IActionResult Filter(int completionStatus, DateTime selectedDate)
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            var filteredTasks = new List<TaskViewModel>();
            var DatefilteredTasks = new List<TaskViewModel>();
            bool isDateFilter = false;
            if (selectedDate.ToString("yyyy-MM-dd") != "0001-01-01")
            {
                isDateFilter = true;
                for (int i = 0; i < allTask.Count; i++)
                {
                    if (allTask[i].DueDate.Date.ToString("yyyy-MM-dd") == selectedDate.Date.ToString("yyyy-MM-dd"))
                    {
                        DatefilteredTasks.Add(allTask[i]);
                    }
                }
            }

            if (completionStatus != 0)
            {
                if (isDateFilter)
                {
                    allTask = DatefilteredTasks;
                }
                for (int i = 0; i < allTask.Count; i++)
                {
                    if (completionStatus == 1 && allTask[i].IsCompleted && !filteredTasks.Contains(allTask[i]))
                    {
                        filteredTasks.Add(allTask[i]);
                    }
                    else if (completionStatus == 2 && !allTask[i].IsCompleted && !filteredTasks.Contains(allTask[i]))
                    {
                        filteredTasks.Add(allTask[i]);
                    }
                }
            }
            else
            {
                filteredTasks = DatefilteredTasks;
            }
            return View("Index", filteredTasks);
        }

        public IActionResult Delete(Guid id)
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            var allTask = GetAllTask();
            var taskToDelete = new TaskViewModel();
            for (int i = 0; i < allTask.Count; i++)
            {
                if (allTask[i].Id == id)
                {
                    taskToDelete = allTask[i];
                    break;
                }
            }
            allTask.Remove(taskToDelete);

            FileHelper.SaveFile(JsonConvert.SerializeObject(allTask, Formatting.Indented), "Database/TaskDetails.json");
            return View("Index", allTask);
        }

        public List<TaskViewModel> GetAllTask()
        {
            //Get existing all task information in list
            string existingTaskDetails = FileHelper.GetFile(@"Database/TaskDetails.json");
            List<TaskViewModel> allTasks = new List<TaskViewModel>();

            if (!string.IsNullOrWhiteSpace(existingTaskDetails))
            {
                allTasks = JsonConvert.DeserializeObject<List<TaskViewModel>>(existingTaskDetails) ?? new List<TaskViewModel>();
            }
            return allTasks;
        }

        public IActionResult AddTask(TaskViewModel newTask)
        {
            ViewData["loggedInEmail"] = LogInSessionHelper.GetLoggedInUserEmail();
            //Get existing all task information in list
            List<TaskViewModel> allTasks = GetAllTask();

            //If not exist Register new user
            //Generate unique id for new user
            Guid newTaskId = Guid.NewGuid();
            newTask.Id = newTaskId;

            allTasks.Add(newTask);
            string taskJsonStr = JsonConvert.SerializeObject(allTasks, Formatting.Indented);

            //Save json file 
            FileHelper.SaveFile(taskJsonStr, "Database/TaskDetails.json");
            return RedirectToAction("Index");
        }

    }
}
