using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly APIDbContext _dbContext;

        public StudentController(APIDbContext dbContext)
        {  
            _dbContext = dbContext; 
        }

        private bool StudentExists(int id)
        {
            return _dbContext.Students.Any(e => e.StudentId == id);
        }

        //Get student API

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _dbContext.Students.ToListAsync();
        }

        //Get student by Id

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if (id == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreatStudent(Student student)
        {
            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudentById), new {id = student.StudentId}, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatStudent(int id, Student student)
        {
            if(id != student.StudentId)
            {
                return BadRequest();
            }

            _dbContext.Entry(student).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!StudentExists(id))
                {
                    return NotFound(id);    
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            if(student == null)
            {
                return NotFound();
            }

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
