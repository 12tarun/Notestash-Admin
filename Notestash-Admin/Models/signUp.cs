using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Kdf;
using PBKDF2 = SecurityDriven.Inferno.Kdf.PBKDF2;
using SecurityDriven.Inferno.Extensions;
using static SecurityDriven.Inferno.SuiteB;
using static SecurityDriven.Inferno.Utils;

namespace Notestash_Admin.Models
{
    public class signUp
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Full Name Required!", AllowEmptyStrings = false)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email Required!", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password should be between 6 to 15 characters!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password Required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password should be between 6 to 15 characters!")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public int IsEmailVerified { get; set; }
        public Guid ActivationCode { get; set; }
        public int AdminOrUser { get; set; }

        public string Create(signUp objUser)
        {
            var sha384Factory = HmacFactory;
            var random = new CryptoRandom();

            byte[] derivedKey;
            string hashedPassword = null;
            string passwordText = objUser.Password;

            byte[] passwordBytes = SafeUTF8.GetBytes(passwordText);
            var salt = random.NextBytes(384 / 8);

            using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, salt, 256 * 1000))
                derivedKey = pbkdf2.GetBytes(384 / 8);


            using (var hmac = sha384Factory())
            {
                hmac.Key = derivedKey;
                hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
            }

            try
            {
                tblUser objTblUser = new tblUser();
                objTblUser.Id = objUser.Id;
                objTblUser.FullName = objUser.FullName;
                objTblUser.Password = hashedPassword;
                objTblUser.Email = objUser.Email;
                objTblUser.Salt = salt;
                objTblUser.ProfilePicture = null;
                objTblUser.IsEmailVerified = 0;
                objTblUser.ActivationCode = Guid.NewGuid();
                objTblUser.Created_at = DateTime.Now;
                objTblUser.AdminOrUser = 2;

                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {
                    DateTime present = DateTime.Now;
                    var userList = db.tblUsers.Where(a => a.IsEmailVerified == 0).ToList();
                    foreach (tblUser user in userList)
                    {
                        DateTime expire = user.Created_at.Value.AddDays(1);
                        if (present >= expire)
                        {
                            db.tblUsers.Remove(user);
                        }
                    }
                    db.SaveChanges();

                    var existingUser = db.tblUsers.FirstOrDefault(e => e.Email.Equals(objUser.Email));

                    if (existingUser == null)
                    {
                        db.tblUsers.Add(objTblUser);
                        db.SaveChanges();
                        return objUser.Email+" "+objTblUser.ActivationCode.ToString();
                    }
                    else
                    {
                        return "exists";
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return "error";              
            }
        }
    }
}