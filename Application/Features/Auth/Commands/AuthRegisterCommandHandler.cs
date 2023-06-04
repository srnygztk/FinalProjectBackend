using Application.Common.Interfaces;
using Domain.Dtos;

namespace Application.Features.Auth.Commands.Register
{
    public class AuthRegisterCommandHandler
    {

        private readonly IEmailService _emailService;


        public AuthRegisterCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
    }
}