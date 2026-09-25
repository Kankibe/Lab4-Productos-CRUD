using Lab_4_creo;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProyectoProductos
{
    public class Conexion
    {
        private static string cadenaConexion = "Server=localhost; Database=productosdb;Uid=root;Pwd= -----";

        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                // Crear un tipo de dato de MySQLConnection
                MySqlConnection conexion = new MySqlConnection(cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
                return null;
            } // fin del catch
        } // fin del método estático ObtenerConexion

        public static List<Producto> GetProductos(string filtro)
        {
            List<Producto> listaProductos = new List<Producto>();
            string query = "SELECT id, nombre, precio, cantidad, imagen FROM productos";

            if (!string.IsNullOrEmpty(filtro))
            {
                query += " WHERE id LIKE @filtro OR nombre LIKE @filtro " +
                         " OR precio LIKE @filtro OR cantidad LIKE @filtro";
            }

            using (MySqlConnection conn = ObtenerConexion())
            {
                if (conn == null) return listaProductos;

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }

                    using (MySqlDataReader mReader = cmd.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            Producto prod = new Producto();

                            prod.Id = Convert.ToInt32(mReader["id"]);
                            prod.Nombre = mReader["nombre"].ToString();
                            prod.Precio = Convert.ToDecimal(mReader["precio"]);
                            prod.Cantidad = Convert.ToInt32(mReader["cantidad"]);
                            prod.Imagen = mReader["imagen"] != DBNull.Value ? (byte[])mReader["imagen"] : null;

                            listaProductos.Add(prod);
                        }
                        mReader.Close();
                    }
                }
            }

            return listaProductos;
        } // fin del método GetProductos

        // Método para insertar registros usando consultas parametrizadas dinámicas (Página 37-38)
        public static bool InsertSeguro(string tbName, Dictionary<string, object> data)
        {
            var columns = string.Join(", ", data.Keys);
            var placeholders = "@" + string.Join(", @", data.Keys);
            string sql = $"INSERT INTO {tbName} ({columns}) VALUES ({placeholders})";

            try
            {
                // Pedimos la conexión usando nuestro método de esta misma clase
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en INSERT: " + ex.Message);
                return false;
            }
        } // fin del método InsertSeguro

        public static bool UpdateSeguro(string tbName, Dictionary<string, object> data, string idColumn, int idValue)
        {
            // Construimos la cláusula SET: columna1 = @columna1, columna2 = @columna2 ...
            var setParts = new List<string>();
            foreach (var key in data.Keys)
            {
                setParts.Add($"{key} = @{key}");
            }

            string setClause = string.Join(", ", setParts);

            // Armamos la consulta SQL completa incluyendo el WHERE
            string sql = $"UPDATE {tbName} SET {setClause} WHERE {idColumn} = @idCondicion";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        // Agregamos los parámetros de los campos a actualizar
                        foreach (var kvp in data)
                        {
                            stmt.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }

                        // Agregamos el parámetro para la condición WHERE de forma segura
                        stmt.Parameters.AddWithValue("@idCondicion", idValue);

                        stmt.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error en UPDATE: " + ex.Message);
                return false;
            }
        } // fin del método UpdateSeguro

        public static bool DeleteSeguro(string tbName, string idColumn, int idValue)
        {
            string sql = $"DELETE FROM {tbName} WHERE {idColumn} = @idCondicion";

            try
            {
                using (MySqlConnection conexion = ObtenerConexion())
                {
                    if (conexion == null) return false;

                    using (MySqlCommand stmt = new MySqlCommand(sql, conexion))
                    {
                        stmt.Parameters.AddWithValue("@idCondicion", idValue);
                        int filasAfectadas = stmt.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en DELETE: " + ex.Message);
                return false;
            }
        } // fin del método DeleteSeguro
    }
}
