using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;
using System.Drawing;
using TopazApi;
using TopazCommonDefs;

namespace ZedChartLib
{
    
    class ZedChart
    {
        LineItem myCurve;
        GraphPane m_pane;
        double[] x;
        double[] y;
        int m_lineWidth;
        Color[] m_paneColors;
        Color m_color;
        bool addPane = false;
        string m_legend;
        bool m_addOnce = false;
        public ZedChart(GraphPane pane, 
                        string legend,
                        string strXTitle, 
                        string strYTitle, 
                        string title,
                        int lineWidth,
                        Color color)
                      
        {
            m_pane = pane;
            m_lineWidth = lineWidth;
            m_pane.Title.Text = title;
            m_pane.XAxis.Title.Text = strXTitle;
            m_pane.YAxis.Title.Text = strYTitle;
            m_color = color;
            m_legend = legend;
            setDefaultColors();

        }
        public bool ClearPoints(int iSeriesIndex)
        {
            return true;
        }

        public void CreateSeries(int size)
        {
            x = new double[size];
            y = new double[size];
        }

        public void RemoveAll()
        {
           for (int i = 0 ; i < m_pane.CurveList.Count ; i++)
              m_pane.CurveList.RemoveAt(i);
        }
       
        public void Paint()
        {
            try
            {              
                PointPairList spl1 = new PointPairList(x, y);
                if (m_addOnce)
                    myCurve = m_pane.AddCurve("", spl1, m_color, SymbolType.None);
                else
                    myCurve = m_pane.AddCurve(m_legend, spl1, m_color, SymbolType.None);
                m_addOnce = true;
                myCurve.Line.Width = m_lineWidth;
            }
            catch (Exception err)
            {

            }
        }


        public void ClearBarView(ZedGraphControl z1)
        {
            GraphPane myPane = z1.GraphPane;
            myPane.CurveList.Clear();
            myPane.GraphObjList.Clear();
        }

        public void Clear()
        {
            try
            {
                if (myCurve != null)
                    myCurve.Clear();
                if (x != null)
                    Array.Clear(x, 0, x.Length);
                if (y != null)
                    Array.Clear(y, 0, y.Length);
            }
            catch (Exception err)
            {

            }
        }

        public bool Draw(int index, double xValue , double yValue)
        {             
            x[index] = xValue;
            y[index] = yValue;
            return true;             
        }

        // Call this method from the Form_Load method, passing your ZedGraphControl
        public void CreateGraph_ForwardPowerGradientByZBars(ZedGraphControl z1,  
                                                            double[] value, 
                                                            int numEntries, 
                                                            int channel)
        {
            //m_pane.Title.Text = "Forward Power";
            //m_pane.XAxis.Title.Text = "Bar Number";
            //m_pane.YAxis.Title.Text = "Value";

            PointPairList list = new PointPairList();         

            for (int i = 0; i < numEntries; i++)
            {
              
                double x = 2400 + i;
                double y = value[i];
                double z = i / 4.0;
                list.Add(x, y, z);
            }

//          if (addPane == false)
            {
                m_pane.CurveList.Clear();
                m_pane.GraphObjList.Clear();
                BarItem myCurve = m_pane.AddBar("Forward power in Heat table", list, Color.Blue);
                Color[] colors = { Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple };
                myCurve.Bar.Fill = new Fill(colors);
                myCurve.Bar.Fill.Type = FillType.GradientByZ;

                myCurve.Bar.Fill.RangeMin = 0;
                myCurve.Bar.Fill.RangeMax = 4;

                m_pane.Chart.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45);
                m_pane.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 225), 45);
            }
            addPane = true;
            // Tell ZedGraph to calculate the axis ranges
            z1.AxisChange();
        }

        private void setDefaultColors()
        {
           
            m_paneColors = new Color[5];
            m_paneColors[0] = Color.Red;
            m_paneColors[1] = Color.Yellow;
            m_paneColors[2] = Color.Green;
            m_paneColors[3] = Color.Blue;
            m_paneColors[4] = Color.Purple;

             
        }

        public void setColors()
        {
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                m_paneColors[i] = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));
            }

        }
        public void CreateGraph_DRGradientByZBars(ZedGraphControl z1,
                                                  DR[] drBuffer,
                                                  int numEntries,
                                                  bool showall,
                                                  bool clear = true)
        {
            try
            {
                GraphPane myPane = z1.GraphPane;
                myPane.XAxis.Title.Text = "Frequencies";
                myPane.YAxis.Title.Text = "Value";

                PointPairList list = new PointPairList();
                double f = 2400;
                for (int i = 0; i < numEntries; i++)
                {
                    if (drBuffer[i].valid  == 1 || showall)
                    {
                        double x = drBuffer[i].freq;
                        if (x < 2400 || x > 2500)
                            continue;
                        double y = drBuffer[i].value;
                        double z = i / 4.0;
                        list.Add(x, y, z);
                        f++;
                    }
                }

                //          
                if (clear == true)
                {
                    myPane.CurveList.Clear();
                    myPane.GraphObjList.Clear();
                }
                //BarItem myCurve = myPane.AddBar("DR in Heat table", list, Color.Blue);
                BarItem myCurve = myPane.AddBar("", list, Color.Blue);

                myCurve.Bar.Fill = new Fill(m_paneColors);
                myCurve.Bar.Fill.Type = FillType.GradientByZ;

                myCurve.Bar.Fill.RangeMin = 0;
                myCurve.Bar.Fill.RangeMax = 4;


                myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45);
                myPane.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 225), 45);

                // Tell ZedGraph to calculate the axis ranges
                z1.AxisChange();
            }
            catch (Exception err)
            {

            }
        }
        public void CreateGraph_EnergyGradientByZBars(ZedGraphControl z1,
                                                      EnergyData[] Buffer,
                                                      int numEntries,
                                                      bool clear = true)
        {
            try
            {
                GraphPane myPane = z1.GraphPane;
                myPane.XAxis.Title.Text = "Frequencies";
                myPane.YAxis.Title.Text = "Value";

                PointPairList list = new PointPairList();
                for (int i = 0; i < numEntries; i++)
                {
                    if (Buffer[i].freq != 0)
                    {
                        double x = Buffer[i].freq;
                        double y = Buffer[i].value;
                        double z = i / 4.0;
                        list.Add(x, y, z);
                    }
                }

                //          
                if (clear == true)
                {
                    myPane.CurveList.Clear();
                    myPane.GraphObjList.Clear();
                }
                //BarItem myCurve = myPane.AddBar("DR in Heat table", list, Color.Blue);
                BarItem myCurve = myPane.AddBar("", list, Color.Blue);

                myCurve.Bar.Fill = new Fill(m_paneColors);
                myCurve.Bar.Fill.Type = FillType.GradientByZ;

                myCurve.Bar.Fill.RangeMin = 0;
                myCurve.Bar.Fill.RangeMax = 4;


                myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45);
                myPane.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 225), 45);

                // Tell ZedGraph to calculate the axis ranges
                z1.AxisChange();
            }
            catch (Exception err)
            {

            }
        }
    }
}
