using System.Reflection;
using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportGeneratorService.Dal;
using ReportGeneratorService.DataModel;
using ReportGeneratorService.DataModule;
using ReportGeneratorService.Interface;
using ReportGeneratorService.ReportModule;
using log4net;

namespace ReportGeneratorService.TemplateHandle
{
    public abstract class WeekReport : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public WeekReport(TemplateHandlerPrams param):base(param)
        {
           // TemplateHandlerPrams = param;
            try
            {
                SensorProduct = DataAccess.GetProductInfo(param.Structure.Id, param.Factor.Id);
                SensorsId = GetSensorsByStrunctIdAndFactorId(param.Structure.Id, param.Factor.Id);
                Cdbh = GetCdbhByStructIdAndFactorId(TemplateHandlerPrams.Structure.Id, TemplateHandlerPrams.Factor.Id);
            }
            catch (Exception ex)
            {
              logger.Warn(ex.Message);
            }
         
        }
        /// <summary>
        /// 传感器设备信息
        /// </summary>
        public SensorProductInfo SensorProduct { get; set; }
        /// <summary>
        /// 传感器id
        /// </summary>
        public int[] SensorsId { get; set; }
        /// <summary>
        /// 测点编号（11-26）
        /// </summary>
        public string[] Cdbh { get; set; }
        /// <summary>
        /// 获取测点编号
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string[] GetCdbhByStructIdAndFactorId(int sid, int fid)
        {
            List<string> lst = new List<string>();
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid, fid);
            foreach (var item in list)
            {
                lst = item.Sensors.OrderBy(p => p.SensorId).Select(p => p.Location).ToList();
            }

