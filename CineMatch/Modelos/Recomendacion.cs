namespace CineMatch.Modelos;

public sealed class Recomendacion
{
    public Pelicula Pelicula { get; }
    public double Puntuacion { get; }
    public string Motivo { get; }

    public Recomendacion(Pelicula pelicula, double puntuacion, string motivo)
    {
        Pelicula = pelicula;
        Puntuacion = puntuacion;
        Motivo = motivo;
    }
}
