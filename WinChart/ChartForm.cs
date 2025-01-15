using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraCharts;

namespace WinChart
{
    public partial class ChartForm : Form
    {
        // 创建ChartControl
        ChartControl chart = null;

        public ChartForm()
        {
            InitializeComponent();
            CreateChart();
        }

        /// <summary>
        /// 窗体加载完成后执行，修改数据背景色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChartForm_Load(object sender, System.EventArgs e)
        {
            // 获取 XYDiagram
            var diagram = (XYDiagram)chart.Diagram;

            // 设置默认面板背景色为纯色
            var defaultPane = diagram.DefaultPane;
            defaultPane.BackColor = Color.FromArgb(7, 9, 62);  // 设置背景颜色

            // 禁用渐变效果，确保使用纯色背景
            defaultPane.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Solid;

            // 设置默认面板的边框颜色
            defaultPane.BorderColor = Color.FromArgb(7, 9, 62);

            // 设置默认面板的阴影颜色
            defaultPane.Shadow.Color = Color.FromArgb(7, 9, 62);

            // 设置窗体为最大化
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 创建图表方法
        /// </summary>
        private void CreateChart()
        {
            // 初始化ChartControl并设置属性
            chart = new ChartControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(7, 9, 62),
            };
            this.panel2.Controls.Add(chart);

            // 准备数据
            var dates = new[] { "2025-1-11", "2025-1-12", "2025-1-13", "2025-1-14" };
            var defectiveRecords = new[] { 12, 11, 9, 7 };
            var defectiveRepairs = new[] { 6, 11, 9, 5 };
            var defectiveInputs = new[] { 5, 11, 9, 6 };
            var completionRates = new[] { 98, 160, 100, 86 };

            // 创建堆叠柱状图系列
            var defectiveRecordSeries = CreateStackedBarSeries("不良品登记", dates, defectiveRecords, Color.FromArgb(79, 129, 189));
            var defectiveRepairSeries = CreateStackedBarSeries("不良品修理", dates, defectiveRepairs, Color.FromArgb(192, 80, 77));
            var defectiveInputSeries = CreateStackedBarSeries("不良品投入", dates, defectiveInputs, Color.FromArgb(155, 187, 89));

            // 创建折线图系列
            var completionRateSeries = CreateLineSeries("完成率(%)", dates, completionRates);

            // 添加系列到图表
            chart.Series.AddRange(new[] { defectiveRecordSeries, defectiveRepairSeries, defectiveInputSeries, completionRateSeries });

            // 设置主Y轴的字体样式和颜色
            this.CustomizePrimaryYAxis();

            // 设置主X轴的字体样式和颜色
            this.CustomizeXAxis();

            // 创建第二个Y轴并绑定折线图系列
            var completionRateAxis = CreateSecondaryYAxis("CompletionRateAxis", "完成率(%)");
            ((LineSeriesView)completionRateSeries.View).AxisY = completionRateAxis;

            // 创建一个 Panel 用于显示标题背景图
            Panel titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50, // 设置标题区域高度
                BackColor = Color.Transparent
            };
            titlePanel.Paint += TitlePanel_Paint;
            // 添加双击事件
            titlePanel.MouseDoubleClick += TitlePanel_MouseDoubleClick;

            // 将 Panel 添加到窗体或容器中
            this.panel2.Controls.Add(titlePanel);
        }

