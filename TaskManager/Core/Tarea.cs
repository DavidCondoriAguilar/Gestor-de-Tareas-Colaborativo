using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; } // Opcional

        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        public EstadoTarea Estado { get; set; }

        // Relación con Usuario
        public int? UsuarioAsignadoId { get; set; }
        public Usuario? UsuarioAsignado { get; set; }
    }
}
