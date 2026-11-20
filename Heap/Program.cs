// Programa 2: instancias y referencias compartidas en el heap administrado.
internal sealed class Cuenta
{
    public string Titular { get; }
    public decimal Saldo { get; private set; }
    public Cuenta(string titular, decimal saldo)
    {
        if (string.IsNullOrWhiteSpace(titular) || saldo < 0)
            throw new ArgumentException("Datos de cuenta invalidos");
        Titular = titular; Saldo = saldo;
    }
    public void Depositar(decimal cantidad)
    {
        if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));
        Saldo += cantidad;
    }
}
internal static class Program
{
    private static void Check(bool ok, string name)
    {
        if (!ok) throw new InvalidOperationException("FALLO: " + name);
        Console.WriteLine("PASS " + name);
    }
    private static void Main()
    {
        Console.WriteLine("TAREA 1.2 | OBJETOS EN HEAP Y ENCAPSULAMIENTO");
        Cuenta? principal = new Cuenta("Axel", 100m);
        Cuenta? alias = principal; // Copia la referencia; no clona el objeto.
        Check(ReferenceEquals(principal, alias), "H1 ambas referencias apuntan al mismo objeto");
        alias.Depositar(50m);
        Check(principal.Saldo == 150m, "H2 cambio por alias visible: saldo=150");
        var distinta = new Cuenta("Axel", 150m);
        Check(!ReferenceEquals(principal, distinta), "H3 mismos datos, instancia independiente");
        principal = null;
        Check(alias.Saldo == 150m, "H4 quitar una referencia no destruye el objeto");
        try { alias.Depositar(-5m); throw new Exception("Deposito negativo aceptado"); }
        catch (ArgumentOutOfRangeException) { Console.WriteLine("PASS H5 encapsulamiento rechaza deposito negativo"); }
        var cuentas = new List<Cuenta>();
        for (int i = 0; i < 3; i++) cuentas.Add(new Cuenta("Cuenta " + i, i * 10m));
        Check(cuentas.Sum(c => c.Saldo) == 30m, "H6 coleccion de 3 instancias; suma=30");
        cuentas.Clear();
        alias = null;
        Console.WriteLine("Se eliminaron referencias; el GC decide cuando recuperar memoria.");
        Console.WriteLine("No se fuerza GC.Collect ni se infiere una direccion fisica.");
        Console.WriteLine("RESULTADO: 6 casos aprobados");
    }
}
