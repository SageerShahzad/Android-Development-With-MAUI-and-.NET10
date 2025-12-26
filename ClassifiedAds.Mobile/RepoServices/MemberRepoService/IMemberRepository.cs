using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MemberRepoService
{
    public interface IMemberRepository
    {
        Task<UserDto?> GetCurrentMemberAsync();
        Task<MemberDto?> GetMemberProfileAsync(string memberId);
        Task<bool> UpdateMemberAsync(MemberUpdateDto memberUpdate);
        Task<PhotoDto?> UploadPhotoAsync(FileResult file);
        Task<bool> SetMainPhotoAsync(int photoId);
    }
}