            return lst.ToArray();
        }

        /// <summary>
        /// 获取传感器id
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public int[] GetSensorsByStrunctIdAndFactorId(int sid, int fid)
        {

            List<int> lst = new List<int>();
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid, fid);
            foreach (var item in list)
            {
                lst = item.Sensors.Select(q => q.SensorId).ToList();

            }
            return lst.ToArray();
        }
       

        /// <summary>
        /// 填充基础信息
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void FillBasicInformation(Workbook ww, int x, int y, string type, string value)
        {
            var sheet = ww.Worksheets[0];
            string tpm = sheet.Cells[x, y].StringValue;
            sheet.Cells[x, y].PutValue(tpm.Replace(type, value));
        }

        /// <summary>
        /// 填充周报监测日期
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dt"></param>
        public void FillDateTime(Workbook ww, int x, int y, string type, DateTime dt)
        {
            var sheet = ww.Worksheets[0];
            string tpm = sheet.Cells[x, y].StringValue;
            sheet.Cells[x, y].PutValue(tpm.Replace(type, dt.ToShortDateString()));

        }
        /// <summary>
        /// 填充周报每日日期
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="jh"></param>
        public void FillDateTime(Workbook ww, int x, int y, DateTime jh)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(jh.ToShortDateString());
        }

        /// <summary>
        /// 填充每日数据
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="dt"></param>
        public void FillEveryDayData(Workbook ww, int a, int b,int d,int v, int x, int y, string type, Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[a, b].PutValue(dt.ContainsKey(type) ? Convert.ToString(Convert.ToDouble(dt[type].Data[d].Values[v]) - Convert.ToDouble(sheet.Cells[x, y].StringValue)*1000) : "-" + Convert.ToDecimal(sheet.Cells[x, y].StringValue)*1000);
        }

        public void FillEveryDayData(Workbook ww, int a, int b, int d, int v, int y, string type, Dictionary<string, MonitorData> dt)
        {
             var sheet = ww.Worksheets[0];
            var X = dt.ContainsKey(type) ?Convert.ToString(dt[type].Data[d].Values[v]) : "0";
            var Y = dt.ContainsKey(type) ? Convert.ToString(dt[type].Data[d].Values[v+1]) : "0";
            var Cx=sheet.Cells[a,y].StringValue;
            var Cy=sheet.Cells[a,y+1].StringValue;
            var Xc = Convert.ToDouble(X) - Convert.ToDouble(Cx)*1000;
            var Yc = Convert.ToDouble(Y) - Convert.ToDouble(Cy)*1000;
            sheet.Cells[a, b].PutValue(Math.Sqrt(Xc*Xc+Yc*Yc).ToString("0.00"));
        }
        /// <summary>
        /// 填充周报本周变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="dy"></param>
        /// <param name="zh"></param>
        public void FillVariation(Workbook ww, int x, int y,int d,int v, string type, Dictionary<string, MonitorData> dy, Dictionary<string, MonitorData> zh)
        {
            var sheet = ww.Worksheets[0];
            var dyData = dy.ContainsKey(type) ? Convert.ToString(dy[type].Data[d].Values[v]) : "0";
            var zhData = zh.ContainsKey(type) ? Convert.ToString(zh[type].Data[d].Values[v]) : "0";
            sheet.Cells[x, y].PutValue((Convert.ToDouble(zhData) - Convert.ToDouble(dyData)).ToString("0.00"));
        }
        /// <summary>
        ///  填充周报本周变化量(水平位移)
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <param name="dy"></param>
        /// <param name="zh"></param>
        public void FillHsVariation(Workbook ww, int x, int y, int d, int v, string type, Dictionary<string, MonitorData> dy, Dictionary<string, MonitorData> zh)
        {
            var sheet = ww.Worksheets[0];
            var dyx = dy.ContainsKey(type) ? dy[type].Data[d].Values[v] : 0;
            var dyy = dy.ContainsKey(type) ? dy[type].Data[d].Values[v+1] : 0;
            var zhx = zh.ContainsKey(type) ? dy[type].Data[d].Values[v] : 0;
            var zhy = zh.ContainsKey(type) ? dy[type].Data[d].Values[v + 1] : 0;
            var Xc = Convert.ToDouble( zhx - dyx);
            var Yc =Convert.ToDouble( zhy - dyy);
            sheet.Cells[x, y].PutValue(Math.Sqrt(Xc*Xc+Yc*Yc).ToString("0.00"));
        }
        /// <summary>
        /// 本周累计变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void FillLJVariation(Workbook ww, int x, int y)
        {
            var sheet = ww.Worksheets[0];
            double variation = 0;

            for (int i = 0; i < 7; i++)
            {
                variation += Convert.ToDouble(sheet.Cells[x, i + 3].StringValue);
            }
            sheet.Cells[x, y].PutValue(Convert.ToString(variation));
        }

        /// <summary>
        ///本周变化速率
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void FillVaryRate(Workbook ww, int x, int y)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(Convert.ToString(double.Parse(Convert.ToDouble(Convert.ToDouble(sheet.Cells[x, y - 2].StringValue) / 7).ToString("0.000"))));
        }

        /// <summary>
        ///  填充上周累计变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public void FillLastWeekLJVariation(Workbook ww, int x, int y,int d,int v, string type, Dictionary<string, MonitorData> first, Dictionary<string, MonitorData> last)
        {
            var sheet = ww.Worksheets[0];
            var start = first.ContainsKey(type) ? Convert.ToDouble(first[type].Data[d].Values[v]) : 0;
            var end = last.ContainsKey(type) ? Convert.ToDouble(last[type].Data[d].Values[v]) : 0;
            sheet.Cells[x, y].PutValue((end - start).ToString("0.000"));
        }
        public void FillLastWeekLJVariation(Workbook ww,int x,int y,int z,int k)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue( Convert.ToString(Convert.ToDouble(sheet.Cells[x,z].StringValue) - Convert.ToDouble(sheet.Cells[x,k].StringValue)));

        }
        /// <summary>
        /// 填充本次高程
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="dt"></param>
        public void FillBCData(Workbook ww, int x,int y,int d,int v,string type,Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            var data = dt.ContainsKey(type) ? Convert.ToDouble(dt[type].Data[d].Values[v]/1000) : 0;
            sheet.Cells[x, y].PutValue(data);
        }
      
        /// <summary>
        ///（返回的是Date格式），数据库数据只到8月份
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DateTime GetLastWeekEachDay(string str, DateTime dt)
        {
            DateTime date = DateTime.Now;
            int xq = Convert.ToInt32(dt.DayOfWeek);
            switch (str)
            {
                case "Monday":
                    date = dt.Date.AddDays(-xq).AddDays(-6).Date.ToLocalTime();
                    break;
                case "Tuesday":
                    date = dt.Date.AddDays(-xq).AddDays(-5).Date.ToLocalTime();
                    break;
                case "Wednesday":
                    date = dt.Date.AddDays(-xq).AddDays(-4).Date.ToLocalTime();
                    break;
                case "Thursday":
                    date = dt.Date.AddDays(-xq).AddDays(-3).Date.ToLocalTime();
                    break;
                case "Friday":
                    date = dt.Date.AddDays(-xq).AddDays(-2).Date.ToLocalTime();
                    break;
                case "Saturday":
                    date = dt.Date.AddDays(-xq).AddDays(-1).Date.ToLocalTime();
                    break;
                case "Sunday":
                    date = dt.Date.AddDays(-xq).Date.ToLocalTime();
                    break;
            }
            return date;
        }
    }
}
