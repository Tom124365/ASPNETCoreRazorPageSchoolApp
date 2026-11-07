using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exercise1.Models;

namespace Exercise1.Pages.EnrollmentPages;

public class DeleteModel : PageModel
{
    private readonly SchoolContext _context;

    public DeleteModel(SchoolContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Enrollment Enrollment { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(m => m.EnrollmentId == id);
        if (enrollment is null)
        {
            return NotFound();
        }
        else
        {
            Enrollment = enrollment;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            Enrollment = enrollment;
            _context.Enrollments.Remove(Enrollment);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
