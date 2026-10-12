using System;

namespace ProyectoProductos
{
    // 1. Contrato que cumplen todos los validadores
    public interface IValidadorCampo
    {
        bool EsValido(string valor);
        string MensajeError { get; }
    }

    // 2. Validador para texto (ej. Nombre del producto)
    public class ValidadorTexto : IValidadorCampo
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                MensajeError = "El campo de texto no puede estar vacío.";
                return false;
            }
            MensajeError = string.Empty;
            return true;
        }
    }

    // 3. Validador para decimales (ej. Precio)
    public class ValidadorDecimal : IValidadorCampo
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || !decimal.TryParse(valor, out decimal resultado) || resultado < 0)
            {
                MensajeError = "Debe ingresar un número decimal válido.";
                return false;
            }
            MensajeError = string.Empty;
            return true;
        }
    }

    // 4. Validador para enteros (ej. Cantidad)
    public class ValidadorEntero : IValidadorCampo
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || !int.TryParse(valor, out int resultado) || resultado < 0)
            {
                MensajeError = "Debe ingresar un número entero válido.";
                return false;
            }
            MensajeError = string.Empty;
            return true;
        }
    }
}
