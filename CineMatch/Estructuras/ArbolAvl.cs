using CineMatch.Modelos;

namespace CineMatch.Estructuras;

public sealed class ArbolAvl
{
    private sealed class Nodo
    {
        public Pelicula Pelicula { get; }
        public Nodo? Izquierdo { get; set; }
        public Nodo? Derecho { get; set; }
        public int Altura { get; set; } = 1;

        public Nodo(Pelicula pelicula) => Pelicula = pelicula;
    }

    private Nodo? raiz;
    public int Altura => MedirAltura(raiz);

    public void Insertar(Pelicula pelicula) => raiz = Insertar(raiz, pelicula);

    private static Nodo Insertar(Nodo? nodo, Pelicula pelicula)
    {
        if (nodo is null) return new Nodo(pelicula);

        var comparacion = Comparar(pelicula, nodo.Pelicula);
        if (comparacion < 0)
            nodo.Izquierdo = Insertar(nodo.Izquierdo, pelicula);
        else if (comparacion > 0)
            nodo.Derecho = Insertar(nodo.Derecho, pelicula);
        else
            throw new InvalidOperationException("La película ya existe en el índice.");

        ActualizarAltura(nodo);
        var balance = Balance(nodo);

        if (balance > 1)
        {
            if (Comparar(pelicula, nodo.Izquierdo!.Pelicula) > 0)
                nodo.Izquierdo = RotarIzquierda(nodo.Izquierdo);
            return RotarDerecha(nodo);
        }

        if (balance < -1)
        {
            if (Comparar(pelicula, nodo.Derecho!.Pelicula) < 0)
                nodo.Derecho = RotarDerecha(nodo.Derecho);
            return RotarIzquierda(nodo);
        }

        return nodo;
    }

    private static int Comparar(Pelicula a, Pelicula b)
    {
        var orden = string.Compare(a.Titulo, b.Titulo, StringComparison.OrdinalIgnoreCase);
        return orden != 0 ? orden : a.Id.CompareTo(b.Id);
    }

    private static int MedirAltura(Nodo? nodo) => nodo?.Altura ?? 0;
    private static int Balance(Nodo nodo) => MedirAltura(nodo.Izquierdo) - MedirAltura(nodo.Derecho);

    private static void ActualizarAltura(Nodo nodo) =>
        nodo.Altura = 1 + Math.Max(MedirAltura(nodo.Izquierdo), MedirAltura(nodo.Derecho));

    private static Nodo RotarDerecha(Nodo nodo)
    {
        var nuevoPadre = nodo.Izquierdo!;
        nodo.Izquierdo = nuevoPadre.Derecho;
        nuevoPadre.Derecho = nodo;
        ActualizarAltura(nodo);
        ActualizarAltura(nuevoPadre);
        return nuevoPadre;
    }

    private static Nodo RotarIzquierda(Nodo nodo)
    {
        var nuevoPadre = nodo.Derecho!;
        nodo.Derecho = nuevoPadre.Izquierdo;
        nuevoPadre.Izquierdo = nodo;
        ActualizarAltura(nodo);
        ActualizarAltura(nuevoPadre);
        return nuevoPadre;
    }

    public IEnumerable<Pelicula> RecorrerOrdenado() => Recorrer(raiz);

    private static IEnumerable<Pelicula> Recorrer(Nodo? nodo)
    {
        if (nodo is null) yield break;
        foreach (var pelicula in Recorrer(nodo.Izquierdo)) yield return pelicula;
        yield return nodo.Pelicula;
        foreach (var pelicula in Recorrer(nodo.Derecho)) yield return pelicula;
    }
}
