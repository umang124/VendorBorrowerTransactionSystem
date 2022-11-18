using LoanManagementSystem.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;

namespace LoanManagementSystem
{
    public class WebRoleProvider : RoleProvider
    {
        LMSDbContext _dbContext = new LMSDbContext();
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }
        public override string[] GetRolesForUser(string username)
        {
            var role = _dbContext.Database.SqlQuery<string>("select ut.name from [user] as u inner join usertype as ut on ut.Id = u.UserTypeId inner join AccountCredentials as a on a.UserId = u.Id where a.PhoneNumber = @username", new SqlParameter("@username", username)).ToArray();
            return role;
        }
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}