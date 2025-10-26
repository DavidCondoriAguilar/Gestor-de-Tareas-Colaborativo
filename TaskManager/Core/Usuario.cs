namespace TaskManager.Core
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Tarea> TareasAsignadas { get; set; } = new List<Tarea>();
    }
}
