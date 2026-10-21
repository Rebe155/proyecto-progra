namespace CineMatch.Estructuras;

// Grafo bipartito ponderado, representado mediante listas de adyacencia.
public sealed class GrafoValoraciones
{
    public sealed class Arista
    {
        public int IdUsuario { get; }
        public int IdPelicula { get; }
        public int Valor { get; private set; }

        public Arista(int idUsuario, int idPelicula, int valor)
        {
            IdUsuario = idUsuario;
            IdPelicula = idPelicula;
            Valor = valor;
        }

        public void Actualizar(int valor) => Valor = valor;
    }

    private readonly TablaHash<ListaEnlazada<Arista>> porUsuario = new();
    private readonly TablaHash<ListaEnlazada<Arista>> porPelicula = new();

    public void Registrar(int idUsuario, int idPelicula, int valor)
    {
        if (valor is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(valor));

        var calificacionesUsuario = ObtenerOAgregar(porUsuario, idUsuario);
        var existente = calificacionesUsuario.Buscar(a => a.IdPelicula == idPelicula);
        if (existente is not null)
        {
            existente.Actualizar(valor);
            return;
        }

        var nueva = new Arista(idUsuario, idPelicula, valor);
        calificacionesUsuario.Agregar(nueva);
        ObtenerOAgregar(porPelicula, idPelicula).Agregar(nueva);
    }

    private static ListaEnlazada<Arista> ObtenerOAgregar(
        TablaHash<ListaEnlazada<Arista>> tabla, int id)
    {
        if (tabla.IntentarObtener(id, out var lista)) return lista;
        lista = new ListaEnlazada<Arista>();
        tabla.Agregar(id, lista);
        return lista;
    }

    public IEnumerable<Arista> CalificacionesDeUsuario(int idUsuario)
    {
        if (porUsuario.IntentarObtener(idUsuario, out var lista))
            foreach (var arista in lista) yield return arista;
    }

    public IEnumerable<Arista> CalificacionesDePelicula(int idPelicula)
    {
        if (porPelicula.IntentarObtener(idPelicula, out var lista))
            foreach (var arista in lista) yield return arista;
    }

    public IEnumerable<int> UsuariosRelacionados(int idUsuario)
    {
        var encontrados = new TablaHash<bool>();
        foreach (var calificacion in CalificacionesDeUsuario(idUsuario))
        {
            foreach (var otra in CalificacionesDePelicula(calificacion.IdPelicula))
            {
                if (otra.IdUsuario == idUsuario || encontrados.IntentarObtener(otra.IdUsuario, out _))
                    continue;

                encontrados.Agregar(otra.IdUsuario, true);
                yield return otra.IdUsuario;
            }
        }
    }
}
