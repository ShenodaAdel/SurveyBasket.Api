using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Services.Auth.Dtos
{
    public record ResetPasswordRequest(string Email, string Code, string NewPassword);
}
