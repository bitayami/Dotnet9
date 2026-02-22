using AutoMapper;
using Dotnet9.Data;
using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CoursesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            var courses = _db.Courses.ToList();
            return Ok(courses);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetCourse(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }
        [HttpPost]
        public IActionResult PostCourse(CourseDto course)
        {
            //Courses courses = new Courses
            //{
            //    Name = course.Name,
            //    StudentId = course.StudentId
            //};
            //_db.Courses.Add(courses);

            var courses = _mapper.Map<Courses>(course);
            _db.Courses.Add(courses);
            _db.SaveChanges();
            return Ok(courses);
        }
        [HttpPut("{id:int}")]
        public IActionResult PutCourse(int id, CourseDto course)
        {
            var existingCourse = _db.Courses.Find(id);
            if (existingCourse == null)
            {
                return NotFound();
            }
            existingCourse.Name = course.Name;
            existingCourse.StudentId = course.StudentId;
            _db.SaveChanges();
            return Ok(existingCourse);
        }
         [HttpDelete("{id:int}")]
         public IActionResult DeleteCourse(int id) {
            var existingCourse = _db.Courses.Find(id);
            if (existingCourse == null)
            {
                return NotFound();
            }
            _db.Courses.Remove(existingCourse);
            _db.SaveChanges();
            return Ok(existingCourse);
        }

    }
}
