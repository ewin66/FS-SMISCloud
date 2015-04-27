using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FS.DbHelper;
using FS.SMIS_Cloud.DAC.DataValidator.Window;
using NUnit.Framework;
using Aspose.Cells;
using FS.SMIS_Cloud.DAC.Task;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Model.Sensors;
using DbType = FS.DbHelper.DbType;

namespace DAC.DataValidator.Test
{

    public class Item
    {
        public int SensorId { get; set; }
        public string ConnectionString { get; set; }
        public List<ConfigInfo> Configs { get; set; }
        public string TableName { get; set; }//表
        public string FiledName { get; set; }//字段
        public string Theme { get; set; }

        public override string ToString()
        {
            return string.Format("S-{0}-{1}", SensorId, Theme);
        }
    }


    [TestFixture]
    public class AlgorithmTest
    {
        public static void GetGuangfuTest(Item item)
        {
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, item.ConnectionString);
            string sql =
                string.Format(
                    @"Select [{0}] as orginDatas from [{1}] where SensorId ={2} and  [CollectTime]>'2015-01-13 00:13:08.000' order by [CollectTime] ",
                    item.FiledName, item.TableName, item.SensorId);
            sqlHelper.ExecuteSql(sql);
            var dt = sqlHelper.Query(sql).Tables[0];
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];
            worksheet.Name = item.ToString();
            var cells = worksheet.Cells;
            var j = 1; //column
            var z = 0;
            cells[0, 0].PutValue("基点2");

