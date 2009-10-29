//  
//  Copyright (c) 2009 by Johann Duscher (alias Jonny Dee)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace DJ.Util.IO
{
    [TestFixture]
    public class TestSymbolicLinkHelper
    {
        [Test]
        public void TestCreateSymbolicLinkToDirectory()
        {
            bool result = SymbolicLinkHelper.CreateSymbolicLink(new FileInfo("file"), new FileInfo("link_to_file"));
            Assert.IsTrue(result);
        }

        [Test]
        public void TestCreateSymbolicLinkToFile()
        {
            bool result = SymbolicLinkHelper.CreateSymbolicLink(new DirectoryInfo("/"), new FileInfo("link_to_directory"));
            Assert.IsTrue(result);
        }
    }
}