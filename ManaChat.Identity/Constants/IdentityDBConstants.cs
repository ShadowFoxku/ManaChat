namespace ManaChat.Identity.Constants
{
    public static class IdentityDBConstants
    {
        public static class StoredProcedures
        {
            public const string GetUserById = "usp_get_user_by_id";
            public const string GetUserByUsername = "usp_get_user_by_username";
            public const string SaveUser = "usp_save_user";
            public const string UpdateUserPassword = "usp_save_user_password";
            public const string DeleteUser = "usp_delete_user";

            public const string GetUserIdentitiesForUser = "usp_get_user_identities_for_user";
            public const string GetUserIdentity = "usp_get_user_identity";
            public const string SaveUserIdentity = "usp_save_user_identity";
            public const string DeleteUserIdentity = "usp_delete_user_identity";

            public const string GetUserRelationships = "usp_get_user_relationships_for_user";
            public const string GetUserRelationBetweenUsers = "usp_get_user_relationships_between_users";
            public const string SaveUserRelationship = "usp_save_user_relationship";
        }
    }
}