            const decimal firstValue = 5.4055m;
            foreach (DataRow row in dt.Rows)
            {
                z++;
                var value = new AnalysisValue((Convert.ToDecimal(row[0])-firstValue)*100);
                cells[z, 0].PutValue(value.RawValue);
            }
            foreach (var config in item.Configs)
            {
                var k = 1; //row
                var window = new ValidateWindow(config);
                foreach (DataRow row in dt.Rows)
                {
                    var value = new AnalysisValue((Convert.ToDecimal(row[0]) - firstValue) * 100);
                    window.ProcessValue(value);
                    cells[k, j].PutValue(value.ValidValue);
                    k++;
                }
                cells[0, j].PutValue(config.ToString());
                j++;
            }
            workbook.Save(item.ToString() + ".xls");
        }

        [Category("MANUAL")]
        [Test]
        public void TestGuangfo()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.01m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 50,
                IsOpenWindow = true
            };
            var config1 = new ConfigInfo()
            {
                WindowSize = 60,
                KThreshold = 0.01m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 60,
                IsOpenWindow = true
            };
            var config2 = new ConfigInfo()
            {
                WindowSize = 40,
                KThreshold = 0.01m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 40,
                IsOpenWindow = true
            };
            var config3 = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.01m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 50,
                IsOpenWindow = true

            };
            var item = new Item();
            var configs = new List<ConfigInfo> { config, config1, config2, config3 };
            var sensorId = new int[] { 934 };
            foreach (int t in sensorId)
            {
                item.SensorId = t;
                item.Configs = configs;
                item.ConnectionString = "server=192.168.1.30;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123;pooling=false";
                item.TableName = "T_DATA_ORIGINAL";
                item.FiledName = "Value1";
                item.Theme = "沉降";
                GetGuangfuTest(item);
            }  
        }
        public static void ProcessItem(Item item)
        {

            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, item.ConnectionString);
            string sql =
                string.Format(
                    @"Select [{0}] as orginDatas from [{1}] where SENSOR_ID ={2} and  [ACQUISITION_DATETIME]>'2014-12-24 22:36:00.000' order by [ACQUISITION_DATETIME] ",
                    item.FiledName, item.TableName, item.SensorId);
            sqlHelper.ExecuteSql(sql);
            var dt = sqlHelper.Query(sql).Tables[0];
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets[0];
            worksheet.Name = item.ToString();
            var cells = worksheet.Cells;
            var j = 1; //column
            var z = 0;
            cells[0, 0].PutValue("orgin");
            foreach (DataRow row in dt.Rows)
            {
                z++;
                var value = new AnalysisValue(Convert.ToDecimal(row[0]));
                cells[z, 0].PutValue(value.RawValue);
            }
            foreach (var config in item.Configs)
            {
                var k = 1; //row
                var window = new ValidateWindow(config);
                foreach (DataRow row in dt.Rows)
                {
                    var value = new AnalysisValue(Convert.ToDecimal(row[0]));
                    window.ProcessValue(value);
                    cells[k, j].PutValue(value.ValidValue);
                    k++;
                }
                cells[0, j].PutValue(config.ToString());
                j++;
            }
            workbook.Save(item.ToString() + ".xls");
        }



        [Category("MANUAL")]
        [Test]
        public void TestXinJiaAn()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.1m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 15,
                IsOpenWindow = true
            };
            var config1 = new ConfigInfo()
            {
                WindowSize =60,
                KThreshold =0.01m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 60,
                IsOpenWindow = true
            };
            var config2 = new ConfigInfo()
            {
                WindowSize =60,
                KThreshold = 0.3m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold =60,
                IsOpenWindow = true
            };
            var config3 = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.3m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 50,
                IsOpenWindow = true

            };
            var item = new Item();
            var configs = new List<ConfigInfo> {config, config1, config2, config3};
            var sensorId=new int[]{1567,1568,1569,1570,1571,1572};
            foreach (int t in sensorId)
            {
                item.SensorId = t;
                item.Configs=configs;
                item.ConnectionString ="server=192.168.1.30;database=xinjiaan;uid=sa;pwd=Fas123;pooling=false";
                item.TableName = "T_THEMES_DEFORMATION_SETTLEMENT";
                item.FiledName = "Press_Original";
                item.Theme = "沉降";
                ProcessItem(item);
            }
        }

        [Test]
        public void TestGetDistanceValues()
        {
            //三个相同的数
            var values = new decimal[] {1, 1, 1};
            var distanceValues = ValidateWindow.GetDistanceValues(values);
            Assert.AreEqual(distanceValues.Length, 3);
            Assert.AreEqual(0, distanceValues[0]);

            //抓住异常
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.GetDistanceValues(null));

            //5个数
            var values1 = new decimal[] {1, 2, 3, 4, 5};
            var distanceValues1 = ValidateWindow.GetDistanceValues(values1);
            Assert.AreEqual(distanceValues1.Length, 10);
            Assert.AreEqual(1, distanceValues1[0]);
            Assert.AreEqual(1, distanceValues1[9]);

            //50个数，差异比较大包括小数
            var values2 = new decimal[]
            {
                2.7m, 2.7123m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            var distanceValues2 = ValidateWindow.GetDistanceValues(values2);
            Assert.AreEqual(distanceValues2.Length, 1225);
            Assert.AreEqual(0.0123, distanceValues2[0]);
            Assert.AreEqual(8.3, distanceValues2[1]);
            Assert.AreEqual(5, distanceValues2[1224]);

        }

        [Test]
        public void TestCalcRValue()
        {
            //5个
            var values = new decimal[] {1, 2, 3, 4, 5};
            var r = ValidateWindow.CalcRValue(values);
            Assert.AreEqual(2, r);

            //抓住异常--公式没有换
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.CalcRValue(null));

            ////50个数据的类型各式各样
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            var r2 = ValidateWindow.CalcRValue(values2);
            Assert.AreEqual(35.48, Math.Round(r2, 2));

        }

        [Test]
        public void TestCalcMeanValue()
        {
            var values = new decimal[] {1, 2, 3, 4, 5};
            var u = ValidateWindow.CalcMeanValue(values);
            Assert.AreEqual(3, u);


            //抓住异常
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.CalcMeanValue(null));

            ////50个数据的类型各式各样
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22.444m, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };

            var u2 = ValidateWindow.CalcMeanValue(values2);
            Assert.AreEqual(24.53, Math.Round(u2, 2));

            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.CalcRValue(new decimal[] {}));
        }

        [Test]
        public void TestGetSdValue()
        {
            var values = new decimal[] {1, 2, 3, 4, 5};
            var sd = ValidateWindow.GetSdValue(values);
            Assert.AreEqual(Math.Round(Math.Sqrt(2), 2), Math.Round(sd, 2));

            //抓住异常
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.GetSdValue(null));


            ////数据的类型各式各样--需要知道我们数据返回的小数点个数！
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22.444m, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            var sd2 = ValidateWindow.GetSdValue(values2);
            Assert.AreEqual(62.4, Math.Round(sd2, 1));

        }

        [Test]
        public void TestGetCoefficientOfVariationValue()
        {
            var values = new decimal[] {1, 2, 3, 4, 5};
            var cv = ValidateWindow.GetCoefficientOfVariationValue(values);
            Assert.AreEqual(0.4714, Math.Round(cv, 4));

            //抓住异常
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.GetCoefficientOfVariationValue(null));

            ////数据的类型各式各样--需要知道我们数据返回的小数点个数！
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22.444m, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            var cv2 = ValidateWindow.GetCoefficientOfVariationValue(values2);
            Assert.AreEqual(2.54, Math.Round(cv2, 2));


            //均值为0的时候
            var values3 = new decimal[] {-2, -2, 2, 2};
            var cv3 = ValidateWindow.GetCoefficientOfVariationValue(values3);
            Assert.AreEqual(2, cv3);

        }

        [Test]
        public void TestInitCalcRValue()
        {
            var values = new decimal[] {1, 2, 3, 4, 5};
            const decimal kT = 0.5m;
            var initR = ValidateWindow.CalcRValue(values, kT);
            Assert.AreEqual(2, initR);

            //抓住异常1
            var values1 = new decimal[] {1, 34, 56, 90, 222, 555};
            Assert.Throws(typeof (UnStableWindowExcepiton),
                () => ValidateWindow.CalcRValue(values1, kT));

            //抓住异常2
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.CalcRValue(null, kT));


            ////50个数据的类型各式各样--需要知道我们数据返回的小数点个数！
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            const decimal kT2 = 3;
            var initR2 = ValidateWindow.CalcRValue(values2, kT2);
            Assert.AreEqual(35.48, Math.Round(initR2, 2));

            //抓住异常1
            const decimal kT3 = 2;
            Assert.Throws(typeof (UnStableWindowExcepiton),
                () => ValidateWindow.CalcRValue(values2, kT3));

        }

        [Test]
        public void TestGetDiscreteCountThanR()
        {
            var values = new decimal[] {1, 2, 3, 4, 5};
            const int value1 = 90;
            const int r = 2;
            var count1 = ValidateWindow.GetDiscreteCount(value1, values, r);
            Assert.AreEqual(5, count1);

            //抓住异常
            Assert.Throws(typeof (InvalidParameterExcepiton),
                () => ValidateWindow.GetDiscreteCount(value1, null, r));

            const int value2 = 3;
            var count2 = ValidateWindow.GetDiscreteCount(value2, values, r);
            Assert.AreEqual(0, count2);

            ////50个数据的类型各式各样
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            const int value4 = 90;
            const int r2 = 2;
            var count3 = ValidateWindow.GetDiscreteCount(value4, values2, r2);
            Assert.AreEqual(50, count3);
        }

        [Test]
        public void TestValidWindow()
        {
            var window = new ValidateWindow();
            for (int i = 0; i < window.WindowSize; i++)
            {
                window.ProcessValue(new AnalysisValue(100));
            }
            var value = new AnalysisValue(100);
            window.ProcessValue(value);
            Assert.AreEqual(true, value.IsValid);
            Assert.AreEqual(100, value.ValidValue);
        }

        [Test]
        public void TestValidWindow1()
        {
            var window = new ValidateWindow();
            var values2 = new decimal[]
            {
                2, 2.7m, 11, 0.5m, 6.9m, 11.88m, 4, 5.8m, 44, 63, 25.7m,
                44, 11, 23, 35, 444, 2, 22, 1, 3, 0, -0.6m, -9, -11, 66, 45, 33, 21, 31, 29, 54, -8.9m,
                3, 6, 9, 8, 10, 11, 21, 14, 16, 17, 19, 21, 11, 13, 23, 1, 3, 8
            };
            for (int i = 0; i < window.WindowSize; i++)
            {
                window.ProcessValue(new AnalysisValue(values2[i]));
            }
            var value = new AnalysisValue(500);
            window.ProcessValue(value);
            Assert.AreEqual(true, value.IsValid);
            Assert.AreEqual(500, value.ValidValue);
        }


        [Test]
        public void TestValidWindowvalueNull()
        {
            var window = new ValidateWindow();
            window.ProcessValue(null);
        }

        [Test]
        public void TestValidWindowReClacR()
        {
            var window = new ValidateWindow();

            for (int i = 0; i < window.WindowSize; i++)
            {
                window.ProcessValue(new AnalysisValue(100)); //稳定数据
                Console.Write(100 + "\n");
            }
            const int size = 100;
            AnalysisValue value = null;
            for (int i = 0; i < size; i++)
            {
                value = new AnalysisValue(500);
                window.ProcessValue(value);
                Console.Write(value.ValidValue + "\n");
            }
            Assert.AreEqual(true, value != null && value.IsValid);
            if (value != null) Assert.AreEqual(500, value.ValidValue);
        }

        [Test]
        public void TestValidWindowClosed()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.01m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = false
            };

            var window = new ValidateWindow(config);

            for (int i = 0; i < 50; i++)
            {
                window.ProcessValue(new AnalysisValue(100)); //稳定数据
            }

            var value = new AnalysisValue(500);
            window.ProcessValue(value);
            Assert.AreEqual(500, value.RawValue);
        }
        
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.1m,
                DiscreteThreshold = 15,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
