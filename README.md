# Unidad 1 - Estructuras de datos

**Alumno:** Axel Giovani Gonzalez Lopez · **Matrícula:** I24050379  
**Docente:** Ruben Riojas · **Lenguaje:** C# · **Plataforma:** .NET 10

## Versión con ventanas y solución de Visual Studio

Abrir **Unidad1.sln** en Visual Studio con soporte para desarrollo de escritorio
en .NET 10. Elegir **Arreglos.Visual** o **Heap.Visual** como proyecto de inicio y
presionar F5. Cada formulario incluye su archivo `MainForm.Designer.cs`.

```powershell
dotnet build Unidad1.sln -c Release -m:1
dotnet run --project Arreglos.Visual -c Release
dotnet run --project Heap.Visual -c Release
```

- **Arreglos.Visual:** permite ingresar longitud, rango y semilla para generar
  valores. Se puede redimensionar el arreglo, editar un índice, agregar o quitar
  elementos de List y medir cuatro estrategias con el tamaño elegido.
- **Heap.Visual:** permite ingresar el nombre de referencia, titular y saldo.
  Se pueden crear instancias, crear alias, depositar, retirar, comparar identidad
  y soltar referencias. La tabla refleja el estado real de los objetos.
- **Laboratorio.Core:** contiene la lógica y las validaciones; no depende de
  Windows Forms. Los formularios invocan esa lógica mediante sus eventos.
- **Verificaciones:** contiene 30 comprobaciones automatizadas del dominio.
- **PruebasInterfaz:** comprueba los eventos de los controles y renderiza los
  formularios para revisar su distribución; se ejecuta en Windows.

Los tamaños iniciales y los límites de entrada son valores predeterminados
editables o validaciones, no resultados simulados. Las ventanas inician sin
cuentas ni arreglos creados. Las pruebas y los ejemplos de consola sí usan
valores conocidos para comprobar resultados esperados. El benchmark usa la
misma secuencia de 0 a n-1 en las cuatro estrategias para comparar el mismo
trabajo; n lo elige el usuario y los tiempos y bytes se miden en cada ejecución.

Los ID de Heap identifican objetos de esta sesión, no direcciones físicas.
Los datos permanecen en memoria mientras la ventana está abierta; no se usa
base de datos. Esta es una práctica educativa, no una aplicación bancaria.

Para reproducir las pruebas adicionales:

```powershell
dotnet run --project Verificaciones -c Release
dotnet run --project PruebasInterfaz -c Release -- evidencias
```

Las imágenes `Arreglos_formulario.png` y `Heap_formulario.png` se generaron
directamente con `DrawToBitmap` a partir de los formularios ejecutados durante
las pruebas. Son renders de los controles reales, no capturas del escritorio.
La captura del escritorio no pudo verificarse: la herramienta de captura de
ventanas agotó el tiempo de espera en dos intentos.

## Requisitos y ejecución

Instalar el SDK .NET 10. Desde esta carpeta:

```powershell
dotnet build Arreglos -c Release
dotnet build Heap -c Release
dotnet run --project Arreglos -c Release --no-build -- 5000
dotnet run --project Heap -c Release --no-build
```

`Arreglos` admite un entero de 1 a 20000 (predeterminado: 5000).
Una entrada inválida produce código de salida 2. Las comprobaciones fallidas
lanzan una excepción y finalizan con código distinto de cero.

## Qué demuestra cada programa

- **Arreglos:** longitud fija, crecimiento y reducción con `Array.Resize`, alias
  al arreglo anterior, tamaño cero, rechazo de tamaño negativo, crecimiento de
  `List<int>` y mediciones de cuatro estrategias de llenado.
- **Heap:** creación de instancias, alias, identidad frente a igualdad de datos,
  validación de operaciones mediante encapsulamiento y eliminación de referencias.

En C#, `int[]` y `List<int>` son objetos administrados: un arreglo fijo no es
sinónimo de almacenamiento estático. `Array.Resize` reemplaza la referencia al
crear un arreglo de otra longitud. El recolector determina cuándo libera objetos
inalcanzables; asignar `null` no acredita una liberación inmediata.

## Verificación reproducible

```powershell
1000,5000,10000 | ForEach-Object {
    dotnet run --project Arreglos -c Release --no-build -- $_
}
dotnet run --project Heap -c Release --no-build
dotnet run --project Arreglos -c Release --no-build -- -1
# Este último comando debe terminar con código 2.
```

Se ejecutaron 7 comprobaciones funcionales por tamaño, 6 de referencias y
4 entradas inválidas: 31 verificaciones en total. Los TXT de `evidencias/`
contienen las salidas reales. Las imágenes *_registro.png son reproducciones tipográficas de las salidas;
no son capturas de pantalla. Los HTML permiten consultar los registros en un
navegador y no vuelven a ejecutar C#. Las capturas originales quedan pendientes
porque la herramienta de captura se detuvo al no poder verificar la URL.

## Método de medición

Compilación Release; tres calentamientos por estrategia y siete muestras.
Se reportan medianas independientes de tiempo y bytes asignados por el hilo
actual. La creación y el llenado están incluidos; no se imprime dentro de la
región medida. Se consume una suma para observar los resultados.
No es un benchmark estadístico exhaustivo: el JIT, GC, orden de ejecución y
actividad del equipo pueden cambiar los tiempos. Los bytes acumulados no son
memoria residente ni el pico de memoria. No se fuerza `GC.Collect`.

Para n=5000, la suma esperada es n(n-1)/2 = 12497500. El redimensionamiento
exacto copia 0+1+...+(n-1) elementos: costo cuadrático. Preasignar la capacidad
o permitir crecimiento amortizado evita copiar en cada inserción.

## Repositorio en GitHub

El código fuente de la Unidad 1 se publica en:

https://github.com/Giovani4l/unidad-1-estructuras-de-datos

## Referencias

Microsoft. (s. f.). *Array.Resize<T>(T[], Int32) Method*. Microsoft Learn.
https://learn.microsoft.com/en-us/dotnet/api/system.array.resize?view=net-10.0

Microsoft. (s. f.). *List<T>.Add(T) Method*. Microsoft Learn.
https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.add?view=net-10.0

Microsoft. (s. f.). *Fundamentals of garbage collection*. Microsoft Learn.
https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals
