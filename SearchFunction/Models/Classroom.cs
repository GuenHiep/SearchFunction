using System.ComponentModel.DataAnnotations;

namespace SearchFunction.Models
{
    public class Classroom
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Classroom>? Classrooms { get; set; }
    }
}