select top 2000 * from (
　select (case when [DEFLECTION_VALUE] > 20 or [DEFLECTION_VALUE] < 0 then NULL else [DEFLECTION_VALUE] end) as orginDatas  
　from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100) t
where t.orginDatas is not null");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb2()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.5m,
                DiscreteThreshold = 15,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
select top 2000 * from (
　select (case when [DEFLECTION_VALUE] > 20 or [DEFLECTION_VALUE] < 0 then NULL else [DEFLECTION_VALUE] end) as orginDatas  
　from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100) t
where t.orginDatas is not null");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }




        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb1()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.3m,
                DiscreteThreshold = 15,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEFLECTION_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb3()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.3m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEFLECTION_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb4()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.3m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 15,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEFLECTION_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
              //  Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb5()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.3m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 15,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEFLECTION_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
              // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb6()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.5m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 15,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEFLECTION_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_BRIDGE_DEFLECTION] where SENSOR_ID =100 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb7()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.3m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 15,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [CRACK_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_CRACK] where SENSOR_ID =123 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb8()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.2m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 10,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [CRACK_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_CRACK] where SENSOR_ID =123 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            //print value
            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb9()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 20,
                KThreshold = 0.2m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 10,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEEP_DISPLACEMENT_X_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_DEEP_DISPLACEMENT] where SENSOR_ID =34 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestThemeValueFormDb10()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.2m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 30,
                IsOpenWindow = true
            };

            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEEP_DISPLACEMENT_X_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_DEEP_DISPLACEMENT] where SENSOR_ID =34 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        
        [Test]
        public void TestThemeValueFormDb11()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 40,
                KThreshold = 0.2m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 30,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);
            //查数据　，循环处理
            string cs = "server=192.168.1.250;database=DW_iSecureCloud_Empty;uid=sa;pwd=Fas123_;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql = string.Format(@"
       select top 2000 [DEEP_DISPLACEMENT_X_VALUE] as orginDatas  
