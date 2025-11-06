using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using FluentValidation;

namespace BarrocIntens.Data.Validation
{
    internal class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(e => e.Password)
                .NotEmpty()
                .When(e => !string.IsNullOrEmpty(e.Name))
                .WithMessage("Password is required.");
        }
    }
}
