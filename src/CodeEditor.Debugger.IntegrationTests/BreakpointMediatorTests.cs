using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.IntegrationTests
{
    [TestFixture]
    class BreakpointMediatorTests : DebuggerTestBase
    {
        [Test]
        public void CanSetBreakpointOnLine()
        {
            _vm.OnVMStart += e => {
                Console.WriteLine ("instructing vm to resume...");
                _vm.Resume ();
            };
            _vm.OnTypeLoad += e => {
                _vm.Resume();
            //    Finish();
            };
            _vm.OnAssemblyLoad += e => _vm.Resume();

            var breakpointProvider = new BreakpointProvider ();
            breakpointProvider.ToggleBreakPointAt (LocationOfSourceFile, 8);
            new BreakpointMediator (_vm, breakpointProvider);
            
            _vm.OnBreakpoint += e => 
                {
                    Assert.AreEqual("Main", e.Method.FullName);
                    Finish();
                };
            
            WaitUntilFinished();
        }
    }
}
