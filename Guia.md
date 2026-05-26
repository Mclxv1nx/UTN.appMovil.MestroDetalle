# Guía: Crear una App .NET MAUI con SQLite y Dapper (CRUD completo)

> Basada en la estructura del proyecto `MauiAppUTN2026003`, con buenas prácticas aplicadas desde el inicio.

---

## Antes de empezar: lo que vas a construir

Una aplicación multiplataforma (Android, iOS, Windows) con una sola pantalla que permite **Crear, Leer, Actualizar y Eliminar** registros de una entidad a tu elección, almacenados en una base de datos SQLite local.

---

## Paso 1 — Crear el proyecto

1. Abrir **Visual Studio 2022** (o superior).
2. Crear un nuevo proyecto → seleccionar **".NET MAUI App"**.
3. Dar un nombre descriptivo al proyecto, por ejemplo: `MauiAppProductos`.
4. Seleccionar **.NET 10** (o la versión disponible).
5. Hacer clic en **Crear**.

Al terminar, Visual Studio genera la estructura base con `App.xaml`, `AppShell.xaml`, `MainPage.xaml` y `MauiProgram.cs`. No modificar nada todavía.

---

## Paso 2 — Agregar los paquetes NuGet

Agregar los siguientes paquetes mediante el **Administrador de paquetes NuGet** o la consola:

| Paquete                 | Para qué sirve                             |
| ----------------------- | ------------------------------------------ |
| `Microsoft.Data.Sqlite` | Driver oficial de SQLite para .NET         |
| `Dapper`                | Micro-ORM que simplifica las consultas SQL |

> **Nota:** Verificar que la versión de `Microsoft.Data.Sqlite` sea compatible con el `.TargetFramework` del proyecto.

---

## Paso 3 — Definir el modelo (la entidad)

Crear un archivo `[NombreEntidad].cs` en la raíz del proyecto.

Este archivo representa **una fila de la tabla** en la base de datos. Debe contener:

- Una propiedad `Id` de tipo `int` (clave primaria).
- Las demás propiedades que describan la entidad, con sus tipos de dato correspondientes (`string`, `int`, `decimal`, etc.).

**Buena práctica:** Inicializar las propiedades de tipo `string` con `string.Empty` para evitar advertencias de nullable que genera el compilador en .NET moderno.

---

## Paso 4 — Crear la clase repositorio (acceso a datos)

Crear un archivo `[NombreEntidadPlural].cs` en la raíz del proyecto. Esta clase es responsable de **toda la comunicación con la base de datos**. No debe haber ninguna lógica de base de datos en otro lugar.

### 4.1 — Constructor

El constructor debe:

1. Calcular la ruta del archivo `.db` usando `FileSystem.AppDataDirectory`. Esto garantiza que funcione en todas las plataformas (Android, iOS, Windows) sin rutas hardcodeadas.
2. Construir el `connectionString` con esa ruta.
3. Crear y abrir la conexión (`SqliteConnection`).
4. Ejecutar el `CREATE TABLE IF NOT EXISTS` con todas las columnas de la entidad.

**Buena práctica sobre el `Id`:** Declarar la columna `Id` como `INTEGER PRIMARY KEY AUTOINCREMENT` en la tabla para que la base de datos asigne el ID automáticamente. Así se evitan errores de duplicación por IDs manuales ingresados por el usuario.

**Buena práctica sobre la conexión:** Implementar `IDisposable` en la clase para poder cerrar la conexión correctamente cuando ya no se necesite, evitando fugas de recursos.

### 4.2 — Método Create

- Recibe como parámetros todos los campos de la entidad **excepto el Id** (porque se autoincrementa).
- Construye un objeto de tipo `[NombreEntidad]` con esos valores.
- Ejecuta un `INSERT INTO` con Dapper usando parámetros nombrados (`@Nombre`, `@Precio`, etc.).
- Verifica que `recordsAffected > 0`. Si no, lanzar una excepción descriptiva.
- Retorna el objeto creado.

### 4.3 — Método ReadById

- Recibe un `int id`.
- Ejecuta un `SELECT * FROM ... WHERE Id = @Id` con Dapper.
- Retorna el primer objeto encontrado, o `null` si no existe.

### 4.4 — Método ReadAll

- Sin parámetros.
- Ejecuta un `SELECT * FROM ...` con Dapper.
- Retorna una `List<[NombreEntidad]>`.

### 4.5 — Método Update

- Recibe `id` y todos los campos modificables.
- Ejecuta un `UPDATE ... SET ... WHERE Id = @Id`.
- Verifica que `recordsAffected > 0`. Si no, lanzar excepción (significa que el Id no existe).

### 4.6 — Método Delete

