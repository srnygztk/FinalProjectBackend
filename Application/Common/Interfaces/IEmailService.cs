using Domain.Dtos;

namespace Application.Common.Interfaces;

public interface IEmailService
{

    void SendEmailConfirmation(SendEmailConfirmationDto sendEmailConfirmationDto);
}