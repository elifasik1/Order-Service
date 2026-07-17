using FluentValidation;
public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerName)
        .NotEmpty().WithMessage("Müşteri adı boş olamaz.")
        .MinimumLength(3).WithMessage("Müşteri adı en az 3 karakter olmalıdır.")
        .MaximumLength(20).WithMessage("Müşteri adı 20 karakterden uzun olamaz");
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email boş olamaz")
        .EmailAddress().WithMessage("Geçerli bir email adresi giriniz");
        RuleFor(x =>x.PhoneNumber)
        .NotEmpty().WithMessage("Telefon numarası boş olamaz")
        .MinimumLength(10).WithMessage("Telefon numarası en az 10 karakter olmalıdır");
        RuleFor(x=> x.Address)
        .NotEmpty().WithMessage("Adres boş olamaz")
        .MinimumLength(10).WithMessage("Adres en az 10 karakter olmalıdır");
        RuleFor(x => x.ProductID)
        .GreaterThan(0).WithMessage("Geçerli bir ürün ID'si girin");
        RuleFor(x => x.Quantity)
        .GreaterThan(0).WithMessage("Geçerli ürün adedi girin");

        
    }

    
}