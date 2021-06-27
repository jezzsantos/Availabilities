using Availabilities.Apis.ServiceOperations.Bookings;
using ServiceStack.FluentValidation;

namespace Availabilities.Apis.Validators
{
    // ReSharper disable once UnusedType.Global
    internal class DeleteBookingRequestValidator : AbstractValidator<DeleteBookingRequest>
    {
        public DeleteBookingRequestValidator()
        {
            RuleFor(dto => dto.Id)
                .NotEmpty()
                .WithMessage("Id is either missing or invalid");
        }
    }
}