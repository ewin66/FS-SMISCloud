using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    using System.Configuration;
    using System.Text;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Common;
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Web.Http;
    public class VibrationController : ApiController
    {
        /// <summary>
        /// 获取结构物下的传感器列表
        /// </summary>
        /// <param name="structId"> 结构物编号 </param>
        /// <returns> 传感器列表 </returns>
        [AcceptVerbs("Post")]
        [LogInfo("微震计算", false)]
        public object VibrationCalc([FromBody]SensorCond form)
        {
            using (var entity = new SecureCloud_Entities())
            {
                return calc(form).GetData();
            }
        }

        /// <summary>
        /// 矩阵运算,得出震源坐标
        /// </summary>
        /// <param name="items"></param>
        /// <param name="iterationEnd"></param>
        /// <returns></returns>
        private Matrix calc(SensorCond form)
        {
            try
            {
                Matrix os = null;
                ShockData minData = form.items[0];
                int length = form.items.Count - 1;
                foreach (var item in form.items)
                {
                    if (item.t < minData.t)
                    {
                        minData = item;
                    }
                }
                minData.isCalc = false;
                Matrix minMt = new Matrix(1, 4, minData.getArray());
                do
                {
                    double[,] dou = new double[length, 4];
                    double[] yi = new double[length];
                    int count = -1;
                    for (int i = 0; i < form.items.Count; i++)
                    {
                        var item = form.items[i];
                        if (!item.isCalc)
                        {
                            item.isCalc = true;
                            continue;
                        }
                        count++;
                        item.tci = minMt[0, 3] + (Math.Sqrt(Math.Pow(item.x - minMt[0, 0], 2) + Math.Pow(item.y - minMt[0, 1], 2) + Math.Pow(item.z - minMt[0, 2], 2)) / item.speed);
                        yi[count] = (item.T - item.tci);
                        item.Ri = Math.Sqrt(Math.Pow(item.x - minMt[0, 0], 2) + Math.Pow(item.y - minMt[0, 1], 2) + Math.Pow(item.z - minMt[0, 2], 2));
                        double[] itemDou = item.calc(minMt);
                        dou[count, 0] = itemDou[0];
                        dou[count, 1] = itemDou[1];
                        dou[count, 2] = itemDou[2];
                        dou[count, 3] = itemDou[3];
                    }

                    Matrix mtY = new Matrix(1, yi.Length, yi).Transpose();
                    Matrix mtA = new Matrix(dou);
                    Matrix mtAt = mtA.Transpose();
                    Matrix mtANi = (mtAt * mtA);
                    mtANi.InvertSsgj();
                    Matrix resultMtx = mtANi * mtAt * mtY;
                    os = resultMtx.Transpose();
                    length = form.items.Count;
                } while (calcIteration(os, minMt, form.Cond));
                return globalMtx;
            }
            catch (Exception)
            {

            }
            return null;
        }

        private int iterationCount = 0;
        private Matrix prevMtx;
        private Matrix globalMtx;
        private bool calcIteration(Matrix os, Matrix minMt, double cond)
        {
            iterationCount++;
            if (iterationCount > 10000)
            {
                return false;
            }
            var numberA = 0.0;
            if (iterationCount > 1)
            {
                numberA = Math.Sqrt(Math.Pow(os[0, 0] - prevMtx[0, 0], 2) + Math.Pow(os[0, 1] - prevMtx[0, 1], 2) + Math.Pow(os[0, 2] - prevMtx[0, 2], 2));
                if (numberA <= cond)
                {
                    return false;
                }
            }
            prevMtx = os;
            if (null == globalMtx)
            {
                globalMtx = minMt + os;
            }
            else
            {
                globalMtx += os;
            }
            minMt[0, 0] = globalMtx[0, 0];
            minMt[0, 1] = globalMtx[0, 1];
            minMt[0, 2] = globalMtx[0, 2];
            minMt[0, 3] = globalMtx[0, 3];
            return true;
        }
    }
}