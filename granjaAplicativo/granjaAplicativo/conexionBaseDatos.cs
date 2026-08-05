using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace granjaAplicativo
{
    public class conexionBaseDatos
    {
        public MySqlConnection Conectar()
        {
            List<string> getIPdispositivo = GetLocalIPAddress();
            string servidor;
            string[] listasIPPermitidas = new string[] { "192.168.1.17", "192.168.1.10", "192.168.1.20", "192.168.1.42" };
            if (getIPdispositivo.Any(ip => listasIPPermitidas.Contains(ip)))
            {
                servidor = "192.168.1.17";
            }
            else
            {
                //Usaremos la VPN para conectarnos de forma remota a la red privada 'NQ25', de esta manera fingiremos estar fisicamente conectados ahí.
                servidor = "";
            }
            string usuario = "PCmultiples";
            string baseDeDatos = "granjadatos";
            string password = "D!0s_P0der0s0#G@rd1@n_9857";
            string cadena = $"Database={baseDeDatos}; Data Source={servidor}; user id={usuario}; Password={password}; SslMode=Required;";
            MySqlConnection conexion = new MySqlConnection(cadena);
            conexion.Open();
            return conexion;
        }
        private List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Select(ip => ip.ToString())
                .ToList();
            return ipAddress;
        }
        public bool verificarExisteTabla(string nombreMarrana)
        {
            try
            {
                string consultaSQL = "SELECT COUNT(*) FROM namemarranas WHERE REPLACE(LOWER(TRIM(name)), ' ', '_') = @nombreMarrana;";
                using (MySqlConnection cone = Conectar())
                {
                    using (MySqlCommand comando = new MySqlCommand(consultaSQL, cone))
                    {
                        comando.Parameters.AddWithValue("@nombreMarrana", nombreMarrana);
                        int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                        return cantidad > 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un problema: " + ex.Message);
                return false;
            }
        }
        public bool insertarMarrana(string nombreMarrana, string codigoFila)
        {
            string nombreTablas = nombreMarrana.ToLower().Replace(" ", "_");

            string consultaSQL1 = "INSERT INTO namemarranas(name, codigoFila) VALUES(@valueName, @valorCodigo);";

            string sqlTablaRegistro = $@"CREATE TABLE registro_{nombreTablas} ( Id INT AUTO_INCREMENT PRIMARY KEY,
                                        MarranaNro VARCHAR(50), Raza VARCHAR(50), NP INT, IndParto varchar(50), PartoCalc DATE,
                                        PartoReal DATE, CamadaNo VARCHAR(50), MachoNro VARCHAR(50), RazaMacho VARCHAR(50),
                                        FechaServicio DATE, HoraInicioParto TIME, HoraFinParto TIME, NroParidera VARCHAR(50));";

            string sqlTablaLechones = $@"CREATE TABLE lechones_{nombreTablas} ( Id INT AUTO_INCREMENT PRIMARY KEY,
                                        NroLechon INT, Sexo VARCHAR(10), PezonI INT, PezonD INT,PesoNacimiento VARCHAR(50),
                                        PesoTransferencia VARCHAR(50), PesoDestete VARCHAR(50), Observaciones VARCHAR(255));";

            using (MySqlConnection conexion = Conectar())
            {
                using(MySqlTransaction transac = conexion.BeginTransaction())
                {
                    try
                    {
                        using(MySqlCommand comando1 = new MySqlCommand(consultaSQL1, conexion, transac))
                        {
                            comando1.Parameters.AddWithValue("@valueName", nombreMarrana);
                            comando1.Parameters.AddWithValue("@valorCodigo", codigoFila);
                            comando1.ExecuteNonQuery();
                        }
                        using(MySqlCommand comando2 = new MySqlCommand(sqlTablaRegistro, conexion, transac))
                        {
                            comando2.ExecuteNonQuery();
                        }
                        using (MySqlCommand comando3 = new MySqlCommand(sqlTablaLechones, conexion, transac))
                        {
                            comando3.ExecuteNonQuery();
                        }
                        transac.Commit();
                        return true;
                    }
                    catch(Exception es)
                    {
                        transac.Rollback();
                        MessageBox.Show("Ocurrio un error: " + es.Message);
                        return false;
                    }
                }
            }           
        }
        public List<Tuple<string, string>> listaNombres()
        {
            List<Tuple<string, string>> resulat = new List<Tuple<string, string>>();
            string consultaSQL = "SELECT * FROM namemarranas;";
            using (MySqlConnection coneci = Conectar())
            {
                using (MySqlCommand comando = new MySqlCommand(consultaSQL, coneci))
                {
                    using (MySqlDataReader lerr = comando.ExecuteReader())
                    {
                        while (lerr.Read())
                        {
                            string nombew = lerr["Name"] == DBNull.Value ? "" : lerr["Name"].ToString();
                            string codigoRow = lerr["codigoFila"] == DBNull.Value ? "" : lerr["codigoFila"].ToString();
                            resulat.Add(new Tuple<string, string>(nombew, codigoRow));
                        }                                                           
                    }
                }
            }
            return resulat;
        }
        public bool eliminarMarrana(string codigo)
        {
            try
            {
                string consultaSQL = "DELETE FROM namemarranas WHERE codigoFila = @codigoArgumento;";
                using (MySqlConnection conect = Conectar())
                {
                    using (MySqlCommand comando = new MySqlCommand(consultaSQL, conect))
                    {
                        comando.Parameters.AddWithValue("@codigoArgumento", codigo);
                        int filasEliminadas = comando.ExecuteNonQuery();
                        return filasEliminadas > 0;
                    }
                }
            }
            catch (Exception es)
            {
                MessageBox.Show("Error al eliminar marrana: " + es.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
