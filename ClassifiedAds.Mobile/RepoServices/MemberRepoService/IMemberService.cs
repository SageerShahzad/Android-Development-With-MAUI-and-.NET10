using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MemberRepoService
{
    public interface IMemberService
    {
        Task<(bool Success, string Message)> UpdateProfileAsync(MemberUpdateDto updateDto, FileResult? newPhoto);
        Task<MemberDto?> GetUserProfileAsync(string memberId);
    }
}