using Lesson52.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Lesson52.ViewModels
{
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public SelectList Companies { get; set; }
        public string Name { get; set;}

        public PageViewModel PageViewModel { get; set; }
    }
}
