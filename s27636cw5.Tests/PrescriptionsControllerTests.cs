using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using s27636cw5.Controllers;
using s27636cw5.Data;
using s27636cw5.DTO;
using s27636cw5.DTOs;
using s27636cw5.Models;
using Xunit;

namespace s27636cw5.Tests;

public class PrescriptionsControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var context = new AppDbContext(options);

        context.Medicaments.Add(new Medicament
        {
            IdMedicament = 1,
            Name = "Apap",
            Description = "bol glowy",
            Type = "Tabletka"
        });

        context.Doctors.Add(new Doctor
        {
            IdDoctor = 1,
            FirstName = "Krzysztof",
            LastName = "Kubakci",
            Email = "test@test.pl"
        });

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task AddPrescription_ShouldReturnOk_WhenValid()
    {
        
        var context = GetDbContext();
        var controller = new PrescriptionsController(context);

        var request = new PrescriptionRequestDto
        {
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "rafal",
                LastName = "trzaskowski",
                Birthdate = new DateTime(1990, 1, 1)
            },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto
                {
                    IdMedicament = 1,
                    Dose = 2,
                    Description = "Po jedzeniu"
                }
            },
            Date = new DateTime(2023, 1, 1),
            DueDate = new DateTime(2023, 1, 10)
        };

        var result = await controller.AddPrescription(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddPrescription_ShouldReturnBadRequest_WhenTooManyMedicaments()
    {
        
        var context = GetDbContext();
        var controller = new PrescriptionsController(context);

        var request = new PrescriptionRequestDto
        {
            Patient = new PatientDto
            {
                IdPatient = 1,
                FirstName = "karol",
                LastName = "nawrocki",
                Birthdate = new DateTime(1990, 1, 1)
            },
            Medicaments = Enumerable.Range(1, 11).Select(i => new MedicamentDto
            {
                IdMedicament = 1,
                Dose = 1,
                Description = "desc"
            }).ToList(),
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1)
        };

        var result = await controller.AddPrescription(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
