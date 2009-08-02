//  Copyright (C) 2009 by Johann Duscher (alias Jonny Dee)
//
//  This file is part of MarkEmptyDirs.
//
//  MarkEmptyDirs is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  MarkEmptyDirs is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with MarkEmptyDirs.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace DJ.Util.Template
{
    abstract public class TemplateVariable
    {
        public const string Prefix = "§";
        public const string Postfix = "§";
        public const string Separator = ":";
        public const string Pattern = @"(?<Variable>§(?<Name>[a-zA-Z][a-zA-Z_]*)(\:(?<Args>[^§]*))?§)";
        
        private readonly string _name;

        protected TemplateVariable(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }

        abstract public string Description { get; }
        
        abstract public bool CanHaveArgument { get; }

        abstract public string ArgumentDescription { get; }

        abstract public string ArgumentIdentifier { get; }
        
        abstract public bool ArgumentMandatory { get; }

        abstract public string GetValueFor(TemplateEngine ctx, string arg);
    }
}