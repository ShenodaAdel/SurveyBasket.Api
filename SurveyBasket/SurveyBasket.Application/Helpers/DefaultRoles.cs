namespace SurveyBasket.Application.Helpers
{
    public static class DefaultRoles
    {
        public partial class Admin
        {
            public const string Name = nameof(Admin);
            public const string Id = "de21f544-014a-4e54-8faa-fff937973eb1";
            public const string ConcurrencyStamp = "290db1bb-60bc-4322-ae2f-fb93402828f7";
        }

        public partial class User
        {
            public const string Name = nameof(User);
            public const string Id = "de21f544-014a-4e54-8faa-fff937973eb2";
            public const string ConcurrencyStamp = "290db1bb-60bc-4322-ae2f-fb93402828f8";
        }

    }
}
