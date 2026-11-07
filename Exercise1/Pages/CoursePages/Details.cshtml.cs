using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exercise1.Models;

namespace Exercise1.Pages.CoursePages;

public class DetailsModel : PageModel
{
    private readonly SchoolContext _context;
    public DetailsModel(SchoolContext context)
    {
        _context = context;
    }

    public Course Course { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FirstOrDefaultAsync(m => m.CourseId == id);
        if (course is null)
        {
            return NotFound();
        }
        else
        {
            Course = course;
        }

        return Page();
    }
}
