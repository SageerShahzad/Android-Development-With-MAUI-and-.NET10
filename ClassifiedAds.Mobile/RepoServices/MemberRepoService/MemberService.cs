using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MemberRepoService
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<MemberDto?> GetUserProfileAsync(string memberId)
        {
            return await _memberRepository.GetMemberProfileAsync(memberId);
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(MemberUpdateDto updateDto, FileResult? newPhoto)
        {
            // 1. Update Text Fields
            bool textUpdateSuccess = await _memberRepository.UpdateMemberAsync(updateDto);
            if (!textUpdateSuccess)
            {
                // If this fails, it's likely a duplicate name/email or server error
                return (false, "Failed to update profile info. Username or Email might be taken.");
            }

            // 2. Upload Photo (if one was selected)
            if (newPhoto != null)
            {
                var photoDto = await _memberRepository.UploadPhotoAsync(newPhoto);

                if (photoDto != null)
                {
                    // 3. Set as Main Photo
                    // The API 'AddPhoto' endpoint creates the photo. 
                    // We must then explicitly call 'set-main-photo' to update the user's avatar.
                    bool setMainSuccess = await _memberRepository.SetMainPhotoAsync(photoDto.Id);

                    if (!setMainSuccess)
                        return (true, "Profile updated, but failed to set new photo as main.");
                }
                else
                {
                    return (true, "Profile updated, but photo upload failed.");
                }
            }

            return (true, "Profile updated successfully!");
        }
    }
}