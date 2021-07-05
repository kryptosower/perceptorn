using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Perceptron
{
    public partial class Form1 : Form
    {
        public Bitmap inpImageBitmap;
        public int[,] inpImageArray;

        protected bool validData;
        string path;
        protected Image image;
        protected Thread getImageThread;

        private int height = 50;
        private int width = 30;

        private Neuron[] neurArray;
        private bool[] neurResultArray;
        private const int NEUR_NUM = 10;

        bool mode; //true - обучение, false - проверка

        public Form1()
        {
            InitializeComponent();

            inpImageArray = new int[width, height];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = false;
            comboBox2.SelectedIndex = 1;

            mode = false;

            neurArray = new Neuron[NEUR_NUM];

            for (int i = 0; i < NEUR_NUM; i++)
            {
                neurArray[i] = new Neuron(i);
                neurArray[i].LoadWeights();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Neuron n in neurArray)
            {
                n.SaveWeights();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            string filename;
            validData = GetFilename(out filename, e);
            if (validData)
            {
                path = filename;
                getImageThread = new Thread(new ThreadStart(LoadImage));
                getImageThread.Start();
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private bool GetFilename(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                Array data = ((IDataObject)e.Data).GetData("FileDrop") as Array;
                if (data != null)
                {
                    if ((data.Length == 1) && (data.GetValue(0) is String))
                    {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (validData)
            {
                while (getImageThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }
                pictureBox1.Image = image;
            }

            if(mode)
            {
                //Обучение
                Learn();
            }
            else
            {
                //Проверка
                Recognize();
                updateListbox();
            }
        }

        protected void LoadImage()
        {
            image = new Bitmap(path);
        }

        private void Learn()
        {
            int n = Convert.ToInt32(comboBox1.Text);

            bool continueFlag = true;

            Recognize();

            while (continueFlag)
            {
                continueFlag = false;
                for (int i = 0; i < NEUR_NUM; i++)
                {
                    if (i == n && neurResultArray[i] == false)
                    {
                        neurArray[i].IncreaseWeights();
                        continueFlag = true;
                    }
                    if (i != n && neurResultArray[i] == true)
                    {
                        neurArray[i].DecreaseWeights();
                        continueFlag = true;
                    }
                }
                Recognize();
            }
            updateListbox();
        }

        private void Recognize()
        {
            inpImageBitmap = null;
            neurResultArray = new bool[NEUR_NUM];

            inpImageBitmap = pictureBox1.Image as Bitmap;

            if (inpImageBitmap != null)
            {
                int num;

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        num = (inpImageBitmap.GetPixel(w, h).R);
                        if (num >= 250) num = 0;
                        else num = 1;
                        inpImageArray[w, h] = num;
                    }
                }

                for (int i = 0; i < NEUR_NUM; i++)
                {
                    neurArray[i].Input(inpImageArray);
                    neurResultArray[i] = neurArray[i].Result();
                }

                int res = -1;
                for (int i = 0; i < NEUR_NUM; i++)
                {
                    if (neurResultArray[i] == true) res = i;
                }

                lblResult.Text = "Результат: " + res;
            }
            else return;
        }

        private void updateListbox()
        {
            if(mode)
            {
                //Обучение
                listBox1.Items.Clear();
                neurArray[Convert.ToInt32(comboBox1.Text)].DisplayWeights(listBox1);
            }
            else
            {
                //Проверка
                listBox1.Items.Clear();
                neurArray[Convert.ToInt32(Convert.ToString(lblResult.Text.Last()))].DisplayWeights(listBox1);
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            updateListbox();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                //Обучение
                comboBox1.Enabled = true;
                mode = true;
                listBox1.Items.Clear();
                pictureBox1.Image = null;
                lblResult.Text = "Результат: ";
            }
            else
            {
                //Проверка
                comboBox1.Enabled = false;
                mode = false;
                listBox1.Items.Clear();
                pictureBox1.Image = null;
                lblResult.Text = "Результат: ";
            }
        }
    }
}
