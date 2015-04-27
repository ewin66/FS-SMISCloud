namespace FS.SMIS_Cloud.NGET.DataValidator.Window
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using log4net;

    using Newtonsoft.Json;

    public class SimpleWindow
    {
        public List<AnalysisValue> WindowDatas { get; set; }
        public int InvalidCount { get; set; }
    }
    /// <summary>
    /// 校验值
    /// </summary>
    public class AnalysisValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rawValue">原始值</param>
        /// <param name="isValid"></param>
        public AnalysisValue(decimal rawValue, bool isValid = true)
        {
            this.RawValue = rawValue;
            this.IsValid = isValid;
        }        

        /// <summary>
        /// 原始值
        /// </summary>
        public decimal RawValue { get; set; }

        /// <summary>
        /// 是否通过校验
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 校验后值
        /// </summary>
        public decimal ValidValue { get; set; }

        /// <summary>
        /// 返回过滤的信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var analysisValue = string.Format("Raw value:{0,15}, Valid value:{1,15}, Is valid:{2,5}", this.RawValue,
                this.ValidValue,
                this.IsValid);
            return analysisValue;
        }
    }


    ///<summary>
    /// 配置参数
    /// </summary>
    public class ConfigInfo
    {
        /// <summary>
        /// 窗口大小的设置
        /// </summary>
        public int WindowSize { get; set; }

        /// <summary>
        /// 是否过滤
        /// </summary>
        public bool IsOpenWindow { get; set; }

        /// <summary>
        /// 离散系数，如果待校验值与滑窗中的其他值欧式距离大于此数，则认为是无效值
        /// </summary>
        public int DiscreteThreshold { get; set; }

        /// <summary>
        /// 重新计算R值得阈值，当滑窗中无效值数量大于此阈值时，重新计算R值
        /// </summary>
        public int ReCalcRValueThreshold { get; set; }

        /// <summary>
        /// 变异系数
        /// </summary>
        public decimal KThreshold { get; set; }

        /// <summary>
        /// 是否打印
        /// </summary> 
        public bool NeedLog { get; set; }


        /// <summary>
        /// 传感器Id
        /// </summary> 
        public int SensorId { get; set; }

        /// <summary>
        /// 过滤数据的索引
        /// </summary> 
        public int ValueIndex { get; set; }

        public override string ToString()
        {
            return string.Format("K:{0}/R:{1}/D:{2}/W:{3}", this.KThreshold, this.ReCalcRValueThreshold, this.DiscreteThreshold,
                this.WindowSize);
        }
    }


    /// <summary>
    /// 窗口校验实现
    /// </summary>
    public class ValidateWindow
    {
        private const Decimal Rinitvalue = -1;

        private const Decimal SeedInit = -100000;

        private const string DataDirectory = "WindowData";

        private static ILog _log = LogManager.GetLogger("DAC.ValidateWindow");

        /// <summary>
        /// 计算一组数据的欧式距离的集合
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal[] GetDistanceValues(decimal[] values)
        {
            if (values != null && values.Length != 0)
            {
                var distance = new List<decimal>(); //欧式距离
                for (var i = 0; i < values.Length - 1; i++)
                {
                    for (var j = i + 1; j < values.Length; j++)
                    {
                        distance.Add(Math.Abs(values[i] - values[j])); //为了数据真实性，需要使用decimal类型
                    }
                }

                return distance.ToArray();
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid GetDistanceValues data array.");
            }
        }

        /// <summary>
        /// 计算一组数据的R
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal CalcRValue(decimal[] values)
        {
            if (values != null && values.Length != 0)
            {
                var distances = GetDistanceValues(values);
                decimal sum = distances.Aggregate<decimal, decimal>(0, (current, t) => current + t);
                return sum/distances.Length;
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid CalcRValue data array.");
            }
        }

        /// <summary>
        /// 计算一组数据的均值
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal CalcMeanValue(decimal[] values)
        {
            if (values != null && values.Length != 0)
            {
                var sum = values.Aggregate<decimal, decimal>(0, (current, t) => current + t);
                return sum/values.Length;
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid CalcMeanValue data array.");
            }
        }

        /// <summary>
        /// 计算一组数据的标准差
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal GetSdValue(decimal[] values)
        {
            if (values != null && values.Length != 0)
            {
                var u = CalcMeanValue(values);
                var s = values.Aggregate<decimal, decimal>(0, (current, t) => current + (t - u)*(t - u));
                var su = s/values.Length;
                return Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(su)));
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid GetSdValue data array.");
            }
        }

        /// <summary>
        /// 计算一组数据的CV
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal GetCoefficientOfVariationValue(decimal[] values)
        {
            if (values != null && values.Length != 0)
            {
                var u = CalcMeanValue(values);
                var sd = GetSdValue(values);
                if (u != 0)
                {
                    return sd/Math.Abs(u);
                }
                else
                {
                    return sd;
                }
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid GetCoefficientOfVariationValue data array.");
            }
        }

        /// <summary>
        /// 计算一组数据的R值--初始化
        /// </summary>
        /// <param name="values">数据</param>
        /// <param name="kThreshold">变异系数(或者标准差），该组数据必须满足此系数才进行R值计算</param>
        /// <returns></returns>
        public static decimal CalcRValue(decimal[] values, decimal kThreshold)
        {
            if (values != null && values.Length != 0)
            {
                var cv = GetCoefficientOfVariationValue(values);
                if (cv <= kThreshold)
                {
                    return CalcRValue(values);
                }
                else
                {
                    throw new UnStableWindowExcepiton("Un stable window.");
                }
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid Init CalcRValue data array.");
            }
        }

        /// <summary>
        /// 计算一个数据与一组数据超过R值的数量
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static int GetDiscreteCount(decimal value, decimal[] values, decimal rValue)
        {
            if (values != null)
            {
                var greaterThanR = values.Select(t => Math.Abs(t - value)).Where(dij => dij > rValue).ToList();
                return greaterThanR.Count;
            }
            else
            {
                throw new InvalidParameterExcepiton("Invalid  GetDiscreteCount data array.");
            }
        }

        [JsonProperty(PropertyName = "WindowDatas")] private List<AnalysisValue> _windowDatas =
            new List<AnalysisValue>();

        private int _windowSize;

        [JsonProperty(PropertyName = "InvalidCount")] private int _invalidCount = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="windowSize">滑窗大小</param>
        /// <param name="kThreshold">变异系数</param>
        /// <param name="discreteThreshold">离散系数，如果待校验值与滑窗中的其他值欧式距离大于此数，则认为是无效值</param>
        /// <param name="reCalcRValueThreshold">重新计算R值得阈值，当滑窗中无效值数量大于此阈值时，重新计算R值</param>
        /// <param name="sensorId">传感器Id</param>
        /// <param name="valueIndex">过滤数据的索性</param>
        public ValidateWindow(int windowSize = 50, decimal kThreshold = 0.01m, int discreteThreshold = 10,
                              int reCalcRValueThreshold = 25, int sensorId =1, int valueIndex = 1)
            : this(new ConfigInfo
                {
                    WindowSize = windowSize,
                    KThreshold = kThreshold,
                    DiscreteThreshold = discreteThreshold,
                    ReCalcRValueThreshold = reCalcRValueThreshold,
                    IsOpenWindow = true,
                    NeedLog = false, //默认为不打印 
                    SensorId = sensorId,
                    ValueIndex = valueIndex,
                })
        {
        }


        /// <summary>
        /// 构造函数--数据库传过来的参数
        /// </summary>
        /// <param name="values">配置信息</param>
        public ValidateWindow(ConfigInfo values)
        {
            if (values == null)
            {
                return;
            }
            this.WindowSize = values.WindowSize;
            this.DiscreteThreshold = values.WindowSize < values.DiscreteThreshold
                                    ? values.WindowSize
                                    : values.DiscreteThreshold;
            this.ReCalcRValueThreshold = values.WindowSize < values.ReCalcRValueThreshold
                                        ? values.WindowSize
                                        : values.ReCalcRValueThreshold;
            this.R = Rinitvalue;
            this.Seed = SeedInit;
            this.KThreshold = values.KThreshold;
            this.IsOpenWindow = values.IsOpenWindow;
            this.NeedLog = values.NeedLog;
            this.SensorId = values.SensorId;
            this.ValueIndex = values.ValueIndex;
            //初始化的时候从文件里面读数据
            this.Load();
            //进程结束时把数据写到文件中
            FS.SMIS_Cloud.Services.ConsoleCtrlManager.ConsoleCtrlManager.Instance.Exit += this.Dump;
        }

        private void Load()
        {
            try
            {
                var dataFile = this.GetDataFile();
                if (System.IO.File.Exists(dataFile))
                {
                    //反序列化
                    var simpleWindow = JsonConvert.DeserializeObject<SimpleWindow>(System.IO.File.ReadAllText(dataFile));
                    this._windowDatas = simpleWindow.WindowDatas;
                    this._invalidCount = simpleWindow.InvalidCount;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to read dumped data.", ex);
            }
        }

        [JsonIgnore]
        public bool IsOpenWindow { get; set; }

        [JsonIgnore]
        public int SensorId { get; set; }

        [JsonIgnore]
        public int ValueIndex { get; set; }

        /// <summary>
        /// 窗口容量
        /// </summary>
        [JsonIgnore]
        public int WindowSize
        {
            get { return this._windowSize; }
            set
            {
                this._windowSize = value;
                if (this._windowDatas.Count > this._windowSize)
                {
                    var windowExtraItems = this._windowDatas.Count - this._windowSize; //窗口超了
                    this._windowDatas.RemoveRange(0, windowExtraItems);
                }
            }
        }

        /// <summary>
        /// 判断异常点的标准
        /// </summary>
        [JsonIgnore]
        public int DiscreteThreshold { get; set; }

        /// <summary>
        /// 重新计算R的标准
        /// </summary>
        [JsonIgnore]
        public int ReCalcRValueThreshold { get; set; }

        /// <summary>
        /// 数据稳定的标准
        /// </summary>
        [JsonIgnore]
        public decimal KThreshold { get; set; }

        /// <summary>
        /// 窗口的R值
        /// </summary>
        [JsonIgnore]
        public decimal R { get; private set; }

        /// <summary>
        /// 数据的初值
        /// </summary>
        [JsonIgnore]
        public decimal Seed { get; private set; }

        ///<summary>
        /// 打印窗口信息
        /// </summary>
        [JsonIgnore]
        public bool NeedLog { get; set; }

        /// <summary>
        /// 初始化稳定窗口
        /// </summary>
        /// <param name="seed"></param>
        //public void ReCreateWindow(decimal seed)
        //{
        //    if (seed == SeedInit)
        //    {
        //    }
        //    _windowDatas.Clear();
        //    for (var i = 0; i < _windowSize; i++)
        //    {
        //        _windowDatas.Add(new AnalysisValue(seed));
        //    }
        //}

        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <param name="value"></param>
        public void ProcessValue(AnalysisValue value)
        {
            if (this.IsOpenWindow)
            {
                _log.Debug(this.ToSimpleString());
            }
            if (value == null)
            {
                return;
            }
            //复制一份放在窗口里面
            var tempValue = new AnalysisValue(value.RawValue, value.IsValid);
            tempValue.ValidValue = tempValue.RawValue;
            //未过滤之前数据就是原始值
            value.ValidValue = value.RawValue;
            try
            {
                //考虑到先对数据过滤，后面又暂停了！
                if (!this.IsOpenWindow)
                {
                    if (this._windowDatas.Count < this._windowSize)
                    {
                        this.InsertDate(tempValue);
                    }
                    else
                    {
                        this.RemoveInsertData(tempValue);
                    }
                    return;
                }

                if (this._windowDatas.Count == this._windowSize && this._windowDatas.Count > 0) //窗口已满,有数据
                {
                    //包含了窗口和变异系数的变化
                    if (this.R == Rinitvalue)
                    {
                        this.R = ValidateWindow.CalcRValue(this.RawValues, this.KThreshold); //初始化
                    }
                    if (this.ValidateValue(tempValue) == false) //值无效
                    {
                        value.IsValid = tempValue.IsValid;
                        value.ValidValue = tempValue.ValidValue;
                        if (this._invalidCount++ >= this.ReCalcRValueThreshold)
                        {
                            this.RebuildWindow();
                        }
                    }
                    //调整位置
                    var cv = ValidateWindow.GetCoefficientOfVariationValue(this.ValidValues);
                    if (cv > this.KThreshold)
                    {
                        this.R = ValidateWindow.CalcRValue(this.RawValues, this.KThreshold); //初始化
                    }
                    if (this.NeedLog)
                    {
                        //打印出数据
                        _log.Debug(this.ToString());
                        _log.Debug(tempValue.ToString());
                    }
                }
                else //窗口未满
                {
                    this.InsertDate(tempValue);
                }
            }
            catch (UnStableWindowExcepiton ex)
            {
                //不稳定怎么处理--继续滑动窗口
                this.RemoveInsertData(tempValue);
            }
            //Dump(null, null);
        }

        private void RebuildWindow()
        {
            //重新计算r
            this.R = ValidateWindow.CalcRValue(this.RawValues); //计算R值
            var cv = ValidateWindow.GetCoefficientOfVariationValue(this.RawValues);
            if (cv < this.KThreshold)
            {
                this._invalidCount = 0;
                foreach (var value in this._windowDatas)
                {
                    value.ValidValue = value.RawValue;
                    value.IsValid = true;
                }
            }
        }

        private void InsertDate(AnalysisValue value)
        {
            value.IsValid = true;
            value.ValidValue = value.RawValue;
            this._windowDatas.Add(value);
        }

        private void RemoveInsertData(AnalysisValue value)
        {
            // value.IsValid = true;
            //value.ValidValue = value.RawValue;
            this._windowDatas.RemoveAt(0);
            this._windowDatas.Add(value);
        }

        private bool ValidateValue(AnalysisValue value)
        {
            var validValues = this.ValidValues;
            if (ValidateWindow.GetDiscreteCount(value.RawValue, validValues, this.R) > this.DiscreteThreshold)
            {
                value.IsValid = false;
                value.ValidValue = ValidateWindow.CalcMeanValue(validValues);
            }
            else
            {
                value.IsValid = true;
                value.ValidValue = value.RawValue;
            }
            //过滤完就填充
            this._windowDatas.RemoveAt(0);
            this._windowDatas.Add(value);
            return value.IsValid;
        }

        [JsonIgnore]
        public decimal[] ValidValues
        {
            get { return this._windowDatas.Select(v => v.ValidValue).ToArray(); }
        }

        [JsonIgnore]
        public decimal[] RawValues
        {
            get { return this._windowDatas.Select(v => v.RawValue).ToArray(); }
        }


        public string ToSimpleString()
        {
            var windowInfo = new StringBuilder();
            windowInfo.Append("Window infomation:\r\n");
            windowInfo.AppendFormat("{0,15}", "SensorId");
            windowInfo.AppendFormat("{0,15}", "ValueIndex");
            windowInfo.AppendFormat("{0,15}", "WindowSize");
            windowInfo.AppendFormat("{0,15}", "KT");
            windowInfo.AppendFormat("{0,15}", "DT");
            windowInfo.AppendFormat("{0,15}", "ReCalcR");
            windowInfo.AppendFormat("{0,15}", "IsOpen");
            windowInfo.AppendFormat("{0,15}", "NeedLog");
            windowInfo.AppendFormat("{0,15}", "Count");
            windowInfo.Append("\r\n");
            windowInfo.AppendFormat("{0,15}", this.SensorId);
            windowInfo.AppendFormat("{0,15}", this.ValueIndex);
            windowInfo.AppendFormat("{0,15}", this.WindowSize);
            windowInfo.AppendFormat("{0,15}", this.KThreshold);
            windowInfo.AppendFormat("{0,15}", this.DiscreteThreshold);
            windowInfo.AppendFormat("{0,15}", this.ReCalcRValueThreshold);
            windowInfo.AppendFormat("{0,15}", this.IsOpenWindow);
            windowInfo.AppendFormat("{0,15}", this.NeedLog);
            windowInfo.AppendFormat("{0,15}", this._windowDatas.Count);
            windowInfo.Append("\r\n");
            return windowInfo.ToString();
        }

        public override string ToString()
        {
            var allInfos = new StringBuilder();
            var windowInfo = this.ToSimpleString();

            allInfos.Append(windowInfo);
            if (this._windowDatas.Count > 0)
            {
                var data = new StringBuilder();
                data.Append("Data:" + "\r\n");
                var column = 5;
                for (int i = 0; i < column; i++)
                {
                    data.Append(string.Format("{0,33} ", i));
                }
                data.Append("\r\n");
                for (var i = 0; i < this._windowDatas.Count; i++)
                {
                    if ((i + 1)%column == 0)
                    {
                        data.Append(string.Format("{0,15}/{1,15}/{2,1}|", this._windowDatas[i].RawValue,
                                                  this._windowDatas[i].ValidValue, this._windowDatas[i].IsValid ? 0 : 1))
                            .Append("\r\n");
                    }
                    else
                    {
                        data.Append(string.Format("{0,15}/{1,15}/{2,1}|", this._windowDatas[i].RawValue,
                                                  this._windowDatas[i].ValidValue, this._windowDatas[i].IsValid ? 0 : 1));
                    }
                }
                allInfos.Append(data);
            }
            return allInfos.ToString();
        }

        public void Dump()
        {
            try
            {
                var dataFile = this.GetDataFile();
                var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataDirectory);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                var simpleWindow = new SimpleWindow
                    {
                        WindowDatas = this._windowDatas,
                        InvalidCount = this._invalidCount
                    };
                //序列化、格式化
                var json = JsonConvert.SerializeObject(simpleWindow, Formatting.Indented);
                System.IO.File.WriteAllText(dataFile, json);
                _log.InfoFormat("数据保存完成！");
            }
            catch (Exception ex)
            {
                _log.Error("Failed to dump window.", ex);
            }
        }

        public string GetDataFile()
        {
            var dataFile = string.Format("{0}-{1}.json", this.SensorId, this.ValueIndex);
            dataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataDirectory, dataFile);
            return dataFile;
        }
    }


    /// <summary>
    /// 异常
    /// </summary>
    public class UnStableWindowExcepiton : Exception
    {
        public UnStableWindowExcepiton(string exceptionMsg) : base(exceptionMsg)
        {
        }
    }


    public class InvalidParameterExcepiton : Exception
    {
        public InvalidParameterExcepiton(string exceptionMsg)
            : base(exceptionMsg)
        {

        }
    }
}
