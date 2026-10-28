namespace Exercise1.Models
{
    public class StudentGradeReportRow
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public decimal? Grade { get; set; }
    }
}
