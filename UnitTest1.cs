using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IP_PROJECT;
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;

namespace IP_PROJECT_TESTS
{
    [TestClass]
    public class UnitTest1
    {
        
        const string testName = "mirel", testPasswd = "asd123";

        [TestInitialize]
        public void Setup()
        {
            PlanManager.Init();
        }

        [TestMethod]
        public void AddPlans()
        {
            // Adauga Planuri
            Plan fixedPlan = new Plan("Fixed Plan", "Description", new FixedStrategy(DateTime.Now.AddDays(1)));
            PlanManager.RegisterPlan(fixedPlan);

            Plan intervalPlan = new Plan("Interval Plan", "Description", new IntervalStrategy(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3)));
            PlanManager.RegisterPlan(intervalPlan);

            Plan notificationPlan = new Plan("Notification Plan", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));
            PlanManager.RegisterPlan(notificationPlan);

            // Vezi daca sunt adaugate
            Assert.IsTrue(PlanManager.GetAllPlans().Contains(fixedPlan));
            Assert.IsTrue(PlanManager.GetAllPlans().Contains(intervalPlan));
            Assert.IsTrue(PlanManager.GetAllPlans().Contains(notificationPlan));

            Assert.AreEqual(3, PlanManager.GetValids().Count);
        }

        [TestMethod]
        public void FixedPlan()
        {
            // Adauga Plan
            Plan fixedPlan = new Plan("Fixed Plan", "Description", new FixedStrategy(DateTime.Now.AddSeconds(0.1)));
            PlanManager.RegisterPlan(fixedPlan);

            Assert.AreEqual(PlanState.valid, PlanManager.GetAllPlans()[0].GetState());

            // trecem peste deadline
            System.Threading.Thread.Sleep(200);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.expirat, PlanManager.GetAllPlans()[0].GetState());
        }

        [TestMethod]
        public void NotificationPlan()
        {
            // Adauga Plan
            Plan notificationPlan = new Plan("Notification Plan", "Description", new NotificareStrategy(DateTime.Now.AddSeconds(0.2)));
            PlanManager.RegisterPlan(notificationPlan);

            Assert.AreEqual(PlanState.valid, PlanManager.GetAllPlans()[0].GetState());

            // trecem peste deadline
            System.Threading.Thread.Sleep(300);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.expirat, PlanManager.GetAllPlans()[0].GetState());
        }

        [TestMethod]
        public void IntervalPlan()
        {
            Plan intervPlan = new Plan("Interval Plan", "Description", new IntervalStrategy(DateTime.Now.AddSeconds(0.1), DateTime.Now.AddSeconds(0.2)));
            PlanManager.RegisterPlan(intervPlan);

            Assert.AreEqual(PlanState.valid, PlanManager.GetAllPlans()[0].GetState());

            // trecem peste deadline
            System.Threading.Thread.Sleep(150);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.inceput, PlanManager.GetAllPlans()[0].GetState());

            System.Threading.Thread.Sleep(150);

            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.expirat, PlanManager.GetAllPlans()[0].GetState());

        }

        [TestMethod]
        public void SignUp()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignUp(testName, testPasswd);

            // nicio exceptie inseamna succes
        }

        [TestMethod]
        public void SignIn()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn(testName, testPasswd, false);

            // nicio exceptie inseamna succes
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FailedSignInUsername()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn("wrong_username", testPasswd, false);

            // nicio exceptie inseamna succes
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FailedSignInPasswd()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn(testName, "wrong_password", false);

            // nicio exceptie inseamna succes
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FailedSignInBoth()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn("wrong_username", "wrong_password", false);

            // nicio exceptie inseamna succes
        }

        [TestMethod]
        public void CreateFixedPlan()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn(testName, testPasswd, false);
            PlanCreator pc = new PlanCreator();

            Plan fixedPlan = new Plan("Fixed Plan", "Description", new FixedStrategy(DateTime.Now.AddDays(2)));

            pc.SalveazaPlan(authentificator, "fix", fixedPlan);
        }

        [TestMethod]
        public void CreateNotifPlan()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn(testName, testPasswd, false);
            PlanCreator pc = new PlanCreator();

            Plan notifPlan = new Plan("Notification Plan", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));

            pc.SalveazaPlan(authentificator, "notificare", notifPlan);
        }

        [TestMethod]
        public void CreateIntervalPlan()
        {
            Database _database = new Database();
            Auth authentificator = new Auth(_database);

            authentificator.SignIn(testName, testPasswd, false);
            PlanCreator pc = new PlanCreator();

            Plan intervPlan = new Plan("Interval Plan", "Description", 
                new IntervalStrategy(DateTime.Now.AddSeconds(0.1), DateTime.Now.AddSeconds(0.2)));

            pc.SalveazaPlan(authentificator, "interval", intervPlan);
        }

        [TestMethod]
        public void GetPlans()
        {
            Database _database = new Database();
            List<Plan> plans = _database.ReadPlans(testName, testPasswd);

            foreach (Plan plan in plans)
            {
                string strr = plan.ToString();
            }

            Assert.AreNotEqual(0, plans.Count);
        }

        [TestMethod]
        public void DeletePlan()
        {
            Database database = new Database();

            // create plan to delete
            Plan notifPlan = new Plan("Dentist", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));
            database.WritePlan(testName, testPasswd, notifPlan);

            // delete the plan
            database.DeletePlan(testName, testPasswd, notifPlan.Title());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FailDeletePlan()
        {
            Database database = new Database();

            // create plan to delete
            Plan notifPlan = new Plan("Dentist", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));
            database.WritePlan(testName, testPasswd, notifPlan);

            // delete the plan
            database.DeletePlan(testName, testPasswd, "wrong_title");
        }

        [TestMethod]
        public void UpdatePlan()
        {
            Database database = new Database();

            // create plan to delete
            Plan notifPlan = new Plan("Dentist", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));
            Plan notifPlanModified = new Plan("Dentist", "Au Maseaua", new NotificareStrategy(DateTime.Now.AddHours(4)));
            database.WritePlan(testName, testPasswd, notifPlan);

            // delete the plan
            database.UpdatePlan(testName, testPasswd, notifPlanModified);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FailUpdatePlan()
        {
            Database database = new Database();

            // create plan to delete
            Plan notifPlan = new Plan("Dentist", "Description", new NotificareStrategy(DateTime.Now.AddHours(1)));
            Plan notifPlanModified = new Plan("wrong_title", "Au Maseaua", new NotificareStrategy(DateTime.Now.AddHours(4)));
            database.WritePlan(testName, testPasswd, notifPlan);

            // delete the plan
            database.UpdatePlan(testName, testPasswd, notifPlanModified);
        }

        [TestMethod]
        public void FixedPlanBeforeDate()
        {
            // Adauga Plan
            Plan fixedPlan = new Plan("Fixed Plan", "Description", new FixedStrategy(DateTime.Now.AddDays(-19)));
            PlanManager.RegisterPlan(fixedPlan);

            Assert.AreEqual(PlanState.valid, PlanManager.GetAllPlans()[0].GetState());

            // trecem peste deadline
            System.Threading.Thread.Sleep(200);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.expirat, PlanManager.GetAllPlans()[0].GetState());
        }

        [TestMethod]
        public void NotificationPlanBeforeDate()
        {
            // Adauga Plan
            Plan notificationPlan = new Plan("Notification Plan", "Description", new NotificareStrategy(DateTime.Now.AddDays(-1)));
            PlanManager.RegisterPlan(notificationPlan);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.valid, PlanManager.GetAllPlans()[0].GetState());
        }

        [TestMethod]
        public void IntervalPlanBeforeDate()
        {
            Plan intervPlan = new Plan("Interval Plan", "Description", new IntervalStrategy(DateTime.Now.AddDays(-99), DateTime.Now.AddSeconds(0.3)));
            PlanManager.RegisterPlan(intervPlan);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            Assert.AreEqual(PlanState.inceput, PlanManager.GetAllPlans()[0].GetState());

            // trecem peste deadline
            System.Threading.Thread.Sleep(150);

            System.Threading.Thread.Sleep(150);

            // update la planuri
            foreach (Plan plan in PlanManager.GetAllPlans())
                PlanManager.CheckPlan(plan, false);

            // vezi daca s-a schimbat starea
            Assert.AreEqual(PlanState.expirat, PlanManager.GetAllPlans()[0].GetState());
        }


    }
}
