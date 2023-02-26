using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;

namespace PersonalPhotos.Test
{
    public class LoginsControllerTests
    {
        private LoginsController _controller;
        private ILogins _logins;
        private IHttpContextAccessor _httpContextAccessor;


        [NUnit.Framework.SetUp]
        public void Setup()
        {
            var mockedLogins = new Mock<ILogins>();
            var mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var session = Mock.Of<ISession>();
            var httpContext = Mock.Of<HttpContext>(x=> x.Session == session);
            mockedHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var mockedUser = Mock.Of<User>(x=> x.Email=="a@b.com" && x.Password=="123");

            mockedLogins.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(mockedUser);

            _logins = mockedLogins.Object;
            _httpContextAccessor = mockedHttpContextAccessor.Object;

            _controller = new LoginsController(_logins, _httpContextAccessor);
        }

        [Test]
        public void Index_Given_NoReturnUrl_ReturnsLoginView()
        {
            var result = _controller.Index() as ViewResult;

            //Assert.IsNotNull(result);
            //Assert.AreEqual("Login", result.ViewName);

            Assert.Multiple( () => {

                Assert.That(result, Is.Not.Null);
                Assert.That(result?.ViewName, Is.EqualTo("Login").IgnoreCase);
            });
        }

        [Test]
        public async Task Login_Given_InvalidModelState_ReturnsLoginView()
        {
            _controller.ModelState.AddModelError("Test", "Test");

            var result= await _controller.Login(Mock.Of<LoginViewModel>()) as ViewResult;

            Assert.That(result?.ViewName, Is.EqualTo("Login").IgnoreCase);
        }

        [Test]
        public async Task Login_Given_CorrectPassword_RedirectToPhotos()
        {
            var viewModel = Mock.Of<LoginViewModel>(x=> x.Password =="123" && x.Email=="a@b.com");

            var result = await _controller.Login(viewModel);

            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var controller = result as RedirectToActionResult;

            Assert.AreEqual("Photos", controller.ControllerName);
            Assert.AreEqual("Display", controller.ActionName);
        }

    }
}