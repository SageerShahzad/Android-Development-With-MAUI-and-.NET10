using ClassifiedAds.Common.DTOs;
using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Extensions;
using ClassifiedAds.Common.Helpers;
using ClassifiedAds.Common.Interfaces;
using ClassifiedAds.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers;

public class MessagesController(IUnitOfWork uow, IPhotoService photoService) : BaseApiController
{
    [HttpPost]
    // [FromForm] is crucial here to allow File Uploads + JSON data in one request
    public async Task<ActionResult<MessageDto>> CreateMessage([FromForm] CreateMessageDto createMessageDto)
    {
        var sender = await uow.MemberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (recipient == null || sender == null) return BadRequest("Cannot send this message");
        if (sender.Id == createMessageDto.RecipientId) return BadRequest("You cannot send messages to yourself");

        // VALIDATION: Ensure at least Text OR File is present
        if (string.IsNullOrEmpty(createMessageDto.Content) && createMessageDto.File == null)
        {
            return BadRequest("You must send either text or an attachment.");
        }

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        // IMAGE UPLOAD LOGIC
        if (createMessageDto.File != null)
        {

            // 1. DOS FIX: Check File Size (e.g., 5 MB limit)
            const int MaxFileSize = 5 * 1024 * 1024; // 5 MB
            if (createMessageDto.File.Length > MaxFileSize)
            {
                return BadRequest("File size exceeds 5MB limit.");
            }

            // 2. EXECUTION FIX: Check Allowed Extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(createMessageDto.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type. Only image and PDF files are allowed.");
            }
            // Assuming your photoService.AddPhotoAsync returns an object with Url and PublicId
            var uploadResult = await photoService.UploadPhotoAsync(createMessageDto.File);

            if (uploadResult.Error != null)
                return BadRequest(uploadResult.Error.Message);

            message.AttachmentUrl = uploadResult.SecureUrl.AbsoluteUri;
            message.AttachmentPublicId = uploadResult.PublicId;
        }

        uow.MessageRepository.AddMessage(message);

        if (await uow.Complete())
        {
            // We manually map to DTO here because the Projection in Repository 
            // is for database queries, but here we have the entity in memory.
            return Ok(new MessageDto
            {
                Id = message.Id,
                SenderId = sender.Id,
                SenderDisplayName = sender.DisplayName, // Assuming property exists
                SenderImageUrl = sender.Photos.FirstOrDefault(x => x.IsApproved)?.Url,
                RecipientId = recipient.Id,
                RecipientDisplayName = recipient.DisplayName,
                RecipientImageUrl = recipient.Photos.FirstOrDefault(x => x.IsApproved)?.Url,
                Content = message.Content,
                AttachmentUrl = message.AttachmentUrl,
                MessageSent = message.MessageSent
            });
        }

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer(
        [FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();

        return await uow.MessageRepository.GetMessagesForMember(messageParams);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        return Ok(await uow.MessageRepository.GetMessageThread(User.GetMemberId(), recipientId));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();

        var message = await uow.MessageRepository.GetMessage(id);

        if (message == null) return BadRequest("Cannot delete this message");

        if (message.SenderId != memberId && message.RecipientId != memberId)
            return BadRequest("You cannot delete this message");

        if (message.SenderId == memberId) message.SenderDeleted = true;
        if (message.RecipientId == memberId) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            uow.MessageRepository.DeleteMessage(message);
        }

        if (await uow.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}
