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
        public registroMarrana()
        {
            InitializeComponent();
        }

        private void registroMarrana_Load(object sender, EventArgs e)
        {
            label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, 0);

            dataGridView1.Columns.Add("Marrana Nro", "Marrana Nro");
            dataGridView1.Columns.Add("Raza", "Raza");

            NumericUpDown numberPartos = new NumericUpDown();
            numberPartos.Minimum = 0;
            numberPartos.Maximum = 100;
            numberPartos.Visible = false;
            numberPartos.Font = new Font("Segoe UI", 11F);
            dataGridView1.Controls.Add(numberPartos);
            dataGridView1.Columns.Add("NP", "NP");

            DateTimePicker fechaServic = new DateTimePicker();
            fechaServic.Format = DateTimePickerFormat.Short;
            fechaServic.Visible = false;
            fechaServic.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaServic);
            dataGridView1.Columns.Add("Fecha Servicio", "Fecha Servicio");

            dataGridView1.Columns.Add("Macho Nro", "Macho Nro");
            dataGridView1.Columns.Add("Raza", "Raza");
            dataGridView1.Columns.Add("Ind. Parto", "Ind. Parto");

            DateTimePicker fechaCalcu = new DateTimePicker();
            fechaCalcu.Format = DateTimePickerFormat.Short;
            fechaCalcu.Visible = false;
            fechaCalcu.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaCalcu);
            dataGridView1.Columns.Add("Parto Calc", "Parto Calc");

            DateTimePicker fechaRealParto = new DateTimePicker();
            fechaRealParto.Format = DateTimePickerFormat.Short;
            fechaRealParto.Visible = false;
            fechaRealParto.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaRealParto);
            dataGridView1.Columns.Add("Parto Real", "Parto Real");

            dataGridView1.Columns.Add("Camada No", "Camada No");
            dataGridView1.Columns.Add("Hora inicio parto", "Hora inicio parto");
            dataGridView1.Columns.Add("Hora fin parto", "Hora fin parto");
            dataGridView1.Columns.Add("Nro Paridera", "Nro Paridera");
            estilosDatagridview(dataGridView1);
            dataGridView1.Rows.Add();

            dataGridView1.CellBeginEdit += (es, er) =>
            {
                Control controlActivo = null;
                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["NP"].Index)
                    {
                        controlActivo = numberPartos; //Subir o bajar número
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Calc"].Index)
                    {
                        controlActivo = fechaCalcu; //Fecha
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        controlActivo = fechaRealParto; //Fecha
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha Servicio"].Index)
                    {
                        controlActivo = fechaServic; //Fecha
                    }
                }
                if (controlActivo != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(er.ColumnIndex, er.RowIndex, true);
                    controlActivo.Size = rect.Size;
                    controlActivo.Location = rect.Location;
                    controlActivo.Visible = true;

                    if (dataGridView1.CurrentCell != null)
                    {
                        if (controlActivo is DateTimePicker dtp)
                            dtp.Value = DateTime.TryParse(dataGridView1.CurrentCell.Value?.ToString(), out DateTime fechaValor) ? fechaValor : DateTime.Now;

                        else if (controlActivo is NumericUpDown num)
                            num.Value = int.TryParse(dataGridView1.CurrentCell.Value?.ToString(), out int numPezones) ? numPezones : 0;
                    }
                    else
                    {
                        if (controlActivo is DateTimePicker dtp) dtp.Value = DateTime.Now;
                        else if (controlActivo is NumericUpDown num) num.Value = 0;
                    }
                }
            };
            dataGridView1.CellEndEdit += (es, er) =>
            {
                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["NP"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["NP"].Value = (int)numberPartos.Value;
                        numberPartos.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Calc"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Parto Calc"].Value = fechaCalcu.Value.ToShortDateString();
                        fechaCalcu.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Parto Real"].Value = fechaRealParto.Value.ToShortDateString();
                        fechaRealParto.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha Servicio"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Fecha Servicio"].Value = fechaServic.Value.ToShortDateString();
                        fechaServic.Visible = false;
                    }
                }
            };

            dataGridView2.Columns.Add("Nro lechón", "Nro lechón");

            DataGridViewComboBoxColumn sexo = new DataGridViewComboBoxColumn();
            sexo.Name = "Sexo";
            sexo.HeaderText = "Sexo";
            sexo.Items.AddRange("H", "M");
            dataGridView2.Columns.Add(sexo);

            NumericUpDown numIzquier = new NumericUpDown();
            numIzquier.Minimum = 0;
            numIzquier.Maximum = 20;
            numIzquier.Visible = false;
            numIzquier.Font = new Font("Segoe UI", 11F);
            dataGridView2.Controls.Add(numIzquier);
            dataGridView2.Columns.Add("Pezon I", "Pezon I");

            NumericUpDown numDerecho = new NumericUpDown();
            numDerecho.Minimum = 0;
            numDerecho.Maximum = 20;
            numDerecho.Visible = false;
            numDerecho.Font = new Font("Segoe UI", 11F);
            dataGridView2.Controls.Add(numDerecho);
            dataGridView2.Columns.Add("Pezon D", "Pezon D");

            dataGridView2.Columns.Add("Peso Nacim.", "Peso Nacim.");

            DateTimePicker fechaNaci = new DateTimePicker();
            fechaNaci.Format = DateTimePickerFormat.Short;
            fechaNaci.Visible = false;
            fechaNaci.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView2.Controls.Add(fechaNaci);
            dataGridView2.Columns.Add("Fecha Nacim.", "Fecha Nacim.");

            dataGridView2.Columns.Add("Peso Transf", "Peso Transf");

            DateTimePicker fechaTrans = new DateTimePicker();
            fechaTrans.Format = DateTimePickerFormat.Short;
            fechaTrans.Visible = false;
            fechaTrans.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView2.Controls.Add(fechaTrans);
            dataGridView2.Columns.Add("Fecha Trans.", "Fecha Trans.");

            dataGridView2.Columns.Add("Peso Destete", "Peso Destete");

            DateTimePicker fechaDestete = new DateTimePicker();
            fechaDestete.Format = DateTimePickerFormat.Short;
            fechaDestete.Visible = false;
            fechaDestete.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView2.Controls.Add(fechaDestete);
            dataGridView2.Columns.Add("Fecha Destete", "Fecha Destete.");

            dataGridView2.Columns.Add("Observaciones", "Observaciones");

            dataGridView2.CellBeginEdit += (s, e) =>
            {
                Control controlClic = null;
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == dataGridView2.Columns["Pezon I"].Index)
                        controlClic = numIzquier;

                    else if (e.ColumnIndex == dataGridView2.Columns["Pezon D"].Index)
                        controlClic = numDerecho;

                    else if (e.ColumnIndex == dataGridView2.Columns["Fecha Nacim."].Index)
                        controlClic = fechaNaci;

                    else if (e.ColumnIndex == dataGridView2.Columns["Fecha Trans."].Index)
                        controlClic = fechaTrans;

                    else if (e.ColumnIndex == dataGridView2.Columns["Fecha Destete"].Index)
                        controlClic = fechaDestete;
                }

                if (controlClic != null)
                {
                    Rectangle formaRect = dataGridView2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    controlClic.Size = new Size(formaRect.Width, formaRect.Height);
                    controlClic.Location = new Point(formaRect.X, formaRect.Y);
                    controlClic.Visible = true;

                    if (dataGridView2.CurrentCell != null)
                    {
                        if (controlClic is NumericUpDown numerito)
                            numerito.Value = int.TryParse(dataGridView2.CurrentCell.Value?.ToString(), out int numer) ? numer : 0;

                        else if (controlClic is DateTimePicker dato)
                            dato.Value = DateTime.TryParse(dataGridView2.CurrentCell.Value?.ToString(), out DateTime fechaPas) ? fechaPas : DateTime.Now;
                    }
                    else
                    {
                        if (controlClic is NumericUpDown num) num.Value = 0;
                        else if (controlClic is DateTimePicker dtp) dtp.Value = DateTime.Now;
                    }
                }
            };
            dataGridView2.CellEndEdit += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (e.ColumnIndex == dataGridView2.Columns["Pezon I"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (int)numIzquier.Value;
                    numIzquier.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Pezon D"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (int)numDerecho.Value;
                    numDerecho.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Fecha Nacim."].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fechaNaci.Value.ToShortDateString();
                    fechaNaci.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Fecha Trans."].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = fechaTrans.Value.ToShortDateString();
                    fechaTrans.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Fecha Destete"].Index)
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

            var DatosTraidos = conection.valoresTraidos(label1.Text.Trim().ToLower().Replace(" ", "_"));
            if (DatosTraidos.Count > 0)
            {
                DataGridViewRow fila = dataGridView1.Rows[0];
                for (int i = 0; i < DatosTraidos.Count; i++)
                {
                    if (DateTime.TryParse(DatosTraidos[i]?.ToString(), out DateTime fecha))
                    {
                        fila.Cells[i].Value = fecha.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        fila.Cells[i].Value = DatosTraidos[i] ?? "";
                    }
                }
            }

            var datosTabla2 = conection.obtenerLechonesBD(label1.Text.Trim().ToLower().Replace(" ", "_"));
            if (datosTabla2.Count > 0)
            {
                foreach (var valores in datosTabla2)
                {
                    dataGridView2.Rows.Add(valores.numeroLechon, valores.sexo, valores.pezonIzquierdo, valores.pezonDerecho,
                        valores.nacimiento, valores.fechaNacim?.ToString("dd/MM/yyyy"), valores.transferencia,
                        valores.fechaTransfer?.ToString("dd/MM/yyyy"), valores.destete, valores.fechaDeste?.ToString("dd/MM/yyyy"),
                        valores.observaciones);
                }
            }
            var fechasTraidas = conection.seleccionarFechas(label1.Text.Trim().ToLower().Replace(" ", "_"));
            if(fechasTraidas.Count > 0)
            {               
                for (int i = 0; i < fechasTraidas.Count && i < dataGridView3.Rows.Count; i++)
                {
                    dataGridView3.Rows[i].Cells["Fecha"].Value = fechasTraidas[i]?.ToString("dd/MM/yyyy");
                }             
            }
        }
        private void estilosDatagridview(DataGridView data, bool estado = false, int? altura = null)
        {
            data.RowHeadersVisible = false;
            data.AllowUserToAddRows = false;
            data.AllowUserToResizeRows = false;
            data.AllowUserToResizeColumns = false;
            data.EnableHeadersVisualStyles = false;
            data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            data.GridColor = ColorTranslator.FromHtml("#E5E7EB");
            data.DefaultCellStyle.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
            data.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", (!estado) ? 12f : 10f, FontStyle.Regular);
            data.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            data.ColumnHeadersHeight = 35;
            data.Width = estado ? 1520 : data.Width;
            data.Location = estado ? new Point((this.ClientSize.Width - data.Width) / 2, altura ?? data.Location.Y) : data.Location;
            data.RowTemplate.Height = (!estado) ? 30 : 27;
            data.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#334155");
            data.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F0EDED");
            data.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            foreach (DataGridViewColumn column in data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (!estado)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    column.ReadOnly = (column.HeaderText == "D pp" || column.HeaderText == "Datos") ? true : false;
                    if (column.HeaderText == "Observaciones" || column.HeaderText == "Datos")
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        column.Width = 100;
                    }
                }
            }
        }

        private void agregarFila_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Insert(0, 1);
        }

        private void GuardarCamadas_Click(object sender, EventArgs e)
        {
            DataGridViewRow fila1 = dataGridView1.Rows[0];

            string NumberMarrana = fila1.Cells[0].Value?.ToString().Trim();
            string razaMarrana = fila1.Cells[1].Value?.ToString().Trim();
            int? numeroPezones = int.TryParse(fila1.Cells[2].Value?.ToString(), out int numero) ? numero : null;
            DateTime? fechaServicio = DateTime.TryParse(fila1.Cells[3].Value?.ToString(), out DateTime fechaServiu) ? fechaServiu : null;
            string machoNumber = fila1.Cells[4].Value?.ToString().Trim();
            string razaMacho = fila1.Cells[5].Value?.ToString().Trim();
            string indParto = fila1.Cells[6].Value?.ToString().Trim();
            DateTime? partoCal = DateTime.TryParse(fila1.Cells[7].Value?.ToString(), out DateTime fechaCal) ? fechaCal : null;
            DateTime? partoReal = DateTime.TryParse(fila1.Cells[8].Value?.ToString(), out DateTime fechaReal) ? fechaReal : null;
            string camadaNur = fila1.Cells[9].Value?.ToString().Trim();
            string horaInicio = fila1.Cells[10].Value?.ToString().Trim();
            string horaFin = fila1.Cells[11].Value?.ToString().Trim();
            string numeroParidera = fila1.Cells[12].Value?.ToString().Trim();

            if (conection.insertarCamadas(NumberMarrana, razaMarrana, numeroPezones, fechaServicio, machoNumber, razaMacho
                , indParto, partoCal, partoReal, camadaNur, horaInicio, horaFin, numeroParidera, label1.Text.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardo existosamente!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var valoresIngreso = new List<(string numeroLechon, string sexo, int? pezonIzquierdo, int? pezonDerecho, string nacimiento,
                DateTime? fechaNacim, string transferencia, DateTime? fechaTrans, string destete, DateTime? fechaDest, string observaciones)>();

            foreach (DataGridViewRow fila in dataGridView2.Rows)
            {
                string numeroLechon = fila.Cells[0].Value?.ToString();
                string sexo = fila.Cells[1].Value?.ToString();
                int? pezonIzquierdo = int.TryParse(fila.Cells[2].Value?.ToString(), out int izquierdo) ? izquierdo : null;
                int? pezonDerecho = int.TryParse(fila.Cells[3].Value?.ToString(), out int derecho) ? derecho : null;
                string nacimiento = fila.Cells[4].Value?.ToString()?.Trim();
                DateTime? fechaNacim = DateTime.TryParse(fila.Cells[5].Value?.ToString(), out DateTime fechaNa) ? fechaNa : null;
                string transferencia = fila.Cells[6].Value?.ToString()?.Trim();
                DateTime? fechaTrans = DateTime.TryParse(fila.Cells[7].Value?.ToString(), out DateTime fechaTra) ? fechaTra : null;
                string destete = fila.Cells[8].Value?.ToString()?.Trim();
                DateTime? fechaDest = DateTime.TryParse(fila.Cells[9].Value?.ToString(), out DateTime fechaDes) ? fechaDes : null;
                string observaciones = fila.Cells[10].Value?.ToString()?.Trim();

                valoresIngreso.Add((numeroLechon, sexo, pezonIzquierdo, pezonDerecho, nacimiento, fechaNacim, transferencia,
                    fechaTrans, destete, fechaDest, observaciones));
            }
            if (conection.insertarLechonesBD(valoresIngreso, label1.Text.Trim().ToLower().Replace(" ", "_")))
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
            if (conection.insertarFechas(fechasLechones, label1.Text.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardó exitosamente");
            }
        }
    }
}
