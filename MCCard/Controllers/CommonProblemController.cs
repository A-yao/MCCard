using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data;

namespace MCCard.Controllers
{
    public class CommonProblemController : BaseController
    {
        // GET: CommonProblem
        public ActionResult Index()
        {

            Dictionary<string, string> imgs = common.LoadSubBanner(MCCard.Common.CommonFun.SubBanner.CommonProblemBanner, MCCard.Common.CommonFun.SubBanner.CommonProblemMobileBanner);
            ViewBag.MobileImg = imgs.Values.First();
            ViewBag.Img = imgs.Keys.First();

            //Select ss = new Select();
            string strSql = string.Empty;
            DataTable dt_problem_type = null, dt_problem_detail = null;
            try
            {
                strSql = common.Select.CommonProblemType();
                dt_problem_type = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql);

                strSql = common.Select.CommonProblemDetail();
                dt_problem_detail = common.DBConn.GeneralSqlCmd.ExecuteToDataTable(strSql);
                
                var model = new List<QualityAssuranceAllModel>();
                var qa = new List<QualityAssuranceModel>();

                if (dt_problem_type != null && dt_problem_type.Rows.Count > 0)
                {
                    for (int d = 0; d < dt_problem_detail.Rows.Count; d++)
                    {
                        qa.Add(new QualityAssuranceModel
                        {
                            TypeID = dt_problem_detail.Rows[d]["TypeID"].ToString(),
                            Seq = d.ToString(),// dt_problem_detail.Rows[d]["Seq"].ToString(),
                            Question = dt_problem_detail.Rows[d]["Question"].ToString(),
                            Answer = dt_problem_detail.Rows[d]["Answer"].ToString()
                        });
                    }
                    for (int t = 0; t < dt_problem_type.Rows.Count; t++)
                    {
                        
                        model.Add(new QualityAssuranceAllModel
                        {
                            Question = qa,
                            Title = dt_problem_type.Rows[t]["TypeName"].ToString(),
                            Seq = t.ToString(),//dt_problem_type.Rows[t]["Seq"].ToString(),
                            UID = dt_problem_type.Rows[t]["UID"].ToString()
                        });
                    }
                }
                ViewData.Model = model;
            }
            catch (System.Exception ex) {
                common.WriteLog(Log.Mode.LogMode.ERROR, ex.ToString());
            }
            finally
            {
                common.CloseConn();
            }      

            return View();
        }
    }

    public class QualityAssuranceAllModel
    {
        public List<QualityAssuranceModel> Question { get; set; }
        public string Title { get; set; }
        public string Seq { get; set; }
        public string UID { get; set; }
    }
    public class QualityAssuranceModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Seq { get; set; }
        public string TypeID { get; set; }
    }
}