using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s27636cw5.Data;
using s27636cw5.DTO;
using s27636cw5.DTOs;
using s27636cw5.Models;

namespace s27636cw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrescriptionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequestDto request)
    {
        if (request.Medicaments.Count > 10)
            return BadRequest("Recepta może zawierać maksymalnie 10 leków.");

        if (request.DueDate < request.Date)
            return BadRequest("Data ważności nie może być wcześniejsza niż data wystawienia!");

        var medicamentIds = request.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicamentIds = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        if (existingMedicamentIds.Count != medicamentIds.Count)
            return BadRequest("Jeden lub więcej leków nie istnieje w bazie danych.");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.IdPatient == request.Patient.IdPatient);

        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = request.Patient.IdPatient,
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                Birthdate = request.Patient.Birthdate
            };
            _context.Patients.Add(patient);
        }

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == 1);
        if (doctor == null)
            return BadRequest("Brakuje lekarza o IdDoctor = 1");

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            Patient = patient,
            Doctor = doctor,
            PrescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Description = m.Description
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Recepta została dodana." });
    }
}
