using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OptimaJet.Workflow;
using WorkflowLib;

namespace DesignerApp.Controllers
{
    public class DesignerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Obsolete]
        public async Task<IActionResult> Api()
        {
            Stream? filestream = null;
            var isPost = Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase);
            if (isPost && Request.Form.Files != null && Request.Form.Files.Count > 0)
                filestream = Request.Form.Files[0].OpenReadStream();

            var pars = new NameValueCollection();
            foreach (var q in Request.Query)
            {
                pars.Add(q.Key, q.Value.First());
            }


            if (isPost)
            {
                var parsKeys = pars.AllKeys;
                //foreach (var key in Request.Form.AllKeys)
                foreach (string key in Request.Form.Keys)
                {
                    if (!parsKeys.Contains(key))
                    {
                        pars.Add(key, Request.Form[key]);
                    }
                }
            }

            (string res, bool hasError) = await WorkflowInit.Runtime.DesignerAPIAsync(pars, filestream);

            var operation = pars["operation"]?.ToLower();
            if (operation == "downloadscheme" && !hasError)
                return File(Encoding.UTF8.GetBytes(res), "text/xml");
            else if (operation == "downloadschemebpmn" && !hasError)
                return File(UTF8Encoding.UTF8.GetBytes(res), "text/xml");

            return Content(res);
        }
    }
}