using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GraderApi.Models
{
    public class Course
    {
        //Scalar Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CourseNumber { get; set; }

        [DataType(DataType.Text)]
        public string ShortName { get; set; }

        public int Semester { get; set; }

        [DataType(DataType.Date)]
        public DateTime StarTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        //Navigation properties
        public int OwnerId { get; set; }

        /*
        [ForeignKey("OwnerId")]
        public virtual gawga Owner { get; set; }
        */
    }

    public interface ICourseRepository : IDisposable
    {
        Task<IQueryable<Course>> GetAll();
        Task<Course> Get(int id);
        Task<bool> Add(Course item);
        Task<bool> Remove(int id);
        Task<bool> Update(Course item);
    }

    public class CourseRepository : ICourseRepository
    {
        private DatabaseContext _db = new DatabaseContext();

        public async Task<IQueryable<Course>> GetAll()
        {
            return _db.Courses;
        }

        public async Task<Course> Get(int id)
        {
            return _db.Courses.FirstOrDefault(c => c.Id == id);
        }

        public async Task<bool> Add(Course item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Courses.Add(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> Remove(int id)
        {
            var item = _db.Courses.FirstOrDefault(c => c.Id == id);
            if (item == null)
            {
                throw new ArgumentNullException("id");  
            }

            try
            {
                _db.Courses.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> Update(Course item)
        {
            var dbItem = _db.Courses.FirstOrDefault(c => c.Id == item.Id);
            if (dbItem == null)
            {
                throw new ArgumentNullException("item");
            }

            try
            {
                _db.Courses.Remove(dbItem);
                _db.Courses.Add(item);
                await _db.SaveChangesAsync();

                return true;
            }
            catch(DBConcurrencyException)
            {
                return false;
            }
        }


        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_db == null) return;

            _db.Dispose();
            _db = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}