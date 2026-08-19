using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace granjaAplicativo
{
    public partial class registroMarrana : Form
    {
        conexionBaseDatos conection = new conexionBaseDatos();
        string nameMarrana = null;
        string[] nombresEjecutar = new string[] { "Peso Nacim.", "Peso Ingreso", "Peso Salida", "Control 1", "Control 2", "Destete" };
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
            dataGridView1.Columns.Add("F. IA", "F. IA");

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

            dataGridView1.Columns.Add("Tmp Gest.", "Tmp Gest.");

            DateTimePicker horaInicio = new DateTimePicker();
            horaInicio.Format = DateTimePickerFormat.Custom;
            horaInicio.CustomFormat = "HH:mm";
            horaInicio.Visible = false;
            horaInicio.ShowUpDown = true;
            horaInicio.Font = new Font("Segoe UI", 11F);
            dataGridView1.Controls.Add(horaInicio);
            dataGridView1.Columns.Add("H. ini. parto", "H. ini. parto");

            DateTimePicker horaFin = new DateTimePicker();
            horaFin.Format = DateTimePickerFormat.Custom;
            horaFin.CustomFormat = "HH:mm";
            horaFin.Visible = false;
            horaFin.Font = new Font("Segoe UI", 11F);
            horaFin.ShowUpDown = true;
            dataGridView1.Controls.Add(horaFin);
            dataGridView1.Columns.Add("H. fin parto", "H. fin parto");

            dataGridView1.Columns.Add("Tmp parto", "Tmp parto");
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
                    else if (er.ColumnIndex == dataGridView1.Columns["F. IA"].Index)
                    {
                        controlActivo = fechaServic;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Ind. Parto"].Index)
                    {
                        controlActivo = fechaIndParto;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["H. ini. parto"].Index)
                    {
                        controlActivo = horaInicio;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["H. fin parto"].Index)
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
                    else if (er.ColumnIndex == dataGridView1.Columns["F. IA"].Index)
                    {
                        DateTime fecha = fechaServic.Value;
                        dataGridView1.Rows[er.RowIndex].Cells["F. IA"].Value = fecha.ToShortDateString();
                        dataGridView1.Rows[er.RowIndex].Cells["Parto Calc"].Value = fecha.AddDays(115).ToString("dd/MM/yyyy");
                        fechaServic.Visible = false;
                        diasReturn(dataGridView1.Rows[0]);
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Ind. Parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["Ind. Parto"].Value = fechaIndParto.Value.ToShortDateString();
                        fechaIndParto.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["H. ini. parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["H. ini. parto"].Value = horaInicio.Value.ToString("HH:mm");
                        horaInicio.Visible = false;
                        horasReturn(dataGridView1.Rows[0]);
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["H. fin parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells["H. fin parto"].Value = horaFin.Value.ToString("HH:mm");
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

            dataGridView2.Columns.Add("Camada Donante", "Camada Donante");
            dataGridView2.Columns.Add("Camada Recep.", "Camada Recep.");
            dataGridView2.Columns.Add("Control 1", "Control 1");
            dataGridView2.Columns.Add("Control 2", "Control 2");
            dataGridView2.Columns.Add("Destete", "Destete");
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

                string nameColumna = dataGridView2.Columns[e.ColumnIndex].Name;

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
                if (nombresEjecutar.Contains(nameColumna))
                {
                    float sumaValores = dataGridView2.Rows.Cast<DataGridViewRow>().Sum(fila => float.TryParse(
                        fila.Cells[nameColumna].Value?.ToString(), out float valor) ? valor : 0);
                    int countLechones = dataGridView2.Rows.Cast<DataGridViewRow>().Count(fila => float.TryParse(fila.Cells[nameColumna].Value?.ToString(), out _));
                    float promedio = sumaValores / countLechones;

                    float sumaCuadrados = 0;
                    foreach (DataGridViewRow fila in dataGridView2.Rows)
                    {
                        if (float.TryParse(fila.Cells[nameColumna].Value?.ToString(), out float valor))
                        {
                            float diferencia = valor - promedio;
                            float cuadrado = diferencia * diferencia;
                            sumaCuadrados += cuadrado;
                        }
                    }
                    float desviacionEstandar = (float)Math.Sqrt(sumaCuadrados / countLechones);
                    float coeficienVariacion = (desviacionEstandar / promedio) * 100;
                    switch (nameColumna)
                    {
                        case "Peso Nacim.":
                            Label[] labels1 = new Label[] { label1, label3, label5, label9, label11 };
                            label1.Text = $"Peso total:  {sumaValores:0.000} Kg";
                            label3.Text = $"Cantidad lechones:  {countLechones}";
                            label5.Text = $"Promedio:  {promedio:0.000} kg";
                            label9.Text = $"Desviación estandar:  {desviacionEstandar:0.000} kg";
                            label11.Text = $"Coeficiente variación:  {coeficienVariacion:0.000} %";
                            break;

                        case "Peso Ingreso":
                            ejecLabels(new[] { label6, label7, label8, label10, label12 }, 
                                sumaValores, countLechones, promedio, desviacionEstandar, coeficienVariacion);
                            break;

                        case "Peso Salida":
                            ejecLabels(new[] { label13, label14, label15, label16, label17 },
                                sumaValores, countLechones, promedio, desviacionEstandar, coeficienVariacion);
                            break;

                        case "Control 1":
                            ejecLabels(new[] { label18, label19, label20, label21, label22 },
                                sumaValores, countLechones, promedio, desviacionEstandar, coeficienVariacion);
                            break;

                        case "Control 2":
                            ejecLabels(new[] { label23, label24, label25, label26, label27 },
                                sumaValores, countLechones, promedio, desviacionEstandar, coeficienVariacion);
                            break;

                        case "Destete":
                            ejecLabels(new[] { label28, label29, label30, label31, label32 },
                                sumaValores, countLechones, promedio, desviacionEstandar, coeficienVariacion);
                            break;
                    }
                }
            };
            estilosDatagridview(dataGridView2, true, 201);
            dataGridView1.KeyDown += pasarCelda;
            dataGridView2.KeyDown += pasarCelda;
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
                        valores.destete, valores.fechaDeste?.ToString("dd/MM/yyyy"), valores.camaDona, valores.camaRece, valores.observaciones);
                }
            }
            altoFila(dataGridView2, EventArgs.Empty);
        }
        private void ejecLabels(Label[] listaLabels, float sumaValor, int countLech, float promedio, float desvia, float coefic)
        {
            listaLabels[0].Text = $"{sumaValor:0.000} kg";
            listaLabels[1].Text = $"{countLech}";
            listaLabels[2].Text = $"{promedio:0.000} kg";
            listaLabels[3].Text = $"{desvia:0.000} kg";
            listaLabels[4].Text = $"{coefic:0.000} %";
            foreach (Label lbl in listaLabels) lbl.ForeColor = Color.Black;     
        }
        private void pasarCelda(object sender, KeyEventArgs e)
        {
            DataGridView dataEstamos = (DataGridView)sender;
            if (dataEstamos == null) return;
            if (dataEstamos.CurrentCell == null) return;

            int countColumns = dataEstamos.Columns.Count;
            int filaEstamos = dataEstamos.CurrentCell.RowIndex;
            int columnaEstamos = dataEstamos.CurrentCell.ColumnIndex;

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (columnaEstamos < countColumns - 1)
                {
                    dataEstamos.CurrentCell = dataEstamos.Rows[filaEstamos].Cells[columnaEstamos + 1];                                                   
                }
            }
        }
        private void diasReturn(DataGridViewRow fila)
        {
            if (DateTime.TryParse(fila.Cells["F. IA"].Value?.ToString(), out DateTime fechaIAA) &&
                DateTime.TryParse(fila.Cells["Parto Real"].Value?.ToString(), out DateTime fechaPart))
            {
                int restaDias = (fechaPart - fechaIAA).Days;
                fila.Cells["Tmp Gest."].Value = restaDias;
            }
        }
        private void horasReturn(DataGridViewRow fila)
        {
            if (TimeSpan.TryParse(fila.Cells["H. ini. parto"].Value?.ToString(), out TimeSpan horaInicio) &&
                TimeSpan.TryParse(fila.Cells["H. fin parto"].Value?.ToString(), out TimeSpan horaFin))
            {
                TimeSpan diferencia = horaFin - horaInicio;
                fila.Cells["Tmp parto"].Value = diferencia.ToString(@"hh\:mm");
            }
        }


        string[] vari60 = new string[] {"Sexo", "Pezon I", "Pezon D", "NP", "Tmp Gest.", "Tmp parto", 
            "Camada Donante", "Camada Recep.", "H. fin parto", "H. ini. parto"};

        string[] vari90 = new string[] { "Fecha Ingreso", "Fecha Salida", "Parto Calc", "Peso Nacim.", "Peso Ingreso", "Peso Salida",
            "Parto Real", "Ind. Parto", "Macho Nro", "F. IA", "Marrana Nro", "D pp", "Fecha", "Nro lechón", "Raza", 
            "Raza Macho", "Control 1", "Control 2", "Destete"};
        private void estilosDatagridview(DataGridView data, bool estado = false, int? posiciYY = null)
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
            data.ColumnHeadersHeight = 52;
            data.Width = estado ? 1530 : data.Width;          
            data.Location = new Point(estado ? (this.ClientSize.Width - data.Width) / 2 : data.Location.X, posiciYY ?? data.Location.Y);          
            data.RowTemplate.Height = (!estado) ? 30 : 27;
            data.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#334155");
            data.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F0EDED");
            data.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
       
            foreach (DataGridViewColumn column in data.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;             
                if (vari60.Contains(column.HeaderText))
                {
                    column.Width = 64;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
                else if(vari90.Contains(column.HeaderText))
                {
                    column.Width = 90;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                column.ReadOnly = column.HeaderText == "Parto Calc" || column.HeaderText == "Tmp Gest." 
                    || column.HeaderText == "Marrana Nro" || column.HeaderText == "Tmp parto";
            }
        }
        private void altoFila(object sender, EventArgs e)
        {
            DataGridView data = (DataGridView)sender;
            if (data == null) return;

            int cantidadFilas = data.Rows.Count;
            int altoDatagrid = data.ColumnHeadersHeight + (cantidadFilas * data.RowTemplate.Height);
            const int ALTURA_MAXIMA = 500; // Cambia este valor según tu formulario
            data.Height = Math.Min(altoDatagrid, ALTURA_MAXIMA);

            int columnaPeso = data.Columns["Pezon D"].Index;
            int columnaNaci = data.Columns["Peso Ingreso"].Index;
            int columnaPesoSal = data.Columns["Peso Salida"].Index;
            int control1 = data.Columns["Control 1"].Index;
            int control2 = data.Columns["Control 2"].Index;
            int destete = data.Columns["Destete"].Index;
            Rectangle rect1 = data.GetCellDisplayRectangle(columnaPeso, -1, true);
            Rectangle rect2 = data.GetCellDisplayRectangle(columnaNaci, -1, true);
            Rectangle rect3 = data.GetCellDisplayRectangle(columnaPesoSal, -1, true);
            Rectangle cont1 = data.GetCellDisplayRectangle(control1, -1, true);
            Rectangle cont2 = data.GetCellDisplayRectangle(control2, -1, true);
            Rectangle deste = data.GetCellDisplayRectangle(destete, -1, true);

            label1.Left = 389; label1.Top = data.Bottom + 20;
            label3.Left = 330; label3.Top = label1.Bottom + 10;
            label5.Left = 388; label5.Top = label3.Bottom + 10;
            label9.Left = 317; label9.Top = label5.Bottom + 10;
            label11.Left = 312; label11.Top = label9.Bottom + 10;

            label6.Left = data.Left + rect2.Left; label6.Top = data.Bottom + 20;
            label7.Left = data.Left + rect2.Left; label7.Top = label6.Bottom + 10;
            label8.Left = data.Left + rect2.Left; label8.Top = label7.Bottom + 10;
            label10.Left = data.Left + rect2.Left; label10.Top = label8.Bottom + 10;
            label12.Left = data.Left + rect2.Left; label12.Top = label10.Bottom + 10;

            label13.Left = data.Left + rect3.Left - 5; label13.Top = data.Bottom + 20;
            label14.Left = data.Left + rect3.Left - 5; label14.Top = label13.Bottom + 10;
            label15.Left = data.Left + rect3.Left - 5; label15.Top = label14.Bottom + 10;
            label16.Left = data.Left + rect3.Left - 5; label16.Top = label15.Bottom + 10;
            label17.Left = data.Left + rect3.Left - 5; label17.Top = label16.Bottom + 10;

            label18.Left = data.Left + cont1.Left - 5; label18.Top = data.Bottom + 20;
            label19.Left = data.Left + cont1.Left - 5; label19.Top = label18.Bottom + 10;
            label20.Left = data.Left + cont1.Left - 5; label20.Top = label19.Bottom + 10;
            label21.Left = data.Left + cont1.Left - 5; label21.Top = label20.Bottom + 10;
            label22.Left = data.Left + cont1.Left - 5; label22.Top = label21.Bottom + 10;

            label23.Left = data.Left + cont2.Left - 5; label23.Top = data.Bottom + 20;
            label24.Left = data.Left + cont2.Left - 5; label24.Top = label23.Bottom + 10;
            label25.Left = data.Left + cont2.Left - 5; label25.Top = label24.Bottom + 10;
            label26.Left = data.Left + cont2.Left - 5; label26.Top = label25.Bottom + 10;
            label27.Left = data.Left + cont2.Left - 5; label27.Top = label26.Bottom + 10;

            label28.Left = data.Left + deste.Left - 5; label28.Top = data.Bottom + 20;
            label29.Left = data.Left + deste.Left - 5; label29.Top = label28.Bottom + 10;
            label30.Left = data.Left + deste.Left - 5; label30.Top = label29.Bottom + 10;
            label31.Left = data.Left + deste.Left - 5; label31.Top = label30.Bottom + 10;
            label32.Left = data.Left + deste.Left - 5; label32.Top = label31.Bottom + 10;
        }
        private void agregarFila_Click(object sender, EventArgs e)
        {        
            dataGridView2.Rows.Add();
            altoFila(dataGridView2, EventArgs.Empty);
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
                string transferencia, DateTime? fechaTrans, string destete, DateTime? fechaDest, string camadaDonan, string camadaRecep,
                string control1, string control2, string destet, string observaciones)>();

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
                string camadaDonan = fila.Cells[9].Value?.ToString()?.Trim();
                string camadaRecep = fila.Cells[10].Value?.ToString()?.Trim();
                string control1 = fila.Cells[11].Value?.ToString()?.Trim();
                string control2 = fila.Cells[12].Value?.ToString()?.Trim();
                string destet = fila.Cells[13].Value?.ToString()?.Trim();
                string observaciones = fila.Cells[14].Value?.ToString()?.Trim();

                valoresIngreso.Add((numeroLechon, sexo, pezonIzquierdo, pezonDerecho, nacimiento, transferencia, fechaTrans, 
                    destete, fechaDest, camadaDonan, camadaRecep, control1, control2, destet, observaciones));
            }
            if (conection.insertarLechonesBD(valoresIngreso, nameMarrana.Trim().ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Se guardó exitosamente");
            }
        }
    }
}
