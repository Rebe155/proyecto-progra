using System.Collections;

namespace CineMatch.Estructuras;

public sealed class ListaEnlazada<T> : IEnumerable<T>
{
    private sealed class Nodo
    {
        public T Dato { get; }
        public Nodo? Siguiente { get; set; }

        public Nodo(T dato) => Dato = dato;
    }

    private Nodo? cabeza;
    private Nodo? cola;
    public int Cantidad { get; private set; }

    public void Agregar(T dato)
    {
        var nuevo = new Nodo(dato);
        if (cabeza is null)
            cabeza = nuevo;
        else
            cola!.Siguiente = nuevo;

        cola = nuevo;
        Cantidad++;
    }

    public T? Buscar(Predicate<T> criterio)
    {
        for (var actual = cabeza; actual is not null; actual = actual.Siguiente)
        {
            if (criterio(actual.Dato)) return actual.Dato;
        }
        return default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var actual = cabeza; actual is not null; actual = actual.Siguiente)
            yield return actual.Dato;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
