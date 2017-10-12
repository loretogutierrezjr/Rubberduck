using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rubberduck.UI;
using Rubberduck.UI.Command;
using Rubberduck.UnitTesting;
using Rubberduck.VBEditor.SafeComWrappers.VB.Enums;
using RubberduckTests.Mocks;
using System.Linq;
using System.Windows.Forms;

namespace RubberduckTests.Commands
{
    [TestClass]
    public class ExportAllTests
    {
        private const string _path = @"C:\Users\Rubberduck\Desktop\ExportAll";
        private const string _projectPath = @"C:\Users\Rubberduck\Documents\Subfolder";
        private const string _projectFullPath = @"C:\Users\Rubberduck\Documents\Subfolder\Project.xlsm";
        private const string _projectFullPath2 = @"C:\Users\Rubberduck\Documents\Subfolder\Project2.xlsm";

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_CanExecute_PassedNull_ExpectTrue()
        {
            var builder = new MockVbeBuilder();
            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected);
            var project = projectMock.Build();
            var vbe = builder.AddProject(project).Build();

            vbe.SetupGet(m => m.ActiveVBProject.VBComponents.Count).Returns(1);

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            Assert.IsTrue(ExportAllCommand.CanExecute(null));
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_CanExecute_PassedNull_NoComponents_ExpectFalse()
        {
            var builder = new MockVbeBuilder();
            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected);
            var project = projectMock.Build();
            var vbe = builder.AddProject(project).Build();

            vbe.SetupGet(m => m.ActiveVBProject.VBComponents.Count).Returns(0);

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            Assert.IsFalse(ExportAllCommand.CanExecute(null));
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_CanExecute_PassedIVBProject_ExpectTrue()
        {
            var builder = new MockVbeBuilder();
            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected);
            var project = projectMock.Build();

            var vbe = builder.AddProject(project).Build();

            project.SetupGet(m => m.VBComponents.Count).Returns(1);

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            Assert.IsTrue(ExportAllCommand.CanExecute(vbe.Object.VBProjects.First()));
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_CanExecute_PassedIVBProject_NoComponents_ExpectFalse()
        {
            var builder = new MockVbeBuilder();
            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected);
            var project = projectMock.Build();
            var vbe = builder.AddProject(project).Build();

            vbe.SetupGet(m => m.ActiveVBProject.VBComponents.Count).Returns(0);

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            Assert.IsFalse(ExportAllCommand.CanExecute(vbe.Object));
        }


        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedNull_SingleProject_ExpectExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project = projectMock.Build();
            project.SetupGet(m => m.IsSaved).Returns(true);
            project.SetupGet(m => m.FileName).Returns(_projectFullPath);

