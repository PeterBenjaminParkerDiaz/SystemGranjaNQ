using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace granjaAplicativo
{
    public partial class registroMarrana : Form
    {
        public registroMarrana()
        {
            InitializeComponent();
        }

        private void registroMarrana_Load(object sender, EventArgs e)
        {
            label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, 0);

            dataGridView1.Columns.Add("Marrana Nro", "Marrana Nro");
            dataGridView1.Columns.Add("Raza", "Raza");
            dataGridView1.Columns.Add("NP", "NP");
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
            dataGridView1.Columns.Add("Macho Nro", "Macho Nro");
            dataGridView1.Columns.Add("Raza", "Raza");

            DateTimePicker fechaServic = new DateTimePicker();
            fechaServic.Format = DateTimePickerFormat.Short;
            fechaServic.Visible = false;
            fechaServic.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(fechaServic);
            dataGridView1.Columns.Add("Fecha Servicio", "Fecha Servicio");

            DateTimePicker horaInicio = new DateTimePicker();
            horaInicio.Format = DateTimePickerFormat.Custom;
            horaInicio.CustomFormat = "HH:mm:ss"; // o "hh:mm tt" si quieres AM/PM
            horaInicio.ShowUpDown = true;
            horaInicio.Visible = false;
            horaInicio.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(horaInicio);
            dataGridView1.Columns.Add("Hora inicio parto", "Hora inicio parto");

            DateTimePicker horaFinal = new DateTimePicker();
            horaFinal.Format = DateTimePickerFormat.Custom;
            horaFinal.CustomFormat = "HH:mm:ss"; // o "hh:mm tt" si quieres AM/PM
            horaFinal.ShowUpDown = true;
            horaFinal.Visible = false;
            horaFinal.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGridView1.Controls.Add(horaFinal);
            dataGridView1.Columns.Add("Hora fin parto", "Hora fin parto");

            dataGridView1.Columns.Add("Nro Paridera", "Nro Paridera");
            estilosDatagridview(dataGridView1);
            dataGridView1.Rows.Add();

            dataGridView1.CellBeginEdit += (es, er) =>
            {
                DateTimePicker cualDioCli = null;

                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["Parto Calc"].Index)
                    {
                        cualDioCli = fechaCalcu; // DateTimePicker de fecha
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        cualDioCli = fechaRealParto; // DateTimePicker de fecha
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha Servicio"].Index)
                    {
                        cualDioCli = fechaServic; // DateTimePicker de fecha
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora inicio parto"].Index)
                    {
                        cualDioCli = horaInicio; // DateTimePicker de hora
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora fin parto"].Index)
                    {
                        cualDioCli = horaFinal; // DateTimePicker de hora
                    }
                }

                if (cualDioCli != null)
                {
                    Rectangle rectangulo = dataGridView1.GetCellDisplayRectangle(er.ColumnIndex, er.RowIndex, true);
                    cualDioCli.Size = rectangulo.Size;
                    cualDioCli.Location = rectangulo.Location;
                    cualDioCli.Visible = true;
                    if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
                    {
                        cualDioCli.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                    }
                    else
                    {
                        cualDioCli.Value = DateTime.Now;
                    }
                }
            };
            dataGridView1.CellEndEdit += (es, er) =>
            {
                if (er.RowIndex >= 0)
                {
                    if (er.ColumnIndex == dataGridView1.Columns["Parto Calc"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = fechaCalcu.Value.ToShortDateString();
                        fechaCalcu.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Parto Real"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = fechaRealParto.Value.ToShortDateString();
                        fechaRealParto.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Fecha Servicio"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = fechaServic.Value.ToShortDateString();
                        fechaServic.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora inicio parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = horaInicio.Value.ToString("HH:mm:ss");
                        horaInicio.Visible = false;
                    }
                    else if (er.ColumnIndex == dataGridView1.Columns["Hora fin parto"].Index)
                    {
                        dataGridView1.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = horaFinal.Value.ToString("HH:mm:ss");
                        horaFinal.Visible = false;
                    }
                }
            };

            NumericUpDown numeroLech = new NumericUpDown();
            numeroLech.Minimum = 0;
            numeroLech.Maximum = 120;
            numeroLech.Visible = false;
            numeroLech.Font = new Font("Segoe UI", 11F);
            dataGridView2.Controls.Add(numeroLech);
            dataGridView2.Columns.Add("Nro lechón", "Nro lechón");

            DataGridViewComboBoxColumn sexo = new DataGridViewComboBoxColumn();
            sexo.Name = "Sexo";
            sexo.HeaderText = "Sexo";
            sexo.Items.AddRange("Hembra", "Macho");
            dataGridView2.Columns.Add(sexo);

            NumericUpDown numDerecho = new NumericUpDown();
            numDerecho.Minimum = 0;
            numDerecho.Maximum = 20;
            numDerecho.Visible = false;
            numDerecho.Font = new Font("Segoe UI", 11F);
            dataGridView2.Controls.Add(numDerecho);
            dataGridView2.Columns.Add("Pezon I", "Pezon I");

            NumericUpDown numIzquierdo = new NumericUpDown();
            numIzquierdo.Minimum = 0;
            numIzquierdo.Maximum = 20;
            numIzquierdo.Visible = false;
            numIzquierdo.Font = new Font("Segoe UI", 11F);
            dataGridView2.Controls.Add(numIzquierdo);
            dataGridView2.Columns.Add("Pezon D", "Pezon D");

            dataGridView2.Columns.Add("Peso Nacim.", "Peso Nacim.");
            dataGridView2.Columns.Add("Peso Transf", "Peso Transf");
            dataGridView2.Columns.Add("Peso Destete", "Peso Destete");
            dataGridView2.Columns.Add("Observaciones", "Observaciones");

            dataGridView2.CellBeginEdit += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                Rectangle rect = dataGridView2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                if (e.ColumnIndex == dataGridView2.Columns["Nro lechón"].Index)
                {
                    numeroLech.Bounds = rect;
                    numeroLech.Visible = true;
                    numeroLech.Value = decimal.TryParse(dataGridView2.CurrentCell.Value?.ToString(), out decimal val) ? val : 0;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Pezon I"].Index)
                {
                    numIzquierdo.Bounds = rect;
                    numIzquierdo.Visible = true;
                    numIzquierdo.Value = decimal.TryParse(dataGridView2.CurrentCell.Value?.ToString(), out decimal val) ? val : 0;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Pezon D"].Index)
                {
                    numDerecho.Bounds = rect;
                    numDerecho.Visible = true;
                    numDerecho.Value = decimal.TryParse(dataGridView2.CurrentCell.Value?.ToString(), out decimal val) ? val : 0;
                }
            };
            dataGridView2.CellEndEdit += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (e.ColumnIndex == dataGridView2.Columns["Nro lechón"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = numeroLech.Value;
                    numeroLech.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Pezon I"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = numIzquierdo.Value;
                    numIzquierdo.Visible = false;
                }
                else if (e.ColumnIndex == dataGridView2.Columns["Pezon D"].Index)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = numDerecho.Value;
                    numDerecho.Visible = false;
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
                ("Día 1", "Ingesta de calostro, pesaje, limpieza, desinfección de ombligo, corte de colmillos y descole."),
                ("Día 3", "Aplicación de hierro para prevenir anemia."),
                ("Día 7", "Suplemento alimenticio (Piggy Milk / Baby Pig)."),
                ("", "Destete con control de peso.")
            };
            foreach (var item in listaValores)
            {
                dataGridView3.Rows.Add(item.Item1,"",item.Item2);
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
                if(er.RowIndex >= 0 && er.ColumnIndex == dataGridView3.Columns["Fecha"].Index)
                {
                    dataGridView3.Rows[er.RowIndex].Cells[er.ColumnIndex].Value = fechIngresami.Value.ToShortDateString();
                    fechIngresami.Visible = false;
                }
            };
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
            data.Width = estado ? 1082 : data.Width;
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
    }
}
