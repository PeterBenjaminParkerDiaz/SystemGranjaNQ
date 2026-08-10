using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            string sqlTablaRegistro = $@"CREATE TABLE registro_{nombreTablas} (Id INT AUTO_INCREMENT PRIMARY KEY,
                                        MarranaNro VARCHAR(50), Raza VARCHAR(50), NP INT, FechaServicio DATE, MachoNro VARCHAR(50), RazaMacho VARCHAR(50),
                                        IndParto varchar(50), PartoCalc DATE, PartoReal DATE, CamadaNo VARCHAR(50), 
                                        HoraInicioParto Varchar(50), HoraFinParto varchar(50), NroParidera VARCHAR(50));";

            string sqlTablaLechones = $@"CREATE TABLE lechones_{nombreTablas} (Id INT AUTO_INCREMENT PRIMARY KEY,
                                        NroLechon VARCHAR(50), Sexo VARCHAR(10), PezonI INT, PezonD INT, PesoNacimiento VARCHAR(50), FechaNacim DATE,
                                        PesoTransfer VARCHAR(50), FechaTransfer DATE, PesoDestete VARCHAR(50), FechaDestete DATE, Observaciones VARCHAR(255));";

            string fechasAcum = $"CREATE TABLE fechas_{nombreTablas} (Id INT AUTO_INCREMENT PRIMARY KEY, FechasLechones DATE);";

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
                        using (MySqlCommand comando4 = new MySqlCommand(fechasAcum, conexion, transac))
                        {
                            comando4.ExecuteNonQuery();
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

        public List<object> valoresTraidos(string nombreTabla)
        {
            List<object> datosDevolver = new List<object>();
            string consultaSQL = $"SELECT * FROM registro_{nombreTabla};";
            using(MySqlConnection connec = Conectar())
            {
                using(MySqlCommand comando = new MySqlCommand(consultaSQL, connec))
                {
                    using(MySqlDataReader leer = comando.ExecuteReader())
                    {
                        if (leer.Read())
                        {
                            for(int i = 1; i < 14; i++)
                            {
                                datosDevolver.Add(leer.IsDBNull(i) ? null : leer.GetValue(i));
                            }
                        }
                    }
                }
            }
            return datosDevolver;
        }
        public bool insertarCamadas(string NumberMarrana, string razaMarrana, int? numeroPezones, DateTime? fechaServicio, 
            string machoNumber, string razaMacho, string indParto, DateTime? partoCal, DateTime? partoReal, string camadaNur, 
            string horaInicio, string horaFin, string numeroParidera, string marranatabla){
            try
            {
                string verificarExiste = $"SELECT COUNT(*) FROM registro_{marranatabla};";

                using (MySqlConnection conn = Conectar())
                {
                    bool existeFila = false;
                    using (MySqlCommand comando = new MySqlCommand(verificarExiste, conn))
                    {
                        int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                        existeFila = cantidad > 0;
                    }
                    if (existeFila) // Existe fila → UPDATE
                    {
                        string sql = $@"UPDATE registro_{marranatabla} SET MarranaNro = @MarranaNro, Raza = @Raza, NP = @NP, IndParto = @IndParto, PartoCalc = @PartoCalc,
                                        PartoReal = @PartoReal, CamadaNo = @CamadaNo, MachoNro = @MachoNro, RazaMacho = @RazaMacho, FechaServicio = @FechaServicio,
                                        HoraInicioParto = @HoraInicioParto, HoraFinParto = @HoraFinParto, NroParidera = @NroParidera;";

                        using (MySqlCommand comando = new MySqlCommand(sql, conn))
                        {
                            comando.Parameters.AddWithValue("@MarranaNro", NumberMarrana ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@Raza", razaMarrana ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@NP", numeroPezones ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@IndParto", indParto ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@PartoCalc", partoCal ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@PartoReal", partoReal ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@CamadaNo", camadaNur ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@MachoNro", machoNumber ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@RazaMacho", razaMacho ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@FechaServicio", fechaServicio ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@HoraInicioParto", horaInicio ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@HoraFinParto", horaFin ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@NroParidera", numeroParidera ?? (object)DBNull.Value);
                            int filasAfectadas = comando.ExecuteNonQuery();
                            return filasAfectadas > 0;
                        }
                    }
                    else // No existe fila → INSERT
                    {
                        string sql = $@" INSERT INTO registro_{marranatabla} (MarranaNro, Raza, NP,IndParto, PartoCalc, PartoReal, CamadaNo, MachoNro, 
                                        RazaMacho, FechaServicio, HoraInicioParto, HoraFinParto, NroParidera) VALUES(@MarranaNro, @Raza, @NP, @IndParto,
                        @PartoCalc, @PartoReal, @CamadaNo, @MachoNro, @RazaMacho, @FechaServicio, @HoraInicioParto, @HoraFinParto, @NroParidera);";

                        using (MySqlCommand comando = new MySqlCommand(sql, conn))
                        {
                            comando.Parameters.AddWithValue("@MarranaNro", NumberMarrana ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@Raza", razaMarrana ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@NP", numeroPezones ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@IndParto", indParto ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@PartoCalc", partoCal ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@PartoReal", partoReal ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@CamadaNo", camadaNur ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@MachoNro", machoNumber ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@RazaMacho", razaMacho ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@FechaServicio", fechaServicio ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@HoraInicioParto", horaInicio ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@HoraFinParto", horaFin ?? (object)DBNull.Value);
                            comando.Parameters.AddWithValue("@NroParidera", numeroParidera ?? (object)DBNull.Value);
                            int filasAfectadas = comando.ExecuteNonQuery();
                            return filasAfectadas > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un problema: " + ex.Message);
                return false;
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
        public bool eliminarMarrana(string codigo, string nameTablaBorrar)
        {
            try
            {
                string consultaSQL1 = $"DROP TABLE IF EXISTS registro_{nameTablaBorrar}, lechones_{nameTablaBorrar}, fechas_{nameTablaBorrar};";
                string consultaSQL2 = "DELETE FROM namemarranas WHERE codigoFila = @codigoArgumento;";
                using (MySqlConnection conect = Conectar())
                {
                    using (MySqlTransaction transa = conect.BeginTransaction())
                    {
                        try
                        {
                            using (MySqlCommand comando1 = new MySqlCommand(consultaSQL1, conect, transa))
                            {
                                comando1.ExecuteNonQuery();
                            }

                            using (MySqlCommand comando2 = new MySqlCommand(consultaSQL2, conect, transa))
                            {
                                comando2.Parameters.AddWithValue("@codigoArgumento", codigo);
                                comando2.ExecuteNonQuery();
                            }

                            transa.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transa.Rollback();
                            MessageBox.Show("Ocurrió un error: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch (Exception es)
            {
                MessageBox.Show("Error al eliminar marrana: " + es.Message);
                return false;
            }
        }
        public bool insertarLechonesBD(List<(string numeroLechon, string sexo, int? pezonIzquierdo, int? pezonDerecho,
            string nacimiento, DateTime? fechaNacim, string transferencia, DateTime? fechaTransfer,
            string destete, DateTime? fechaDestete, string observaciones)> valores, string tablaName)
        {
            try
            {
                string consultaSQL1 = $"DELETE FROM lechones_{tablaName};";

                string consultaSQL2 = $"INSERT INTO lechones_{tablaName}(NroLechon, Sexo, PezonI, PezonD, PesoNacimiento, FechaNacim, " +
                    "PesoTransfer, FechaTransfer, PesoDestete, FechaDestete, Observaciones) VALUES (@NroLechon, @sexo, @PezonI, @PezonD, @PesoNacimiento, " +
                    "@FechaNacim, @PesoTransfer, @FechaTransfer, @PesoDestete, @FechaDestete, @Observaciones);";

                using (MySqlConnection cone = Conectar())
                {
                    using (MySqlTransaction trasa = cone.BeginTransaction())
                    {
                        try
                        {
                            using (MySqlCommand comando = new MySqlCommand(consultaSQL1, cone, trasa))
                            {
                                comando.ExecuteNonQuery();
                            }
                            foreach (var valor in valores)
                            {
                                using (MySqlCommand cmd = new MySqlCommand(consultaSQL2, cone, trasa))
                                {
                                    cmd.Parameters.AddWithValue("@NroLechon", valor.numeroLechon ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@sexo", valor.sexo ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PezonI", valor.pezonIzquierdo ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PezonD", valor.pezonDerecho ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PesoNacimiento", valor.nacimiento ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@FechaNacim", valor.fechaNacim ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PesoTransfer", valor.transferencia ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@FechaTransfer", valor.fechaTransfer ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@PesoDestete", valor.destete ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@FechaDestete", valor.fechaDestete ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Observaciones", valor.observaciones ?? (object)DBNull.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            trasa.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            trasa.Rollback();
                            MessageBox.Show("Problema al guardar los lechones:\n\n" + ex.Message,"Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<(string numeroLechon, string sexo, int? pezonIzquierdo, int? pezonDerecho, string nacimiento, DateTime? fechaNacim,
            string transferencia, DateTime? fechaTransfer, string destete, DateTime? fechaDeste, string observaciones)> obtenerLechonesBD(string tablaName)
        {
            var valores = new List<(string numeroLechon, string sexo, int? pezonIzquierdo, int? pezonDerecho, string nacimiento, DateTime? fechaNacim, 
                string transferencia, DateTime? fechaTransfer, string destete, DateTime? fechaDeste, string observaciones)>();

            string consultaSQL = $"SELECT NroLechon, Sexo, PezonI, PezonD, PesoNacimiento, FechaNacim," +
                $"PesoTransfer, FechaTransfer, PesoDestete, FechaDestete, Observaciones FROM lechones_{tablaName};";

            using (MySqlConnection cone = Conectar())
            {
                using (MySqlCommand cmd = new MySqlCommand(consultaSQL, cone))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string numeroLechon = reader.IsDBNull(0) ? null : reader.GetString(0);
                            string sexo = reader.IsDBNull(1) ? null : reader.GetString(1);
                            int? pezonIzquierdo = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                            int? pezonDerecho = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                            string nacimiento = reader.IsDBNull(4) ? null : reader.GetString(4);
                            DateTime? fechaNacimiento = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                            string transferencia = reader.IsDBNull(6) ? null : reader.GetString(6);
                            DateTime? fechaTransfer = reader.IsDBNull(7) ? null : reader.GetDateTime(7);
                            string destete = reader.IsDBNull(8) ? null : reader.GetString(8);
                            DateTime? fechaDestete = reader.IsDBNull(9) ? null : reader.GetDateTime(9);
                            string observaciones = reader.IsDBNull(10) ? null : reader.GetString(10);
                            valores.Add((numeroLechon, sexo, pezonIzquierdo, pezonDerecho, nacimiento, fechaNacimiento,
                                transferencia, fechaTransfer, destete, fechaDestete, observaciones));
                        }
                    }
                }
            }
            return valores;
        }
        public bool insertarFechas(List<DateTime?> fechas, string nameTable)
        {
            string consulta = $"INSERT INTO fechas_{nameTable} (FechasLechones) VALUES(@fecha);";
            if (fechas.Count == 0) return false;
            try
            {
                using (MySqlConnection con = Conectar())
                {
                    foreach (var valores in fechas)
                    {
                        using (MySqlCommand comando = new MySqlCommand(consulta, con))
                        {
                            comando.Parameters.AddWithValue("@fecha", valores);
                            comando.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception es)
            {
                MessageBox.Show("Ocurrio un error: " + es.Message);
                return false;
            }
        }
        public List<DateTime?> seleccionarFechas(string nameTable)
        {
            List<DateTime?> fechas = new List<DateTime?>();
            string consulta = $"SELECT FechasLechones FROM fechas_{nameTable};";
            
            using (MySqlConnection con = Conectar())
            {
                using (MySqlCommand comando = new MySqlCommand(consulta, con))
                {
                    using (MySqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            DateTime? fechaEstamo = lector["FechasLechones"] == DBNull.Value ? null : Convert.ToDateTime(lector["FechasLechones"]);
                            fechas.Add(fechaEstamo);
                        }
                    }
                }               
            }
            return fechas;
        }
    }
}
