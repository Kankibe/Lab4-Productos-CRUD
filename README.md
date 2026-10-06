# Universidad Tecnológica de Panamá

# Facultad de Ingeniería de Sistemas Computacionales


## Objetivos

- Aplicar el manejo de operaciones CRUD (Crear, Leer, Actualizar, Borrar) sobre una base de datos desde una aplicación de escritorio.
- Consolidar el uso de Windows Forms para la construcción de interfaces de usuario en C#.
- Implementar la conexión entre la aplicación y la base de datos (MySQL Workbench) mediante clases de acceso a datos.
- Validar la entrada de datos del usuario antes de enviarlos a la base de datos.

## Introducción

Este laboratorio consiste en el desarrollo de una aplicación de escritorio en C# con Windows Forms para la gestión de productos, aplicando las cuatro operaciones básicas de persistencia de datos: crear, leer, actualizar y borrar (CRUD).

La aplicación permite registrar productos con su nombre, precio, cantidad e imagen, visualizarlos en una tabla, buscarlos por nombre, seleccionarlos para editarlos y eliminarlos de la base de datos. Toda la comunicación con la base de datos se maneja a través de una clase de conexión dedicada, separando la lógica de acceso a datos de la lógica del formulario.

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

**Acceso a Datos (Conexion.cs):** contiene los métodos encargados de comunicarse con la base de datos: consultar productos, insertar, actualizar y borrar registros de forma segura.

**Formulario (Form1.cs / Form1.Designer.cs):** contiene la interfaz gráfica y la lógica de interacción con el usuario: carga de productos en la tabla, validación de campos, búsqueda, selección de un producto para editar, y los eventos de los botones de guardar, modificar y borrar.

## Validaciones Implementadas

Antes de guardar o modificar un producto, la aplicación valida que:

- El nombre del producto no esté vacío.
- El precio no esté vacío y sea un valor numérico válido.
- La cantidad no esté vacía y sea un valor numérico entero válido.

Si alguna validación falla, se muestra un mensaje al usuario y se detiene el proceso de guardado.

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
   - Abrir el proyecto en **Visual Studio **.
   - Revisar la cadena de conexión en la clase `Conexion.cs` (usuario, contraseña, servidor y puerto).
4. **Ejecutar:**
   - Presionar `F5` o el botón **Iniciar** en Visual Studio.

---

## Información del Estudiante

Este laboratorio ha sido desarrollado por la estudiante de la Universidad Tecnológica de Panamá:

Nombre: Kankibe González

Curso: Herramientas de la Programación Aplicada III

Instructora: Irina Fong
