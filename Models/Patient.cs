using System.ComponentModel.DataAnnotations;

namespace s27636cw5.Models;

public class Patient
{
    [Key]
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}