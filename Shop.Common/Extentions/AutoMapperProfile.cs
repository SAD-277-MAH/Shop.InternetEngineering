using AutoMapper;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Extentions
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Setting, SiteSettingViewModel>();
            CreateMap<SiteSettingViewModel, Setting>();

            CreateMap<Setting, MessageSenderSettingViewModel>();
            CreateMap<MessageSenderSettingViewModel, Setting>();

            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            CreateMap<Product, ProductAddViewModel>();
            CreateMap<Product, ProductEditViewModel>();
            CreateMap<ProductAddViewModel, Product>();
            CreateMap<ProductEditViewModel, Product>();

            CreateMap<Social, SocialViewModel>();
            CreateMap<SocialViewModel, Social>();
        }
    }
}
