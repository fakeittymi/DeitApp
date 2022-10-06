using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;


namespace DeitApp
{
    class UserService
    {
        public void ActualizeUserData(Users user)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                try
                {
                    var _user = db.Users.Find(user.id);
                    if (_user == null)
                    {                       
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                    else
                    {
                        _user.Name = user.Name;
                        _user.Age = user.Age;
                        _user.Sex = user.Sex;
                        _user.Weight = user.Weight;
                        _user.Height = user.Height;                    
                        _user.Activity = user.Activity;
                        _user.DietGoal = user.DietGoal;
                        db.SaveChanges();
                    }                    
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                    {
                        Console.Write("Object: " + validationError.Entry.Entity.ToString());
                        foreach (DbValidationError err in validationError.ValidationErrors)
                        {
                            Console.Write(err.ErrorMessage + " ");
                        }
                    }
                }
                
            }
        }

        public static float[] activityLevels = { 1.2f, 1.375f, 1.55f, 1.725f, 1.9f };
        public const int MALECOEF = 5;
        public const int FEMALECOEF = -161;
    }
}
