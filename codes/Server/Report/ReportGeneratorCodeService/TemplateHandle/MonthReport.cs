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
    public abstract class MonthReport : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
         public MonthReport(TemplateHandlerPrams param):base(param)
        {
             try
             {
                 //TemplateHandlerPrams = param;
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

        public string[] Cdbh { get; set; }
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
        /// 填充基础信息（除数据部分）
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
        /// 填充月报监测日期
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dt"></param>
        public void FillDateTimeRange(Workbook ww, int x, int y, DateTime dt)
        {
            var sheet = ww.Worksheets[0];
            string tpm = sheet.Cells[x, y].StringValue;
            if (dt.Month == 1)
            {
                sheet.Cells[x, y].PutValue(tpm.Replace("dateFrom", new DateTime(dt.Year-1, 12, 1).ToShortDateString()));
                tpm = sheet.Cells[x, y].StringValue;
                sheet.Cells[x, y].PutValue(tpm.Replace("dateTo", new DateTime(dt.Year, dt.Month, 1).AddDays(-1).ToShortDateString()));
            }
            else
            {
                sheet.Cells[x, y].PutValue(tpm.Replace("dateFrom", new DateTime(dt.Year, dt.Month - 1, 1).ToShortDateString()));
                tpm = sheet.Cells[x, y].StringValue;
                sheet.Cells[x, y].PutValue(tpm.Replace("dateTo", new DateTime(dt.Year, dt.Month, 1).AddDays(-1).ToShortDateString()));
            }
        }
        public void FillDateTimeRange(Workbook ww, int x, int y, int scnt,DateTime dt)
        {
            var sheet = ww.Worksheets[scnt];
            string tpm = sheet.Cells[x, y].StringValue;
            if (dt.Month == 1)
            {
                sheet.Cells[x, y].PutValue(tpm.Replace("dateFrom", new DateTime(dt.Year - 1, 12, 1).ToShortDateString()));
                tpm = sheet.Cells[x, y].StringValue;
                sheet.Cells[x, y].PutValue(tpm.Replace("dateTo", new DateTime(dt.Year, dt.Month, 1).AddDays(-1).ToShortDateString()));
            }
            else
            {
                sheet.Cells[x, y].PutValue(tpm.Replace("dateFrom", new DateTime(dt.Year, dt.Month - 1, 1).ToShortDateString()));
                tpm = sheet.Cells[x, y].StringValue;
                sheet.Cells[x, y].PutValue(tpm.Replace("dateTo", new DateTime(dt.Year, dt.Month, 1).AddDays(-1).ToShortDateString()));
            }
        }
        /// <summary>
        /// 根据月份动态列扩展
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="dt">日期</param>
        /// <param name="column">插入位置</param>
        public void ExtensionColumn(Workbook ww, DateTime dt, int column)
        {
            var sheet = ww.Worksheets[0];
            if (dt.Month == 1)
            {
                for (int i = 0; i < DateTime.DaysInMonth(dt.Year - 1, 12) - 7; i++)
                {
                    sheet.Cells.InsertColumn(column);
                }
            }
            else
            {
                for (int i = 0; i < DateTime.DaysInMonth(dt.Year , dt.Month - 1) - 7; i++)
                {
                    sheet.Cells.InsertColumn(column);
                }
            }
        }

        public void ExtensionColumn(Workbook ww, DateTime dt, int scnt,int column)
        {
            var sheet = ww.Worksheets[scnt];
            if (dt.Month == 1)
            {
                for (int i = 0; i < DateTime.DaysInMonth(dt.Year - 1, 12) - 7; i++)
                {
                    sheet.Cells.InsertColumn(column);
                }
            }
            else
            {
                for (int i = 0; i < DateTime.DaysInMonth(dt.Year, dt.Month - 1) - 7; i++)
                {
                    sheet.Cells.InsertColumn(column);
                }
            }
        }
        /// <summary>
        /// 填充本月变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <param name="dt"></param>
        /// <param name="dy"></param>
        /// <param name="zh"></param>
        public void FillVariation(Workbook ww, int x, int d, int v, string type, int length, Dictionary<string, MonitorData> dy, Dictionary<string, MonitorData> zh)
        {
            var sheet = ww.Worksheets[0];
            //int length = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
            var dyData = dy.ContainsKey(type) ? Convert.ToString(dy[type].Data[d].Values[v]) : "0";
            var zhData = zh.ContainsKey(type) ? Convert.ToString(zh[type].Data[d].Values[v]) : "0";
            sheet.Cells[x, length + 3].PutValue((Convert.ToDouble(zhData) - Convert.ToDouble(dyData)).ToString("0.000"));
        }
        /// <summary>
        ///  填充周报本月变化量(水平位移)
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <param name="dy"></param>
        /// <param name="zh"></param>
        public void FillHsVariation(Workbook ww, int x,  int d, int v, string type,int length, Dictionary<string, MonitorData> dy, Dictionary<string, MonitorData> zh)
        {
            var sheet = ww.Worksheets[0];
            var dyx = dy.ContainsKey(type) ? dy[type].Data[d].Values[v] : 0;
            var dyy = dy.ContainsKey(type) ? dy[type].Data[d].Values[v + 1] : 0;
            var zhx = zh.ContainsKey(type) ? dy[type].Data[d].Values[v] : 0;
            var zhy = zh.ContainsKey(type) ? dy[type].Data[d].Values[v + 1] : 0;
            var Xc = Convert.ToDouble(zhx - dyx);
            var Yc = Convert.ToDouble(zhy - dyy);
            sheet.Cells[x, length+3].PutValue(Math.Sqrt(Xc * Xc + Yc * Yc).ToString("0.00"));
        }

        /// <summary>
        /// 填充累计变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="dt"></param>
        public void FillLJVariation(Workbook ww, int x, int length)
        {
            var sheet = ww.Worksheets[0];
            double variation = 0;
            //int length = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
            for (int i = 0; i < length; i++)
            {
                variation += Convert.ToDouble(sheet.Cells[x, i + 3].StringValue);
            }
            sheet.Cells[x, length + 4].PutValue(Convert.ToString(variation));
        }

        /// <summary>
        /// 填充本月变化速率，保存到小数点后3位
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="dt"></param>
        public void FillVaryRate(Workbook ww, int x, int length)
        {
            var sheet = ww.Worksheets[0];
            //int length = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
            sheet.Cells[x, length + 5].PutValue(Convert.ToString(double.Parse(Convert.ToDouble(Convert.ToDouble(sheet.Cells[x, length + 3].StringValue) / length).ToString("0.000"))));
        }

        /// <summary>
        /// 填充月报每日日期
        /// </summary>
        /// <param name="ww">excel对象</param>
        /// <param name="x">目标行</param>
        /// <param name="y">目标列</param>
        /// <param name="jh">获取到的日期</param>
        public void FillEveryDayDate(Workbook ww, int x, int y, DateTime jh)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(jh.ToShortDateString());
        }
        public void FillEveryDayDate(Workbook ww, int x, int y, int scnt,DateTime jh)
        {
            var sheet = ww.Worksheets[scnt];
            sheet.Cells[x, y].PutValue(jh.ToShortDateString());
        }
     
        /// <summary>
        /// 填充月报每日数据
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
        public void FillEveryDayData(Workbook ww, int a, int b, int d,int v,int x, int y, string type, Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[a, b].PutValue(dt.ContainsKey(type) ? Convert.ToString(Convert.ToDouble(dt[type].Data[d].Values[v]) - Convert.ToDouble(sheet.Cells[x, y].StringValue)*1000) : "-" + Convert.ToDecimal( sheet.Cells[x, y].StringValue)*1000);
        }
        public void FillEveryDayData(Workbook ww, int a, int b, int d, int v, int y, string type, Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            var X = dt.ContainsKey(type) ? Convert.ToDouble(dt[type].Data[d].Values[v]) : 0;
            var Y = dt.ContainsKey(type) ? Convert.ToDouble(dt[type].Data[d].Values[v + 1]) : 0;
            var Cx = sheet.Cells[a, y].StringValue;
            var Cy = sheet.Cells[a, y + 1].StringValue;
            var Xc = Convert.ToDouble(X) - Convert.ToDouble(Cx) * 1000;
            var Yc = Convert.ToDouble(Y) - Convert.ToDouble(Cy) * 1000;
            sheet.Cells[a, b].PutValue(Math.Sqrt(Xc * Xc + Yc * Yc).ToString("0.00"));
        }
        /// <summary>
        /// 填充月报上月累计变化量
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public void FillLastMonthLJVariation(Workbook ww, int x, int y, int d,int v,string type, Dictionary<string, MonitorData> first, Dictionary<string, MonitorData> last)
        {
            var sheet = ww.Worksheets[0];
            var start = first.ContainsKey(type) ? Convert.ToDouble(first[type].Data[d].Values[v]) : 0;
            var end = last.ContainsKey(type) ? Convert.ToDouble(last[type].Data[d].Values[v]) : 0;
            sheet.Cells[x, y].PutValue((end - start).ToString("0.000"));
        }
        public void FillLastMonthLJVariation(Workbook ww,int x,int y,int z)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue((Convert.ToDouble(sheet.Cells[x, z+4].StringValue) - Convert.ToDouble(sheet.Cells[x, z+3].StringValue)).ToString("0.00"));
        }

        /// <summary>
        /// 填充本次高程
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="length"></param>
        /// <param name="cnt"></param>
        /// <param name="type"></param>
        /// <param name="dt"></param>
        public void FillBCData(Workbook ww, int x, int d, int v, int length,int cnt,string type, Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            var data = dt.ContainsKey(type) ? Convert.ToDouble(dt[type].Data[d].Values[v] / 1000) : 0;
            sheet.Cells[x, length+cnt].PutValue(data);
        }

        /// <summary>
        /// 获取上月每天日期
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DateTime GetLastMonthEachDay(string str, DateTime dt)
        {
            DateTime date = DateTime.Now;
            switch (str)
            {
                case "1":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 1).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 1).Date;
                    }
                    break;
                case "2":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 2).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 2).Date;
                    }
                    break;
                case "3":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 3).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 3).Date;
                    }
                    break;
                case "4":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 4).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year , dt.Month - 1, 4).Date;
                    }
                    break;
                case "5":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 5).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 5).Date;

                    }
                    break;
                case "6":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 6).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 6).Date;

                    }
                    break;
                case "7":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 7).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 7).Date;

                    }
                    break;
                case "8":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 8).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 8).Date;

                    }
                    break;
                case "9":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 9).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 9).Date;

                    }
                    break;
                case "10":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 10).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 10).Date;

                    }
                    break;
                case "11":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 11).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 11).Date;

                    }
                    break;
                case "12":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year-1, 12, 12).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 12).Date;

                    }
                    break;
                case "13":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 13).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 13).Date;

                    }
                    break;
                case "14":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 14).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 14).Date;

                    }
                    break;
                case "15":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 15).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 15).Date;

                    }
                    break;
                case "16":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 16).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 16).Date;

                    }
                    break;
                case "17":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 17).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 17).Date;

                    }
                    break;
                case "18":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 18).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 18).Date;

                    }
                    break;
                case "19":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 19).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 19).Date;

                    }
                    break;
                case "20":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1,12, 20).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 20).Date;

                    }
                    break;
                case "21":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 21).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 21).Date;

                    }
                    break;
                case "22":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 22).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 22).Date;

                    }
                    break;
                case "23":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 23).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 23).Date;

                    }
                    break;
                case "24":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 24).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 24).Date;

                    }
                    break;
                case "25":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 25).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 25).Date;

                    }
                    break;
                case "26":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 26).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 26).Date;

                    }
                    break;
                case "27":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 27).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 27).Date;

                    }
                    break;
                case "28":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 28).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 28).Date;

                    }
                    break;
                case "29":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 29).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 29).Date;

                    }
                    break;
                case "30":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 30).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 30).Date;

                    }
                    break;
                case "31":
                    if (dt.Month == 1)
                    {
                        date = new DateTime(dt.Year - 1, 12, 31).Date;
                    }
                    else
                    {
                        date = new DateTime(dt.Year, dt.Month - 1, 31).Date;

                    }
                    break;



            }
            return date;
        }

    }
}
