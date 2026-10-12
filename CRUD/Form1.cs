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
        // Declaración de variables globales del formulario
        private List<(TextBox txt, IValidadorCampo validador)> camposAValidar;
        private int idProducto = 0;
        private bool cargando = false;
        private List<Producto> listaProductos;
        private Dictionary<string, object> myProducto = new Dictionary<string, object>();

        // Constructor del formulario
        public Form1()
        {
            InitializeComponent();
            camposAValidar = new List<(TextBox txt, IValidadorCampo validador)>
            {
                (txtNombre, new ValidadorTexto()),
                (txtPrecio, new ValidadorDecimal()),
                (txtCantidad, new ValidadorEntero())
            };

            this.Load += Form1_Load;
            listaProductos = new List<Producto>();

            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;

            foreach (DataGridViewColumn col in dgvProductos.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            txtId.ReadOnly = true;
            btnModificar.Enabled = false;
        }

        // Evento que carga los datos al abrir la ventana
        private void Form1_Load(object sender, EventArgs e)
        {
            cargarProductos();
        }

        private void cargarProductos(string filtro = "")
        {
            cargando = true;
            try
            {
                dgvProductos.Rows.Clear();
                listaProductos = Conexion.GetProductos(filtro);

                foreach (var prod in listaProductos)
                {
                    System.Drawing.Image img = ImagenHelper.ByteArrayToImage(prod.Imagen, 80, 60);
                    dgvProductos.Rows.Add(prod.Id, prod.Nombre, prod.Precio, prod.Cantidad, img);
                }

                dgvProductos.ClearSelection();
            }
            finally
            {
                cargando = false;
            }
        } // fin de cargarProductos

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (cargando) return;
            if (dgvProductos.SelectedRows.Count == 0) return;

            DataGridViewRow fila = dgvProductos.SelectedRows[0];
            if (fila.Cells["Id"].Value == null) return;

            idProducto = Convert.ToInt32(fila.Cells["Id"].Value);
            txtId.Text = idProducto.ToString();
            txtNombre.Text = Convert.ToString(fila.Cells["Producto"].Value);
            txtPrecio.Text = Convert.ToDecimal(fila.Cells["Precio"].Value).ToString();
            txtCantidad.Text = Convert.ToInt32(fila.Cells["Cantidad"].Value).ToString();

            // Se toma la imagen original de la lista (no la miniatura del grid)
            Producto prod = listaProductos.Find(p => p.Id == idProducto);
            PictureBox.Image = prod != null ? ImagenHelper.ByteArrayToImage(prod.Imagen) : null;
            PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            btnGuardar.Enabled = false;
            btnModificar.Enabled = true;
        } // fin dgvProductos_SelectionChanged

        private void label1_Click(object sender, EventArgs e)
        {
            //no
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (idProducto == 0)
            {
                MessageBox.Show("Selecciona un producto del grid antes de modificar");
                return;
            }

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

        // Clic en la foto del producto: abre el selector de imagen
        private void PictureBox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Seleccionar imagen del producto";
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    PictureBox.Image = Image.FromFile(openFileDialog.FileName);
                    PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
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

            if (PictureBox.Image != null)
            {
                // Convierte una imagen en un array de bytes
                myProducto["imagen"] = ImagenHelper.ImageToByteArray(PictureBox.Image);
            }
            else
            {
                myProducto["imagen"] = null;
            }
        } // fin CargarDatosProductos

        private bool datosCorrectos()
        {
            bool todoOk = true;

            foreach (var item in camposAValidar)
            {
                if (!item.validador.EsValido(item.txt.Text.Trim()))
                {
                    errorProvider1.SetError(item.txt, item.validador.MensajeError);
                    todoOk = false;
                }
                else
                {
                    errorProvider1.SetError(item.txt, string.Empty);
                }
            }

            return todoOk;
        }

        private void limpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtCantidad.Clear();
            PictureBox.Image = null;
            idProducto = 0;
            btnGuardar.Enabled = true;
            btnModificar.Enabled = false;
            dgvProductos.ClearSelection();
            errorProvider1.Clear();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
        } // fin btnLimpiar_Click

        private void ModificarDatosBD()
        {
            CargarDatosProductos();

            bool resultado = Conexion.UpdateSeguro("productos", myProducto, "id", idProducto);

            if (resultado)
            {
                MessageBox.Show("Producto actualizado correctamente");
                cargarProductos();
                limpiarCampos();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar el producto");
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

        // Clic en la lupa del panel de búsqueda: recarga el grid
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            cargarProductos(txtBusqueda.Text.Trim());
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}