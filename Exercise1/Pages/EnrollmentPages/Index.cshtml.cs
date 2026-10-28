using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exercise1.Models;

namespace Exercise1.Pages.EnrollmentPages;

public class IndexModel : PageModel
{
    private readonly SchoolContext _context;

    public IndexModel(SchoolContext context)
    {
        _context = context;
    }

    public IList<Enrollment> Enrollment { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Enrollment = await _context.Enrollments.ToListAsync();
    }
}
