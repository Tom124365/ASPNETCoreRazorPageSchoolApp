using Exercise1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;

namespace Exercise1.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public List<SelectListItem> Courses { get; set; } = new();

        private string ConnectionString =>
            _configuration.GetConnectionString("SchoolDBConnectionString")
            ?? throw new InvalidOperationException(
                "Check the connection string name in appsettings.json.");

        public async Task OnGetAsync()
        {
            Courses.Clear();

            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT CourseID, Title FROM Courses ORDER BY Title",
                connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Courses.Add(new SelectListItem
                {
                    Value = reader["CourseID"].ToString(),
                    Text = reader["Title"].ToString()
                });
            }
        }

        public async Task<IActionResult> OnGetPdfAsync(int courseId)
        {
            if (courseId <= 0)
                return BadRequest("Please select a course.");

            var rows = new List<StudentGradeReportRow>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sql = @"
                    SELECT s.StudentID, s.FirstName, s.LastName,
                           c.Title AS CourseTitle, e.Grade
                    FROM Students s
                    INNER JOIN Enrollments e
                        ON s.StudentID = e.StudentID
                    INNER JOIN Courses c
                        ON e.CourseID = c.CourseID
                    WHERE c.CourseID = @CourseID
                    ORDER BY s.LastName, s.FirstName";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CourseID", courseId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    rows.Add(new StudentGradeReportRow
                    {
                        StudentID = Convert.ToInt32(reader["StudentID"]),
                        FirstName = reader["FirstName"].ToString() ?? "",
                        LastName = reader["LastName"].ToString() ?? "",
                        CourseTitle = reader["CourseTitle"].ToString() ?? "",
                        Grade = reader["Grade"] == DBNull.Value
                            ? null
                            : Convert.ToDecimal(reader["Grade"])
                    });
                }
            }

            string reportPath = Path.Combine(
                _environment.ContentRootPath,
                "Reports",
                "StudentGrades.rdlc");

            if (!System.IO.File.Exists(reportPath))
                return NotFound("StudentGrades.rdlc was not found.");

            using var stream = System.IO.File.OpenRead(reportPath);
            var report = new LocalReport();
            report.LoadReportDefinition(stream);
            report.DataSources.Add(
                new ReportDataSource("StudentGrades", rows));

            byte[] pdf = report.Render("PDF");

            if (pdf.Length == 0)
            {
                throw new Exception("The generated PDF is empty.");
            }

            return File(
                pdf,
                "application/pdf",
                $"StudentGrades_{courseId}.pdf");
        }
    }
}
