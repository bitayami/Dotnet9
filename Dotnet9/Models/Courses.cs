using System.ComponentModel.DataAnnotations.Schema;

namespace Dotnet9.Models
{
    public class Courses
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