　     from [T_THEMES_DEFORMATION_DEEP_DISPLACEMENT] where SENSOR_ID =34 ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }

        //青岛地铁的数据过滤
        [Category("MANUAL")]
        [Test]
        public void TestSettlement()
        {
            var config = new ConfigInfo()
            {
                WindowSize =50,
                KThreshold = 0.4m,
                DiscreteThreshold =10,
                ReCalcRValueThreshold = 20,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            //不兼容空格
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1568 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1568()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 0.4m,
                DiscreteThreshold = 10,
                ReCalcRValueThreshold = 30,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1568 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1569()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.5m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1569 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1569_2()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.5m,
                DiscreteThreshold =5,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1569 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1570()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 1,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1570 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1570_2()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 1,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold =40,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1570 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1571()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 0.7m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 27,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1571 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1571_2()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 35,
                KThreshold = 0.7m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold =30,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1571 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
                //Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1572()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 30,
                KThreshold = 1,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 25,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1572 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
        [Category("MANUAL")]
        [Test]
        public void TestSettlement1572_2()
        {
            var config = new ConfigInfo()
            {
                WindowSize = 50,
                KThreshold = 1.6m,
                DiscreteThreshold = 5,
                ReCalcRValueThreshold = 40,
                IsOpenWindow = true
            };
            var window = new ValidateWindow(config);

            string cs = "server=192.168.1.30;database=DW_iSecureCloud_Empty21;uid=sa;pwd=Windows2008;pooling=false";
            ISqlHelper sqlHelper = SqlHelperFactory.Create(DbType.MSSQL, cs);
            string sql =
                string.Format(
                    @"Select top 1000 [SETTLEMENT_VALUE] as orginDatas from [T_THEMES_DEFORMATION_SETTLEMENT] where SENSOR_ID =1572 order by [ACQUISITION_DATETIME] ");
            sqlHelper.ExecuteSql(sql);
            DataTable dt = sqlHelper.Query(sql).Tables[0];

            foreach (DataRow item in dt.Rows)
            {
                var value = new AnalysisValue(Convert.ToDecimal(item[0]));
                window.ProcessValue(value);
               // Console.Write(value.ValidValue + "\n");
            }
        }
    }
}
