using ClassifiedAds.Common.Interfaces;
using ClassifiedAds.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Common.Data;

public class UnitOfWork(ClassifiedAdsDbContext context) : IUnitOfWork
{
    private IMemberRepository? _memberRepository;
    private IMessageRepository? _messageRepository;
    private ILikesRepository? _likesRepository;
    private IPhotoRepository? _photoRepository;

    public IMemberRepository MemberRepository => _memberRepository
        ??= new MemberRepository(context);

    public IMessageRepository MessageRepository => _messageRepository
        ??= new MessageRepository(context);

    public ILikesRepository LikesRepository => _likesRepository
        ??= new LikesRepository(context);

    public IPhotoRepository PhotoRepository => _photoRepository
        ??= new PhotoRepository(context);

    public async Task<bool> Complete()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occured while saving changes", ex);
        }
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
