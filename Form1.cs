using ProyectoProductos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging; // Necesario para imágenes
using System.IO;              // Necesario para MemoryStream
using System.Windows.Forms;

namespace Lab_4_creo
{
    public partial class Form1 : Form
    {
        // Declaración de variables globales del formulario (Página 23 y 26)
        private int idProducto = 0;
        private List<Producto> listaProductos;
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();

        // Constructor del formulario
        public Form1()
        {
            InitializeComponent();
            listaProductos = new List<Producto>();
        }

        // Evento que carga los datos al abrir la ventana
        private void Form1_Load(object sender, EventArgs e)
        {
            cargarProductos();
        }

        private void cargarProductos(string filtro = "")
        {
            dgvProductos.Rows.Clear();
            dgvProductos.Refresh();

            listaProductos = Conexion.GetProductos(filtro);

            foreach (var prod in listaProductos)
            {
                Image img = null;

                if (prod.Imagen != null && prod.Imagen.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(prod.Imagen))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            img = new Bitmap(bmp); // Clona la imagen para liberar el MemoryStream
                        }
                    }
                }

                // Agrega los valores a las filas del DataGridView
                dgvProductos.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
            }
        } // fin de cargarProductos



        private void label1_Click(object sender, EventArgs e)
        {
            //no
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return;
            }

            ModificarDatosBD();
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!datosCorrectos())
            {
                return; // No hace nada si la validación falla; se detiene el flujo
            }

            CargarDatosProductos();

            if (Conexion.InsertSeguro("productos", myProducto))
            {
                MessageBox.Show("Se ha guardado satisfactoriamente el registro");
                // Refresca el DataGridView volviendo a consultar la base de datos
                cargarProductos();
                limpiarCampos();
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                return null;

            using (MemoryStream mMemoryStream = new MemoryStream())
            {
                image.Save(mMemoryStream, ImageFormat.Png);
                return mMemoryStream.ToArray();
            }
        }

        private void CargarDatosProductos()
        {
            myProducto["cantidad"] = int.Parse(txtCantidad.Text.Trim());
            myProducto["precio"] = decimal.Parse(txtPrecio.Text.Trim());
            myProducto["nombre"] = txtNombre.Text.Trim();

            if (pictureBox1.Image != null)
            {
                // Convierte una imagen en un array de bytes 1 y ceros
                myProducto["imagen"] = ImageToByteArray(pictureBox1.Image);
            }
            else
            {
                myProducto["imagen"] = null;
            }
        } // fin CargarDatosProductos

        private bool datosCorrectos()
        {
            if (txtNombre.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese el Nombre del Producto");
                return false;
            }
            if (txtPrecio.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese el Precio");
                return false;
            }
            if (txtCantidad.Text.Trim().Equals(""))
            {
                MessageBox.Show("Ingrese la Cantidad");
                return false;
            }
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio))
            {
                MessageBox.Show("Ingrese un Precio correcto");
                return false;
            }
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad))
            {
                MessageBox.Show("Ingrese una cantidad correcta");
                return false;
            }
            return true;
        }

        private void limpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            pictureBox1.Image = null;
            idProducto = 0;
            btnGuardar.Enabled = true;
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // evita error si se hace click en el encabezado

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

            idProducto = Convert.ToInt32(fila.Cells["Id"].Value);
            txtId.Text = idProducto.ToString();
            txtNombre.Text = Convert.ToString(fila.Cells["Producto"].Value);
            txtPrecio.Text = Convert.ToDecimal(fila.Cells["Precio"].Value).ToString();
            txtCantidad.Text = Convert.ToInt32(fila.Cells["Cantidad"].Value).ToString();

            btnGuardar.Enabled = false;
            btnModificar.Enabled = true;
        } // fin dgvProductos_CellClick

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
            btnModificar.Enabled = false;
        } // fin btnLimpiar_Click

        private void ModificarDatosBD()
        {
            CargarDatosProductos();
            //Asegurar que mi arreglo tiene los datos
            //MessageBox.Show("el Nombre del Producto es: " + myProducto["Nombre"]);
            // Actualiza el producto donde el id_producto sea igual a 5

            MessageBox.Show("el id del producto es: " + idProducto);

            bool resultado = Conexion.UpdateSeguro("productos", myProducto, "id", idProducto);

            if (resultado)
            {
                Console.WriteLine("Actualización exitosa.");
            }
        } // fin ModificarDatosBD

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (idProducto == 0)
            {
                MessageBox.Show("Selecciona un producto del grid antes de borrar");
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Seguro que deseas borrar este producto?",
                "Confirmar",
                MessageBoxButtons.YesNo);

            if (confirmacion == DialogResult.Yes)
            {
                if (Conexion.DeleteSeguro("productos", "id", idProducto))
                {
                    MessageBox.Show("Producto borrado correctamente");
                    cargarProductos();
                    limpiarCampos();
                }
                else
                {
                    MessageBox.Show("No se pudo borrar el producto");
                }
            }
        } // fin btnBorrar_Click

    }
}
