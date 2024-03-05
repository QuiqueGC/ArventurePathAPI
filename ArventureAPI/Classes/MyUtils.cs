using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ArventureAPI.Classes
{
    public static class MyUtils
    {
        /// <summary>
        /// Extrae el mensaje de error de la excepción pertinente
        /// </summary>
        /// <param name="ex">la excepción que ha ocurrido</param>
        /// <returns>el mensaje de la excepción</returns>
        public static String ErrorMessage(SqlException ex)
        {
            String msg = "";

            switch (ex.Number)
            {
                case 2:
                    msg = "Servidor no operativo";
                    break;
                case 547:
                    msg = "El registro tiene datos relacionados";
                    break;
                case 2601:
                    msg = "Registro duplicado";
                    break;
                case 4060:
                    msg = "No se puede abrir la base de datos";
                    break;
                case 18456:
                    msg = "Error al iniciar sesión";
                    break;
                default:
                    msg = ex.Number + " : " + ex.Message;
                    break;
            }
            return msg;
        }
    }
}