using CineMatch.Estructuras;
using CineMatch.Modelos;

namespace CineMatch.Servicios;

public sealed class MotorRecomendacion
{
    private sealed class Acumulado
    {
        public int IdPelicula { get; }
        public double SumaPonderada { get; private set; }
        public double SumaSimilitudes { get; private set; }

        public Acumulado(int idPelicula) => IdPelicula = idPelicula;

        public void Agregar(double similitud, int calificacion)
        {
            SumaPonderada += similitud * calificacion;
            SumaSimilitudes += similitud;
        }
    }

    private readonly TablaHash<Usuario> usuarios = new();
    private readonly TablaHash<Pelicula> peliculas = new();
    private readonly ArbolAvl indicePeliculas = new();
    private readonly GrafoValoraciones grafo = new();
    private int siguienteIdCalificacion = 1;

    public void RegistrarUsuario(Usuario usuario) => usuarios.Agregar(usuario.Id, usuario);

    public void RegistrarPelicula(Pelicula pelicula)
    {
        peliculas.Agregar(pelicula.Id, pelicula);
        indicePeliculas.Insertar(pelicula);
    }

    public bool BuscarUsuario(int id, out Usuario usuario) => usuarios.IntentarObtener(id, out usuario!);
    public bool BuscarPelicula(int id, out Pelicula pelicula) => peliculas.IntentarObtener(id, out pelicula!);
    public IEnumerable<Pelicula> CatalogoOrdenado() => indicePeliculas.RecorrerOrdenado();

    public void Calificar(int idUsuario, int idPelicula, int valor)
    {
        if (!BuscarUsuario(idUsuario, out var usuario))
            throw new KeyNotFoundException("No existe el usuario indicado.");
        if (!BuscarPelicula(idPelicula, out _))
            throw new KeyNotFoundException("No existe la película indicada.");
        if (valor is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(valor), "La calificación debe estar entre 1 y 5.");

        var anterior = usuario.BuscarCalificacion(idPelicula);
        if (anterior is null)
            usuario.Calificaciones.Agregar(new Calificacion(siguienteIdCalificacion++, idUsuario, idPelicula, valor));
        else
            anterior.Actualizar(valor);

        grafo.Registrar(idUsuario, idPelicula, valor);
    }

    public double CalcularSimilitud(int idUsuarioA, int idUsuarioB)
    {
        if (!BuscarUsuario(idUsuarioA, out var usuarioA) || !BuscarUsuario(idUsuarioB, out var usuarioB))
            throw new KeyNotFoundException("No existe uno de los usuarios.");

        double productos = 0, cuadradosA = 0, cuadradosB = 0;
        var coincidencias = 0;

        foreach (var a in usuarioA.Calificaciones)
        {
            var b = usuarioB.BuscarCalificacion(a.IdPelicula);
            if (b is null) continue;

            productos += a.Valor * b.Valor;
            cuadradosA += a.Valor * a.Valor;
            cuadradosB += b.Valor * b.Valor;
            coincidencias++;
        }

        if (coincidencias == 0) return 0;
        return productos / (Math.Sqrt(cuadradosA) * Math.Sqrt(cuadradosB));
    }

    public ListaEnlazada<Recomendacion> GenerarRecomendaciones(int idUsuario, int cantidad, int vecinos = 3)
    {
        if (!BuscarUsuario(idUsuario, out var usuario))
            throw new KeyNotFoundException("No existe el usuario indicado.");
        if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));
        if (vecinos <= 0) throw new ArgumentOutOfRangeException(nameof(vecinos));

        var similares = new ColaPrioridad<int>();
        foreach (var candidato in grafo.UsuariosRelacionados(idUsuario))
        {
            var similitud = CalcularSimilitud(idUsuario, candidato);
            if (similitud > 0) similares.Encolar(candidato, similitud);
        }

        if (similares.EstaVacia) return RecomendacionesRespaldo(usuario, cantidad);

        var acumulados = new TablaHash<Acumulado>();
        var procesados = 0;
        while (!similares.EstaVacia && procesados < vecinos)
        {
            var candidato = similares.Desencolar();
            var similitud = CalcularSimilitud(idUsuario, candidato);

            foreach (var arista in grafo.CalificacionesDeUsuario(candidato))
            {
                if (arista.Valor < 4 || usuario.BuscarCalificacion(arista.IdPelicula) is not null)
                    continue;

                if (!acumulados.IntentarObtener(arista.IdPelicula, out var acumulado))
                {
                    acumulado = new Acumulado(arista.IdPelicula);
                    acumulados.Agregar(arista.IdPelicula, acumulado);
                }
                acumulado.Agregar(similitud, arista.Valor);
            }
            procesados++;
        }

        var ordenadas = new ColaPrioridad<Recomendacion>();
        foreach (var acumulado in acumulados)
        {
            if (acumulado.SumaSimilitudes == 0) continue;
            peliculas.IntentarObtener(acumulado.IdPelicula, out var pelicula);
            var puntuacion = acumulado.SumaPonderada / acumulado.SumaSimilitudes;
            ordenadas.Encolar(new Recomendacion(pelicula, puntuacion, "Usuarios con valoraciones similares"), puntuacion);
        }

        if (ordenadas.EstaVacia) return RecomendacionesRespaldo(usuario, cantidad);

        var resultado = new ListaEnlazada<Recomendacion>();
        while (!ordenadas.EstaVacia && resultado.Cantidad < cantidad)
            resultado.Agregar(ordenadas.Desencolar());
        return resultado;
    }

    private ListaEnlazada<Recomendacion> RecomendacionesRespaldo(Usuario usuario, int cantidad)
    {
        var ordenadas = new ColaPrioridad<Recomendacion>();
        foreach (var pelicula in CatalogoOrdenado())
        {
            if (usuario.BuscarCalificacion(pelicula.Id) is not null) continue;

            double suma = 0;
            var total = 0;
            foreach (var calificacion in grafo.CalificacionesDePelicula(pelicula.Id))
            {
                suma += calificacion.Valor;
                total++;
            }
            var promedio = total == 0 ? 0 : suma / total;
            var recomendacion = new Recomendacion(pelicula, promedio, "Promedio del catálogo (respaldo)");
            ordenadas.Encolar(recomendacion, promedio);
        }

        var resultado = new ListaEnlazada<Recomendacion>();
        while (!ordenadas.EstaVacia && resultado.Cantidad < cantidad)
            resultado.Agregar(ordenadas.Desencolar());
        return resultado;
    }
}
