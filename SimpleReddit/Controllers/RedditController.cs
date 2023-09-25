using Microsoft.AspNetCore.Mvc;
using SimpleReddit.Contracts;
using SimpleReddit.Models;

namespace SimpleReddit.Controllers
{
    public class RedditController : Controller
    {
        private readonly ILogger<RedditController> _logger;

        private readonly IAppService _appService;

        public RedditController(ILogger<RedditController> logger, IAppService appService)
        {
            this._logger = logger;
            this._appService = appService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString = "Dallas", string category = "all")
        {
            var products = await GetSubreddit(ref searchString, category);
            return this.View(products);
        }

        [HttpGet]
        public JsonResult LoadSubredditAjax(string searchString = "Dallas", string category = "all")
        {
            var products = GetSubreddit(ref searchString, category);
            return Json(products.Result.PostDTOs);
        }

        private Task<SubRedditResponse> GetSubreddit(ref string searchString, string category)
        {
            this.ViewBag.SearchString = searchString;
            this.ViewBag.Category = category;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = $"searchString={searchString}";
                if (!string.IsNullOrEmpty(category))
                {
                    searchString = $"{searchString}&subReddit={category}";
                }
            }

            var products = this._appService.GetSubRedditAsync(searchString);
            return products;
        }
    }
}
