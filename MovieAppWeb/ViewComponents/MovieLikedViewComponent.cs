using Microsoft.AspNetCore.Mvc;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Utility;
using System.Security.Claims;

namespace MovieAppWeb.ViewComponents
{
    public class MovieLikedViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieLikedViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SessionConstants.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SessionConstants.SessionCart,
                    _unitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SessionConstants.SessionCart));

            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }


        }
    }
}
