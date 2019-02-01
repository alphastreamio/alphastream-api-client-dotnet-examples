using AlphaStream.ApiClient.Watchlists;
using AlphaStream.ApiClient.Watchlists.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Environment = AlphaStream.ApiClient.Core.Environment;

namespace MvcExample.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly WatchlistService _watchlistService;
        private const string UserId = "test-example-user-1";

        public WatchlistController()
        {
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            _watchlistService = WatchlistApiClientFactory.Create(Environment.Production, clientId, clientSecret);
        }

        public async Task<ActionResult> Index(bool saved = false, bool? deleted = null)
        {
            TempData.Clear();

            var usersWatchlists = await _watchlistService.GetAllWatchlistsAsync(req => req.UserId = UserId);

            TempData.Add("saved", saved.ToString().ToLower());

            if (deleted.HasValue)
                TempData.Add("deleted", deleted.ToString().ToLower());

            return View(usersWatchlists);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateWatchlistRequest() { UserId = UserId });
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateWatchlistRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var saveResult = await _watchlistService.CreateWatchlistAsync(model);
           
            if (saveResult.IsError)
            {
                ModelState.AddModelError("", "There was an error trying to create this watchlist");
                return View(model);
            }

            return RedirectToAction("Index", new { saved = true });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string watchlistId)
        {
            var deleteResponse = await _watchlistService.DeleteWatchlistAsync(req =>
            {
                req.UserId = UserId;
                req.WatchlistIds.Add(watchlistId);
            });

            return RedirectToAction("Index", "Watchlist", new { deleted = !deleteResponse.IsError });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string watchlistId)
        {
            var watchlist = await _watchlistService.GetWatchlistsAsync(req =>
            {
                req.UserId = UserId;
                req.WatchlistId = watchlistId;
            });

            if (watchlist.IsError)
                return HttpNotFound();

            return View(watchlist.Payload);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Watchlist model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var updateResult = await _watchlistService.UpdateWatchlistAsync(req =>
            {
                req.UserId = UserId;
                req.WatchlistId = model.WatchlistId;
                req.Name = model.Name;
            });

            if (updateResult.IsError)
            {
                ModelState.AddModelError("", "There was an error trying to update this watchlist");
                return View(model);
            }

            return RedirectToAction("Edit", new { Updated = true });
        }

        [HttpPost]
        public async Task<ActionResult> AddMember(string watchlistId, string entityId)
        {
            var addMemberResult = await _watchlistService.AddWatchlistMembersAsync(req =>
            {
                req.UserId = UserId;
                req.WatchlistId = watchlistId;
                req.AddExternalMember(entityId);
            });

            return RedirectToAction("Edit", new { WatchlistId = watchlistId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteMember(string watchlistId, string entityId)
        {
            var removeMemberResult = await _watchlistService.DeleteWatchlistMembersAsync(req =>
            {
                req.UserId = UserId;
                req.WatchlistId = watchlistId;
                req.AddExternalMember(entityId);
            });

            return RedirectToAction("Edit", new { WatchlistId = watchlistId });
        }
    }
}