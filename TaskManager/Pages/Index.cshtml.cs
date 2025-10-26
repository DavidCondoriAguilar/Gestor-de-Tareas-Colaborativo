using TaskManager.Core;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Tarea> Tareas { get; set; } = new List<Tarea>();

        [BindProperty]
        public Tarea NuevaTarea { get; set; } = new();

        // Propiedad para el dropdown de usuarios
        public List<Usuario> UsuariosDisponibles { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Carga las tareas y los usuarios
            Tareas = await _context.Tareas.Include(t => t.UsuarioAsignado).ToListAsync();
            UsuariosDisponibles = await _context.Usuarios.ToListAsync();
        }

        // Este método maneja el POST del formulario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Si falla, recarga los datos necesarios para la página
                Tareas = await _context.Tareas.Include(t => t.UsuarioAsignado).ToListAsync();
                UsuariosDisponibles = await _context.Usuarios.ToListAsync();
                return Page(); // Devuelve la página con los errores de validación
            }

            // Npgsql exige UTC para 'timestamp with time zone'. Si viene Unspecified desde el <input type="date">, forzamos UTC
            if (NuevaTarea.FechaVencimiento.Kind == DateTimeKind.Unspecified)
            {
                NuevaTarea.FechaVencimiento = DateTime.SpecifyKind(NuevaTarea.FechaVencimiento, DateTimeKind.Utc);
            }
            else
            {
                NuevaTarea.FechaVencimiento = NuevaTarea.FechaVencimiento.ToUniversalTime();
            }

            // Si es válido, guarda la nueva tarea
            _context.Tareas.Add(NuevaTarea);
            await _context.SaveChangesAsync();

            // Marcar creación exitosa para mostrar modal en el siguiente GET
            TempData["TaskCreated"] = true;

            return RedirectToPage(); // Redirige a la misma página (OnGet)
        }

        // Handler para actualizar estado de tarea vía AJAX
        public async Task<IActionResult> OnPostUpdateEstadoAsync(int tareaId, EstadoTarea nuevoEstado)
        {
            var tarea = await _context.Tareas.FindAsync(tareaId);
            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            // Devolvemos solo la tarea actualizada como JSON
            return new JsonResult(new { message = "Estado actualizado" }); 
        }

        // Handler para eliminar una tarea
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
