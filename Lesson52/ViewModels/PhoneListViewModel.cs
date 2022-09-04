using Lesson52.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Lesson52.ViewModels
{
    public class PhoneListViewModel
    {
        public IEnumerable<Phone> Phones { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public SelectList Companies { get; set; }
        public string Name { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }


    }
}