            var vbe = builder.AddProject(project).Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.OK);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(null);

            project.Verify(m => m.ExportSourceFiles(_path), Times.Once);
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedIVBProject_SingleProject_ExpectExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project = projectMock.Build();
            project.SetupGet(m => m.IsSaved).Returns(true);
            project.SetupGet(m => m.FileName).Returns(_projectFullPath);

            var vbe = builder.AddProject(project).Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.OK);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(project.Object);

            project.Verify(m => m.ExportSourceFiles(_path), Times.Once);
        }


        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedNull_MultipleProjects_ExpectExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock1 = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var projectMock2 = builder.ProjectBuilder("TestProject2", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project1 = projectMock1.Build();
            var project2 = projectMock2.Build();
            project1.SetupGet(m => m.IsSaved).Returns(true);
            project1.SetupGet(m => m.FileName).Returns(_projectFullPath);
            project2.SetupGet(m => m.IsSaved).Returns(true);
            project2.SetupGet(m => m.FileName).Returns(_projectFullPath2);

            var vbe = builder
                .AddProject(project1)
                .AddProject(project2)
                .Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.OK);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project2.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(null);

            // project2 added last, will be active project
            project1.Verify(m => m.ExportSourceFiles(_path), Times.Never);
            project2.Verify(m => m.ExportSourceFiles(_path), Times.Once);
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedIVBProject_MultipleProjects_ExpectExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock1 = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var projectMock2 = builder.ProjectBuilder("TestProject2", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project1 = projectMock1.Build();
            var project2 = projectMock2.Build();
            project1.SetupGet(m => m.IsSaved).Returns(true);
            project1.SetupGet(m => m.FileName).Returns(_projectFullPath);
            project2.SetupGet(m => m.IsSaved).Returns(true);
            project2.SetupGet(m => m.FileName).Returns(_projectFullPath2);

            var vbe = builder
                .AddProject(project1)
                .AddProject(project2)
                .Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.OK);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project1.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(project1.Object);

            project1.Verify(m => m.ExportSourceFiles(_path), Times.Once);
            project2.Verify(c => c.ExportSourceFiles(_path), Times.Never);
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedNull_SingleProject_BrowserCanceled_ExpectNoExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project = projectMock.Build();
            project.SetupGet(m => m.IsSaved).Returns(true);
            project.SetupGet(m => m.FileName).Returns(_projectFullPath);

            var vbe = builder.AddProject(project).Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.Cancel);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(null);

            project.Verify(m => m.ExportSourceFiles(_path), Times.Never);
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedIVBProject_SingleProject_BrowserCanceled_ExpectNoExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project = projectMock.Build();
            project.SetupGet(m => m.IsSaved).Returns(true);
            project.SetupGet(m => m.FileName).Returns(_projectFullPath);

            var vbe = builder.AddProject(project).Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.Cancel);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(project.Object);

            project.Verify(m => m.ExportSourceFiles(_path), Times.Never);
        }


        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedNull_MultipleProjects_BrowserCanceled_ExpectNoExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock1 = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var projectMock2 = builder.ProjectBuilder("TestProject2", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project1 = projectMock1.Build();
            var project2 = projectMock2.Build();
            project1.SetupGet(m => m.IsSaved).Returns(true);
            project1.SetupGet(m => m.FileName).Returns(_projectFullPath);
            project2.SetupGet(m => m.IsSaved).Returns(true);
            project2.SetupGet(m => m.FileName).Returns(_projectFullPath2);

            var vbe = builder
                .AddProject(project1)
                .AddProject(project2)
                .Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.Cancel);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project2.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(null);

            // project2 added last, will be active project
            project1.Verify(m => m.ExportSourceFiles(_path), Times.Never);
            project2.Verify(m => m.ExportSourceFiles(_path), Times.Never);
        }

        [TestCategory("Commands")]
        [TestMethod]
        public void ExportAllCommand_Execute_PassedIVBProject_MultipleProjects_BrowserCanceled_ExpectNoExecution()
        {
            var builder = new MockVbeBuilder();

            var projectMock1 = builder.ProjectBuilder("TestProject1", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var projectMock2 = builder.ProjectBuilder("TestProject2", ProjectProtection.Unprotected)
                .AddComponent("Module1", ComponentType.StandardModule, "")
                .AddComponent("ClassModule1", ComponentType.ClassModule, "")
                .AddComponent("Document1", ComponentType.Document, "")
                .AddComponent("UserForm1", ComponentType.UserForm, "");

            var project1 = projectMock1.Build();
            var project2 = projectMock2.Build();
            project1.SetupGet(m => m.IsSaved).Returns(true);
            project1.SetupGet(m => m.FileName).Returns(_projectFullPath);
            project2.SetupGet(m => m.IsSaved).Returns(true);
            project2.SetupGet(m => m.FileName).Returns(_projectFullPath2);

            var vbe = builder
                .AddProject(project1)
                .AddProject(project2)
                .Build();

            var mockFolderBrowser = new Mock<IFolderBrowser>();
            mockFolderBrowser.Setup(m => m.SelectedPath).Returns(_path);
            mockFolderBrowser.Setup(m => m.ShowDialog()).Returns(DialogResult.Cancel);

            var mockFolderBrowserFactory = new Mock<IFolderBrowserFactory>();
            mockFolderBrowserFactory.Setup(m => m.CreateFolderBrowser(It.IsAny<string>(), true, _projectPath)).Returns(mockFolderBrowser.Object);
            project1.Setup(m => m.ExportSourceFiles(_path));

            var ExportAllCommand = new ExportAllCommand(vbe.Object, mockFolderBrowserFactory.Object);

            ExportAllCommand.Execute(project1.Object);

            project1.Verify(m => m.ExportSourceFiles(_path), Times.Never);
            project2.Verify(m => m.ExportSourceFiles(_path), Times.Never);
        }
    }
}
