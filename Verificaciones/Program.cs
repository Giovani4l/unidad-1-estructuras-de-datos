using Laboratorio.Core;

// Verificación independiente de la lógica utilizada por las ventanas.
int aprobadas = 0;
void Check(bool condicion, string caso)
{
    if (!condicion) throw new Exception("FALLO: " + caso);
    Console.WriteLine("PASS " + caso); aprobadas++;
}
void Rechaza(Action operacion, string caso)
{
    try { operacion(); }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    { Check(true, caso); return; }
    throw new Exception("No se rechazó: " + caso);
}
var a = new ArreglosLab();
a.Generar(4, 7, 7, 42);
Check(a.Actual.SequenceEqual(new[] { 7, 7, 7, 7 }), "rango elegido genera valores reales");
Check(a.MismaInstancia, "alias inicial comparte identidad");
a.Cambiar(1, 91);
Check(a.Alias[1] == 91 && a.Vector[1] == 7, "cambio se refleja en alias, no en copia List");
a.Redimensionar(6);
Check(!a.MismaInstancia && a.Alias.Count == 4, "Resize cambia instancia y conserva alias anterior");
Check(a.Actual.SequenceEqual(new[] { 7, 91, 7, 7, 0, 0 }), "crecimiento conserva prefijo e inicializa nuevos elementos");
a.Cambiar(0, -31);
Check(a.Alias[0] == 7, "arreglo nuevo independiente del anterior");
a.Redimensionar(2);
Check(a.Actual.SequenceEqual(new[] { -31, 91 }), "reducción conserva prefijo correcto");
a.Agregar(888);
Check(a.Vector[^1] == 888 && a.Vector.Count == 5, "List agrega valor ingresado");
a.QuitarUltimo();
Check(a.Vector.Count == 4 && a.Capacidad >= a.Vector.Count, "List elimina último y respeta capacidad");
a.Redimensionar(0); Check(a.Actual.Count == 0, "tamaño cero permitido");
Rechaza(() => a.Cambiar(0, 2), "índice fuera del arreglo");
Rechaza(() => a.Redimensionar(-1), "longitud negativa");
Rechaza(() => a.Generar(3, 10, 1, 0), "rango invertido");
a.Generar(0, 0, 1, 0); Rechaza(a.QuitarUltimo, "eliminar de vector vacío");
a.Generar(20000, 0, 1, 0); Rechaza(() => a.Agregar(1), "límite de vector");
var h = new HeapLab();
h.Crear("miCuenta", "Titular de prueba", 73.25m);
h.CrearAlias("segunda", "miCuenta");
Check(h.MismaInstancia("miCuenta", "segunda") && h.InstanciasAlcanzables == 1, "alias comparte instancia real");
h.Obtener("segunda").Depositar(6.75m);
Check(h.Obtener("miCuenta").Saldo == 80m, "depósito por alias actualiza referencia original");
h.Obtener("miCuenta").Retirar(30m);
Check(h.Obtener("segunda").Saldo == 50m, "retiro respeta saldo compartido");
Rechaza(() => h.Obtener("segunda").Depositar(-1), "depósito negativo");
Rechaza(() => h.Obtener("segunda").Retirar(51), "retiro sin fondos");
h.Crear("tercera", "Titular de prueba", 50m);
Check(!h.MismaInstancia("tercera", "segunda") && h.InstanciasAlcanzables == 2, "datos iguales no implican identidad");
h.Soltar("miCuenta");
Check(h.Consultar("miCuenta") is null && h.Obtener("segunda").Saldo == 50m, "soltar conserva el objeto accesible por alias");
h.Soltar("segunda"); Check(h.InstanciasAlcanzables == 1, "última referencia deja de contar la instancia");
Rechaza(() => h.Crear("", "Titular", 0), "nombre de referencia vacío");
Rechaza(() => h.Crear("cuarta", "", 0), "titular vacío");
Rechaza(() => h.Crear("cuarta", "Titular", -1), "saldo inicial negativo");
Rechaza(() => h.CrearAlias("cuarta", "miCuenta"), "alias desde referencia null");
var medidas = Mediciones.Ejecutar(137);
Check(medidas.Count == 4 && medidas.All(x => x.Suma == 137L * 136 / 2), "cuatro estrategias calculan suma de entrada arbitraria");
Check(medidas.All(x => x.Bytes > 0 && x.Milisegundos >= 0), "mediciones reales con asignaciones");
Rechaza(() => Mediciones.Ejecutar(0), "tamaño inválido para medición");
Console.WriteLine($"RESULTADO: {aprobadas} verificaciones aprobadas");
