﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace RetailCoderVBE.UnitTesting.UI
{
    internal partial class TestExplorerWindow : Form, ITestOutput
    {
        private BindingList<TestExplorerItem> _allTests;
        private IList<TestExplorerItem> _tests;

        public TestExplorerWindow()
        {
            InitializeComponent();
            FormClosing += TestExplorerWindow_FormClosing;
            testOutputGridView.CellDoubleClick += OnCellDoubleClick;
            gotoSelectionButton.Click += gotoSelectionButton_Click;
            addTestMethodButton.Click += addTestMethodButton_Click;
            addTestModuleButton.Click += addTestModuleButton_Click;
            addExpectedErrorTestMethodButton.Click += addExpectedErrorTestMethodButton_Click;
            runAllTestsMenuItem.Click += runAllTestsMenuItem_Click;
            runFailedTestsMenuItem.Click += runFailedTestsMenuItem_Click;
            runPassedTestsMenuItem.Click += runPassedTestsMenuItem_Click;
            runNotRunTestsMenuItem.Click += runNotRunTestsMenuItem_Click;
            runLastRunMenuItem.Click += runLastRunMenuItem_Click;
            runSelectedTestMenuItem.Click += runSelectedTestMenuItem_Click;

            _allTests = new BindingList<TestExplorerItem>();
            _tests = new List<TestExplorerItem>();

            testOutputGridView.DataSource = _allTests;

            var messageColumn = testOutputGridView.Columns
                                                  .Cast<DataGridViewColumn>()
                                                  .FirstOrDefault(column => column.HeaderText == "Message");
            if (messageColumn != null)
            {
                messageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void OnButtonClick(EventHandler clickEvent)
        {
            var handler = clickEvent;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<SelectedTestEventArgs> OnRunSelectedTestButtonClick;
        void runSelectedTestMenuItem_Click(object sender, EventArgs e)
        {
            var handler = OnRunSelectedTestButtonClick;
            if (handler != null && _allTests.Any())
            {
                var selection = _allTests.Where(test => testOutputGridView.SelectedRows
                                                                          .Cast<DataGridViewRow>()
                                                                          .Select(row => row.DataBoundItem as TestExplorerItem)
                                                                          .Select(item => item.GetTestMethod())
                                                                          .Contains(test.GetTestMethod()));

                handler(this, new SelectedTestEventArgs(selection));
            }
        }

        public event EventHandler OnRunLastRunTestsButtonClick;
        void runLastRunMenuItem_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnRunLastRunTestsButtonClick);
        }

        public event EventHandler OnRunNotRunTestsButtonClick;
        void runNotRunTestsMenuItem_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnRunNotRunTestsButtonClick);
        }

        public event EventHandler OnRunPassedTestsButtonClick;
        void runPassedTestsMenuItem_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnRunPassedTestsButtonClick);
        }

        public event EventHandler OnRunFailedTestsButtonClick;
        void runFailedTestsMenuItem_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnRunFailedTestsButtonClick);
        }

        public event EventHandler OnRunAllTestsButtonClick;
        void runAllTestsMenuItem_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnRunAllTestsButtonClick);
        }

        public event EventHandler OnAddExpectedErrorTestMethodButtonClick;
        void addExpectedErrorTestMethodButton_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnAddExpectedErrorTestMethodButtonClick);
        }

        public event EventHandler OnAddTestMethodButtonClick;
        void addTestMethodButton_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnAddTestMethodButtonClick);
        }

        public event EventHandler OnAddTestModuleButtonClick;
        void addTestModuleButton_Click(object sender, EventArgs e)
        {
            OnButtonClick(OnAddTestModuleButtonClick);
        }

        void TestExplorerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void ClearProgress()
        {
            _completedCount = 0;
            testProgressBar.Maximum = _allTests.Count;
            testProgressBar.Value = 0;
            UpdateCompletedTestsLabels();
        }

        public void ClearResults()
        {
            _allTests = new BindingList<TestExplorerItem>(_allTests.Select(test => new TestExplorerItem(test.GetTestMethod(), null)).ToList());
            testOutputGridView.DataSource = _allTests;
        }

        private int _completedCount; 
        private void UpdateProgress()
        {
            UpdateCompletedTestsLabels();

            testProgressBar.Maximum = _tests.Count;
            testProgressBar.Value = ++_completedCount;
        }

        private void UpdateCompletedTestsLabels()
        {
            passedTestsLabel.Text = string.Format("{0} Passed", _tests.Count(item => item.Outcome == TestOutcome.Succeeded.ToString()));
            failedTestsLabel.Text = string.Format("{0} Failed", _tests.Count(item => item.Outcome == TestOutcome.Failed.ToString()));
            inconclusiveTestsLabel.Text = string.Format("{0} Inconclusive", _tests.Count(item => item.Outcome == TestOutcome.Inconclusive.ToString()));
        }

        private TestExplorerItem FindItem(IEnumerable<TestExplorerItem> items, TestMethod test)
        {
            return items.FirstOrDefault(item => item.ProjectName == test.ProjectName
                                                 && item.ModuleName == test.ModuleName
                                                 && item.MethodName == test.MethodName);
        }

        public void WriteResult(TestMethod test, TestResult result)
        {
            var gridItem = FindItem(_allTests, test);
            var playListItem = FindItem(_tests, test);
            
            if (gridItem == null)
            {
                var item = new TestExplorerItem(test, result);
                _allTests.Add(item);
            }
            else
            {
                gridItem.SetResult(result);
                playListItem.SetResult(result);
            }

            UpdateProgress();
            testOutputGridView.Refresh();
        }

        public void Refresh(IDictionary<TestMethod,TestResult> tests)
        {
            _allTests = new BindingList<TestExplorerItem>(tests.Select(test => new TestExplorerItem(test.Key, test.Value)).ToList());
            testOutputGridView.DataSource = _allTests;
            testOutputGridView.Refresh();
        }

        public void SetPlayList(IDictionary<TestMethod,TestResult> tests)
        {
            _tests = tests.Select(test => new TestExplorerItem(test.Key, test.Value)).ToList();
            UpdateCompletedTestsLabels();
        }

        public event EventHandler OnRefreshListButtonClick;
        private void RefreshTestsButtonClick(object sender, System.EventArgs e)
        {
            OnButtonClick(OnRefreshListButtonClick);
        }

        public event EventHandler<SelectedTestEventArgs> OnGoToSelectedTest;
        private void OnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var handler = OnGoToSelectedTest;
            if (handler != null)
            {
                handler(this, new SelectedTestEventArgs(_allTests[e.RowIndex]));
            }
        }

        void gotoSelectionButton_Click(object sender, EventArgs e)
        {
            var handler = OnGoToSelectedTest;
            if (handler != null && _allTests.Any())
            {
                handler(this, new SelectedTestEventArgs(_allTests[testOutputGridView.SelectedRows.Cast<DataGridViewRow>().First().Index]));
            }
        }
    }

    internal class SelectedTestEventArgs : EventArgs
    {
        public SelectedTestEventArgs(IEnumerable<TestExplorerItem> items)
        {
            _selection = items.Select(item => item.GetTestMethod());
        }

        public SelectedTestEventArgs(TestExplorerItem item)
            : this(new[] { item })
        { }

        private readonly IEnumerable<TestMethod> _selection;
        public IEnumerable<TestMethod> Selection { get { return _selection; } }
    }

    internal class TestExplorerItem
    {
        public TestExplorerItem(TestMethod test, TestResult result)
        {
            _test = test;
            _result = result;
        }

        private readonly TestMethod _test;
        public TestMethod GetTestMethod()
        {
            return _test;
        }

        private TestResult _result;
        public void SetResult(TestResult result)
        {
            _result = result;
        }

        public Image Result { get { return _result.Icon(); } }
        public string ProjectName { get { return _test.ProjectName; } }
        public string ModuleName { get { return _test.ModuleName; } }
        public string MethodName { get { return _test.MethodName; } }
        public string Outcome { get { return _result == null ? string.Empty : _result.Outcome.ToString(); } }
        public string Message { get { return _result == null ? string.Empty : _result.Output; } }
        public string Duration { get { return _result == null ? string.Empty : _result.Duration.ToString() + " ms"; } }
    }

    internal static class TestResultExtensions
    {
        public static Image Icon(this TestResult result)
        {
            var image = RetailCoderVBE.Properties.Resources.question_white;
            if (result != null)
            {
                switch (result.Outcome)
                {
                    case TestOutcome.Succeeded:
                        image = RetailCoderVBE.Properties.Resources.tick_circle;
                        break;

                    case TestOutcome.Failed:
                        image = RetailCoderVBE.Properties.Resources.minus_circle;
                        break;

                    case TestOutcome.Inconclusive:
                        image = RetailCoderVBE.Properties.Resources.exclamation_circle;
                        break;
                }
            }

            return image;
        }
    }
}
