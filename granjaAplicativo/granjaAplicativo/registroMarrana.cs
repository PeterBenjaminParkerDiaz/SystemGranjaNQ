using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace granjaAplicativo
{
    public partial class registroMarrana : Form
    {
        conexionBaseDatos conection = new conexionBaseDatos();
        string nameMarrana = null;
        public registroMarrana(string nombreMar)
        {
            InitializeComponent();
            nameMarrana = nombreMar;
        }
        private void registroMarrana_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("Marrana Nro", "Marrana Nro");
            dataGridView1.Columns.Add("Raza", "Raza");
            dataGridView1.Columns.Add("NP", "NP");

            DateTimePicker fechaServic = new DateTimePicker();
            fechaServic.Format = DateTimePickerFormat.Short;
            fechaServic.Visible = false;
            fechaServic.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaServic);
            dataGridView1.Columns.Add("Fecha IA", "Fecha IA");

            dataGridView1.Columns.Add("Macho Nro", "Macho Nro");
            dataGridView1.Columns.Add("Raza Macho", "Raza Macho");

            DateTimePicker fechaIndParto = new DateTimePicker();
            fechaIndParto.Format = DateTimePickerFormat.Short;
            fechaIndParto.Visible = false;
            fechaIndParto.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaIndParto);
            dataGridView1.Columns.Add("Ind. Parto", "Ind. Parto");

            dataGridView1.Columns.Add("Parto Calc", "Parto Calc");

            DateTimePicker fechaRealParto = new DateTimePicker();
            fechaRealParto.Format = DateTimePickerFormat.Short;
            fechaRealParto.Visible = false;
            fechaRealParto.Font = new Font("Segoe UI", 11F);
            dataGridView1.Controls.Add(fechaRealParto);
            dataGridView1.Columns.Add("Parto Real", "Parto Real");

            dataGridView1.Columns.Add("Tiempo Gest.", "Tiempo Gest.");

            DateTimePicker horaInicio = new DateTimePicker();
            horaInicio.Format = DateTimePickerFormat.Custom;
            horaInicio.CustomFormat = "HH:mm";
            horaInicio.ShowUpDown = true;
            horaInicio.Font = new Font("Segoe UI", 11F);
            dataGridView1.Controls.Add(horaInicio);
            dataGridView1.Columns.Add("Hora inicio parto", "Hora inicio parto");

            DateTimePicker horaFin = new DateTimePicker();
            horaFin.Format = DateTimePickerFormat.Custom;
            horaFin.CustomFormat = "HH:mm";
            horaFin.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            horaFin.ShowUpDown = true;
            dataGridView1.Controls.Add(horaFin);
            dataGridView1.Columns.Add("Hora fin parto", "Hora fin parto");

            dataGridView1.Columns.Add("Tiempo parto", "Tiempo parto");
            dataGridView1.Columns.Add("Camada No", "Camada No");
            dataGridView1.Columns.Add("Nro Paridera", "Nro Paridera");
            estilosDatagridview(dataGridView1);
            dataGridView1.Rows.Add(nameMarrana);

            dataGridView1.CellBeginEdit += (es, er) =>
            {
                DateTimePicker controlActivo = null;

                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        controlActivo = fechaRealParto;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha IA"].Index)
                    {
                        controlActivo = fechaServic;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Ind. Parto"].Index)
                    {
                        controlActivo = fechaIndParto;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora inicio parto"].Index)
                    {
                        controlActivo = horaInicio;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora fin parto"].Index)
                    {
                        controlActivo = horaFin;
                    }
                }
                if (controlActivo != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(er.ColumnIndex, er.RowIndex, true);
                    controlActivo.Size = rect.Size;
                    controlActivo.Location = rect.Location;
                    controlActivo.Visible = true;

                    var valorCelda = dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value;
                    if (DateTime.TryParse(valorCelda?.ToString(), out DateTime valor))                
                        controlActivo.Value = valor;              
                    else                 
                        controlActivo.Value = DateTime.Now;          
                }
            };

            dataGridView1.CellEndEdit += (es, er) =>
            {
                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Parto Real"].Value = fechaRealParto.Value.ToShortDateString();
                        fechaRealParto.Visible = false;
                        diasReturn(dataGridView1.Rows[0]);
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha IA"].Index)
                    {
                        DateTime fecha = fechaServic.Value;
                        dataGridView1.Rows[er.RowIndex].Cells["Fecha IA"].Value = fecha.ToShortDateString();
                        dataGridView1.Rows[er.RowIndex].Cells["Parto Calc"].Value = fecha.AddDays(115).ToString("dd/MM/yyyy");
                        fechaServic.Visible = false;
                        diasReturn(dataGridView1.Rows[0]);
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Ind. Parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Ind. Parto"].Value = fechaIndParto.Value.ToShortDateString();
                        fechaIndParto.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora inicio parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Hora inicio parto"].Value = horaInicio.Value.ToString("HH:mm");
                        horaInicio.Visible = false;
                        horasReturn(dataGridView1.Rows[0]);
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora fin parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Hora fin parto"].Value = horaFin.Value.ToString("HH:mm");
                        horaFin.Visible = false;
                        horasReturn(dataGridView1.Rows[0]);
                    }
                }
            };

            dataGridView2.Columns.Add("Nro lechón", "Nro lechón");

            DataGridViewComboBoxColumn sexo = new DataGridViewComboBoxColumn();
            sexo.Name = "Sexo";
            sexo.HeaderText = "Sexo";
            sexo.Items.AddRange("H", "M");
            dataGridView2.Columns.Add(sexo);

            dataGridView2.Columns.Add("Pezon I", "Pezon I");
            dataGridView2.Columns.Add("Pezon D", "Pezon D");
            dataGridView2.Columns.Add("Peso Nacim.", "Peso Nacim.");
            dataGridView2.Columns.Add("Peso Ingreso", "Peso Ingreso");

            DateTimePicker fechaTrans = new DateTimePicker();
            fechaTrans.Format = DateTimePickerFormat.Short;
            fechaTrans.Visible = false;
            fechaTrans.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView2.Controls.Add(fechaTrans);
            dataGridView2.Columns.Add("Fecha Ingreso", "Fecha Ingreso");

            dataGridView2.Columns.Add("Peso Salida", "Peso Salida");

            DateTimePicker fechaDestete = new DateTimePicker();
            fechaDestete.Format = DateTimePickerFormat.Short;
            fechaDestete.Visible = false;
            fechaDestete.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView2.Controls.Add(fechaDestete);
            dataGridView2.Columns.Add("Fecha Salida", "Fecha Salida");

            dataGridView2.Columns.Add("Observaciones", "Observaciones");

            dataGridView2.CellBeginEdit += (s, e) =>
            {
                DateTimePicker controlClic = null;
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == dataGridView2.Columns["Fecha Ingreso"].Index)
                        controlClic = fechaTrans;

                    else if (e.ColumnIndex == dataGridView2.Columns["Fecha Salida"].Index)
                        controlClic = fechaDestete;
                }

                if (controlClic != null)
                {
                    Rectangle formaRect = dataGridView2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    controlClic.Size = new Size(formaRect.Width, formaRect.Height);
                    controlClic.Location = new Point(formaRect.X, formaRect.Y);
                    controlClic.Visible = true;

                    var valorCelda = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    if (DateTime.TryParse(valorCelda?.ToString(), out DateTime fechaPas))                
                        controlClic.Value = fechaPas;                  
                    else                  
                        controlClic.Value = DateTime.Now;
                    
                }
            };
            dataGridView2.CellEndEdit += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (e.ColumnIndex == dataGridView2.Columns["Fecha Ingreso"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fechaTrans.Value.ToShortDateString();
                    fechaTrans.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Fecha Salida"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fechaDestete.Value.ToShortDateString();
                    fechaDestete.Visible = false;
                }
            };
            estilosDatagridview(dataGridView2, true, 174);


            dataGridView3.Columns.Add("D pp", "D pp");
            DateTimePicker fechIngresami = new DateTimePicker();
            fechIngresami.Format = DateTimePickerFormat.Short;
            fechIngresami.Visible = false;
            fechIngresami.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView3.Controls.Add(fechIngresami);
            dataGridView3.Columns.Add("Fecha", "Fecha");

            dataGridView3.Columns.Add("Datos", "Datos");
            estilosDatagridview(dataGridView3, true, 543);
            var listaValores = new (string, string)[]
            {
                ("1º", "Ingesta de calostro, pesaje, limpieza, desinfección de ombligo, corte de colmillos y descole."),
                ("3º", "Aplicación de hierro para prevenir anemia."),
                ("7º", "Suplemento alimenticio (Piggy Milk / Baby Pig)."),
                ("", "Destete con control de peso.")
            };
            foreach (var item in listaValores)
            {
                dataGridView3.Rows.Add(item.Item1, "", item.Item2);
            }
            dataGridView3.CellBeginEdit += (es, er) =>
            {
                if (er.RowIndex >= 0 && er.ColumnIndex == dataGridView3.Columns["Fecha"].Index)
                {
                    Rectangle rectangulo = dataGridView3.GetCellDisplayRectangle(er.ColumnIndex, er.RowIndex, true);
                    fechIngresami.Size = new Size(rectangulo.Width, rectangulo.Height);
                    fechIngresami.Location = new Point(rectangulo.X, rectangulo.Y);
                    fechIngresami.Visible = true;

                    var valor = dataGridView3.Rows[er.RowIndex].Cells["Fecha"].Value;
                    if (DateTime.TryParse(valor?.ToString(), out DateTime da))
                    {
                        fechIngresami.Value = da;
                    }
                    else
                    {
                        fechIngresami.Value = DateTime.Now;
                    }
                }
            };
            dataGridView3.CellEndEdit += (es, er) =>
            {
                if (er.RowIndex >= 0 && er.ColumnIndex == dataGridView3.Columns["Fecha"].Index)
                {
                    dataGridView3.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = fechIngresami.Value.ToShortDateString();
                    fechIngresami.Visible = false;
                }
            };

            var DatosTraidos = conection.valoresTraidos(nameMarrana.Trim().ToLower().Replace(" ", "_"));
            if (DatosTraidos.Count > 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[0];
                for (int i = 0; i < DatosTraidos.Count; i++)
                {
                    if (i == 9 || i == 10 || i == 11)
                    {
                        if (TimeSpan.TryParse(DatosTraidos[i]?.ToString(), out TimeSpan hora))
                            fila.Cells[i + 1].Value = hora.ToString(@"hh\:mm");
                        else
                            fila.Cells[i + 1].Value = null;
                    }
                    else if (i == 2 || i == 5 || i == 6 || i == 7)
                    {
                        if (DateTime.TryParse(DatosTraidos[i]?.ToString(), out DateTime fecha))
                            fila.Cells[i + 1].Value = fecha.ToString("dd/MM/yyyy");
                        else
                            fila.Cells[i + 1].Value = null;
                    }
                    else
                    {
                        fila.Cells[i + 1].Value = DatosTraidos[i];
                    }
                }
            }

            var datosTabla2 = conection.obtenerLechonesBD(nameMarrana.Trim().ToLower().Replace(" ", "_"));
            if (datosTabla2.Count > 0)
            {
                foreach (var valores in datosTabla2)
                {
                    dataGridView2.Rows.Add(valores.numeroLechon, valores.sexo, valores.pezonIzquierdo, valores.pezonDerecho,
                        valores.nacimiento, valores.transferencia, valores.fechaTransfer?.ToString("dd/MM/yyyy"), 
                        valores.destete, valores.fechaDeste?.ToString("dd/MM/yyyy"), valores.observaciones);
                }
            }
            var fechasTraidas = conection.seleccionarFechas(nameMarrana.Trim().ToLower().Replace(" ", "_"));
            if(fechasTraidas.Count > 0)
            {               
                for (int i = 0; i < fechasTraidas.Count && i < dataGridView3.Rows.Count; i++)
                {
                    dataGridView3.Rows[i].Cells["Fecha"].Value = fechasTraidas[i]?.ToString("dd/MM/yyyy");
                }             
            }
        }
        private void diasReturn(DataGridViewRow fila)
        {
            if (DateTime.TryParse(fila.Cells["Fecha IA"].Value?.ToString(), out DateTime fechaIAA) &&
                DateTime.TryParse(fila.Cells["Parto Real"].Value?.ToString(), out DateTime fechaPart))
            {
                int restaDias = (fechaPart - fechaIAA).Days;
                fila.Cells["Tiempo Gest."].Value = restaDias;
            }
        }
        private void horasReturn(DataGridViewRow fila)
        {
            if (TimeSpan.TryParse(fila.Cells["Hora inicio parto"].Value?.ToString(), out TimeSpan horaInicio) &&
                TimeSpan.TryParse(fila.Cells["Hora fin parto"].Value?.ToString(), out TimeSpan horaFin))
            {
                TimeSpan diferencia = horaFin - horaInicio;
                fila.Cells["Tiempo parto"].Value = diferencia.ToString(@"hh\:mm");
            }
        }

        string[] vari79 = new string[] {"Sexo", "Pezon I", "Pezon D", "NP", "Peso Nacim.", "Peso Ingreso",
                    "Peso Ingreso", "Peso Salida", "Tiempo Gest.", "Tiempo parto"};

        string[] vari100 = new string[] { "Fecha Ingreso", "Fecha Salida", "Parto Calc", "Parto Real", "Ind. Parto", "Macho Nro",
                "Fecha IA", "Marrana Nro", "D pp", "Fecha", "Nro lechón", "Raza", "Raza Macho", "Hora inicio parto", "Hora fin parto"};
        private void estilosDatagridview(DataGridView data, bool estado = false, int? altura = null)
        {
            data.RowHeadersVisible = false;
            data.AllowUserToAddRows = false;
            data.AllowUserToResizeRows = false;
            data.AllowUserToResizeColumns = false;
            data.EnableHeadersVisualStyles = false;
            data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            data.GridColor = ColorTranslator.FromHtml("#E5E7EB");
            data.DefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            data.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            data.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            data.ColumnHeadersHeight = 48;
            data.Width = estado ? 1520 : data.Width;
            data.Location = estado ? new Point((this.ClientSize.Width - data.Width) / 2, altura ?? data.Location.Y) : data.Location;
            data.RowTemplate.Height = (!estado) ? 30 : 27;
            data.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#334155");
            data.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F0EDED");
            data.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
       
            foreach (DataGridViewColumn column in data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;             
                if (vari79.Contains(column.HeaderText))
                {
                    column.Width = 75;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
                else if(vari100.Contains(column.HeaderText))
                {
                    column.Width = 105;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                column.ReadOnly = column.HeaderText == "Parto Calc" || column.HeaderText == "Tiempo Gest." 
                    || column.HeaderText == "Marrana Nro" || column.HeaderText == "Tiempo parto";
            }
        }

        private void agregarFila_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Insert(0, 1);
        }

        private void GuardarCamadas_Click(object sender, EventArgs e)
        {
            DataGridViewRow fila1 = dataGridView1.Rows[0];

            string razaMarrana = fila1.Cells[1].Value?.ToString().Trim();
            string numeroPezones = fila1.Cells[2].Value?.ToString().Trim();
            DateTime? fechaIA = DateTime.TryParse(fila1.Cells[3].Value?.ToString(), out DateTime fechaServiu) ? fechaServiu : null;
            string machoNumber = fila1.Cells[4].Value?.ToString().Trim();
            string razaMacho = fila1.Cells[5].Value?.ToString().Trim();
            DateTime? indParto = DateTime.TryParse(fila1.Cells[6].Value?.ToString(), out DateTime fechaIn) ? fechaIn : null;
            DateTime? partoCal = DateTime.TryParse(fila1.Cells[7].Value?.ToString(), out DateTime fechaCal) ? fechaCal : null;
            DateTime? partoReal = DateTime.TryParse(fila1.Cells[8].Value?.ToString(), out DateTime fechaReal) ? fechaReal : null;
            int? tiempoGest = int.TryParse(fila1.Cells[9].Value?.ToString(), out int diasCan) ? diasCan : null;         
            TimeSpan? horaInicio = TimeSpan.TryParse(fila1.Cells[10].Value?.ToString(), out TimeSpan horaIni) ? horaIni : null ;
            TimeSpan? horaFin = TimeSpan.TryParse(fila1.Cells[11].Value?.ToString(), out TimeSpan horaFinn) ? horaFinn : null;
            TimeSpan? tiempoPart = TimeSpan.TryParse(fila1.Cells[12].Value?.ToString(), out TimeSpan tiemParto) ? tiemParto : null;
            string camadaNur = fila1.Cells[13].Value?.ToString().Trim();
            string numeroParidera = fila1.Cells[14].Value?.ToString().Trim();

            if (conection.insertarCamadas(razaMarrana, numeroPezones, fechaIA, machoNumber, razaMacho, indParto, partoCal, partoReal, 
                tiempoGest, horaInicio, horaFin, tiempoPart, camadaNur, numeroParidera, nameMarrana.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardo existosamente!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var valoresIngreso = new List<(string numeroLechon, string sexo, string pezonIzquierdo, string pezonDerecho, string nacimiento,
                string transferencia, DateTime? fechaTrans, string destete, DateTime? fechaDest, string observaciones)>();

            foreach (DataGridViewRow fila in dataGridView2.Rows)
            {
                string numeroLechon = fila.Cells[0].Value?.ToString();
                string sexo = fila.Cells[1].Value?.ToString();
                string pezonIzquierdo = fila.Cells[2].Value?.ToString().Trim();
                string pezonDerecho = fila.Cells[3].Value?.ToString().Trim();
                string nacimiento = fila.Cells[4].Value?.ToString()?.Trim();
                string transferencia = fila.Cells[5].Value?.ToString()?.Trim();
                DateTime? fechaTrans = DateTime.TryParse(fila.Cells[6].Value?.ToString(), out DateTime fechaTra) ? fechaTra : null;
                string destete = fila.Cells[7].Value?.ToString()?.Trim();
                DateTime? fechaDest = DateTime.TryParse(fila.Cells[8].Value?.ToString(), out DateTime fechaDes) ? fechaDes : null;
                string observaciones = fila.Cells[9].Value?.ToString()?.Trim();

                valoresIngreso.Add((numeroLechon, sexo, pezonIzquierdo, pezonDerecho, nacimiento, transferencia, fechaTrans, 
                    destete, fechaDest, observaciones));
            }
            if (conection.insertarLechonesBD(valoresIngreso, nameMarrana.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardó exitosamente");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<DateTime?> fechasLechones = new List<DateTime?>();
            foreach(DataGridViewRow filas in dataGridView3.Rows)
            {
                DateTime? fecheActual = DateTime.TryParse(filas.Cells["Fecha"].Value?.ToString(), out DateTime fecha) ? fecha : null;
                fechasLechones.Add(fecheActual);
            }
            if (conection.insertarFechas(fechasLechones, nameMarrana.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardó exitosamente");
            }
        }
    }
}
