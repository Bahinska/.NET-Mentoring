﻿/*
 * This task is a bit harder than the previous two.
 * Feel free to change the E3SLinqProvider and any other classes if needed.
 * Possibly, after these changes you will need to rewrite existing tests to make them work again =).
 *
 * The task: implement support of && operator for IQueryable. The final request generated by FTSRequestGenerator, should
 * imply the following rules: https://kb.epam.com/display/EPME3SDEV/Telescope+public+REST+for+data#TelescopepublicRESTfordata-FTSRequestSyntax
 */

using System;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Task3.E3SQueryProvider.Models.Entities;
using Xunit;

namespace Expressions.Task3.E3SQueryProvider.Test
{
    public class E3SAndOperatorSupportTests
    {
        #region SubTask 3: AND operator support


        [Fact]
        public void TestAndQueryable()
        {
            var translator = new ExpressionToFtsRequestTranslator();
            Expression<Func<IQueryable<EmployeeEntity>, IQueryable<EmployeeEntity>>> expression
                = query => query.Where(e => e.Workstation == "EPRUIZHW006" && e.Manager.StartsWith("John"));

            // Execute the translation
            string translated = translator.Translate(expression);

            // Validate the translation output
            string expected = "(Workstation:(EPRUIZHW006)) AND (Manager:(John*))";
            Assert.Equal(expected, translated);

            #endregion
        }
    }
}
