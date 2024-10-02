using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Xunit;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace StudentAssignment.Tests.StudentTest.RepositoriesTest
{
    public class StudentRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentRepository _studentRepo;

        public StudentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DBConnection")
                .Options;
            _context = new ApplicationDbContext(options);
            _studentRepo = new StudentRepository(_context);
        }

        [Test]
        public async Task AddStudentAsync_ShouldAddStudentToDatabase()
        {
            // Arrange
            var student = new Student { Name = "New Student" };

            // Act
            await _studentRepo.AddAsync(student);
            await _context.SaveChangesAsync();

            // Assert
            var createdStudent = await _context.Students.FirstOrDefaultAsync(s => s.Name == "New Student");
            NUnit.Framework.Assert.That(createdStudent, Is.Not.Null); // Updated assertion
        }

        [Test]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            // Arrange
            _context.Students.Add(new Student { Name = "Student 1" });
            _context.Students.Add(new Student { Name = "Student 2" });
            await _context.SaveChangesAsync();

            // Act
            var students = await _studentRepo.GetAllAsync(1, 10);

            // Assert
            NUnit.Framework.Assert.That(students.Count(), Is.EqualTo(2));
        }
    }
}

