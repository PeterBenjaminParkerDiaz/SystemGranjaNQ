using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace granjaAplicativo
{
    public partial class gestacion : Form
    {
        conexionBaseDatos conect = new conexionBaseDatos();
        registroMarrana nuevoregis = null;
        List<Tuple<string, string>> listasTraidas = null;
        public gestacion()
        {
            InitializeComponent();
            dataGridView1.Columns.Add("Nombres", "Nombres");
            estilosData(dataGridView1);
            listasTraidas = conect.listaNombres().OrderBy(op => op.Item1).ToList();
            if (listasTraidas.Count > 0)
            {
                foreach (var item in listasTraidas)
                {
                    string nombre = item.Item1;
                    string codigo = item.Item2;
                    if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(codigo)) continue;

                    int Filita = dataGridView1.Rows.Add(item.Item1);
                    dataGridView1.Rows[Filita].Tag = item.Item2;

                    //Agregamos al comboBox para eliminar
                    comboBox1.Items.Add(nombre);
                }
            }
            dataGridView1.CellClick += ejecutarMarrana;
        }

        private Form formulario;
        private void ejecutarMarrana(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string codigoFila = dataGridView1.Rows[e.RowIndex].Tag?.ToString();
            if (string.IsNullOrWhiteSpace(codigoFila)) return;

            if (formulario != null && !formulario.IsDisposed)
            {
                if (formulario.Tag?.ToString() == codigoFila)
                {
                    if (formulario.WindowState == FormWindowState.Minimized)
                        formulario.WindowState = FormWindowState.Normal;
                    formulario.BringToFront();
                    formulario.Focus();
                    return;
                }
                formulario.Close();
            }
            formulario = new Form();
            formulario.Size = new Size(405, 130);
            formulario.Tag = codigoFila;
            formulario.Owner = this;
            formulario.ShowIcon = false;
            formulario.ShowInTaskbar = false;
            formulario.StartPosition = FormStartPosition.CenterScreen;
            formulario.MaximizeBox = false;
            formulario.BackColor = Color.White;
            formulario.FormBorderStyle = FormBorderStyle.FixedSingle;

            string nombreMarrana = dataGridView1.Rows[e.RowIndex].Cells["Nombres"].Value?.ToString().Trim();

            Label titulo = new Label();
            titulo.Text = nombreMarrana;
            titulo.AutoSize = true;
            titulo.ForeColor = ColorTranslator.FromHtml("#991600");
            titulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            titulo.Location = new Point((formulario.Width - titulo.Width) / 2, 5);
            formulario.Controls.Add(titulo);

            string[] listaBotones = new string[] { "Registro de camadas", "Tratamiento" };
            int xx = 10;
            foreach (var nombres in listaBotones)
            {
                Button btn = new Button();
                btn.Text = nombres;
                btn.Tag = codigoFila;
                btn.Size = new Size(180, 40);
                btn.Click +=(es, er)=> funcionBotones(es, er, nombreMarrana);
                btn.Location = new Point(xx, 40);
                formulario.Controls.Add(btn);
                xx = btn.Left + btn.Width + 10;
            }
            formulario.Show();
        }
        private void funcionBotones(object sender, EventArgs e, string nombreMarana)
        {
            Button boton = sender as Button;
            if (boton == null) return;
            Form formpadre = (Form)boton.Parent;
            if (formpadre == null) return;
            string codigoBoton = boton.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(codigoBoton)) return;

            if (boton.Text == "Registro de camadas")
            {
                if (nuevoregis != null && !nuevoregis.IsDisposed)
                {
                    if (nuevoregis.Tag?.ToString() == codigoBoton)
                    {
                        if (nuevoregis.WindowState == FormWindowState.Minimized)
                            nuevoregis.WindowState = FormWindowState.Maximized;

                        nuevoregis.BringToFront();
                        nuevoregis.Activate();
                        return;
                    }
                    nuevoregis.Close();
                }
                formpadre.Close();
                nuevoregis = new registroMarrana(nombreMarana);
                nuevoregis.Tag = codigoBoton;
                nuevoregis.Show();
            }
        }
        private void estilosData(DataGridView dataGrid)
        {
            dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGrid.GridColor = Color.LightGray;
            dataGrid.ReadOnly = true;
            dataGrid.EnableHeadersVisualStyles = false;
            dataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            dataGrid.DefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 242, 240);
            dataGrid.RowHeadersVisible = false;
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.AllowUserToResizeColumns = false;
            dataGrid.AllowUserToAddRows = false;
            dataGrid.ColumnHeadersHeight = 35;
            dataGrid.RowTemplate.Height = 30;
            foreach (DataGridViewColumn col in dataGrid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        private void CrearMarrana_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre o identificacion de la marrana", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string valorIngresado = textBox1.Text.Trim();
            string codigofila = "m_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            //verifiamos que sea un nombre válido
            string regex = "^[a-zA-Z0-9]+(?: [a-zA-Z0-9]+)*$";
            if(!Regex.IsMatch(valorIngresado, regex))
            {
                MessageBox.Show("El nombre ingresado no es válido.\n\n" +"Solo se permiten letras, números y espacios entre palabras.\n" +
                        "No se permiten símbolos especiales.", "Nombre no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //Evitamos que se dupliquen las mismas marranas
            if (conect.verificarExisteTabla(valorIngresado.ToLower().Replace(" ", "_")))
            {
                MessageBox.Show("Esta marrana ya esta registrada, ingrese un nombre o identificación diferente", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Insertamos la marrana
            if (conect.insertarMarrana(valorIngresado, codigofila))
            {
                int posicion = 0;
                foreach (DataGridViewRow fila in dataGridView1.Rows)
                {
                    if (string.Compare(valorIngresado, fila.Cells[0].Value?.ToString(), true) < 0)
                    {
                        break;
                    }
                    posicion++;
                }
                //Insertamos la fila en el datagridview
                dataGridView1.Rows.Insert(posicion, valorIngresado);
                dataGridView1.Rows[posicion].Tag = codigofila;

                //Insertamos en el comboBox.
                comboBox1.Items.Insert(posicion, valorIngresado);

                //Insertamos en la lista
                List<Tuple<string, string>> valorINgre = new List<Tuple<string, string>>() { 
                    new Tuple<string, string>(valorIngresado, codigofila )};
                listasTraidas.Insert(posicion, new Tuple<string, string>(valorIngresado, codigofila));
                textBox1.Text = "";
            }
        }

        private void eliminarMarrana_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string nombreMarrana = comboBox1.SelectedItem.ToString();
            DialogResult deseaEliminar = MessageBox.Show(
                $"ADVERTENCIA\n\n" +
                $"Está a punto de eliminar la marrana: {nombreMarrana}\n\n" +
                "Se perderá TODO su historial, registros y datos de gestación.\n\n" +
                "Esta acción es permanente y no se puede deshacer.\n\n" +
                "¿Desea continuar?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (deseaEliminar == DialogResult.Yes)
            {
                int indice = comboBox1.SelectedIndex;
                string codigoEliminarItem = listasTraidas[indice].Item2;
                if (string.IsNullOrWhiteSpace(codigoEliminarItem)) return;

                if (conect.eliminarMarrana(codigoEliminarItem, nombreMarrana.Trim().ToLower().Replace(" ", "_")))
                {
                    DataGridViewRow filaEncon = dataGridView1.Rows.Cast<DataGridViewRow>()
                        .FirstOrDefault(op => op.Tag?.ToString() == codigoEliminarItem);

                    if (filaEncon != null)
                        dataGridView1.Rows.Remove(filaEncon);

                    comboBox1.Items.RemoveAt(indice);
                    listasTraidas.RemoveAt(indice);

                }
            }
        }
    }
}
