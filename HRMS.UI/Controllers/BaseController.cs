using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HRMS.UI.Controllers;

public abstract class BaseController : Controller
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    protected BaseController(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
    }

    protected async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
    {
        var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);
        await using var writer = new StringWriter();

        var viewResult = _viewEngine.FindView(actionContext, viewName, false);
        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"Unable to find view '{viewName}'.");
        }

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), ModelState)
        {
            Model = model
        };
        var tempData = new TempDataDictionary(HttpContext, _tempDataProvider);
        var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, writer, new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}
