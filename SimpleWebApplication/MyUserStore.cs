using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace SimpleWebApplication
{
    public class MyUserStore : IUserStore<MyUser>, IUserPasswordStore<MyUser>, ISecurityStampValidator
    {
        public MyUserStore()
        {
        }

        async Task<string> IUserStore<MyUser>.GetUserIdAsync(MyUser user, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return user.Id;
        }

        async Task<string> IUserStore<MyUser>.GetUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return user.UserName;
        }

        async Task IUserStore<MyUser>.SetUserNameAsync(MyUser user, string userName, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            user.UserName = userName;
        }

        async Task<string> IUserStore<MyUser>.GetNormalizedUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return user.NormalizedUserName;
        }

        async Task IUserStore<MyUser>.SetNormalizedUserNameAsync(MyUser user, string normalizedName, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            user.NormalizedUserName = normalizedName;
        }

        async Task<IdentityResult> IUserStore<MyUser>.CreateAsync(MyUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "insert into PluralsightUsers([Id]," +
                    "[UserName]," +
                    "[NormalizedUserName]," +
                    "[PasswordHash]) " +
                    "Values(@id,@userName,@normalizedUserName,@passwordHash)",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IUserStore<MyUser>.UpdateAsync(MyUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "update PluralsightUsers " +
                    "set [Id] = @id," +
                    "[UserName] = @userName," +
                    "[NormalizedUserName] = @normalizedUserName," +
                    "[PasswordHash] = @passwordHash " +
                    "where [Id] = @id",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        Task<IdentityResult> IUserStore<MyUser>.DeleteAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<MyUser> IUserStore<MyUser>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MyUser>(
                    "select * From PluralsightUsers where Id = @id",
                    new { id = userId });
            }
        }

        async Task<MyUser> IUserStore<MyUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MyUser>(
                    "select * From PluralsightUsers where NormalizedUserName = @name",
                    new { name = normalizedUserName });
            }
        }

        async Task IUserPasswordStore<MyUser>.SetPasswordHashAsync(MyUser user, string passwordHash, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            user.PasswordHash = passwordHash;
        }

        async Task<string> IUserPasswordStore<MyUser>.GetPasswordHashAsync(MyUser user, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return user.PasswordHash;
        }

        async Task<bool> IUserPasswordStore<MyUser>.HasPasswordAsync(MyUser user, CancellationToken cancellationToken)
        {
            return await ((IUserPasswordStore<MyUser>)this).GetPasswordHashAsync(user, cancellationToken) != null;
        }

        protected static IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB;" +
                                               "database=PluralsightDemo;" +
                                               "trusted_connection=yes;");
            connection.Open();
            return connection;
        }

        void IDisposable.Dispose()
        {
        }

        async Task ISecurityStampValidator.ValidateAsync(CookieValidatePrincipalContext context)
        {
            await Task.CompletedTask;
        }
    }
}
