using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data.Context;
using Shop.Data.ViewModels.Admin;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presentation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public CommentController(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var resultComments = await _db.CommentRepository.GetAsync(c => c.Status == 0, o => o.OrderByDescending(c => c.DateCreated), "User,Product");
            var comments = _mapper.Map<List<CommentViewModel>>(resultComments);
            return View(comments);
        }

        public async Task<IActionResult> ApprovedComments()
        {
            var resultComments = await _db.CommentRepository.GetAsync(c => c.Status == 1, o => o.OrderByDescending(c => c.DateCreated), "User,Product");
            var comments = _mapper.Map<List<CommentViewModel>>(resultComments);
            return View(comments);
        }

        public async Task<IActionResult> UnApprovedComments()
        {
            var resultComments = await _db.CommentRepository.GetAsync(c => c.Status == -1, o => o.OrderByDescending(c => c.DateCreated), "User,Product");
            var comments = _mapper.Map<List<CommentViewModel>>(resultComments);
            return View(comments);
        }
        #endregion

        #region ApproveComment
        public async Task<IActionResult> ApproveComment(string Id)
        {
            var comment = await _db.CommentRepository.GetAsync(Id);
            if (comment == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost, ActionName("ApproveComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCommentConfirm(string Id)
        {
            if (ModelState.IsValid)
            {
                var comment = await _db.CommentRepository.GetAsync(Id);
                if (comment == null)
                {
                    return NotFound();
                }

                comment.Status = 1;
                _db.CommentRepository.Update(comment);
                await _db.SaveAsync();

                return Redirect("/Admin/Comment");
            }

            return View();
        }
        #endregion

        #region UnApproveComment
        public async Task<IActionResult> UnApproveComment(string Id)
        {
            var comment = await _db.CommentRepository.GetAsync(Id);
            if (comment == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost, ActionName("UnApproveComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnApproveCommentConfirm(string Id)
        {
            if (ModelState.IsValid)
            {
                var comment = await _db.CommentRepository.GetAsync(Id);
                if (comment == null)
                {
                    return NotFound();
                }

                comment.Status = -1;
                _db.CommentRepository.Update(comment);
                await _db.SaveAsync();

                return Redirect("/Admin/Comment");
            }

            return View();
        }
        #endregion
    }
}
