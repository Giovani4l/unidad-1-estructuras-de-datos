namespace Laboratorio.Core;

/// <summary>Estado del experimento. Los valores y tamaños provienen del usuario.</summary>
public sealed class ArreglosLab
{
    public const int Limite = 20000;
    private int[] actual = Array.Empty<int>();
    private int[] alias = Array.Empty<int>();
    private readonly List<int> vector = new();
    public IReadOnlyList<int> Actual => Array.AsReadOnly(actual);
    public IReadOnlyList<int> Alias => Array.AsReadOnly(alias);
    public IReadOnlyList<int> Vector => vector.AsReadOnly();
    public int Capacidad => vector.Capacity;
    public bool MismaInstancia => ReferenceEquals(actual, alias);

    public void Generar(int cantidad, int minimo, int maximo, int semilla)
    {
        ValidarLongitud(cantidad);
        if (minimo > maximo || minimo < -1000000 || maximo > 1000000)
            throw new ArgumentException("El rango debe estar entre -1000000 y 1000000, con mínimo <= máximo.");
        var random = new Random(semilla);
        actual = new int[cantidad];
        for (int i = 0; i < cantidad; i++) actual[i] = random.Next(minimo, maximo + 1);
        alias = actual; // Alias real, sin copiar el arreglo.
        vector.Clear(); vector.AddRange(actual);
    }

    public void Redimensionar(int longitud)
    {
        ValidarLongitud(longitud);
        alias = actual;
        Array.Resize(ref actual, longitud);
    }

    public void Cambiar(int indice, int valor)
    {
        if (indice < 0 || indice >= actual.Length)
            throw new ArgumentOutOfRangeException(nameof(indice), "El índice está fuera del arreglo actual.");
        actual[indice] = valor;
    }
    public void Agregar(int valor)
    {
        if (vector.Count >= Limite) throw new InvalidOperationException("Se alcanzó el límite didáctico de 20000 elementos.");
        vector.Add(valor);
    }
    public void QuitarUltimo()
    {
        if (vector.Count == 0) throw new InvalidOperationException("El vector está vacío.");
        vector.RemoveAt(vector.Count - 1);
    }
    private static void ValidarLongitud(int n)
    {
        if (n < 0 || n > Limite) throw new ArgumentOutOfRangeException(nameof(n), "La longitud debe estar entre 0 y 20000.");
    }
}
