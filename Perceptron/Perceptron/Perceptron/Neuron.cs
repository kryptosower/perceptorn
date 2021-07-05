using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Perceptron
{
    public class Neuron
    {
        private int width = 30;
        private int height = 50;
        private int limit = 300;

        private int[,] weights;
        private int[,] signals;
        private int[,] input;

        private int sum;
        private int associateNumber;
        private string fileName;

        public Neuron(int number)
        {
            weights = new int[width, height];
            /*Random r = new Random(number);
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    weights[w, h] = r.Next(0, 2);
                }
            }*/
            signals = new int[width, height];
            input = new int[width, height];

            associateNumber = number;
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Нейрон " + associateNumber + ".txt");
        }

        public void Input(int [,] inp)
        {
            input = inp;
        }

        public void MutiplyWeights()
        {
            for(int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    signals[w, h] = input[w, h] * weights[w, h];
                }
            }
        }

        public void SumSignals()
        {
            sum = 0;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    sum += signals[w, h];
                }
            }
        }

        public bool Result()
        {
            MutiplyWeights();
            SumSignals();

            if (sum > limit) return true;
            else return false;
        }

        public int CallNumber()
        {
            return associateNumber;
        }

        public void SaveWeights()
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.ASCII, 1500))
            {
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        sw.Write(weights[w, h]);
                    }
                }
            }
        }

        public void LoadWeights()
        {
            if (File.Exists(fileName))
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.ASCII, true))
                {
                    char c;
                    string s;
                    for (int h = 0; h < height; h++)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            if (sr.Peek() == '-')
                            {
                                sr.Read();
                                c = (char)sr.Read();
                                s = c.ToString();
                                weights[w, h] = -(Convert.ToInt32(s));
                            }
                            else
                            {
                                c = (char)sr.Read();
                                s = c.ToString();
                                weights[w, h] = Convert.ToInt32(s);
                            }
                        }
                    }
                }
            }
            else SaveWeights();
        }

        public void DisplayWeights(System.Windows.Forms.ListBox lb)
        {
            string temp = "";

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if (weights[w, h] >= 0)
                    {
                        temp += " " + weights[w, h];
                    }
                    else temp += weights[w, h];
                }
                lb.Items.Add(temp);
                temp = "";
            }
        }

        public void IncreaseWeights()
        {
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if(weights[w,h] < 9)
                    {
                        weights[w, h] += input[w, h];
                    }
                }
            }
        }

        public void DecreaseWeights()
        {
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    if(weights[w,h] > -9)
                    {
                        weights[w, h] -= input[w, h];
                    }
                }
            }
        }
    }
}
