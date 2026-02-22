namespace Dotnet9.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Courses> courses { get; set; }
    }
}
