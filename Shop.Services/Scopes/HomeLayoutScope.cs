using AutoMapper;
using Shop.Data.Context;
using Shop.Data.ViewModels.Site;
using Shop.Repo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Scopes
{
    public class HomeLayoutScope
    {
        private readonly IUnitOfWork<DatabaseContext> _db;
        private readonly IMapper _mapper;

        public HomeLayoutScope(IUnitOfWork<DatabaseContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<SiteCategoryViewModel>> GetCategories()
        {
            var resultCategories = await _db.CategoryRepository.GetAsync();
            var categories = _mapper.Map<List<SiteCategoryViewModel>>(resultCategories);

            return categories;
        }

        public async Task<List<SiteSocialViewModel>>  GetSocials()
        {
            var resultSocials = await _db.SocialRepository.GetAsync();
            var socials = _mapper.Map<List<SiteSocialViewModel>>(resultSocials);

            return socials;
        }

        public async Task<List<SiteLicenseViewModel>> GetLicenses()
        {
            var resultLicenses = await _db.LicenseRepository.GetAsync();
            var licenses = _mapper.Map<List<SiteLicenseViewModel>>(resultLicenses);

            return licenses;
        }
    }
}
