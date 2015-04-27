using System.Linq;
using System.Web.Http;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.DAU.Controllers
{
    using FreeSun.FS_SMISCloud.Server.CloudApi.DAL;
    using FreeSun.FS_SMISCloud.Server.CloudApi.Log;

    public class DtuProductController : ApiController
    {
        [AcceptVerbs("Get")]
        [LogInfo("获取dtu产品列表", false)]
        public object GetDtuProducts()
        {
            using (var db = new SecureCloud_Entities())
            {
                var list = db.T_DIM_DTU_PRODUCT.ToList();

                return
                    list.GroupBy(p => p.DtuFactory)
                        .Select(
                            f =>
                            new
                                {
                                    dtuFactory = f.Key,
                                    models =
                                f.Select(
                                    p =>
                                    new { productId = p.ProductId, dtuModel = p.DtuModel, networkType = p.NetworkType })
                                });
            }
        }
    }
}
