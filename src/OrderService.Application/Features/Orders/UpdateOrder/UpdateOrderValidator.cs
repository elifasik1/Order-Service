using FluentValidation;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
     RuleFor(x => x.CustomerName)
    .NotEmpty()
    .MaximumLength(100);

  RuleFor(x => x.Email)
    .NotEmpty().WithMessage("E-posta boş olamaz.")
    .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

    RuleFor(x => x.PhoneNumber)
    .NotEmpty()
    .WithMessage("Telefon numarası boş olamaz.");

    RuleFor(x => x.Address)
    .NotEmpty()
    .MaximumLength(250);
    
    RuleFor(x => x.ProductID)
    .GreaterThan(0)
    .WithMessage("Geçerli bir ürün seçiniz.");

    RuleFor(x => x.Quantity)
    .GreaterThan(0)
    .WithMessage("Miktar 0'dan büyük olmalıdır.");
    }
}