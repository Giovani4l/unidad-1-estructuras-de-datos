using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

// Programa 1: tamaño fijo, redimensionamiento y costo de distintas estrategias.
internal static class Program
{
    private static long checksum;
    private static void Check(bool ok, string name)
    {
        if (!ok) throw new InvalidOperationException("FALLO: " + name);
        Console.WriteLine("PASS " + name);
    }

    private static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (args.Length > 1 || (args.Length == 1 &&
            (!int.TryParse(args[0], out int size) || size < 1 || size > 20000)))
        {
            Console.Error.WriteLine("Uso: Arreglos [n entre 1 y 20000]");
            Environment.ExitCode = 2;
            return;
        }
        int n = args.Length == 0 ? 5000 : int.Parse(args[0]);
        Console.WriteLine("TAREA 1.2 | ARREGLOS Y VECTOR DINAMICO");
        Console.WriteLine($"Runtime: {RuntimeInformation.FrameworkDescription}; SO: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"Arquitectura: {RuntimeInformation.ProcessArchitecture}; n={n}");
        // El arreglo tiene longitud fija; su objeto tambien reside en el heap.
        int[] fijo = new int[3];
        Check(fijo.Length == 3, "C1 longitud fija = 3");
        int[] a = { 10, 20, 30 };
        int[] alias = a;
        Array.Resize(ref a, 5); // Crea otro arreglo y copia el prefijo.
        Check(a.SequenceEqual(new[] { 10, 20, 30, 0, 0 }), "C2 crecer: 10,20,30,0,0");
        Check(!ReferenceEquals(a, alias) && alias.Length == 3, "C3 alias conserva arreglo original");
        Array.Resize(ref a, 2);
        Check(a.SequenceEqual(new[] { 10, 20 }), "C4 reducir: 10,20");
        Array.Resize(ref a, 0);
        Check(a.Length == 0, "C5 longitud cero valida");
        try { Array.Resize(ref a, -1); throw new Exception("Se acepto -1"); }
        catch (ArgumentOutOfRangeException) { Console.WriteLine("PASS C6 negativo rechazado"); }
        var vector = new List<int>();
        int cambios = 0, anterior = vector.Capacity;
        for (int i = 0; i < n; i++)
        {
            vector.Add(i);
            if (vector.Capacity != anterior) { cambios++; anterior = vector.Capacity; }
        }
        Check(vector.Count == n && vector[0] == 0 && vector[^1] == n - 1,
            "C7 List conserva extremos y cantidad");
        Console.WriteLine($"List: Count={vector.Count}; Capacity={vector.Capacity}; cambios={cambios}");
        Console.WriteLine("BENCHMARK: mediana de 7 muestras, tras 3 calentamientos");
        Console.WriteLine("metodo;ms;bytes_asignados_hilo");
        Measure("arreglo_una_asignacion", () => Fixed(n));
        Measure("resize_por_elemento", () => Exact(n));
        Measure("list_crecimiento", () => ListFill(n, false));
        Measure("list_capacidad_inicial", () => ListFill(n, true));
        Console.WriteLine($"Checksum de control={checksum}");
        Console.WriteLine("RESULTADO: 7 casos aprobados");
    }
    private static long Fixed(int n)
    {
        var a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        long sum = 0; foreach (int x in a) sum += x;
        return sum;
    }
    private static long Exact(int n)
    {
        int[] a = Array.Empty<int>();
        for (int i = 0; i < n; i++) { Array.Resize(ref a, i + 1); a[i] = i; }
        long sum = 0; foreach (int x in a) sum += x;
        return sum;
    }
    private static long ListFill(int n, bool reserve)
    {
        var a = reserve ? new List<int>(n) : new List<int>();
        for (int i = 0; i < n; i++) a.Add(i);
        long sum = 0; foreach (int x in a) sum += x;
        return sum;
    }
    private static void Measure(string name, Func<long> operation)
    {
        for (int k = 0; k < 3; k++) checksum = operation();
        var times = new double[7]; var bytes = new long[7];
        var timer = new Stopwatch(); // Instrumentacion fuera del tramo medido.
        for (int k = 0; k < 7; k++)
        {
            long before = GC.GetAllocatedBytesForCurrentThread();
            timer.Restart(); checksum = operation(); timer.Stop();
            bytes[k] = GC.GetAllocatedBytesForCurrentThread() - before;
            times[k] = timer.Elapsed.TotalMilliseconds;
        }
        Array.Sort(times); Array.Sort(bytes);
        Console.WriteLine($"{name};{times[3]:F4};{bytes[3]}");
    }
}
