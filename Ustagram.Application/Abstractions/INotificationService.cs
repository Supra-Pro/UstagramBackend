using Ustagram.Domain.DTOs;
using Ustagram.Domain.Model;

namespace Ustagram.Application.Abstractions;

public interface INotificationService
{
    public Task<string> CreateNotification(NotificationDTO NotificationDto);
    public Task<Notification> GetPostNotification(Guid NotificationId);
    public Task<string> UpdateNotification(Guid NotificationId, NotificationDTO NotificationDto);
    public Task<string> DeleteNotification(Guid NotificationId);
    public Task<List<Notification>> GetAllPNotification();
}