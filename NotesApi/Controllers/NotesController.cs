using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace NotesApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("notes")]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var mojeNotatki = await _context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new { id = n.Id, content = n.Content })
                .ToListAsync();
            return Ok(mojeNotatki);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Note note)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            note.UserId = userId;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return Created($"/notes/{note.Id}", new { id = note.Id, content = note.Content });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Note note)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var dbNote = await _context.Notes.FindAsync(id);

            if (dbNote == null) return NotFound();
            if (dbNote.UserId != userId) return Forbid();

            dbNote.Content = note.Content;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var dbNote = await _context.Notes.FindAsync(id);

            if (dbNote == null) return NotFound();
            if (dbNote.UserId != userId) return Forbid();

            _context.Notes.Remove(dbNote);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}