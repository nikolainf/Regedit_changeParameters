// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;
using System.Linq;
using System.Security;

Dictionary<string, string> currentAndNewValues = new Dictionary<string, string>()
{
    { "C:\\Users\\Админ\\", "C:\\Users\\Admin\\" }
    //{ "c:\\Users\\Админ\\", "c:\\Users\\Admin\\" },
    //{ "c:\\users\\Админ\\", "c:\\users\\Admin\\" }
};

var current_new = currentAndNewValues.First();

Console.WriteLine("Hello, World!");

RegistryKey registryKey = Registry.Users;

var allSubkeyNames = GetAllSubKeyNames(registryKey, string.Empty);

var a = allSubkeyNames.First(n => n.StartsWith("S-1-5-21-1382416535-229451203-2106293434-1001\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\UFH\\SHC"));

var adminValues = new List<string>();   
foreach (var subKeyName in allSubkeyNames)
{
    adminValues.AddRange(GetAdminValues(subKeyName, registryKey));
}

var c = adminValues.Count;

static List<string> GetAllSubKeyNames(RegistryKey rk, string rootKeyFullName)
{
    var resultList = new List<string>();
    var subKeyNames = rk.GetSubKeyNames();

    foreach(var subKeyName in subKeyNames)
    {
        var subKeyFullName = string.IsNullOrWhiteSpace(rootKeyFullName)
            ? subKeyName
            : Path.Combine(rootKeyFullName, subKeyName);

        resultList.Add(subKeyFullName);
        try
        {
            RegistryKey sk = rk.OpenSubKey(subKeyName)!;
            resultList.AddRange(GetAllSubKeyNames(sk, subKeyFullName));
        }
        catch(SecurityException securityException)
        {
            continue;
        }
    }

    return resultList;
}

string[] GetAdminValues(string subKeyName, RegistryKey rk)
{

    //if (subKeyName.StartsWith("S-1-5-21-1382416535-229451203-2106293434-1001\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\UFH\\SHC"))
    //{

    //}
    RegistryKey subKey;

    try
    {
        subKey = rk.OpenSubKey(subKeyName, true)!;
    }
    catch (SecurityException securityException) 
    {
        return new string[0];
    }
   var valueNames = subKey?.GetValueNames();
    if (valueNames?.Length > 0)
    {
        List<string> values = new List<string>();
        foreach (var valueName in valueNames)
        {
           

            object objValue = subKey.GetValue(valueName);
           
            if (objValue is string[])
            {
                string[] valueAsArray = (string[]) objValue;

                bool hasNewValue = false;
                for(int i = 0; i < valueAsArray.Length; i++)
                {
                    if (valueAsArray[i].Contains(current_new.Key))
                    {
                        var newValue = valueAsArray[i].Replace(current_new.Key, current_new.Value);
                        adminValues.Add(valueAsArray[i]);
                        valueAsArray[i] = newValue;
                        hasNewValue = true;


                        adminValues.Add(valueAsArray[i]);
                    }
                }

                if (hasNewValue)
                {
                    subKey.SetValue(valueName, valueAsArray);
                }

            }
            else
            {
                var value = subKey.GetValue(valueName).ToString();
                if (value.Contains(current_new.Key))
                {
                    string newValue = value.Replace(current_new.Key, current_new.Value);
                    subKey.SetValue(valueName, newValue);
                    adminValues.Add(value);
                    adminValues.Add(newValue);
                }
            }

            
        }

       

        return values.ToArray();
    }
    return new string[0];
  
}


//foreach (var sk in skNames)
//{
//    string pathSK = Path.Combine(path, sk);

//    RegistryKey subRK = Registry.LocalMachine.OpenSubKey(pathSK, true);
//    var valueNames = subRK.GetValueNames();

//    foreach(var valueName in valueNames)
//    {
//        var value = subRK.GetValue(valueName).ToString();
//        if(value.StartsWith("C:\\Users\\Админ\\AppData\\Local\\Programs\\Python"))
//        {
//            string newValue = value.Replace("\\Админ", "\\Admin");

//            subRK.SetValue(valueName, newValue);
//        }
       
//    }
    
//}


