using s27636cw5.DTOs;

namespace s27636cw5.DTO;


public class PrescriptionRequestDto
{
    public PatientDto Patient { get; set; } = null!;
    public List<MedicamentDto> Medicaments { get; set; } = new();
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}
