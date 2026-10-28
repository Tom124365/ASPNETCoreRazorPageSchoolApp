using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exercise1.Models;

namespace Exercise1.Pages.InstructorPages;

public class DetailsModel : PageModel
{
    private readonly SchoolContext _context;
    public DetailsModel(SchoolContext context)
    {
        _context = context;
    }

    public Instructor Instructor { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? instructorid)
    {
        if (instructorid is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.InstructorId == instructorid);
        if (instructor is null)
        {
            return NotFound();
        }
        else
        {
            Instructor = instructor;
        }

        return Page();
    }
}
