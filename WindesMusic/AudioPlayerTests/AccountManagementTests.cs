using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using WindesMusic;
using System.Linq;

namespace UnitTestWindesMusic
{
    [TestClass]
    public class AccountManagementTests
    {
        [TestMethod]
        public void TestLoginWithFalseInformation()
        {
            Database db = new Database();
            User resultUser = db.Login("nonexistentemail@gmail.com", "test123");
            Assert.AreEqual(resultUser, new User());
        }

        [TestMethod]
        public void TestRegisterEmailAlreadyExists()
        {
            Database db = new Database();
            User resultUser = db.Register("test", "test@test.com", "test123", "testSalt");
            Assert.AreEqual(resultUser, new User());
        }

        [TestMethod]
        public void TestSetArtistAccount()
        {
            Database db = new Database();
            User resultUser = db.GetUserData(23);
            Assert.IsNotNull(resultUser.Email);
        }
    }
}
