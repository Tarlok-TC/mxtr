using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Entities;
using mxtrAutomation.Data.Repository;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Corporate.Data.Services
{
    public class UserService : MongoRepository<User>, IUserServiceInternal
    {
        private readonly int _saltLength = 32;

        public CreateNotificationReturn CreateUser(mxtrUser userData)
        {
            Password pw = new Password(userData.Password);
            HashedPassword hash = pw.Hash(_saltLength);

            User entry = new User()
            {
                MxtrUserID = userData.MxtrUserID,
                MxtrAccountID = userData.MxtrAccountID,
                AccountObjectID = userData.AccountObjectID,
                FullName = userData.FullName,
                Email = userData.Email,
                UserName = userData.UserName,
                Phone = userData.Phone,
                CellPhone = userData.CellPhone,
                EZShredAccountMappings = userData.EZShredAccountMappings != null ? userData.EZShredAccountMappings.Select(x => new EZShredAccountMapping
                {
                    AccountObjectId = x.AccountObjectId,
                    EZShredId = x.EZShredId,
                }).ToList() : new List<EZShredAccountMapping>(),
                Role = userData.Role,
                Permissions = userData.Permissions,
                CreateDate = userData.CreateDate,
                IsActive = userData.IsActive,
                Password = hash.Value,
                PasswordSalt = hash.Salt,
                IsApproved = userData.IsApproved,
                IsLockedOut = userData.IsLockedOut,
                FailedLoginAttempts = userData.FailedLoginAttempts,
                SharpspringPassword = userData.SharpspringPassword,
                SharpspringUserName = userData.SharpspringUserName,
            };
            //apply check if user already exists with same username
            if (IsUserExist(entry.UserName))
            {
                return new CreateNotificationReturn { Success = false, ObjectID = "User already exists with this username." };
            }
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                try
                {
                    repo.Add(entry);

                    //add user object id to the account

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public mxtrUser GetUserByUserObjectID(string userObjectID)
        {
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                return repo
                    .Where(u => u.Id == userObjectID)
                    .Select(u => new mxtrUser
                    {
                        ObjectID = u.Id,
                        MxtrUserID = u.MxtrUserID,
                        MxtrAccountID = u.MxtrAccountID,
                        AccountObjectID = u.AccountObjectID,
                        FullName = u.FullName,
                        Email = u.Email,
                        UserName = u.UserName,
                        Phone = u.Phone,
                        CellPhone = u.CellPhone,
                        EZShredAccountMappings = u.EZShredAccountMappings != null ? u.EZShredAccountMappings.Select(x => new EZShredAccountDataModel
                        {
                            AccountObjectId = x.AccountObjectId,
                            EZShredId = x.EZShredId,
                        }).ToList() : new List<EZShredAccountDataModel>(),
                        Role = u.Role,
                        Permissions = u.Permissions,
                        KlipfolioSSOToken = u.KlipfolioSSOToken,
                        CreateDate = u.CreateDate,
                        IsActive = u.IsActive,
                        IsApproved = u.IsApproved,
                        IsLockedOut = u.IsLockedOut,
                        SharpspringPassword = u.SharpspringPassword,
                        SharpspringUserName = u.SharpspringUserName,
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<mxtrUser> GetUsersByAccountObjectID(string accountObjectID)
        {
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                return repo
                    .Where(u => u.AccountObjectID == accountObjectID)
                    .Select(u => new mxtrUser
                    {
                        ObjectID = u.Id,
                        MxtrUserID = u.MxtrUserID,
                        MxtrAccountID = u.MxtrAccountID,
                        AccountObjectID = u.AccountObjectID,
                        FullName = u.FullName,
                        Email = u.Email,
                        UserName = u.UserName,
                        Phone = u.Phone,
                        CellPhone = u.CellPhone,
                        EZShredAccountMappings = u.EZShredAccountMappings != null ? u.EZShredAccountMappings.Select(x => new EZShredAccountDataModel
                        {
                            AccountObjectId = x.AccountObjectId,
                            EZShredId = x.EZShredId,
                        }).ToList() : new List<EZShredAccountDataModel>(),
                        Role = u.Role,
                        Permissions = u.Permissions,
                        CreateDate = u.CreateDate,
                        IsActive = u.IsActive,
                        IsApproved = u.IsApproved,
                        IsLockedOut = u.IsLockedOut,
                        SharpspringPassword = u.SharpspringPassword,
                        SharpspringUserName = u.SharpspringUserName,
                    });
            }
        }

        public mxtrUserCookie Login(string username, string password)
        {
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                User user = repo.SingleOrDefault(u => u.UserName.ToLower() == username.ToLower());

                if (user == null)
                {
                    throw new Exception("Incorrect username or password.");
                }

                if (IsPasswordVerified(user, password))
                {
                    return new mxtrUserCookie
                    {
                        ObjectID = user.Id,
                        MxtrUserID = user.MxtrUserID,
                        MxtrAccountID = user.MxtrAccountID,
                        AccountObjectID = user.AccountObjectID,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Role = user.Role,
                        SharpspringPassword = user.SharpspringPassword,
                        SharpspringUserName = user.SharpspringUserName,
                    };
                }

                return null;
            }
        }

        public bool IsCurrentPasswordVerified(string username, string password)
        {
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                User user = repo.SingleOrDefault(u => u.UserName == username);
                return IsPasswordVerified(user, password);
            }
        }

        public bool UpdateUserPassword(string userObjectID, string newPassword)
        {
            Password pw = new Password(newPassword);
            HashedPassword hash = pw.Hash(_saltLength);
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                var user = repo.Where(u => u.Id == userObjectID).FirstOrDefault();
                user.Password = hash.Value;
                user.PasswordSalt = hash.Salt;
                repo.Update(user);
            }
            return true;
        }

        public CreateNotificationReturn DeleteUser(string userObjectID)
        {
            try
            {
                using (MongoRepository<User> repo = new MongoRepository<User>())
                {
                    User user = repo.Where(w => w.Id == userObjectID).FirstOrDefault();
                    if (user != null)
                    {
                        repo.Delete(user);
                        return new CreateNotificationReturn { Success = true, ObjectID = string.Empty };
                    }
                    else
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                    }
                }
            }
            catch (Exception)
            {
                return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
            }

        }

        public bool IsUserExist(string userName)
        {
            try
            {
                using (MongoRepository<User> repo = new MongoRepository<User>())
                {
                    User user = repo.Where(w => w.UserName == userName).FirstOrDefault();
                    if (user != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public CreateNotificationReturn UpdateUser(mxtrUser userData)
        {
            Password pw = new Password(userData.Password);
            HashedPassword hash = pw.Hash(_saltLength);

            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                try
                {
                    var entry = repo.FirstOrDefault(x => x.Id == userData.ObjectID);
                    if (entry != null)
                    {
                        entry.MxtrUserID = userData.MxtrUserID;
                        entry.MxtrAccountID = userData.MxtrAccountID;
                        entry.AccountObjectID = userData.AccountObjectID;
                        entry.FullName = userData.FullName;
                        entry.Email = userData.Email;
                        //entry.UserName = userData.UserName;
                        entry.Phone = userData.Phone;
                        entry.CellPhone = userData.CellPhone;
                        entry.EZShredAccountMappings = userData.EZShredAccountMappings != null ? userData.EZShredAccountMappings.Select(x => new EZShredAccountMapping
                        {
                            AccountObjectId = x.AccountObjectId,
                            EZShredId = x.EZShredId,
                        }).ToList() : new List<EZShredAccountMapping>();
                        entry.Role = userData.Role;
                        entry.Permissions = userData.Permissions;
                        entry.IsActive = userData.IsActive;
                        //entry.Password = hash.Value;
                        //entry.PasswordSalt = hash.Salt;
                        entry.IsApproved = userData.IsApproved;
                        entry.IsLockedOut = userData.IsLockedOut;
                        entry.FailedLoginAttempts = userData.FailedLoginAttempts;
                        entry.SharpspringPassword = userData.SharpspringPassword;
                        entry.SharpspringUserName = userData.SharpspringUserName;
                        repo.Update(entry);
                    }
                    else
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                    }

                    //add user object id to the account

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }

        public CreateNotificationReturn UpdateUserKlipfolioSSOToken(mxtrUser userData)
        {
            using (MongoRepository<User> repo = new MongoRepository<User>())
            {
                try
                {
                    var entry = repo.FirstOrDefault(x => x.Id == userData.ObjectID);
                    if (entry != null)
                    {
                        entry.KlipfolioSSOToken = userData.KlipfolioSSOToken;

                        repo.Update(entry);
                    }
                    else
                    {
                        return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                    }

                    //add user object id to the account

                    return new CreateNotificationReturn { Success = true, ObjectID = entry.Id };
                }
                catch (Exception e)
                {
                    return new CreateNotificationReturn { Success = false, ObjectID = string.Empty };
                }
            }
        }
        #region Private Methods

        private bool UsernameExists(string username, MongoRepository<User> repo)
        {
            return repo.Any(u => u.UserName.ToLower() == username.ToLower());
        }

        private bool IsPasswordVerified(User user, string password)
        {
            HashedPassword hashedPassword = new HashedPassword(user.Password, user.PasswordSalt);
            if (hashedPassword.IsMatch(password))
                return true;

            return false;
        }
        #endregion

    }
}
