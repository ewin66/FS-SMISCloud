// // --------------------------------------------------------------------------------------------
// // <copyright file="OriginalDataReportHandle.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141031
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService.TemplateHandle
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Data;

    using Aspose.Cells;

    using ReportGeneratorService.Dal;
    using ReportGeneratorService.DataModule;
    using ReportGeneratorService.Interface;
    using ReportGeneratorService.ReportModule;

    public class OriginalDataReportHandle:TemplateHandleBase
    {
        private List<Factor> factors = new List<Factor>();
        private Dictionary<Factor, List<Sensor>> sensors = new Dictionary<Factor, List<Sensor>>();

        private int structId;
        public OriginalDataReportHandle(TemplateHandlerPrams para)
            : base(para)
        {
            if (base.TemplateHandlerPrams.Structure != null)
            {
                structId = TemplateHandlerPrams.Structure.Id;
                factors = GetAllFactors(structId);
                sensors = this.GetAllSensors(factors);
            }
        }

        private List<Factor> GetAllFactors(int structId)
        {
            var factorList = DataAccess.FindFactorsByStruct(structId);
            List<Factor> factors = new List<Factor>();
            foreach (var theme in factorList)
            {
                factors.AddRange(theme.Children);
            }
            return factors;
        }

        private Dictionary<Factor, List<Sensor>> GetAllSensors(List<Factor> factors)
        {
            Dictionary<Factor, List<Sensor>> result = new Dictionary<Factor, List<Sensor>>();
            foreach (var factor in factors)
            {
                List<SensorList> sensorLists = DataAccess.FindSensorsByStructAndFactor(structId, factor.Id);
                List<Sensor> sensors = new List<Sensor>();
                foreach (var sensorList in sensorLists)
                {
                    sensors.AddRange(sensorList.Sensors);
                }
                result.Add(factor, sensors);
            }

            return result;
        }

        public override void WriteFile()
        {
            if (File.Exists(TemplateHandlerPrams.FileFullName))
            {
                File.Delete(TemplateHandlerPrams.FileFullName);
            }

            FileStream fs = new FileStream(TemplateHandlerPrams.FileFullName, FileMode.Create, FileAccess.Write);
            Workbook wb = new Workbook(fs);

            foreach (var factor in sensors.Keys)
            {
                Worksheet ws = wb.Worksheets.Add(GetSheetName(factor));
                DataTable dt = GetData(factor.Id, sensors[factor]);
                if (!this.WriteOneSheet(ws, dt))
                {
                    throw new Exception();
                }
            }
        }

        private string GetSheetName(Factor factor)
        {
            return factor.NameCN;
        }

        private DataTable GetData(int factorId, List<Sensor> sensors)
        {
            //List<int> sensorIds = new List<int>();
            //foreach (var sensor in sensors)
            //{
            //    sensorIds.Add(sensor.SensorId);
            //}

            List<int> sensorIds = sensors.Select(s => s.SensorId).ToList();

            string sql = ThemeDataSql.GetSql(factorId, sensorIds);
            return OriginalDataDal.GetOriginalData(sql);
        }

        private bool WriteOneSheet(Worksheet worksheet, DataTable dt)
        {

            if (dt == null) return true;

            int rowIndex = 0;  
            int colIndex = 0;  
            int colCount = dt.Columns.Count;  
            int rowCount = dt.Rows.Count;
            bool succeed = false;

            try
            {
                ////列名的处理
                for (int i = 0; i < colCount; i++) 
                {  
                    worksheet.Cells[rowIndex, colIndex].PutValue(dt.Columns[i].ColumnName);  
                    colIndex++;  
                }  

                rowIndex++;  
                for (int i = 0; i < rowCount; i++) 
                {  
                    colIndex = 0;  
                    for (int j = 0; j < colCount; j++) 
                    {  
                        worksheet.Cells[rowIndex, colIndex].PutValue(dt.Rows[i][j].ToString());  
                        colIndex++;  
                    }
  
                    rowIndex++;  
                }  

                worksheet.AutoFitColumns();  
                succeed = true;  
            }
            catch (Exception)
            {
                succeed = false;  
            }

           return succeed;  
        }
    }
}