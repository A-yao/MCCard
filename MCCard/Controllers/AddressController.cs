using MCCard.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MCCard.Controllers
{
    public class AddressController : BaseController
    {
        static List<AddressData> addressData = new CommonFun().GetAddress();

        public JsonResult GetCity()
        {
            List<AddressData> result = addressData.GroupBy(x => x.City)
                      .Select(grp => grp.First())
                      .ToList();
            return Json(new { Result = result });
        }

        public JsonResult GetTown(string city)
        {
            return Json(new { Result = addressData.Where(x => x.City == city).ToList() });
        }

        public JsonResult GetZipCode(string city, string town)
        {
            return Json(new { Result = addressData.Where(x => x.City == city && x.Township == town).ToList() });
        }
    }

    public class AddressData
    {
        public string City { get; set; }
        public string Township { get; set; }
        public int Zipcode { get; set; }
    }
}