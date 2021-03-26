using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler
{
    public static int GetNumber(InputField number)
    {
        var numberStr = number.text;

        if (numberStr.Trim().Length > 0 && numberStr.All(char.IsDigit) == true)
            return int.Parse(numberStr);

        return -1;
    }

    public static int GetCheckedDropdownElement(Dropdown dropdown)
    {
        return int.Parse(dropdown.options[dropdown.value].text);
    }
}
