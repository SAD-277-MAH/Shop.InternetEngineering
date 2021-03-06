﻿using AutoMapper;
using Shop.Data.Models;
using Shop.Data.ViewModels.Admin;
using Shop.Data.ViewModels.Panel;
using Shop.Data.ViewModels.Site;
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

            CreateMap<License, LicenseViewModel>();
            CreateMap<LicenseViewModel, License>();

            CreateMap<User, UserDetailsViewModel>();
            CreateMap<User, UserFullDetailsViewModel>();

            CreateMap<Address, AddressViewModel>();
            CreateMap<AddressViewModel, Address>();
            CreateMap<AddressAddViewModel, Address>();

            CreateMap<Order, OrderDetailsViewModel>();
            CreateMap<OrderDetail, OrderDetailsDetailViewModel>()
                .ForMember(dest => dest.Name, opt =>
                 {
                     opt.MapFrom(src => src.Product.Name);
                 })
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Product.PhotoUrl);
                });
            CreateMap<CouponOrder, OrderDetailsCouponViewModel>()
                .ForMember(dest => dest.Code, opt =>
                {
                    opt.MapFrom(src => src.Coupon.Code);
                });
            CreateMap<Address, AddressSelectListViewModel>()
                .ForMember(dest => dest.Address, opt =>
                {
                    opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Unit) ?
                    $"{src.Province}, {src.City}, {src.PostalAddress}, پلاک {src.NO}, کد پستی: {src.PostalCode}" :
                    $"{src.Province}, {src.City}, {src.PostalAddress}, پلاک {src.NO}, واحد {src.Unit}, کد پستی: {src.PostalCode}"
                    );
                });

            CreateMap<Factor, FactorPanelViewModel>()
                .ForMember(dest => dest.DateCreated, opt =>
                {
                    opt.MapFrom(src => src.DateCreated.ToShamsiDateTime());
                });
            CreateMap<Factor, FactorAdminViewModel>()
                .ForMember(dest => dest.DateCreated, opt =>
                {
                    opt.MapFrom(src => src.DateCreated.ToShamsiDateTime());
                });
            CreateMap<Factor, PaymentAdminViewModel>()
                .ForMember(dest => dest.DateCreated, opt =>
                {
                    opt.MapFrom(src => src.DateCreated.ToShamsiDateTime());
                });

            CreateMap<Category, SiteCategoryViewModel>();
            CreateMap<Social, SiteSocialViewModel>();
            CreateMap<License, SiteLicenseViewModel>();
            CreateMap<Product, ProductCartViewModel>();
            CreateMap<Product, ProductPageViewModel>()
                .ForMember(dest => dest.CategoryName, opt =>
                {
                    opt.MapFrom(src => src.Category.Name);
                });
            CreateMap<ProductImage, ProductImageViewModel>();

            CreateMap<Comment, SiteCommentViewModel>()
                .ForMember(dest => dest.FullName, opt =>
                {
                    opt.MapFrom(src => src.User.FullName);
                })
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.User.PhotoUrl);
                })
                .ForMember(dest => dest.Date, opt =>
                {
                    opt.MapFrom(src => src.DateCreated.ToShamsiDateTime());
                });
            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.Date, opt =>
                {
                    opt.MapFrom(src => src.DateCreated.ToShamsiDateTime());
                })
                .ForMember(dest => dest.FullName, opt =>
                {
                    opt.MapFrom(src => src.User.FullName);
                })
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.MapFrom(src => src.User.Email);
                })
                .ForMember(dest => dest.ProductName, opt =>
                {
                    opt.MapFrom(src => src.Product.Name);
                });
        }
    }
}
