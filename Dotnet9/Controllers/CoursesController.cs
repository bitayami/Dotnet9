using Asp.Versioning;
using AutoMapper;
using Dotnet9.Data;
using Dotnet9.Middleware;
using Dotnet9.Models;
using Dotnet9.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _cache;
        public CoursesController(ApplicationDbContext db, IMapper mapper, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        //[MapToApiVersion("1.0")]
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
            string cacheKey = $"Course_{id}";
            if(_cache.TryGetValue(cacheKey, out Courses cachedCourse))//TryGetValue method checks if the cache contains an entry with the specified key (cacheKey). If it does, it retrieves the cached value and assigns it to the cachedCourse variable. The method returns true if the cache entry exists and is successfully retrieved; otherwise, it returns false.
            {
                return Ok(cachedCourse);
            }

            var course = _db.Courses.Find(id);
            if (course == null)
            {
                //throw new NotFoundException("Course id is not found from Exception");
                return NotFound("Courses not found");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))//SetAbsoluteExpiration method sets an absolute expiration time for the cache entry. In this case, the cache entry will expire and be removed from the cache after 2 minutes, regardless of whether it has been accessed or not.
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));//SetSlidingExpiration method sets a sliding expiration time for the cache entry. In this case, if the cache entry is accessed within 30 seconds of its last access, the expiration time will be reset, and the entry will remain in the cache for another 30 seconds. If the entry is not accessed within 30 seconds, it will expire and be removed from the cache.

            _cache.Set(cacheKey, course, cacheEntryOptions);//Set method adds a cache entry with the specified key (cacheKey), value (course), and cache entry options (cacheEntryOptions). The cache entry will be stored in the cache and will expire according to the defined expiration settings.

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
            _cache.Remove($"Course_{id}");
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
