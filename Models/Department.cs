using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyLine.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Invalid Name.")]
        [MinLength(2, ErrorMessage = "The length is less than 2 characters.")]
        [MaxLength(20, ErrorMessage = "The length is more than 20 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Invalid Description.")]
        [MinLength(10, ErrorMessage = "The length is less than 10 characters.")]
        [MaxLength(20, ErrorMessage = "The length is more than 20 characters.")]
        public string Description { get; set; }

        [Range(150000, 3000000, ErrorMessage = "Budget must be between 150000 EGP and 3000000 EGP")]
        public decimal AnnualBudget { get; set; }   
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        [DataType(DataType.Password)]
        public string SecretCode { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("SecretCode", ErrorMessage = "Code and Confirm Code don't match.")]
        public string ConfirmCode { get; set; }
        [ValidateNever]
        public List<Employee> Employees { get; set; }

        [DisplayName("Image")]
        [ValidateNever]
        public string? ImagePath { get; set; }
    }
}
