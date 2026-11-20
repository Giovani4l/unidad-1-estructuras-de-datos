# Abrir los dos programas

1. Extraer el ZIP completo. No ejecutar desde dentro del archivo comprimido.
2. Abrir la solución `.sln` con Visual Studio y el SDK .NET 10.
3. Seleccionar el proyecto terminado en `.Visual` como proyecto de inicio.
4. Presionar F5. El diseño está en `MainForm.Designer.cs` y los eventos en
   `MainForm.cs`. La lógica está en `Laboratorio.Core`.

También se entrega una carpeta `Ejecutar` con los binarios Release. Abrir el
`.exe` de cada programa. Deben conservarse todos los archivos de esa carpeta.
Requieren Windows y el runtime de escritorio .NET 10; no son ejecutables
autocontenidos. Los archivos `.exe` se compilaron desde el código incluido.

## Programa 1: Arreglos

- Elegir longitud, mínimo, máximo y semilla; pulsar **Generar datos**.
- Escribir una nueva longitud y pulsar **Redimensionar**. El alias conserva el
  arreglo anterior, mientras List sigue siendo una colección independiente.
- Elegir índice y valor para **Cambiar arreglo**.
- Usar ese valor con **Agregar a List** o eliminar con **Quitar último de List**.
- Elegir el número de elementos para medir y pulsar **Comparar estrategias**.

La tabla muestra como máximo los primeros 250 elementos, pero las operaciones
se realizan sobre la colección completa. La longitud admite 0 a 20000 y la
medición 1 a 10000 para mantener manejable el experimento cuadrático.
Cada medición usa tres calentamientos y la mediana de siete muestras. Los
bytes son asignaciones acumuladas del hilo, no memoria residente.

## Programa 2: Heap

1. Escribir una referencia, por ejemplo `principal`, un titular y un saldo.
2. Pulsar **Crear / reemplazar**. Si el nombre ya existe, se cambia el objeto
   al que apunta esa referencia.
3. Seleccionar la referencia activa.
4. Escribir otro nombre en el campo Referencia y pulsar **Crear alias**.
5. Depositar o retirar usando cualquiera de las referencias. Las dos filas
   reflejarán el cambio si comparten el objeto.
6. Usar **Comparar identidad** para comprobarlo con `ReferenceEquals`.
7. **Soltar referencia** asigna null a la seleccionada; las demás conservan
   su objeto. No se fuerza la recolección de basura.

Los importes se escriben mediante el control numérico con el formato regional
del equipo. Los depósitos no positivos, retiros sin fondos y nombres vacíos
se rechazan. El estado no se conserva al cerrar el programa.

## Verificación

La lógica pasó 30 comprobaciones automatizadas. Las pruebas dentro del proceso
comprobaron acciones reales de los botones y la distribución horizontal de los
controles: 48 comprobaciones aprobadas. Los registros están en `evidencias`.
Las imágenes de formularios son renders directos de WinForms, identificados
como tales; las capturas originales del escritorio siguen pendientes.
