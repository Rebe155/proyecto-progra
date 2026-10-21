using CineMatch.Estructuras;

namespace CineMatch.Modelos;

public sealed class Usuario
{
    public int Id { get; }
    public string Nombre { get; }
    public ListaEnlazada<Calificacion> Calificaciones { get; } = new();

    public Usuario(int id, string nombre)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        Id = id;
        Nombre = nombre.Trim();
    }

    public Calificacion? BuscarCalificacion(int idPelicula) =>
        Calificaciones.Buscar(c => c.IdPelicula == idPelicula);
}
