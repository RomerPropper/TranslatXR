using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationHelper : MonoBehaviour
{
    public TMP_Dropdown dropDown;
    public Sprite[] flags;
    
    // Start is called before the first frame update
    void Start()
    {
         // Clear any existing options, just in case
        dropDown.ClearOptions();

        // Create the option list
        List<TMP_Dropdown.OptionData> flagItems = new List<TMP_Dropdown.OptionData>();
        
        // Loop through each sprite
        foreach (var flag in flags)
        {
            // Try and find the '.' in the sprite's name. This is used as a delimiter
            // between the country code and the name of the language
            string flagName = flag.name;
            int dot = flag.name.IndexOf('.');
            if (dot >= 0)
            {
                // Found? Then set the flag name to the characters to the right of the dot
                flagName = flagName.Substring(dot + 1);
            }
            
            // Add the option to the list
            var flagOption = new TMP_Dropdown.OptionData(flagName, flag);
            flagItems.Add(flagOption);
        }

        // Add the options to the drop down box
        dropDown.AddOptions(flagItems);
    }


}
