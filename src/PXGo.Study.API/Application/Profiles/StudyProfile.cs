using AutoMapper;
using PXGo.Study.API.Application.Models;
using PXGo.Study.API.ViewModels;

namespace PXGo.Study.API.Application.Profiles;

public class StudyProfile : Profile
{
    public StudyProfile()
    {
        /*建立合併模型*/
        CreateMap<MessageModel, MessageVo>()
            .ForMember(d => d.TypeTwo, opt => opt.Ignore()); // 忽略TypeTwo 參樹的合併
    }
}
