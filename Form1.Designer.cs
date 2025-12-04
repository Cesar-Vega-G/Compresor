namespace Compresor
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.ArchivoSeleccionado = new System.Windows.Forms.TextBox();
            this.archBuscar = new System.Windows.Forms.Button();
            this.comboAlgoritmo = new System.Windows.Forms.ComboBox();
            this.Compresor = new System.Windows.Forms.Button();
            this.Descompresor = new System.Windows.Forms.Button();
            this.Tiempo = new System.Windows.Forms.Label();
            this.Tasa = new System.Windows.Forms.Label();
            this.Memoria = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ArchivoSeleccionado
            // 
            this.ArchivoSeleccionado.Location = new System.Drawing.Point(12, 70);
            this.ArchivoSeleccionado.Name = "ArchivoSeleccionado";
            this.ArchivoSeleccionado.ReadOnly = true;
            this.ArchivoSeleccionado.Size = new System.Drawing.Size(500, 20);
            this.ArchivoSeleccionado.TabIndex = 0;
            // 
            // archBuscar
            // 
            this.archBuscar.Location = new System.Drawing.Point(528, 70);
            this.archBuscar.Name = "archBuscar";
            this.archBuscar.Size = new System.Drawing.Size(125, 23);
            this.archBuscar.TabIndex = 1;
            this.archBuscar.Text = "Buscar Archivo";
            this.archBuscar.UseVisualStyleBackColor = true;
            this.archBuscar.Click += new System.EventHandler(this.archBuscar_Click);
            // 
            // comboAlgoritmo
            // 
            this.comboAlgoritmo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAlgoritmo.FormattingEnabled = true;
            this.comboAlgoritmo.Location = new System.Drawing.Point(532, 99);
            this.comboAlgoritmo.Name = "comboAlgoritmo";
            this.comboAlgoritmo.Size = new System.Drawing.Size(121, 21);
            this.comboAlgoritmo.TabIndex = 2;
            // 
            // Compresor
            // 
            this.Compresor.Location = new System.Drawing.Point(138, 126);
            this.Compresor.Name = "Compresor";
            this.Compresor.Size = new System.Drawing.Size(92, 23);
            this.Compresor.TabIndex = 3;
            this.Compresor.Text = "Comprimir";
            this.Compresor.UseVisualStyleBackColor = true;
            this.Compresor.Click += new System.EventHandler(this.Compresor_Click_1);
            // 
            // Descompresor
            // 
            this.Descompresor.Location = new System.Drawing.Point(236, 126);
            this.Descompresor.Name = "Descompresor";
            this.Descompresor.Size = new System.Drawing.Size(96, 23);
            this.Descompresor.TabIndex = 4;
            this.Descompresor.Text = "Descomprimir";
            this.Descompresor.UseVisualStyleBackColor = true;
            this.Descompresor.Click += new System.EventHandler(this.Descompresor_Click_1);
            // 
            // Tiempo
            // 
            this.Tiempo.AutoSize = true;
            this.Tiempo.Location = new System.Drawing.Point(23, 191);
            this.Tiempo.Name = "Tiempo";
            this.Tiempo.Size = new System.Drawing.Size(42, 13);
            this.Tiempo.TabIndex = 5;
            this.Tiempo.Text = "Tiempo";
            this.Tiempo.Click += new System.EventHandler(this.label1_Click);
            // 
            // Tasa
            // 
            this.Tasa.AutoSize = true;
            this.Tasa.Location = new System.Drawing.Point(23, 256);
            this.Tasa.Name = "Tasa";
            this.Tasa.Size = new System.Drawing.Size(31, 13);
            this.Tasa.TabIndex = 6;
            this.Tasa.Text = "Tasa";
            this.Tasa.Click += new System.EventHandler(this.label2_Click);
            // 
            // Memoria
            // 
            this.Memoria.AutoSize = true;
            this.Memoria.Location = new System.Drawing.Point(23, 225);
            this.Memoria.Name = "Memoria";
            this.Memoria.Size = new System.Drawing.Size(47, 13);
            this.Memoria.TabIndex = 7;
            this.Memoria.Text = "Memoria";
            this.Memoria.Click += new System.EventHandler(this.label3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1321, 790);
            this.Controls.Add(this.Memoria);
            this.Controls.Add(this.Tasa);
            this.Controls.Add(this.Tiempo);
            this.Controls.Add(this.Descompresor);
            this.Controls.Add(this.Compresor);
            this.Controls.Add(this.comboAlgoritmo);
            this.Controls.Add(this.archBuscar);
            this.Controls.Add(this.ArchivoSeleccionado);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ArchivoSeleccionado;
        private System.Windows.Forms.Button archBuscar;
        private System.Windows.Forms.ComboBox comboAlgoritmo;
        private System.Windows.Forms.Button Compresor;
        private System.Windows.Forms.Button Descompresor;
        private System.Windows.Forms.Label Tiempo;
        private System.Windows.Forms.Label Tasa;
        private System.Windows.Forms.Label Memoria;
    }
}

