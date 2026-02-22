using AutoMapper;
using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public StudentController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            //Lazy loading
            //List<Student> students = _db.Students.ToList();
            ////var count = students.Count;

            //foreach (var student in students)
            //{
            //    student.courses = _db.Courses.Where(c => c.StudentId == student.Id).ToList();
            //}

            //Eager
            var students = _db.Students.Include(s => s.courses).ToList();
            return Ok(students);

            // Explicit loading
            //Student? student = _db.Students.FirstOrDefault(x => x.Id == 1);
            //_db.Entry(student).Collection(s => s.courses).Load();
            //return Ok(student);

            //Courses courses = _db.Courses.Find(1);
            //_db.Entry(courses).Reference(c => c.Student).Load();

            //return Ok(courses);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            var student = _db.Students.Include(s => s.courses).FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }
        [HttpPost]
        public IActionResult PostStudent(StudentDto student)
        {
            //Student students = new Student
            //{
            //    Name = student.Name
            //};

            var stud = _mapper.Map<Student>(student);

            _db.Students.Add(stud);
            _db.SaveChanges();
            return Ok(student);
        }
        [HttpPut("{id}")]
        public IActionResult PutStudent(Student student)
        {
            var existingStudent = _db.Students.Find(student.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }
            existingStudent.Name = student.Name;
            _db.SaveChanges();
            return Ok(existingStudent);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _db.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            _db.Students.Remove(student);
            _db.SaveChanges();
            return Ok(student);
        }

    }
}
