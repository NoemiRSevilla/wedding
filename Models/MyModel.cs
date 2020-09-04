using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Wedding.Models
{
    public class WeddPlan
    {
        [Key]
        public int WeddPlanId { get; set; }

        [Required(ErrorMessage = "Groom Name is required")]
        [MinLength(2, ErrorMessage = "Full Name must be at least 2 characters.")]
        public string WedderOne { get; set; }


        [Required(ErrorMessage = "Bride Name is required")]
        [MinLength(2, ErrorMessage = "Full Name must be at least 2 characters.")]
        public string WedderTwo { get; set; }

        [Required]
        [RestrictedDate(ErrorMessage = "Please select valid Upcoming Date, for Wedding Date !!!")]
        
        public DateTime WeddingDate { get; set; } 

        [Required]
        [MinLength(2, ErrorMessage = "Address must be at least 2 characters.")]
        public string Address { get; set; }
        public List<RSVP> GuestList { get; set; }
        public int WeddingPlanner { get; set; }

    }

    public class RSVP
    {
        [Key]
        public int RSVPId { get; set; }
        public int WeddPlanId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public WeddPlan WeddPlan { get; set; }

    }
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Must be longer than 2")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }
        public List<RSVP> Atending { get; set; }

    }

    public class LoginUser
    {
        // Other fields
        [Required]
        [Display(Name = "Email")]
        public string LoginEmail { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }
    }

    public class RestrictedDate : ValidationAttribute 
        {
        //validation to have a past data, not future
        public override bool IsValid (object submittedDate) 
            {
            DateTime date = (DateTime) submittedDate;
            return date > DateTime.Now;
            }
        }






}

