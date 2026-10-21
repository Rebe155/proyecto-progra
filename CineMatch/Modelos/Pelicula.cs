namespace CineMatch.Modelos;

public sealed class Pelicula
{
    public int Id { get; }
    public string Titulo { get; }
    public string Genero { get; }
    public int Anio { get; }

    public Pelicula(int id, string titulo, string genero, int anio)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("El título es obligatorio.", nameof(titulo));

        Id = id;
        Titulo = titulo.Trim();
        Genero = genero.Trim();
        Anio = anio;
    }
}
