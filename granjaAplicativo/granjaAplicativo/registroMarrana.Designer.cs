namespace granjaAplicativo
{
    partial class registroMarrana
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            dataGridView2 = new DataGridView();
            agregarFila = new Button();
            label2 = new Label();
            button1 = new Button();
            label4 = new Label();
            GuardarCamadas = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.BackgroundColor = SystemColors.ButtonHighlight;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 46);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(806, 90);
            dataGridView1.TabIndex = 0;
            // 
            // dataGridView2
            // 
            dataGridView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            dataGridView2.BackgroundColor = SystemColors.ButtonHighlight;
            dataGridView2.BorderStyle = BorderStyle.None;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(12, 201);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(806, 462);
            dataGridView2.TabIndex = 1;
            // 
            // agregarFila
            // 
            agregarFila.Anchor = AnchorStyles.Top;
            agregarFila.Location = new Point(102, 169);
            agregarFila.Name = "agregarFila";
            agregarFila.Size = new Size(134, 26);
            agregarFila.TabIndex = 2;
            agregarFila.Text = "Agregar fila";
            agregarFila.UseVisualStyleBackColor = true;
            agregarFila.Click += agregarFila_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(7, 22);
            label2.Name = "label2";
            label2.Size = new Size(189, 21);
            label2.TabIndex = 5;
            label2.Text = "REGISTRO DE CAMADAS";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top;
            button1.Location = new Point(242, 169);
            button1.Name = "button1";
            button1.Size = new Size(134, 26);
            button1.TabIndex = 7;
            button1.Text = "Guardar cambios";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(12, 174);
            label4.Name = "label4";
            label4.Size = new Size(89, 21);
            label4.TabIndex = 10;
            label4.Text = "LECHONES";
            // 
            // GuardarCamadas
            // 
            GuardarCamadas.Anchor = AnchorStyles.Top;
            GuardarCamadas.Location = new Point(202, 17);
            GuardarCamadas.Name = "GuardarCamadas";
            GuardarCamadas.Size = new Size(134, 26);
            GuardarCamadas.TabIndex = 11;
            GuardarCamadas.Text = "Guardar cambios";
            GuardarCamadas.UseVisualStyleBackColor = true;
            GuardarCamadas.Click += GuardarCamadas_Click;
            // 
            // registroMarrana
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonHighlight;
            ClientSize = new Size(830, 675);
            Controls.Add(GuardarCamadas);
            Controls.Add(label4);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(agregarFila);
            Controls.Add(dataGridView2);
            Controls.Add(dataGridView1);
            Name = "registroMarrana";
            ShowIcon = false;
            Text = "registroMarrana";
            WindowState = FormWindowState.Maximized;
            Load += registroMarrana_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button agregarFila;
        private Label label2;
        private Button button1;
        private Label label4;
        private Button GuardarCamadas;
    }
}