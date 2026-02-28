namespace ManaChat.Identity.Constants
{
    public static class IdentityDBConstants
    {
        private const string Schema = "identity";
        public static class StoredProcedures
        {
            public const string GetUserById = $"{Schema}.usp_user_get_by_id";
            public const string GetUserByUsername = $"{Schema}.usp_user_get_by_username";
            public const string SearchUserByUsername = $"{Schema}.usp_user_search_by_username";
            public const string SaveUser = $"{Schema}.usp_user_save";
            public const string UpdateUserPassword = $"{Schema}.usp_user_save_password";
            public const string DeleteUser = $"{Schema}.usp_user_delete";
            public const string GetUserSessionByToken = $"{Schema}.usp_session_get_by_token";
            public const string UpdateUserSession = $"{Schema}.usp_session_save";
            public const string LogoutUserSession = $"{Schema}.usp_session_delete";
            public const string VerifyDetailsAvailable = $"{Schema}.usp_user_verify_details_available";

            public const string GetUserIdentitiesForUser = $"{Schema}.usp_identities_get_for_user";
            public const string GetUserIdentity = $"{Schema}.usp_identity_get";
            public const string SaveUserIdentity = $"{Schema}.usp_identity_save";
            public const string DeleteUserIdentity = $"{Schema}.usp_identity_delete";

            public const string GetUserRelationships = $"{Schema}.usp_relationships_get_for_user";
            public const string GetUserRelationBetweenUsers = $"{Schema}.usp_relationship_get_between_users";
            public const string SaveUserRelationship = $"{Schema}.usp_relationship_save";
        }
    }
}
