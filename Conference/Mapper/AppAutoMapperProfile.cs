﻿using AutoMapper;
using Conference.Models.ViewModels;
using Conferency.Data.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Conference.Mapper
{
    public class AppAutoMapperProfile : Profile
    {
        public AppAutoMapperProfile()
        {
            CreateMap<RegistrationViewModel, User>();          
        }
    }
}