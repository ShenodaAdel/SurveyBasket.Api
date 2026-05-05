using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Application.Helpers
{
    public static class DefaultRoles
    {
        public const string Admin = nameof(Admin);
        public const string AdminRoleId = "de21f544-014a-4e54-8faa-fff937973eb1";
        public const string AdminConcurrencyStamp = "290db1bb-60bc-4322-ae2f-fb93402828f7";


        public const string User = nameof(User);
        public const string UserRoleId = "89c59864-cb81-4b1b-b34b-7e2623de448a";
        public const string UserConcurrencyStamp = "6d8ccb85-ce2b-4439-baf2-52066ee1b0a7";
    }
}
