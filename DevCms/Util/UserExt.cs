using DevCms.Db;

namespace DevCms.Util
{
    public static class UserExt
    {
        public const string NotInvitedStatus = "NotInvited";
        public const string InvitedStatus = "Invited";
        public const string AcceptedInvitationStatus = "AcceptedInvitation";
        public const string PropertyImported = "PropertyImported";
        public const string ApprovedStatus = "Approved";
        public const string PendingStatus = "Pending";
        public const string LockedStatus = "Locked";
        public const string EmailPattern = @"^(?![\.@])(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@(([a-z0-9][\w\.-]*[a-z0-9])|[a-z0-9])+\.[a-z][a-z\.]*[a-z]$";

        public static bool IsEmailApproved(this User user)
        {
            return user.EmailStatus == ApprovedStatus;
        }

        public static bool IsRegistrationApproved(this User user)
        {
            return user.RegistrationStatus == ApprovedStatus;
        }

        public static bool IsLocked(this User user)
        {
            return user.RegistrationStatus == LockedStatus;
        }
    }
}
