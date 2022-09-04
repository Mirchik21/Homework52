using System.ComponentModel.DataAnnotations;

namespace Lesson52.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
        public int PhoneId { get; set; }
        public Phone Phone { get; set; }

    }
}
