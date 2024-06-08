using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchFunction.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public double GPA { get; set; }

        [ForeignKey("ClassroomId")]
        public Classroom? Classroom { get; set; }
        public int ClassroomId { get; set; }
    }
}
