﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TfsUtility.Tests
{
    [TestClass]
    public class WorkItemQueryFolderListCommandFixture : UnitTestBase
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
            _systemUnderTest = null;
            Args = null;
        }
        public string[] Args { get; set; }

        private WorkItemQueryFolderListCommand _systemUnderTest;
        public WorkItemQueryFolderListCommand SystemUnderTest
        {
            get
            {
                if (_systemUnderTest == null)
                {
                    _systemUnderTest =
                        new WorkItemQueryFolderListCommand(Args);
                }

                return _systemUnderTest;
            }
        }

        [TestMethod]
        public void ValidateArgumentsSucceedsWithValidArguments()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName));

            Assert.IsNotNull(SystemUnderTest);
        }

        [TestMethod]
        public void ValidateArgumentsSucceedsWithValidArgumentsPlusFilter()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName));

            Assert.IsNotNull(SystemUnderTest);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingArgumentException))]
        public void ValidateArgumentsFailsWithInvalidArguments()
        {
            Args = CreateArgsArray();

            Assert.IsNotNull(SystemUnderTest);
        }

        [TestMethod]
        public void GetResultForNoFilter()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName));

            List<string> folders = SystemUnderTest.GetResult();

            Assert.AreEqual<int>(3, folders.Count, "Folder count is wrong.");

            AssertContainsFolder(folders, "scrum-test-20131023/My Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries/Current Sprint");
        }

        [TestMethod]
        public void GetResultForFilterOnSingleSubirectory()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName),
                GetArgEntry(TfsUtilityConstants.ArgumentNameFolderFilter, "Shared Queries"));

            List<string> folders = SystemUnderTest.GetResult();

            Assert.AreEqual<int>(2, folders.Count, "Folder count is wrong.");

            AssertDoesNotContainFolder(folders, "scrum-test-20131023/My Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries/Current Sprint");
        }

        [TestMethod]
        public void GetResultForFilterOnSingleSubirectoryThatStartsWithSlash()
        {
            // add a leading slash to the folder filter
            // it shouldn't make any difference to the results

            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName),
                GetArgEntry(TfsUtilityConstants.ArgumentNameFolderFilter, "/Shared Queries"));
            
            List<string> folders = SystemUnderTest.GetResult();

            Assert.AreEqual<int>(2, folders.Count, "Folder count is wrong.");

            AssertDoesNotContainFolder(folders, "scrum-test-20131023/My Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries/Current Sprint");
        }

        [TestMethod]
        public void GetResultForFilterOnTwoLevelSubirectory()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName),
                GetArgEntry(TfsUtilityConstants.ArgumentNameFolderFilter, "Shared Queries/Current Sprint"));

            List<string> folders = SystemUnderTest.GetResult();

            Assert.AreEqual<int>(1, folders.Count, "Folder count is wrong.");

            AssertDoesNotContainFolder(folders, "scrum-test-20131023/My Queries");
            AssertDoesNotContainFolder(folders, "scrum-test-20131023/Shared Queries");
            AssertContainsFolder(folders, "scrum-test-20131023/Shared Queries/Current Sprint");
        }

        [TestMethod]
        public void GetResultForFilterOnFolderThatDoesntExistReturnsZeroResults()
        {
            Args = CreateArgsArray(
                TfsUtilityConstants.CommandArgumentNameListWorkItemQueryFolders,
                GetArgEntry(TfsUtilityConstants.ArgumentNameTfsCollection, TfsCollectionUrl),
                GetArgEntry(TfsUtilityConstants.ArgumentNameTeamProject, TeamProjectName),
                GetArgEntry(TfsUtilityConstants.ArgumentNameFolderFilter, "bogus folder name"));

            List<string> folders = SystemUnderTest.GetResult();

            Assert.IsNotNull(folders, "Folders collection should not be null.");

            Assert.AreEqual<int>(0, folders.Count, "Folder count is wrong.");
        }

        private void AssertContainsFolder(List<string> actualFolderPaths, string expectedFolderPath)
        {
            Assert.IsTrue(actualFolderPaths.Contains(expectedFolderPath),
                $"'{expectedFolderPath}' was not in the result.");
        }

        private void AssertDoesNotContainFolder(List<string> actualFolderPaths, string expectedFolderPath)
        {
            Assert.IsFalse(actualFolderPaths.Contains(expectedFolderPath),
                $"'{expectedFolderPath}' was in the result.");
        }
    }
}
