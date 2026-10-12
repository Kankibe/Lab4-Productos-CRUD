# Universidad Tecnológica de Panamá

# Facultad de Ingeniería de Sistemas Computacionales


## Objetivos

- Aplicar el manejo de operaciones CRUD (Crear, Leer, Actualizar, Borrar) sobre una base de datos desde una aplicación de escritorio.
- Consolidar el uso de Windows Forms para la construcción de interfaces de usuario en C#.
- Implementar la conexión entre la aplicación y la base de datos (MySQL Workbench) mediante clases de acceso a datos.
- Validar la entrada de datos del usuario antes de enviarlos a la base de datos, utilizando interfaces para lograr un código reutilizable y extensible.
- Manejar imágenes en la aplicación, convirtiéndolas a bytes para almacenarlas en la base de datos y reconstruyéndolas para mostrarlas, mediante una clase de apoyo.

## Introducción

Este laboratorio consiste en el desarrollo de una aplicación de escritorio en C# con Windows Forms para la gestión de productos, aplicando las cuatro operaciones básicas de persistencia de datos: crear, leer, actualizar y borrar (CRUD).

La aplicación permite registrar productos con su nombre, precio, cantidad e imagen, visualizarlos en una tabla, buscarlos, seleccionarlos para editarlos y eliminarlos de la base de datos. Toda la comunicación con la base de datos se maneja a través de una clase de conexión dedicada, separando la lógica de acceso a datos de la lógica del formulario.

En esta versión se incorporan dos mejoras: la validación de los campos mediante una interfaz (`IValidadorCampo`) con una clase de validación por cada tipo de dato, y una clase estática (`ImagenHelper`) que centraliza la conversión de imágenes a bytes y de bytes a imágenes.

## Tecnologías Utilizadas

- C# / .NET
- Windows Forms
- MySQL Workbench
- Visual Studio 2026

### Tabla de Estructura de Datos

| Campo | Tipo | Nulo | Clave | Descripción |
| :--- | :--- | :--- | :--- | :--- |
| `id` | `INT` | NO | PRI (`AUTO_INCREMENT`) | Identificador único del producto |
| `nombre` | `VARCHAR(100)` | NO | | Nombre o descripción del producto |
| `precio` | `DECIMAL(10,2)` | NO | | Precio unitario |
| `cantidad` | `INT` | NO | | Unidades en inventario |
| `imagen` | `LONGBLOB` | SÍ | | Archivo de imagen almacenado en formato binario |

---

<img width="431" height="302" alt="image" src="https://github.com/user-attachments/assets/9e06ccbf-dc8f-4157-9c4b-b859da6a4b1f" />


## Arquitectura del Proyecto

El proyecto separa responsabilidades en distintos archivos:

**Modelo (Producto.cs):** representa la entidad Producto, con sus propiedades (id, nombre, precio, cantidad, imagen).

**Acceso a Datos (Conexion.cs):** contiene los métodos encargados de comunicarse con la base de datos: consultar productos, insertar, actualizar y borrar registros mediante consultas parametrizadas.

**Validaciones (Validadores.cs):** contiene la interfaz `IValidadorCampo` y las clases que la implementan (`ValidadorTexto`, `ValidadorDecimal` y `ValidadorEntero`). Cada clase define su propia regla y su mensaje de error.

**Imágenes (ImagenHelper.cs):** clase estática con los métodos `ImageToByteArray()`, que convierte una imagen en `byte[]` antes de guardarla en MySQL, y `ByteArrayToImage()`, que convierte los bytes de MySQL de vuelta a `Image` (con opción de generar una miniatura).

**Formulario (Form1.cs / Form1.Designer.cs):** contiene la interfaz gráfica y la lógica de interacción con el usuario: carga de productos en la tabla, validación de campos, búsqueda, selección de un producto para editar, y los eventos de los botones de guardar, modificar y borrar. Para las imágenes llama directamente a `ImagenHelper`.

## Validaciones Implementadas

Las validaciones se construyen con una interfaz que actúa como contrato:

```csharp
public interface IValidadorCampo
{
    bool EsValido(string valor);
    string MensajeError { get; }
}
```

Cada tipo de dato tiene su propia clase que cumple ese contrato:

- **ValidadorTexto:** el nombre del producto no puede estar vacío.
- **ValidadorDecimal:** el precio debe ser un número decimal válido y no negativo.
- **ValidadorEntero:** la cantidad debe ser un número entero válido y no negativo.

En `Form1.cs` los campos se agrupan junto con su validador en una lista, y se recorren con un único `foreach`, sin necesidad de condicionales `if/else` repetidos:

```csharp
camposAValidar = new List<(TextBox txt, IValidadorCampo validador)>
{
    (txtNombre, new ValidadorTexto()),
    (txtPrecio, new ValidadorDecimal()),
    (txtCantidad, new ValidadorEntero())
};
```

Antes de guardar o modificar un producto, se evalúan todos los campos. Si alguno falla, se muestra un icono de error junto al campo (componente `ErrorProvider`) con su mensaje, y se detiene el proceso de guardado. Cuando el campo se corrige, el icono desaparece.

Ventajas de este diseño:

- **Polimorfismo:** el formulario trata a todos los validadores como un `IValidadorCampo`, sin importar el tipo de dato.
- **Mantenimiento:** si cambia una regla (por ejemplo, un precio máximo), solo se modifica la clase correspondiente.
- **Extensibilidad:** para validar un campo nuevo (correo, teléfono, fecha) basta con crear otra clase que implemente la interfaz.
- **Reutilización:** las mismas clases se pueden usar en otros formularios.

## Manejo de Imágenes

La clase estática `ImagenHelper` centraliza el trabajo con imágenes:

- `ImageToByteArray(Image)`: convierte la imagen seleccionada en un arreglo de bytes (formato PNG) para guardarla en la columna `LONGBLOB`.
- `ByteArrayToImage(byte[], ancho, alto)`: reconstruye la imagen a partir de los bytes de la base de datos. Si se indican ancho y alto, devuelve una miniatura.

En la tabla (`DataGridView`) las imágenes se muestran como miniaturas uniformes de 80 x 60 píxeles con el modo de diseño `Zoom`, para que no se vean cortadas ni demasiado grandes. Al seleccionar un producto, el `PictureBox` muestra la imagen original y no la miniatura.

**Imagenes**

interfaz principal

<img width="750" height="673" alt="image" src="https://github.com/user-attachments/assets/3ba3618a-3e6e-48b1-bf19-d73dfacde175" />

## Guía de Instalación y Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   ```
2. **Configurar la Base de Datos:**
   - Abrir **MySQL Workbench**.
   - Ejecutar el archivo sql.
3. **Configurar la Aplicación:**
   - Abrir el proyecto en **Visual Studio**.
   - Revisar la cadena de conexión en la clase `Conexion.cs` (usuario, contraseña, servidor y puerto).
4. **Ejecutar:**
   - Presionar `F5` o el botón **Iniciar** en Visual Studio.

---

## Información del Estudiante

Este laboratorio ha sido desarrollado por la estudiante de la Universidad Tecnológica de Panamá:

Nombre: Kankibe González

Curso: Herramientas de la Programación Aplicada III

Instructora: Irina Fong

Fecha: 11 de octubre de 2026
