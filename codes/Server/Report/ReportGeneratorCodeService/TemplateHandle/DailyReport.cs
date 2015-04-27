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
    public abstract class DailyReport : TemplateHandleBase
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public DailyReport(TemplateHandlerPrams para)
            : base(para)
        {
            try
            {
                SensorProduct = DataAccess.GetProductInfo(TemplateHandlerPrams.Structure.Id,
                                                          TemplateHandlerPrams.Factor.Id);
                SensorsId = GetSensorsByStrunctIdAndFactorId(TemplateHandlerPrams.Structure.Id,
                                                             TemplateHandlerPrams.Factor.Id);
                Cdbh = GetCdbhByStructIdAndFactorId(TemplateHandlerPrams.Structure.Id, TemplateHandlerPrams.Factor.Id);
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);

            }

        }

        public DailyReport(TemplateHandlerPrams para, int identification)
            : base(para)
        {
            try
            {
                SensorProduct = DataAccess.GetProductInfo(TemplateHandlerPrams.Structure.Id,
                                                          TemplateHandlerPrams.Factor.Id);
                SensorsId = GetAnchorSensorsByStrunctIdAndFactorId(TemplateHandlerPrams.Structure.Id,
                                                             TemplateHandlerPrams.Factor.Id, identification);
                Cdbh = GetAnchorCdbhByStructIdAndFactorId(TemplateHandlerPrams.Structure.Id, TemplateHandlerPrams.Factor.Id, identification);
                
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

        public string rptNo
        {
            get
            {
                var sb = new StringBuilder(50);
                sb.Append("FR").AppendFormat("-{0:D6}", TemplateHandlerPrams.Structure.Id).AppendFormat("-{0:D6}", TemplateHandlerPrams.Factor.Id);
                sb.Append("-").Append((long)(TemplateHandlerPrams.Date - new DateTime(1970, 1, 1)).TotalMilliseconds);
                return sb.ToString();
            }
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
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid,fid);
            foreach (var item in list)
            {
                lst = item.Sensors.Select(q => q.SensorId).ToList();
            }
            return lst.ToArray();
        }
        public string[] GetCdbhByStructIdAndFactorId(int sid, int fid)
        {
            List<string> lst = new List<string>();
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid,fid);
            foreach (var item in list)
            {
                lst = item.Sensors.Select(p => p.Location).ToList();
            }
            return lst.ToArray();
        }

        /// <summary>
        /// 获取传感器id
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="fid"></param>
        /// <param name="identification"></param>
        /// <returns></returns>
        public int[] GetAnchorSensorsByStrunctIdAndFactorId(int sid, int fid, int identification)
        {
            List<int> lst = new List<int>();
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid, fid, identification);
            foreach (var item in list)
            {
                lst = item.Sensors.Select(q => q.SensorId).ToList();
            }
            return lst.ToArray();
        }
        public string[] GetAnchorCdbhByStructIdAndFactorId(int sid, int fid, int identification)
        {
            List<string> lst = new List<string>();
            List<SensorList> list = DataAccess.FindSensorsByStructAndFactor(sid, fid, identification);
            foreach (var item in list)
            {
                lst = item.Sensors.Select(p => p.Location).ToList();
            }
            return lst.ToArray();
        }
        /// <summary>
        /// 填充日报基础信息
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
        /// 填充每日数据
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d">Data索引</param>
        /// <param name="v">Value索引</param>
        /// <param name="type"></param>
        /// <param name="dt"></param>
        public void FillDayData(Workbook ww, int x, int y, int d, int v, string type, Dictionary<string, MonitorData> dt)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(dt.ContainsKey(type) ? Convert.ToString(dt[type].Data[d].Values[v]/1000) : "0");
        }

        /// <summary>
        /// 填充日报本次变量（通用）
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void FillVariation(Workbook ww, int x, int y, int z, int i)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(((Convert.ToDouble(sheet.Cells[x, z].StringValue) - Convert.ToDouble(sheet.Cells[x, z - i].StringValue)) * 1000).ToString("0.000"));
        }
        /// <summary>
        /// 水平位移本次变量（重载）
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="m"></param>
        /// <param name="i"></param>
        public void FillVariation(Workbook ww, int x, int y, int z, int m, int i)
        {
            var sheet = ww.Worksheets[0];
            var Xc = Convert.ToDouble(Convert.ToDouble(sheet.Cells[x, z].StringValue) - Convert.ToDouble(sheet.Cells[x, z - i].StringValue)) * 1000;
            var Yc = Convert.ToDouble(Convert.ToDouble(sheet.Cells[x, m].StringValue) * 1000 - Convert.ToDouble(sheet.Cells[x, m - i].StringValue) * 1000);
            //var Xc = 0.3;
            //var Yc = 0.4;
            sheet.Cells[x,y].PutValue(Math.Sqrt(Xc*Xc+Yc*Yc).ToString("0.00"));
        }
        /// <summary>
        /// 填充日报累计变量(通用)
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void FillLJVariation(Workbook ww, int x, int y, int z, int i)
        {
            var sheet = ww.Worksheets[0];
            sheet.Cells[x, y].PutValue(((Convert.ToDouble(sheet.Cells[x, z].StringValue) - Convert.ToDouble(sheet.Cells[x, z - i].StringValue)) * 1000).ToString("0.000"));
        }

        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="rowidx"></param>
        /// <param name="sheetidx"></param>
        public void InsertRow(Workbook ww, int rowidx,int sheetidx)
        {
            if (sheetidx < 0 || ww == null)
            {
                return;
            }
            var sheet = ww.Worksheets[sheetidx];
            sheet.Cells.InsertRow(rowidx);
            sheet.Cells.SetRowHeight(rowidx,27);
        }

        /// <summary>
        /// 新增拷贝sheet页
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="sheetidx"></param>
        public void AddSheetCopy(Workbook ww,  int sheetidx)
        {
            if (sheetidx < 0 || ww == null)
            {
                return;
            }
            ww.Worksheets.AddCopy(sheetidx);
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="rowidx"></param>
        /// <param name="rowcnt"></param>
        /// <param name="sheetidx"></param>
        public void DeleteRows(Workbook ww, int rowidx, int rowcnt,int sheetidx)
        {
            if (sheetidx < 0 || ww == null || rowidx < 0 || rowcnt < 0)
            {
                return;
            }
            ww.Worksheets[sheetidx].Cells.DeleteRows(rowidx, rowcnt);
        }

        /// <summary>
        /// 填写单元格内容
        /// </summary>
        /// <param name="ww"></param>
        /// <param name="sheetidx"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void FillDayDataEx(Workbook ww , int sheetidx,int x, int y, string value)
        {
            if (sheetidx < 0 || ww == null || x < 0 || x < 0)
            {
                return;
            }
            var sheet = ww.Worksheets[sheetidx];
            sheet.Cells[x, y].PutValue(value);
        }

        public void CellMerge(Workbook ww ,int sheetidx,int x,int y,int totalrows,int totalcols)
        {
            if (sheetidx < 0 || ww == null || x < 0 || x < 0)
            {
                return;
            }
            var sheet = ww.Worksheets[sheetidx];
            sheet.Cells.Merge(x,y,totalrows,totalcols);
        }

        public void CellFormula(Workbook ww ,int sheetidx,int x,int y,string formula)
        {
            if (sheetidx < 0 || ww == null || x < 0 || x < 0)
            {
                return;
            }
            var sheet = ww.Worksheets[sheetidx];
            sheet.Cells[x, y].Formula = formula;
        }

    }
}