- Recibe un `int id`.
- Ejecuta un `DELETE FROM ... WHERE Id = @Id`.
- Verifica que `recordsAffected > 0`. Si no, lanzar excepción.

---

## Paso 5 — Registrar el repositorio en el contenedor de dependencias

En `MauiProgram.cs`, antes de `return builder.Build()`, registrar la clase repositorio como servicio singleton:

```
builder.Services.AddSingleton<[NombreEntidadPlural]>();
```

Esto evita instanciar el repositorio manualmente con `new` en la página y permite que MAUI lo inyecte automáticamente.

---

## Paso 6 — Diseñar la interfaz en MainPage.xaml

La UI debe contener:

- Un `Label` de bienvenida o título.
- Un `Entry` por cada campo de la entidad (excepto `Id` si es autoincremental, que puede mostrarse como solo lectura o estar oculto).
- Cuatro botones: **Crear**, **Leer**, **Actualizar** y **Borrar**.

Envolver todo en un `ScrollView > VerticalStackLayout` para que funcione correctamente en pantallas pequeñas.

Asignar nombres (`x:Name`) a cada `Entry` y manejar el evento `Clicked` de cada botón.

---

## Paso 7 — Programar la lógica en MainPage.xaml.cs

### 7.1 — Constructor

Recibir el repositorio como parámetro en el constructor (inyección de dependencias). No usar `new`:

```
public MainPage([NombreEntidadPlural] repositorio)
{
    InitializeComponent();
    _repositorio = repositorio;
}
```

Declarar `_repositorio` como campo privado de la clase.

### 7.2 — Validación de entradas (obligatorio)

Antes de llamar a cualquier método del repositorio, validar que los campos no estén vacíos y que los campos numéricos contengan un número válido. Usar `int.TryParse()` en lugar de `int.Parse()` para evitar excepciones no controladas que crashean la app.

Ejemplo de patrón:

```
if (!int.TryParse(txtCampoNumerico.Text, out int valor))
{
    await DisplayAlert("Error", "El campo debe ser un número válido.", "OK");
    return;
}
```

### 7.3 — Manejadores de eventos (async/await)

**Todos los métodos de evento deben ser `async void`** porque `DisplayAlert` es asíncrono y debe ser awaiteado. Sin `await`, la alerta no se muestra.

**Botón Crear:**

1. Validar entradas.
2. Llamar a `_repositorio.Create(...)`.
3. Limpiar los campos de la UI.
4. Mostrar confirmación con `await DisplayAlert(...)`.

**Botón Leer:**

1. Validar que el campo Id no esté vacío.
2. Llamar a `_repositorio.ReadById(id)`.
3. Si el resultado no es `null`, rellenar los campos de la UI con los datos.
4. Si es `null`, mostrar un mensaje de "no encontrado".

**Botón Actualizar:**

1. Validar entradas.
2. Llamar a `_repositorio.Update(...)` dentro de un `try/catch`.
3. Mostrar confirmación o el mensaje de error si lanzó excepción.

**Botón Borrar:**

1. Validar que el campo Id no esté vacío.
2. Guardar el valor del Id en una variable local **antes** de limpiar la UI.
3. Llamar a `_repositorio.Delete(id)` dentro de un `try/catch`.
4. Limpiar todos los campos de la UI.
5. Mostrar confirmación usando la variable guardada (no el campo de texto, que ya está vacío).

---

## Paso 8 — Verificación final

Antes de correr la app, repasar esta lista:

- [ ] La clase modelo tiene todas las propiedades con `string.Empty` como valor por defecto.
- [ ] La tabla usa `AUTOINCREMENT` en el Id.
- [ ] El repositorio implementa `IDisposable`.
- [ ] El repositorio está registrado en `MauiProgram.cs` con `AddSingleton`.
- [ ] `MainPage` recibe el repositorio por inyección de dependencias.
- [ ] Todos los event handlers son `async void`.
- [ ] Todos los `DisplayAlert` tienen `await`.
- [ ] Las entradas numéricas usan `int.TryParse` con validación.
- [ ] Al borrar, el ID se guarda en variable antes de limpiar los campos.

---

## Resumen de archivos a crear o modificar

| Archivo              | Acción    | Descripción                                 |
| -------------------- | --------- | ------------------------------------------- |
| `[Entidad].cs`       | Crear     | Modelo / clase POCO de la entidad           |
| `[EntidadPlural].cs` | Crear     | Repositorio con los métodos CRUD            |
| `MauiProgram.cs`     | Modificar | Registrar el repositorio en el DI container |
| `MainPage.xaml`      | Modificar | Agregar Entries y Buttons                   |
| `MainPage.xaml.cs`   | Modificar | Lógica de los botones con async/await       |

Los archivos `App.xaml`, `AppShell.xaml` y la carpeta `Platforms/` **no requieren modificaciones** para este ejercicio.
