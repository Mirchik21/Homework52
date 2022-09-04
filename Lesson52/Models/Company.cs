using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Lesson52.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Company()
        {
            Users = new List<User>();
        }
    }
}