        /// <summary>
        /// 双击标题事件
        /// </summary>
        private void TitlePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 双击标题时执行的操作
            panel1.Visible = !panel1.Visible;
        }

        /// <summary>
        /// 绘制标题背景图及文本
        /// </summary>
        private void TitlePanel_Paint(object sender, PaintEventArgs e)
        {
            // 获取面板区域
            var panel = (Panel)sender;
            var titleBounds = panel.ClientRectangle;

            // 绘制背景图（缩放适应面板）
            var imagePath = "E:\\Users\\Administrator\\Pictures\\84d80c35-9867-40c1-b0f8-675d8b68509b.png"; // 替换为你的图片路径
            var backgroundImage = Image.FromFile(imagePath);

            // 使用 Stretch 方式绘制背景图，以适应面板大小
            e.Graphics.DrawImage(backgroundImage, titleBounds);

            // 绘制标题文本
            var font = new Font("Arial", 20, FontStyle.Bold);
            var textColor = Color.FromArgb(68, 213, 250);
            var titleText = "工程不良解析情况";

            // 计算文本的大小
            var textSize = e.Graphics.MeasureString(titleText, font);

            // 计算居中的位置
            var x = (titleBounds.Width - textSize.Width) / 2;  // 水平居中
            var y = (titleBounds.Height - textSize.Height) / 2;  // 垂直居中

            // 创建 StringFormat 实例，设置文本格式
            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center, // 水平居中
                LineAlignment = StringAlignment.Center // 垂直居中
            };

            // 绘制居中的文本
            e.Graphics.DrawString(titleText, font, new SolidBrush(textColor), new RectangleF(x, y, textSize.Width, textSize.Height), stringFormat);
        }

        /// <summary>
        /// 创建堆叠柱状图系列
        /// </summary>
        private Series CreateStackedBarSeries(string name, string[] dates, int[] values, Color color)
        {
            var series = new Series(name, ViewType.StackedBar);
            for (int i = 0; i < dates.Length; i++)
            {
                series.Points.Add(new SeriesPoint(dates[i], values[i]));
            }

            var view = (StackedBarSeriesView)series.View;
            view.Color = color;
            view.FillStyle.FillMode = FillMode.Solid; // 禁用渐变效果

            return series;
        }

        /// <summary>
        /// 创建折线图系列
        /// </summary>
        private Series CreateLineSeries(string name, string[] dates, int[] values)
        {
            var series = new Series(name, ViewType.Line);
            for (int i = 0; i < dates.Length; i++)
            {
                series.Points.Add(new SeriesPoint(dates[i], values[i]));
            }

            var lineView = (LineSeriesView)series.View;
            lineView.AxisYName = "CompletionRateAxis"; // 绑定到第二个Y轴
            lineView.LineMarkerOptions.Kind = MarkerKind.Circle;

            return series;
        }

        /// <summary>
        /// 主Y轴
        /// </summary>
        private void CustomizePrimaryYAxis()
        {
            // 获取主 Y 轴
            AxisY primaryYAxis = ((XYDiagram)chart.Diagram).AxisY;

            // 设置主 Y 轴数值的字体大小、字体和颜色
            primaryYAxis.Label.Font = new Font("Tahoma", 13, FontStyle.Regular); // 设置字体和大小
            primaryYAxis.Label.TextColor = Color.FromArgb(218, 225, 234); // 设置字体颜色为自定义颜色
        }

        /// <summary>
        /// 主X轴
        /// </summary>
        private void CustomizeXAxis()
        {
            // 获取X轴
            AxisX primaryXAxis = ((XYDiagram)chart.Diagram).AxisX;

            // 设置X轴标签的字体大小、字体和颜色
            primaryXAxis.Label.Font = new Font("Tahoma", 13, FontStyle.Regular); // 设置字体和大小
            primaryXAxis.Label.TextColor = Color.FromArgb(218, 225, 234); // 设置字体颜色为自定义颜色
        }

        /// <summary>
        /// 创建第二个Y轴 完成率(%)
        /// </summary>
        private SecondaryAxisY CreateSecondaryYAxis(string axisName, string title)
        {
            SecondaryAxisY axis = new SecondaryAxisY(axisName);

            // 标题
            axis.Title.Text = title;
            axis.Title.Visible = true;
            axis.Title.Font = new Font("Tahoma", 13, FontStyle.Regular);  // 修改字体大小，字体
            axis.Title.TextColor = Color.FromArgb(218, 225, 234);

            // 设置数值的字体大小和颜色
            axis.Label.Font = new Font("Tahoma", 13, FontStyle.Regular);  // 修改字体大小，字体
            axis.Label.TextColor = Color.FromArgb(218, 225, 234);  // 设置字体颜色为自定义颜色
            ((XYDiagram)chart.Diagram).SecondaryAxesY.Add(axis);
            return axis;
        }
    }
}
