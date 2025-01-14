using System.Windows.Forms;
using DevExpress.XtraCharts;

namespace WinChart
{
    public partial class ChartForm : Form
    {
        ChartControl chart = null;

        public ChartForm()
        {
            InitializeComponent();
            CreateChart();
        }

        private void CreateChart()
        {
            // 创建ChartControl
            chart = new ChartControl
            {
                Dock = DockStyle.Fill
            };
            this.panel2.Controls.Add(chart);
            // this.Controls.Add(chart);            

            // 准备数据
            var dates = new[] { "2025-1-11", "2025-1-12", "2025-1-13", "2025-1-14" };
            var defectiveRecords = new[] { 12, 11, 9, 7 };
            var defectiveRepairs = new[] { 6, 11, 9, 5 };
            var defectiveInputs = new[] { 5, 11, 9, 6 };
            var completionRates = new[] { 42, 100, 100, 86 };

            // 创建堆叠柱状图系列
            Series defectiveRecordSeries = CreateStackedBarSeries("不良品登记", dates, defectiveRecords);
            Series defectiveRepairSeries = CreateStackedBarSeries("不良品修理", dates, defectiveRepairs);
            Series defectiveInputSeries = CreateStackedBarSeries("不良品投入", dates, defectiveInputs);

            // 添加柱状图系列到图表
            chart.Series.AddRange(new[] { defectiveRecordSeries, defectiveRepairSeries, defectiveInputSeries });

            // 创建折线图系列
            Series completionRateSeries = new Series("完成率", ViewType.Line);
            for (int i = 0; i < dates.Length; i++)
            {
                completionRateSeries.Points.Add(new SeriesPoint(dates[i], completionRates[i]));
            }
            LineSeriesView lineView = (LineSeriesView)completionRateSeries.View;
            lineView.AxisYName = "CompletionRateAxis"; // 绑定到第二个Y轴
            lineView.LineMarkerOptions.Kind = MarkerKind.Circle;

            // 添加折线图系列到图表
            chart.Series.Add(completionRateSeries);

            // 创建第二个Y轴
            SecondaryAxisY completionRateAxis = new SecondaryAxisY("CompletionRateAxis")
            {
                Title = { Text = "完成率(%)", Visible = true }
            };
            ((XYDiagram)chart.Diagram).SecondaryAxesY.Add(completionRateAxis);

            // 将折线图系列绑定到第二个Y轴
            ((LineSeriesView)completionRateSeries.View).AxisY = completionRateAxis;

            // 设置主标题
            chart.Titles.Add(new ChartTitle { Text = "工程不良解析情况" });

            // 绑定双击事件
            chart.MouseDoubleClick += Chart_MouseDoubleClick;
        }

        private void Chart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 获取鼠标点击的命中信息
            ChartHitInfo hitInfo = chart.CalcHitInfo(e.Location);

            // 检查是否双击了标题 显示检索内容
            if (hitInfo.HitTest.ToString() == "ChartTitle")
            {

                if (this.panel1.Visible)
                {
                    this.panel1.Visible = false;
                }
                else
                {
                    this.panel1.Visible = true;
                }
            }
        }


        private Series CreateStackedBarSeries(string name, string[] arguments, int[] values)
        {
            Series series = new Series(name, ViewType.StackedBar);
            for (int i = 0; i < arguments.Length; i++)
            {
                series.Points.Add(new SeriesPoint(arguments[i], values[i]));
            }
            return series;
        }
    }
}
