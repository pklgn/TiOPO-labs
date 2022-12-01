// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace lab9.Tools.String;

internal class Translit
{
    static Dictionary<char, string> translation = new Dictionary<char, string>()
    {
        { 'а', "a" },
        { 'б', "b" },
        { 'в', "v" },
        { 'г', "g" },
        { 'д', "d" },
        { 'e', "e" },
        { 'ё', "yo" },
        { 'ж', "g" },
        { 'з', "z" },
        { 'и', "i" },
        { 'й', "y" },
        { 'к', "k" },
        { 'л', "l" },
        { 'м', "m" },
        { 'н', "n" },
        { 'о', "o" },
        { 'п', "p" },
        { 'р', "r" },
        { 'с', "s" },
        { 'т', "t" },
        { 'у', "u" },
        { 'ф', "f" },
        { 'х', "kc" },
        { 'ц', "ts" },
        { 'ч', "ch" },
        { 'ш', "sh" },
        { 'щ', "shsh" },
        { 'ъ', "ie" },
        { 'ы', "y" },
        { 'ь', "'" },
        { 'э', "e" },
        { 'ю', "iu" },
        { 'я', "ia" },
        { ' ', "-" },
        { '&', "-and-" },
    };

    string ToTranslit(char c)
    {
        string? result;

        return translation.TryGetValue(c, out result)
            ? result
            : c.ToString();
    }

    string ToTranslit(string src)
    {
        string result = "";

        foreach (char c in src)
        {
            result += ToTranslit(c);
        }

        return result;
    }

}
