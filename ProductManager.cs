using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;

namespace DeitApp
{
    class ProductManager
    {
        public ProductManager() { }
        public ProductManager(Users user)
        {
            this.user = user;
        }

        public void GenerateRecomendation(int _dayCount)
        {
            DailyNorm _dailyNorm = CalculateDailyNorm();
            List<DietProducts> products = new List<DietProducts>();
            for (int dayCount = 1; dayCount <= _dayCount; dayCount++)
            {
                float bfCaloriesNorm = _dailyNorm.proteins * bfPROT * PROTEIN_CAL_COUNT +
                                       _dailyNorm.fats * bfFAT * FAT_CAL_COUNT +
                                       _dailyNorm.carbons * bfCARB * CARBON_CAL_COUNT;
                float lunchCaloriesNorm = _dailyNorm.proteins * lunchPROT * PROTEIN_CAL_COUNT +
                                          _dailyNorm.fats * lunchFAT * FAT_CAL_COUNT +
                                          _dailyNorm.carbons * lunchCARB * CARBON_CAL_COUNT;
                float dnrCaloriesNorm = _dailyNorm.proteins * dnrPROT * PROTEIN_CAL_COUNT +
                                        _dailyNorm.fats * dnrFAT * FAT_CAL_COUNT +
                                        _dailyNorm.carbons * dnrCARB * CARBON_CAL_COUNT;

                List<SuggestionProducts> offer = GetProductsRecomendation();
                SuggestionProducts tmpProd = new SuggestionProducts();
                float mass = 0;
                //Норма ккал на утро * 20%(количесвто каши
                //в ккал от общего кол-ва / на калорийность
                //продукта на 100г * 100% 
                for (int i = 0; i < FOODCATEGORIES.Length; i++)
                {
                    // Breakfast
                    if (FOODCATEGORIES[i].Contains("bf"))
                    {
                        float prot = 0, fat = 0, carb = 0;
                        int cals = 0;
                        tmpProd = offer.Where(e => e.Category == FOODCATEGORIES[i]).FirstOrDefault();
                        if (tmpProd == null)
                            continue;
                        mass = CategoriesProportions[i] * bfCaloriesNorm * 100 / tmpProd.Calories;
                        cals += (int)(tmpProd.Calories * mass / 100);
                        prot += tmpProd.Proteins * mass / 100;
                        fat += tmpProd.Fats * mass / 100;
                        carb += tmpProd.Carbons * mass / 100;
                        products.Add(new DietProducts(user.id, tmpProd.id, tmpProd.Name, cals, prot, fat, carb, mass, tmpProd.Category, DateTime.Now, dayCount));
                        continue;
                    }

                    // Lunch
                    if (FOODCATEGORIES[i].Contains("lunch"))
                    {
                        if (FOODCATEGORIES[i].Contains("Vegets"))
                        {

                            foreach (var veget in offer.Where(e => e.Category == FOODCATEGORIES[i]).ToList())
                            {
                                float protv = 0, fatv = 0, carbv = 0;
                                int calsv = 0;
                                mass = CategoriesProportions[i] * lunchCaloriesNorm * 100 / veget.Calories;
                                calsv += (int)(veget.Calories * mass / 100);
                                protv += veget.Proteins * mass / 100;
                                fatv += veget.Fats * mass / 100;
                                carbv += veget.Carbons * mass / 100;
                                products.Add(new DietProducts(user.id, veget.id, veget.Name, calsv, protv, fatv, carbv, mass, veget.Category, DateTime.Now, dayCount));
                            }
                            continue;
                        }

                        float prot = 0, fat = 0, carb = 0;
                        int cals = 0;
                        tmpProd = offer.Where(e => e.Category == FOODCATEGORIES[i]).FirstOrDefault();
                        if (tmpProd == null)
                            continue;
                        mass = CategoriesProportions[i] * lunchCaloriesNorm * 100 / tmpProd.Calories;
                        cals += (int)(tmpProd.Calories * mass / 100);
                        prot += tmpProd.Proteins * mass / 100;
                        fat += tmpProd.Fats * mass / 100;
                        carb += tmpProd.Carbons * mass / 100;
                        products.Add(new DietProducts(user.id, tmpProd.id, tmpProd.Name, cals, prot, fat, carb, mass, tmpProd.Category, DateTime.Now, dayCount));
                        continue;
                    }

                    // Dinner
                    if (FOODCATEGORIES[i].Contains("dnr"))
                    {
                        if (FOODCATEGORIES[i].Contains("Vegets"))
                        {
                            foreach (var veget in offer.Where(e => e.Category == FOODCATEGORIES[i]).ToList())
                            {
                                float protv = 0, fatv = 0, carbv = 0;
                                int calsv = 0;
                                mass = CategoriesProportions[i] * dnrCaloriesNorm * 100 / veget.Calories;
                                calsv += (int)(veget.Calories * mass / 100);
                                protv += veget.Proteins * mass / 100;
                                fatv += veget.Fats * mass / 100;
                                carbv += veget.Carbons * mass / 100;
                                products.Add(new DietProducts(user.id, veget.id,  veget.Name, calsv, protv, fatv, carbv, mass, veget.Category, DateTime.Now, dayCount));
                            }
                            continue;
                        }

                        float prot = 0, fat = 0, carb = 0;
                        int cals = 0;
                        tmpProd = offer.Where(e => e.Category == FOODCATEGORIES[i]).FirstOrDefault();
                        if (tmpProd == null)
                            continue;
                        mass = CategoriesProportions[i] * dnrCaloriesNorm * 100 / tmpProd.Calories;
                        cals += (int)(tmpProd.Calories * mass / 100);
                        prot += tmpProd.Proteins * mass / 100;
                        fat += tmpProd.Fats * mass / 100;
                        carb += tmpProd.Carbons * mass / 100;
                        products.Add(new DietProducts(user.id, tmpProd.id, tmpProd.Name, cals, prot, fat, carb, mass, tmpProd.Category, DateTime.Now, dayCount));
                    }
                }
            }

            using (BalancedDietEntities db = new BalancedDietEntities())
            {              
                try
                {
                    List<DietProducts> oldUserRecomendation = db.DietProducts.Where(p => p.UserId == user.id).ToList();
                    if (oldUserRecomendation.Count != 0)
                    {
                        foreach (var product in oldUserRecomendation)
                        {
                            db.DietProducts.Remove(product);
                        }
                    }

                    foreach (var product in products)
                    {
                        db.DietProducts.Add(product);
                    }
                    db.SaveChanges();

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

        public struct DailyNorm
        {
            public int calories;
            public float proteins;
            public float fats;
            public float carbons;
        }

        public DailyNorm CalculateDailyNorm()
        {
            DailyNorm dailyNorm = new DailyNorm();
            dailyNorm.calories = (int)((10 * user.Weight + 6.25f * user.Height - 5 * user.Age + user.Sex) * user.Activity);
            GalculatePFC(ref dailyNorm, user.DietGoal);
            return dailyNorm;
        }

        private void GalculatePFC(ref DailyNorm dailyNorm, int goal)
        {
            switch (goal)
            {
                case 0:
                    dailyNorm.calories = (int)(dailyNorm.calories * CALORIES_KEEPING_WEIGHT);
                    dailyNorm.proteins = dailyNorm.calories * PROT_KEEPING_WEIGHT / PROTEIN_CAL_COUNT;
                    dailyNorm.fats = dailyNorm.calories * FAT_KEEPING_WEIGHT / FAT_CAL_COUNT;
                    dailyNorm.carbons = dailyNorm.calories * CARBON_KEEPING_WEIGHT / CARBON_CAL_COUNT;
                    break;
                case 1:
                    dailyNorm.calories = (int)(dailyNorm.calories * CALORIES_LOSING_WEIGHT);
                    dailyNorm.proteins = dailyNorm.calories * PROT_LOSING_WEIGHT / PROTEIN_CAL_COUNT;
                    dailyNorm.fats = dailyNorm.calories * FAT_LOSING_WEIGHT / FAT_CAL_COUNT;
                    dailyNorm.carbons = dailyNorm.calories * CARBON_LOSING_WEIGHT / CARBON_CAL_COUNT;
                    break;
                case 2:
                    dailyNorm.calories = (int)(dailyNorm.calories * CALORIES_GAIN_WEIGHT);
                    dailyNorm.proteins = dailyNorm.calories * PROT_GAIN_WEIGHT / PROTEIN_CAL_COUNT;
                    dailyNorm.fats = dailyNorm.calories * FAT_GAIN_WEIGHT / FAT_CAL_COUNT;
                    dailyNorm.carbons = dailyNorm.calories * CARBON_GAIN_WEIGHT / CARBON_CAL_COUNT;
                    break;
            }
        }

        //private List<SuggestionProducts> GetProductsRecomendation()
        //{
        //    List<SuggestionProducts> offer = new List<SuggestionProducts>();
        //    using (BalancedDietEntities db = new BalancedDietEntities())
        //    {
        //        List<Guid> usedProducts = new List<Guid>();
        //        List<SuggestionProducts> products = new List<SuggestionProducts>(); 
        //        SuggestionProducts product = new SuggestionProducts();
        //        // Breakfast
        //        products = db.SuggestionProducts.Where(e => e.Category == "bfMain").ToList();               
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "bfFruits").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "bfExtra").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "bfNuts").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "bfDrinks").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);

        //        // Lunch
        //        products = db.SuggestionProducts.Where(e => e.Category == "lunchFish" || e.Category == "lunchMeat").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "lunchGrass").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "lunchGarnish").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "lunchOil").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "lunchVegets").ToList();
        //        while (usedProducts.Count < 3)
        //        {
        //            product = products[new Random().Next(products.Count)];
        //            if (usedProducts.Contains(product.id))
        //                continue;
        //            offer.Add(product);
        //            usedProducts.Add(product.id);
        //        }
        //        usedProducts.Clear();

        //        // Dinner                
        //        products = db.SuggestionProducts.Where(e => e.Category == "dnrFish" || e.Category == "dnrMeat").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "dnrFruits").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "dnrGrass").ToList();
        //        offer.Add(products[new Random().Next(products.Count)]);
        //        products = db.SuggestionProducts.Where(e => e.Category == "dnrVegets").ToList();
        //        while (usedProducts.Count < 3)
        //        {
        //            product = products[new Random().Next(products.Count)];
        //            if (usedProducts.Contains(product.id))
        //                continue;
        //            offer.Add(product);
        //            usedProducts.Add(product.id);
        //        }
        //        usedProducts.Clear();
        //    }

        //    return offer;
        //}

        private List<SuggestionProducts> GetProductsRecomendation()
        {
            List<SuggestionProducts> offer = new List<SuggestionProducts>();
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                List<Guid> usedProducts = new List<Guid>();
                List<SuggestionProducts> products = new List<SuggestionProducts>(); 
                // Создаем список из всех продуктов для удобной работы
                foreach (var product in db.SuggestionProducts.ToList()) 
                {
                    products.Add(product);             
                }

                // Удаляем из списка продукты из черного списка
                foreach (var product in db.UserBLProducts.Where(e => e.UserId == user.id).ToList())
                {
                    products.Remove(db.SuggestionProducts.Find(product.ProductId));
                }

                // Добавляем в список пользовательские продукты
                foreach (var product in db.UserWLProducts.Where(e => e.UserId == user.id).ToList())
                {
                    SuggestionProducts tmp = new SuggestionProducts();
                    tmp.id = product.id;
                    tmp.Name = product.Name;
                    tmp.Calories = product.Calories;
                    tmp.Proteins = product.Proteins;
                    tmp.Fats = product.Fats;
                    tmp.Carbons = product.Carbons;
                    tmp.Category = product.Category;
                    products.Add(tmp);
                }

                List<SuggestionProducts> tmpProds = new List<SuggestionProducts>();
                foreach (var category in FOODCATEGORIES)
                {
                    if (category.Contains("Fish") || category.Contains("Meat") || category.Contains("Vegets"))
                        continue;
                    tmpProds = products.Where(e => e.Category == category).ToList();
                    offer.Add(tmpProds[new Random().Next(tmpProds.Count)]);
                }

                tmpProds = products.Where(e => e.Category == "lunchFish" || e.Category == "lunchMeat").ToList();
                offer.Add(tmpProds[new Random().Next(tmpProds.Count)]);
                tmpProds = products.Where(e => e.Category == "dnrFish" || e.Category == "dnrMeat").ToList();
                offer.Add(tmpProds[new Random().Next(tmpProds.Count)]);
                tmpProds = products.Where(e => e.Category == "lunchVegets").ToList();
                while (usedProducts.Count < 3)
                {
                    SuggestionProducts veget = tmpProds[new Random().Next(tmpProds.Count)];
                    if (usedProducts.Contains(veget.id))
                        continue;
                    offer.Add(veget);
                    usedProducts.Add(veget.id);
                }
                usedProducts.Clear();

                tmpProds = products.Where(e => e.Category == "dnrVegets").ToList();
                while (usedProducts.Count < 3)
                {
                    SuggestionProducts veget = tmpProds[new Random().Next(tmpProds.Count)];
                    if (usedProducts.Contains(veget.id))
                        continue;
                    offer.Add(veget);
                    usedProducts.Add(veget.id);
                }
                usedProducts.Clear();
            }
            return offer;
        }

        Users user;
        
        private const float CALORIES_KEEPING_WEIGHT = 1f;
        private const float CALORIES_LOSING_WEIGHT = 0.85f;
        private const float CALORIES_GAIN_WEIGHT = 1.15f;
        private const float PROT_KEEPING_WEIGHT = 0.3f;
        private const float FAT_KEEPING_WEIGHT = 0.3f;
        private const float CARBON_KEEPING_WEIGHT = 0.4f;
        private const float PROT_LOSING_WEIGHT = 0.3f;
        private const float FAT_LOSING_WEIGHT = 0.2f;
        private const float CARBON_LOSING_WEIGHT = 0.5f;
        private const float PROT_GAIN_WEIGHT = 0.35f;
        private const float FAT_GAIN_WEIGHT = 0.3f;
        private const float CARBON_GAIN_WEIGHT = 0.55f;
        private const int PROTEIN_CAL_COUNT = 4;
        private const int FAT_CAL_COUNT = 9;
        private const int CARBON_CAL_COUNT = 4;
        private const float bfPROT = 0.2f;
        private const float bfFAT = 0.4f;
        private const float bfCARB = 0.55f;
        private const float lunchPROT = 0.5f;
        private const float lunchFAT = 0.4f;
        private const float lunchCARB = 0.35f;
        private const float dnrPROT = 0.3f;
        private const float dnrFAT = 0.2f;
        private const float dnrCARB = 0.1f;
        //private const float bfMain = 0.2f;
        //private const float bfExtra = 0.15f;
        //private const float bfFruits = 0.2f;
        //private const float bfNuts = 0.3f;
        //private const float bfDrinks = 0.15f;
        //private const float lunchFishMeat = 0.35f;
        //private const float lunchGrass = 0.01f;
        //private const float lunchOil = 0.35f;
        //private const float lunchGarnish = 0.35f;
        //private const float lunchVegets = 0.03f;
        //private const float dnrFishMeat = 0.55f;
        //private const float dnrGrass = 0.05f;
        //private const float dnrFruits = 0.25f;
        //private const float dnrVegets = 0.05f;
        private static readonly float[] CategoriesProportions = {  0.2f, 0.15f, 0.2f, 0.3f, 0.15f,
                                                                   0.35f, 0.35f, 0.01f, 0.35f, 0.35f, 0.03f,
                                                                   0.55f,  0.55f,  0.05f, 0.25f, 0.05f };

        public static string[] FOODCATEGORIES = { "bfMain", "bfExtra", "bfFruits", "bfNuts", "bfDrinks",
                                                   "lunchFish", "lunchMeat", "lunchGrass", "lunchOil", "lunchGarnish", "lunchVegets",
                                                   "dnrFish", "dnrMeat", "dnrGrass", "dnrFruits", "dnrVegets" };
    }
}
