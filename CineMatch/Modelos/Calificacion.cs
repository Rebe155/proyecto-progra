namespace CineMatch.Modelos;

public sealed class Calificacion
{
    public int Id { get; }
    public int IdUsuario { get; }
    public int IdPelicula { get; }
    public int Valor { get; private set; }
    public DateTime Fecha { get; private set; }

    public Calificacion(int id, int idUsuario, int idPelicula, int valor)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        Id = id;
        IdUsuario = idUsuario;
        IdPelicula = idPelicula;
        Actualizar(valor);
    }

    public void Actualizar(int valor)
    {
        if (valor is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(valor), "La calificación debe estar entre 1 y 5.");

        Valor = valor;
        Fecha = DateTime.Now;
    }
}
