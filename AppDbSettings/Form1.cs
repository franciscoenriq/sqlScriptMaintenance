using System.Collections.Generic;
using System.Windows.Forms;
using System;
namespace AppDbSettings
{
    public partial class Form1 : Form
    {
        private ConectorDB conector;
        private CheckedListBox checkedListBoxBDs;
        private Button btnGuardarSeleccion;
        private ListBox listBdsExistentes;
        private Label labelBdsenSql;
        private CheckBox btnSeleccionarTodos;
        private CheckBox btnSeleccionarTodos_semanal;
        private CheckedListBox checkedListBoxBDs_semanal;
        private TabControl tabControl;
        private Button btnGuardarSeleccion_mantenimiento_semanal;
        private ListBox listBdsExistentes_semanal;
        private Label labelBdsenSql_semanal;
        private Label labelBdDisponibles_semanal;
        private PictureBox pictureBoxLogo;
        private PictureBox pictureBoxLogo2; 


        public Form1()
        {
            InitializeComponent();
            conector = new ConectorDB();
            CargarBasesDeDatos();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            checkedListBoxBDs_semanal = new CheckedListBox();
            labelBdDisponibles = new Label();
            checkedListBoxBDs = new CheckedListBox();
            btnGuardarSeleccion = new Button();
            btnGuardarSeleccion_mantenimiento_semanal = new Button();
            listBdsExistentes = new ListBox();
            listBdsExistentes_semanal = new ListBox();
            labelBdsenSql = new Label();
            btnSeleccionarTodos = new CheckBox();
            btnSeleccionarTodos_semanal = new CheckBox();
            tabControl = new TabControl();
            tabPage1 = new TabPage();
            pictureBoxLogo = new PictureBox();
            tabPage2 = new TabPage();
            labelBdDisponibles_semanal = new Label();
            labelBdsenSql_semanal = new Label();
            tabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // checkedListBoxBDs_semanal
            // 
            checkedListBoxBDs_semanal.FormattingEnabled = true;
            checkedListBoxBDs_semanal.Location = new Point(11, 177);
            checkedListBoxBDs_semanal.Name = "checkedListBoxBDs_semanal";
            checkedListBoxBDs_semanal.Size = new Size(232, 148);
            checkedListBoxBDs_semanal.TabIndex = 0;
            checkedListBoxBDs_semanal.SelectedIndexChanged += checkedListBoxBDs_semanal_SelectedIndexChanged;
            // 
            // labelBdDisponibles
            // 
            labelBdDisponibles.Location = new Point(12, 50);
            labelBdDisponibles.Name = "labelBdDisponibles";
            labelBdDisponibles.Size = new Size(244, 55);
            labelBdDisponibles.TabIndex = 0;
            labelBdDisponibles.Text = "Estas son las BD a las que puedes hacerles BackUp:";
            labelBdDisponibles.Click += labelBdDisponibles_Click;
            // 
            // checkedListBoxBDs
            // 
            checkedListBoxBDs.FormattingEnabled = true;
            checkedListBoxBDs.Location = new Point(11, 177);
            checkedListBoxBDs.Name = "checkedListBoxBDs";
            checkedListBoxBDs.Size = new Size(232, 148);
            checkedListBoxBDs.TabIndex = 0;
            checkedListBoxBDs.SelectedIndexChanged += checkedListBoxBDs_SelectedIndexChanged;
            // 
            // btnGuardarSeleccion
            // 
            btnGuardarSeleccion.Location = new Point(161, 361);
            btnGuardarSeleccion.Name = "btnGuardarSeleccion";
            btnGuardarSeleccion.Size = new Size(300, 36);
            btnGuardarSeleccion.TabIndex = 1;
            btnGuardarSeleccion.Text = "Guardar selección en SQL";
            btnGuardarSeleccion.Click += BtnGuardarSeleccion_Click;
            // 
            // btnGuardarSeleccion_mantenimiento_semanal
            // 
            btnGuardarSeleccion_mantenimiento_semanal.Location = new Point(161, 361);
            btnGuardarSeleccion_mantenimiento_semanal.Name = "btnGuardarSeleccion_mantenimiento_semanal";
            btnGuardarSeleccion_mantenimiento_semanal.Size = new Size(300, 36);
            btnGuardarSeleccion_mantenimiento_semanal.TabIndex = 1;
            btnGuardarSeleccion_mantenimiento_semanal.Text = "Guardar selección en SQL";
            btnGuardarSeleccion_mantenimiento_semanal.Click += BtnGuardarSeleccion_semanal_Click;
            // 
            // listBdsExistentes
            // 
            listBdsExistentes.FormattingEnabled = true;
            listBdsExistentes.Location = new Point(366, 177);
            listBdsExistentes.Name = "listBdsExistentes";
            listBdsExistentes.Size = new Size(221, 154);
            listBdsExistentes.TabIndex = 1;
            // 
            // listBdsExistentes_semanal
            // 
            listBdsExistentes_semanal.FormattingEnabled = true;
            listBdsExistentes_semanal.Location = new Point(366, 177);
            listBdsExistentes_semanal.Name = "listBdsExistentes_semanal";
            listBdsExistentes_semanal.Size = new Size(221, 154);
            listBdsExistentes_semanal.TabIndex = 1;
            // 
            // labelBdsenSql
            // 
            labelBdsenSql.Location = new Point(366, 50);
            labelBdsenSql.Name = "labelBdsenSql";
            labelBdsenSql.Size = new Size(221, 73);
            labelBdsenSql.TabIndex = 0;
            labelBdsenSql.Text = "Estas son las Bd que ya estan en el plan de backups:";
            // 
            // btnSeleccionarTodos
            // 
            btnSeleccionarTodos.AutoSize = true;
            btnSeleccionarTodos.Location = new Point(12, 117);
            btnSeleccionarTodos.Name = "btnSeleccionarTodos";
            btnSeleccionarTodos.Size = new Size(199, 19);
            btnSeleccionarTodos.TabIndex = 2;
            btnSeleccionarTodos.Text = "Seleccionar/Deseleccionar Todos";
            btnSeleccionarTodos.Click += BtnSeleccionarTodos_Click;
            // 
            // btnSeleccionarTodos_semanal
            // 
            btnSeleccionarTodos_semanal.AutoSize = true;
            btnSeleccionarTodos_semanal.Location = new Point(12, 117);
            btnSeleccionarTodos_semanal.Name = "btnSeleccionarTodos_semanal";
            btnSeleccionarTodos_semanal.Size = new Size(199, 19);
            btnSeleccionarTodos_semanal.TabIndex = 2;
            btnSeleccionarTodos_semanal.Text = "Seleccionar/Deseleccionar Todos";
            btnSeleccionarTodos_semanal.Click += BtnSeleccionarTodos_semanal_Click;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.Location = new Point(2, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(756, 559);
            tabControl.TabIndex = 3;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Image = (Image)resources.GetObject("pictureBoxLogo.Image");
            pictureBoxLogo.Location = new Point(525, -11);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(220, 58);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(pictureBoxLogo);
            tabPage1.Controls.Add(labelBdDisponibles);
            tabPage1.Controls.Add(checkedListBoxBDs);
            tabPage1.Controls.Add(btnGuardarSeleccion);
            tabPage1.Controls.Add(listBdsExistentes);
            tabPage1.Controls.Add(labelBdsenSql);
            tabPage1.Controls.Add(btnSeleccionarTodos);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(748, 531);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Plan Mantenimiento Backups";
            
            // 
            // pictureBoxLogo2
            // 
            
            // 
            // tabPage2
            // 
           
            tabPage2.Controls.Add(labelBdDisponibles_semanal);
            tabPage2.Controls.Add(labelBdsenSql_semanal);
            tabPage2.Controls.Add(checkedListBoxBDs_semanal);
            tabPage2.Controls.Add(btnGuardarSeleccion_mantenimiento_semanal);
            tabPage2.Controls.Add(listBdsExistentes_semanal);
            tabPage2.Controls.Add(btnSeleccionarTodos_semanal);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(652, 422);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Plan Mantenimiento";
            // 
            // labelBdDisponibles_semanal
            // 
            labelBdDisponibles_semanal.Location = new Point(12, 50);
            labelBdDisponibles_semanal.Name = "labelBdDisponibles_semanal";
            labelBdDisponibles_semanal.Size = new Size(244, 55);
            labelBdDisponibles_semanal.TabIndex = 0;
            labelBdDisponibles_semanal.Text = "Estas son las BD que puedes añadir al plan de mantenimiento:";
            // 
            // labelBdsenSql_semanal
            // 
            labelBdsenSql_semanal.Location = new Point(366, 50);
            labelBdsenSql_semanal.Name = "labelBdsenSql_semanal";
            labelBdsenSql_semanal.Size = new Size(221, 73);
            labelBdsenSql_semanal.TabIndex = 0;
            labelBdsenSql_semanal.Text = "Estas son las Bd que ya estan en el plan de mantenimiento:";
            // 
            // Form1
            // 
            ClientSize = new Size(770, 574);
            Controls.Add(tabControl);
            
            Name = "Form1";
            Text = "Seleccionar Bases de Datos";
            tabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        private void CargarBasesDeDatos()
        {
            List<string> basesDeDatos = conector.ConectarYConsultar(); // Obtener bases de datos

            if (basesDeDatos.Count > 0)
            {
                checkedListBoxBDs.Items.Clear(); // Limpiar lista antes de agregar elementos
                checkedListBoxBDs_semanal.Items.Clear();
                foreach (string bd in basesDeDatos)
                {
                    checkedListBoxBDs.Items.Add(bd); // Agregar bases de datos a la lista
                    checkedListBoxBDs_semanal.Items.Add(bd);
                }

                //ahora cargamos las bd existentes 
                List<string> basesExistentes = ReadAndWriteSqlScript.ObtenerBDs(AppDbSettings.SettingsApp.Default.rutaBase + "\\" + AppDbSettings.SettingsApp.Default.archivoBackup); // Obtener bases de datos existentes
                List<string> basesExistentesSemanal = ReadAndWriteSqlScript.ObtenerBDs(AppDbSettings.SettingsApp.Default.rutaBase + "\\" + AppDbSettings.SettingsApp.Default.archivoMantenimiento);
                listBdsExistentes.Items.Clear(); // Limpiar lista de bases de datos existentes
                listBdsExistentes_semanal.Items.Clear();
                foreach (string bdExistente in basesExistentes)
                {
                    listBdsExistentes.Items.Add(bdExistente); // Agregar bases de datos ya existentes
                }
                foreach(string bdExistente in basesExistentesSemanal)
                {
                    listBdsExistentes_semanal.Items.Add(bdExistente);
                }
            }
            else
            {
                MessageBox.Show("No se encontraron bases de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void BtnGuardarSeleccion_Click(object sender, EventArgs e)
        {
            List<string> basesSeleccionadas = new List<string>();

            foreach (var item in checkedListBoxBDs.CheckedItems)
            {
                basesSeleccionadas.Add(item.ToString());
            }

            if (basesSeleccionadas.Count > 0)
            {
                string rutaSql = AppDbSettings.SettingsApp.Default.rutaBase + "\\" + AppDbSettings.SettingsApp.Default.archivoBackup;

                // Llamar al método de la clase ReadAndWriteSqlScript para escribir las bases seleccionadas
                ReadAndWriteSqlScript.EscribirBasesDeDatos(rutaSql, basesSeleccionadas);
                // Refrescar la lista de bases de datos existentes
                CargarBasesDeDatos();
                // dejamos el chekbox de selecionar todo apagado 
                btnSeleccionarTodos.Checked = false;

                MessageBox.Show("Bases de datos actualizadas en el script SQL correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Debes seleccionar al menos una base de datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void BtnGuardarSeleccion_semanal_Click(object sender, EventArgs e)
        {
            List<string> basesSeleccionadas = new List<string>();

            foreach (var item in checkedListBoxBDs_semanal.CheckedItems)
            {
                basesSeleccionadas.Add(item.ToString());
            }
            if (basesSeleccionadas.Count > 0)
            {
                string rutaSql = AppDbSettings.SettingsApp.Default.rutaBase + "\\" + AppDbSettings.SettingsApp.Default.archivoMantenimiento;

                // Llamar al método de la clase ReadAndWriteSqlScript para escribir las bases seleccionadas
                ReadAndWriteSqlScript.EscribirBasesDeDatos(rutaSql, basesSeleccionadas);
                // Refrescar la lista de bases de datos existentes
                CargarBasesDeDatos();
                // dejamos el chekbox de selecionar todo apagado 
                btnSeleccionarTodos.Checked = false;

                MessageBox.Show("Bases de datos actualizadas en el script SQL correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Debes seleccionar al menos una base de datos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void BtnSeleccionarTodos_Click(object sender, EventArgs e)
        {
            // Alternar selección
            bool marcar = btnSeleccionarTodos.Checked;

            for (int i = 0; i < checkedListBoxBDs.Items.Count; i++)
            {
                checkedListBoxBDs.SetItemChecked(i, marcar);
            }
        }
        private void BtnSeleccionarTodos_semanal_Click(object sender, EventArgs e)
        {
            // Alternar selección
            bool marcar = btnSeleccionarTodos_semanal.Checked;

            for (int i = 0; i < checkedListBoxBDs.Items.Count; i++)
            {
                checkedListBoxBDs_semanal.SetItemChecked(i, marcar);
            }
        }
        private void checkedListBoxBDs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void checkedListBoxBDs_semanal_SelectedIndexChanged(object sender, EventArgs e) { }

        private void labelBdDisponibles_Click(object sender, EventArgs e)
        {

        }
    }
}
