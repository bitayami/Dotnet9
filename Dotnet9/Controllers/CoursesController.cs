using Asp.Versioning;
using AutoMapper;
using Dotnet9.Data;
using Dotnet9.Middleware;
using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]//api/v1/courses
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
        [MapToApiVersion("1.0")]
        public IActionResult GetCourses()
        {
            var courses = _db.Courses.ToList();
            return Ok(courses);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetCoursesV2()
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
                //throw new NotFoundException("Course id is not found from Exception");
                return NotFound("Courses not found");
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
