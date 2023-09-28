using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Title")]
        public string? Title { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }
        [DisplayName("Completion status")]
        public bool IsCompleted { get; set; }
        [DisplayName("Due Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public Guid AssignedUserId { get; set; }
        [DisplayName("Assigned User")]
        public string AssignedUserName { get; set; }

        public bool IsEditMode { get; set; } //Used for grid
    }
}
