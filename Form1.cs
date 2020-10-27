using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
//Para leer el registro
using Microsoft.Win32;

namespace Verificador_RRHH
{
    public partial class Form1 : Form
    {
        int fila_buscada = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void estado_inicial()
        {
            /***********************************************************
            string AccessDBAsValue = string.Empty;
            RegistryKey rkACDBKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes");
            if (rkACDBKey != null)
            {
                //int lnSubKeyCount = 0;
                //lnSubKeyCount =rkACDBKey.SubKeyCount; 
                foreach (string subKeyName in rkACDBKey.GetSubKeyNames())
                {                   
                    if (subKeyName.Contains("Microsoft.ACE.OLEDB"))
                    {
                        MessageBox.Show(subKeyName);
                    }
                    else if (subKeyName.Contains("Microsoft.Jet.OLEDB"))
                    {
                        MessageBox.Show(subKeyName);
                    }
                }
            }
            *********************************************************/

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            fila_buscada = 0;
            textBox1.Enabled = false;
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                

                OpenFileDialog importacion = new OpenFileDialog();
                importacion.InitialDirectory = @"Documentos";
                importacion.Title = "Buscar asistencias...";
                importacion.Filter = "Excel | *.xls;*.xlsx;";

                if (importacion.ShowDialog() == DialogResult.OK)
                {
                    button1.Text = "Importando...";
                    button1.Enabled = false;
                    estado_inicial();

                    dataGridView2.DataSource = ImportarDatos(importacion.FileName);
                    escribir_datos(dataGridView2);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error importar asistencias: " + error.Message, "Verificador de infracciones de asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        DataView ImportarDatos(string ruta) 
        {
            string conexion = "";
            
            try
            {
                conexion = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {0}; Extended Properties = 'Excel 12.0;'", ruta);
            }
            catch (Exception ex)
            {
                conexion = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source=" + ruta + "Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0" + Convert.ToChar(34).ToString() + ";");
            }

            OleDbConnection conector = new OleDbConnection(conexion);

            conector.Open();

            OleDbCommand consulta = new OleDbCommand("select * from [RAsistIndivObs$]", conector);
            OleDbDataAdapter adaptador = new OleDbDataAdapter
            {
                SelectCommand = consulta
            };
            DataSet ds = new DataSet();
            adaptador.Fill(ds);
            conector.Close();
            return ds.Tables[0].DefaultView;            
        }

        private void escribir_datos(DataGridView datos)
        {
            dataGridView1.Rows.Clear();
            fila_buscada = 0;
            textBox1.Enabled = false;
            button2.Enabled = false;
            
            string dia = "";
            string fecha = "";
            string horario_1er_periodo = "";
            string primera_entrada = "";
            string primera_salida = "";
            string horario_2do_periodo = "";
            string segunda_entrada = "";
            string segunda_salida = "";
            string obs_1ra_entrada = "";
            string obs_1ra_salida = "";
            string obs_2da_entrada = "";
            string obs_2da_salida = "";
            int minutos_atraso = 0;
            int minutos_atraso_totales = 0;
            string auxiliar = "";

            label4.Text = datos.Rows[2].Cells[5].Value.ToString();
            int contador = 1;
            for (int i = 8; i < datos.Rows.Count - 1; i++)
            {
                try
                {
                    dia = datos.Rows[i].Cells[1].Value.ToString();
                    fecha = datos.Rows[i].Cells[2].Value.ToString();
                    horario_1er_periodo = datos.Rows[i].Cells[3].Value.ToString();
                    primera_entrada = datos.Rows[i].Cells[6].Value.ToString();
                    primera_salida = datos.Rows[i].Cells[7].Value.ToString();
                    horario_2do_periodo = datos.Rows[i].Cells[8].Value.ToString();
                    segunda_entrada = datos.Rows[i].Cells[9].Value.ToString();
                    segunda_salida = datos.Rows[i].Cells[10].Value.ToString();
                    obs_1ra_salida = datos.Rows[i].Cells[13].Value.ToString();
                    obs_2da_salida = datos.Rows[i].Cells[17].Value.ToString();

                    obs_1ra_entrada = datos.Rows[i].Cells[12].Value.ToString();
                    if (obs_1ra_entrada.Contains('(') == true)
                    {
                        auxiliar = obs_1ra_entrada.Replace("(", " ");
                        auxiliar = auxiliar.Replace(")", " ");
                        minutos_atraso = minutos_atraso + Math.Abs(int.Parse(auxiliar));
                    }

                    obs_2da_entrada = datos.Rows[i].Cells[15].Value.ToString();
                    if (obs_2da_entrada.Contains('(') == true)
                    {
                        auxiliar = obs_2da_entrada.Replace("(", " ");
                        auxiliar = auxiliar.Replace(")", " ");
                        minutos_atraso = minutos_atraso + Math.Abs(int.Parse(auxiliar));
                    }

                    if (minutos_atraso > 10)
                    {
                        minutos_atraso = minutos_atraso - 10;
                    }
                    else
                    {
                        minutos_atraso = 0;
                    }
                    minutos_atraso_totales = minutos_atraso_totales + minutos_atraso;

                    if (dia == "" && fecha == "" && horario_1er_periodo == "" && primera_entrada == "" && primera_salida != "")
                    {
                        dataGridView1.Rows.Add(contador.ToString(), dia, fecha, horario_1er_periodo, primera_entrada, primera_salida, horario_2do_periodo, segunda_entrada,
                        segunda_salida, obs_1ra_entrada, obs_1ra_salida, obs_2da_entrada, obs_2da_salida, "Min. totales: " + minutos_atraso_totales);
                        minutos_atraso_totales = 0;
                    }
                    else
                    {
                        if (checkBox1.Checked == false)
                        {
                            if (minutos_atraso == 0)
                            {
                                dataGridView1.Rows.Add(contador.ToString(), dia, fecha, horario_1er_periodo, primera_entrada, primera_salida, horario_2do_periodo, segunda_entrada,
                                segunda_salida, obs_1ra_entrada, obs_1ra_salida, obs_2da_entrada, obs_2da_salida, "");
                            }
                            else
                            {
                                dataGridView1.Rows.Add(contador.ToString(), dia, fecha, horario_1er_periodo, primera_entrada, primera_salida, horario_2do_periodo, segunda_entrada,
                                segunda_salida, obs_1ra_entrada, obs_1ra_salida, obs_2da_entrada, obs_2da_salida, minutos_atraso);
                                minutos_atraso = 0;
                            }
                        }
                        else
                        {
                            if (dia.Length > 4)
                            {
                                dataGridView1.Rows.Add(contador.ToString(), dia, fecha, horario_1er_periodo, primera_entrada, primera_salida, horario_2do_periodo, segunda_entrada,
                                segunda_salida, obs_1ra_entrada, obs_1ra_salida, obs_2da_entrada, obs_2da_salida, "");
                            }
                            if (minutos_atraso != 0)
                            {
                                dataGridView1.Rows.Add(contador.ToString(), dia, fecha, horario_1er_periodo, primera_entrada, primera_salida, horario_2do_periodo, segunda_entrada,
                                segunda_salida, obs_1ra_entrada, obs_1ra_salida, obs_2da_entrada, obs_2da_salida, minutos_atraso);
                            }
                            minutos_atraso = 0;
                        }
                    }
                    
                    contador++;
                }
                catch (Exception error)
                {
                    //Hacer nada
                }
            }

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[13].Value != null && dataGridView1.Rows[i].Cells[13].Value.ToString() != null && dataGridView1.Rows[i].Cells[13].Value.ToString() != "0" && dataGridView1.Rows[i].Cells[13].Value.ToString() != "")
                {
                    if (dataGridView1.Rows[i].Cells[13].Value.ToString().Contains("Min") == true)
                    {
                        dataGridView1.Rows[i].Cells[13].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[13].Style.BackColor = Color.Orange;
                    }
                }
            }

            textBox1.Enabled = true;
            button2.Enabled = true;
            button1.Text = "Importar Excel";
            button1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    SaveFileDialog fichero = new SaveFileDialog();
                    fichero.Filter = "Excel (*.xls)|*.xls";
                    if (fichero.ShowDialog() == DialogResult.OK)
                    {
                        Microsoft.Office.Interop.Excel.Application aplicacion;
                        Microsoft.Office.Interop.Excel.Workbook libros_trabajo;
                        Microsoft.Office.Interop.Excel.Worksheet hoja_trabajo;
                        aplicacion = new Microsoft.Office.Interop.Excel.Application();
                        libros_trabajo = aplicacion.Workbooks.Add();
                        hoja_trabajo = (Microsoft.Office.Interop.Excel.Worksheet)libros_trabajo.Worksheets.get_Item(1);

                        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < dataGridView1.Columns.Count; j++)
                            {
                                hoja_trabajo.Cells[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                        }

                        libros_trabajo.SaveAs(fichero.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                        libros_trabajo.Close(true);
                        aplicacion.Quit();

                        MessageBox.Show("Exportacion completada", "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Para exportar a Excel se requiere al menos de 1 registro.", "Exportar a Excel", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error exportar_excel: " + error.Message, "Verificador de infracciones de asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buscar();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.ToUpper();
            textBox1.SelectionStart = textBox1.Text.Length;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            estado_inicial();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                buscar();
            }
        }

        private void buscar()
        {
            bool coincidencia = false;

            if (textBox1.Text != "")
            {
                textBox1.Enabled = false;
                button2.Enabled = false;
                for (int i = fila_buscada; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value.ToString().Contains(textBox1.Text) == true)
                    {
                        dataGridView1.Rows[i].Selected = true;
                        dataGridView1.Rows[i].Cells[0].Selected = true;
                        dataGridView1.FirstDisplayedScrollingRowIndex = i;
                        fila_buscada = i + 1;
                        coincidencia = true;
                        break;
                    }
                }

                if (coincidencia == false)
                {
                    MessageBox.Show("No hay mas coincidencias.", "Verificador de infracciones de asistencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fila_buscada = 0;
                }
                textBox1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            escribir_datos(dataGridView2);
        }
    }
}
